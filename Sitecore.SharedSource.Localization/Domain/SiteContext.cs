using System;
using System.Configuration;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using Sitecore.SharedSource.Localization.Infrastructure;
using Sitecore.Sites;
using System.Collections.Generic;

namespace Sitecore.SharedSource.Localization.Domain
{
    /// <summary>
    /// Helper Class SiteContext. Gives unified access to sitecore 
    /// context using it's own logic for getting context site for page editor. 
    /// </summary>
    internal class SiteContext
    {
        /// <summary>
        /// Gets the sitecore context database.
        /// </summary>
        /// <value>The context database.</value>
        public virtual Database ContextDb
        {
            get { return IsShellSite || IsPageEditor ? MasterDb : Context.Database; }
        }

        /// <summary>
        /// Gets the sitecore master database.
        /// </summary>
        /// <value>The master database.</value>
        public virtual Database MasterDb
        {
            get { return Database.GetDatabase("master"); }
        }

        /// <summary>
        /// Gets a value indicating whether sitecore mode is page editor.
        /// </summary>
        /// <value><c>true</c> if this sitecore mode is page editor; otherwise, <c>false</c>.</value>
        public virtual bool IsPageEditor
        {
            get { return !Context.PageMode.IsNormal; }
        }

        /// <summary>
        /// Gets the dictionary root for current site with context database.
        /// </summary>
        /// <value>The sitecore Item which is mapped to be dictionary root.</value>
        public virtual Item DictionaryRoot
        {
            get { return GetDictionaryRoot(ContextDb); }
        }

        /// <summary>
        /// Gets the context sitecore site name. 
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string GetSiteName()
        {
            if (IsPageEditor)
            {
                var itemIdStr = HttpContext.Current.Request["sc_itemid"];

                Guid itemId;
                if (Guid.TryParse(itemIdStr, out itemId))
                {
                    return ResolveSiteNameUsingContextItem(ContextDb.GetItem(new ID(itemId)));
                }
            }

            return Context.GetSiteName();
        }

        /// <summary>
        /// Gets the dictionary root.
        /// </summary>
        /// <param name="contextDb">The context db.</param>
        /// <returns>Item.</returns>
        public virtual Item GetDictionaryRoot(Database contextDb)
        {
            Item rootItem = null;

            if (!string.IsNullOrEmpty(GetSiteName()))
            {
                var dictionaryRootItemPath = ModuleSettings.GetSiteSpecificDictionaryFolder(GetSiteName());

                if (!string.IsNullOrEmpty(dictionaryRootItemPath))
                {
                    rootItem = contextDb.GetItem(dictionaryRootItemPath) ?? CreateDictionaryRoot(dictionaryRootItemPath);
                }
            }

	        return rootItem ?? (contextDb.GetItem(ModuleSettings.GlobalDictionaryFolder));
        }

        /// <summary>
        /// Publishes the specified item root.
        /// </summary>
        /// <param name="itemRoot">The item root.</param>
        public virtual void Publish(Item itemRoot)
        {
	        if (ModuleSettings.AutoPublishCreatedItems)
	        {
		        var publishingTargets = GetPublishingTargets(itemRoot.Database);
		        var publishingTargetConnectionStrings =
			        publishingTargets.Select(
				        x => ConfigurationManager.ConnectionStrings[x.Name.ToLower()].ConnectionString.ToLower());

		        if (
			        publishingTargetConnectionStrings.All(
				        x => x != ConfigurationManager.ConnectionStrings["master"].ConnectionString.ToLower()))
		        {
			        var targetLanguages = GetTargetLanguages();

			        Logger.Info(string.Format(
				        "Publishing item {0} to '{1}' languages...",
				        itemRoot.Paths.Path,
				        string.Join("', '", targetLanguages.Select(x => x.Name))),
			                    this);

			        PublishManager.PublishItem(itemRoot, publishingTargets, targetLanguages, true, false);
		        }
	        }
	        else
	        {
				Logger.ExtraInfo("Publishing disabled for automatically created items. For enabling autopublishing set 'Localization.AutoPublishCreatedItems' settings to true.", this);
	        }
        }

        #region Private 

        protected virtual bool IsShellSite
        {
            get { return Context.GetSiteName() == Sitecore.Constants.ShellSiteName; }
        }

        protected virtual string ResolveSiteNameUsingContextItem(Item currentItem)
        {
            var siteName = string.Empty;

            var sites = GetAllConfiguredSites();

            if (sites.ContainsKey(currentItem.ID.Guid))
            {
                siteName = sites[currentItem.ID.Guid];
            }

            if (string.IsNullOrEmpty(siteName))
            {
                var rootItem = currentItem.Axes.GetAncestors().SingleOrDefault(x => sites.ContainsKey(x.ID.Guid));

                if (rootItem != null)
                {
                    siteName = sites[rootItem.ID.Guid];
                }
            }

            if (string.IsNullOrEmpty(siteName))
            {
                siteName = Context.GetSiteName();
            }

            Logger.ExtraInfo(string.Format("Site name was resolved as '{0}'", Context.GetSiteName()), this);

            return siteName;
        }

        protected virtual Item CreateDictionaryRoot(string dictionaryRootItemPath)
        {
            using (new SecurityDisabler())
            {
                var nodeNames = dictionaryRootItemPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var currentRoot = MasterDb.GetItem("/" + nodeNames[0] + "/");

                for (var i = 1; i < nodeNames.Length; i++)
                {
                    var targetDictionaryRoot = string.Format("{0}/{1}", currentRoot.Paths.Path, nodeNames[i]);
                    var targetFolder = MasterDb.GetItem(targetDictionaryRoot);

                    if (targetFolder == null)
                    {
                        currentRoot = currentRoot.Add(nodeNames[i], Constants.DictionaryRootTemplateId);
                        Publish(currentRoot);
                    }
                    else
                    {
                        currentRoot = targetFolder;
                    }
                }

                try
                {
                    currentRoot.Editing.BeginEdit();
                    if (string.IsNullOrEmpty(currentRoot["__Icon"]))
                    {
                        currentRoot["__Icon"] = "People/16x16/book_red.png";
                    }

                    var insertOptionsGuidArray = GetInsertOptionsForDictionaryRoot(currentRoot);

                    currentRoot["__Masters"] = string.Join("|", insertOptionsGuidArray);

                    currentRoot.Editing.EndEdit();

                    Logger.Info(string.Format(
                            "Dictionary root folder '{0}' with ID {1} has been created.",
                            currentRoot.Paths.Path,
                            currentRoot.ID.Guid), this);
                }
                catch(Exception ex)
                {
                    Logger.Error(string.Format("Error updating icon and insert options for item {0}", currentRoot.ID.Guid), ex, this);

                    currentRoot.Editing.CancelEdit();
                }

                return currentRoot;
            }
        }

        private static IEnumerable<Guid> GetInsertOptionsForDictionaryRoot(BaseItem currentRoot)
        {
            var insertOptionsGuidArray = new List<Guid>
            {
                Constants.DictionaryEntryTemplateId.ID.Guid,
                Constants.DictionaryFolderTemplateId.ID.Guid
            };

            var insertOptions = currentRoot["__Masters"];

            if (!string.IsNullOrEmpty(insertOptions))
            {
                var insertOptionsArray = insertOptions.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = insertOptionsArray.Length - 1; i >= 0; i--)
                {
                    var s = insertOptionsArray[i];
                    Guid guid;
                    if (Guid.TryParse(s, out guid) && !insertOptionsGuidArray.Contains(guid))
                    {
                        insertOptionsGuidArray.Insert(0, guid);
                    }
                }
            }

            return insertOptionsGuidArray;
        }

        protected Language[] GetTargetLanguages()
        {
            return LanguageManager.GetLanguages(MasterDb).ToArray();
        }

        protected static Database[] GetPublishingTargets(Database sourceDatabase)
        {
            var publishingTargetItems = PublishManager.GetPublishingTargets(sourceDatabase).ToList();

            return publishingTargetItems
                .ToDictionary(item => item.Fields["Target database"].Value, item => item.Name)
                .Select(x => Database.GetDatabase(x.Key)).ToArray();
        }

        protected virtual IDictionary<Guid, string> GetAllConfiguredSites()
        {
            if (_configuredSites.Count == 0)
            {
                lock (_configuredSites)
                {
                    if (_configuredSites.Count == 0)
                    {
                        var ignoredSitesList = new[] { "shell", "modules_shell", "modules_website", "login", "scheduler", "publisher", "system" };
                        var sites = SiteManager.GetSites();

                        foreach (var site in sites)
                        {
                            var itemPath = site.Properties["rootPath"] + "/" + site.Properties["startItem"];
                            itemPath = itemPath.Replace("//", "/").TrimEnd('/');
                            var item = ContextDb.GetItem(itemPath);
                            if (item != null && !ignoredSitesList.Contains(site.Name) && !_configuredSites.ContainsKey(item.ID.Guid))
                            {
                                _configuredSites.Add(item.ID.Guid, site.Name);
                            }
                        }
                    }
                }
            }

            return _configuredSites;
        }

        protected static IDictionary<Guid, string> _configuredSites = new Dictionary<Guid, string>();

        #endregion
    }
}
