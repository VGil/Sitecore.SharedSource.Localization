using System;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Sitecore.SharedSource.Localization.Infrastructure;
using Sitecore.SharedSource.Localization.Infrastructure.Caching;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.SharedSource.Localization.Domain
{
    internal class TranslatorService
    {
        protected SiteContext m_SiteContext = new SiteContext();
        protected SitecoreDictionaryCacheManager m_DictionaryCache = new SitecoreDictionaryCacheManager();

        /// <summary>
        /// Gets translated phrase for context language by specified translation key.
        /// With option of formatting like string.Format() method functionality.
        /// Dictionary entry item will be created if it doesn't exist with default phrase value 
        /// equals to passed parameter (or it will be equal to translation key in case default value is null or empty).
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="formatParams">The format params.</param>
        /// <returns>System.String.</returns>
        public virtual string Text(string key, string defaultValue, params object[] formatParams)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return string.Empty;
                }

                var translationItem = GetTranslationItem(key, defaultValue);

                if (translationItem == null)
                {
                    return string.Empty;
                }

                var fieldRenderer = new FieldRenderer
                {
                    Item = translationItem,
                    FieldName = Constants.DictionaryEntryPhraseFieldName,
                };

                var result = fieldRenderer.Render();

                if (formatParams != null && formatParams.Length > 0 && !m_SiteContext.IsPageEditor)
                {
                    return string.Format(result, formatParams);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error during translating the key '{0}'. {1}", key, ex.Message), ex, this);

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the translation item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Item.</returns>
        public virtual Item GetTranslationItem(string key, string defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            var nestedInFolders = string.Empty;
            if (key.Contains("/"))
            {
                nestedInFolders = key.Substring(0, key.LastIndexOf("/"));
                key = key.Substring(key.LastIndexOf("/") + 1, key.Length - key.LastIndexOf("/") - 1);
            }

            if (string.IsNullOrEmpty(defaultValue))
            {
                defaultValue = key;
            }

            Logger.ExtraInfo(string.Format("Getting translation item for key '{0}' with default value '{1}')", key, defaultValue), this);

            if (ModuleSettings.LocalizationCreateItemsWithDefaultValues)
            {
                return !m_DictionaryCache.DictionaryEntryExists(key)
                    ? CreateDictionaryEntryWithDefaultValue(key, defaultValue, nestedInFolders)
                    : GetDictionaryEntry(key);
            }

            return GetDictionaryEntry(key);
        }

        #region Private

        protected Item GetDictionaryEntry(string key)
        {
            var itemId = m_DictionaryCache.GetCache(key);
            if (itemId != Guid.Empty)
            {
                return m_SiteContext.ContextDb.GetItem(new ID(itemId));
            }

            return null;
        }

        protected virtual Item CreateDictionaryEntryWithDefaultValue(string key, string defaultValue, string nestedInFolders)
        {
            using (new SecurityDisabler())
            {
                var targetDictionaryRoot = EnsureDictionaryFoldersCreated(nestedInFolders);

                var targetDictionaryEntryItemName = GenerateItemName(key);
                var createdDictionaryEntry = m_SiteContext.MasterDb.GetItem(targetDictionaryRoot.Paths.Path + "/" + targetDictionaryEntryItemName);
                
                if (createdDictionaryEntry == null)
                {
                    createdDictionaryEntry = targetDictionaryRoot.Add(targetDictionaryEntryItemName, Constants.DictionaryEntryTemplateId);
                }
                
                if (createdDictionaryEntry.Versions.Count == 0)
                {
                    createdDictionaryEntry = createdDictionaryEntry.Versions.AddVersion();
                }

                try
                {
                    createdDictionaryEntry.Editing.BeginEdit();
                    createdDictionaryEntry[Constants.DictionaryEntryKeyFieldName] = key;
                    createdDictionaryEntry[Constants.DictionaryEntryPhraseFieldName] = defaultValue;
                    createdDictionaryEntry.Editing.EndEdit();

                    EnsureDictionaryTemplateHasCorrectIcon(createdDictionaryEntry.Template.InnerItem, "Applications/32x32/font.png");

                    Logger.Info(string.Format(
                        "Dictionary entry '{0}' with ID {1} created with default value.",
                        createdDictionaryEntry.Paths.Path,
                        createdDictionaryEntry.ID.Guid), this);
                }
                catch(Exception ex)
                {
                    Logger.Error(string.Format("Error editing dictionary entry item {0}.", createdDictionaryEntry.ID.Guid), ex, this);

                    createdDictionaryEntry.Editing.CancelEdit();
                }

                m_DictionaryCache.SetCache(key, createdDictionaryEntry.ID.Guid);
                m_SiteContext.Publish(createdDictionaryEntry);

                return createdDictionaryEntry;
            }
        }

        protected Item EnsureDictionaryFoldersCreated(string translationGroupPath)
        {
            var currentRoot = m_SiteContext.GetDictionaryRoot(m_SiteContext.MasterDb);

            foreach (var folderName in translationGroupPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var safeFolderName = GenerateItemName(folderName);
                var targetDictionaryGroupPath = string.Format("{0}/{1}", currentRoot.Paths.Path, safeFolderName);
                var targetFolder = m_SiteContext.MasterDb.GetItem(targetDictionaryGroupPath);

                if (targetFolder == null)
                {
                    currentRoot = currentRoot.Add(safeFolderName, Constants.DictionaryFolderTemplateId);

                    Logger.Info(string.Format(
                            "Dictionary folder '{0}' with ID {1} created.",
                            currentRoot.Paths.Path,
                            currentRoot.ID.Guid), this);

                    EnsureDictionaryTemplateHasCorrectIcon(currentRoot.Template.InnerItem, "People/32x32/book_yellow.png");

                    m_SiteContext.Publish(currentRoot);
                }
                else
                {
                    currentRoot = targetFolder;
                }
            }

            return currentRoot;
        }

        protected void EnsureDictionaryTemplateHasCorrectIcon(Item createdDictionaryTemplate, string iconFilename)
        {
            var needPublish = false;
            try
            {
                createdDictionaryTemplate.Editing.BeginEdit();
                if (string.IsNullOrEmpty(createdDictionaryTemplate["__Icon"]))
                {
                    createdDictionaryTemplate["__Icon"] = iconFilename;
                    needPublish = true;
                }
                createdDictionaryTemplate.Editing.EndEdit();

                if (needPublish)
                {
                    m_SiteContext.Publish(createdDictionaryTemplate);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error setting the icon to template '{0}'", createdDictionaryTemplate.ID.Guid), ex, this);

                createdDictionaryTemplate.Editing.CancelEdit();
            }
        }

        protected string GenerateItemName(string key)
        {
            key = key.Replace(" ", "-");
            var itemName = key
                .Where(c => c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '_' || c == '-' || c == '/')
                .Aggregate(string.Empty, (current, c) => current + c);

            if (string.IsNullOrEmpty(itemName))
            {
                itemName = string.Format("DictionaryEntry_{0}", key.GetHashCode());
            }

            Logger.ExtraInfo(string.Format("Item name has been generated for key '{0}' as '{1}'", key, itemName), this);

            return itemName;
        }

        #endregion
    }
}
