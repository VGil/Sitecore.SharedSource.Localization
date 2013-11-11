using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SharedSource.Localization.Domain;

namespace Sitecore.SharedSource.Localization.Infrastructure.Caching
{
    /// <summary>
    /// Class SitecoreDictionaryCacheManager
    /// </summary>
    internal class SitecoreDictionaryCacheManager
    {
        protected SiteContext m_SiteContext = new SiteContext();

        /// <summary>
        /// Gets the cached item id.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Guid.</returns>
        public virtual Guid GetCache(string key)
        {
            EnsureCacheIsNotEmpty();

            var result = GetItemIdFromCache(key);

            if (result == Guid.Empty)
            {
                ReloadWholeDictionaryCache();

                result = GetItemIdFromCache(key);
            }

            return result;
        }

        /// <summary>
        /// Specifies whether the dictionary entry exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the dictionary entry exists, <c>false</c> otherwise</returns>
        public virtual bool DictionaryEntryExists(string key)
        {
            var cachedItemId = GetCache(key);
            Item dictionaryItem = null;
            if (cachedItemId != Guid.Empty)
            {
                dictionaryItem = m_SiteContext.ContextDb.GetItem(new ID(cachedItemId));
            }

            return dictionaryItem != null && dictionaryItem.Versions.Count > 0;
        }

        /// <summary>
        /// Sets the item id into the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">Item id</param>
        public virtual void SetCache(string key, Guid value)
        {
            SitecoreDictionaryCache.Instance.SetString(BuildCacheKey(key), value.ToString());
        }

        /// <summary>
        /// Gets a value indicating whether dictionary entry cache empty.
        /// </summary>
        /// <value><c>true</c> if dictionary entry cache empty; otherwise, <c>false</c>.</value>
        public bool IsCacheEmpty
        {
            get { return SitecoreDictionaryCache.Instance.InnerCache.Count == 0; }
        }

        #region Private

        protected virtual Guid GetItemIdFromCache(string key)
        {
            var result = Guid.Empty;

            var value = SitecoreDictionaryCache.Instance.GetString(BuildCacheKey(key));

            if (!string.IsNullOrEmpty(value))
            {
                result = Guid.Parse(value);
            }

            if (result == Guid.Empty)
            {
                Logger.ExtraInfo(string.Format("Can't find cached item Id for translation key '{0}'.", key), this);
            }

            return result;
        }

        protected virtual string BuildCacheKey(string key)
        {
            var translationsRoot = m_SiteContext.DictionaryRoot.Paths.Path;
            if (!string.IsNullOrEmpty(translationsRoot))
            {
                return string.Format("{0}@{1}", translationsRoot, key);
            }

            return key;
        }

        protected virtual void ReloadWholeDictionaryCache()
        {
            Logger.ExtraInfo("Reloading whole dictionary cache for context website...", this);
            Logger.ExtraInfo(string.Format("Context site name was resolved as '{0}'", Context.GetSiteName()), this);
            Logger.ExtraInfo(string.Format(
                    "Dictionary root for site name '{0}' was resolved to Item '{1}' ({2})",
                    m_SiteContext.GetSiteName(),
                    m_SiteContext.DictionaryRoot.Paths.Path,
                    m_SiteContext.DictionaryRoot.ID.Guid),
                this);

            ReloadWholeDictionaryCache(m_SiteContext.DictionaryRoot, new Dictionary<string, string>());

            Logger.Info(string.Format("Dictionary cache have been reloaded. Website '{0}'. Dictionary root '{1}'.",
                    m_SiteContext.GetSiteName(),
                    m_SiteContext.DictionaryRoot.Paths.Path),
                this);

            Logger.ExtraInfo(string.Format("Context database is set to '{0}'", m_SiteContext.ContextDb.Name), this);
            Logger.ExtraInfo(string.Format("IsPageEditor property has been resolved as '{0}'", m_SiteContext.IsPageEditor), this);
        }

        protected virtual void ReloadWholeDictionaryCache(Item dictionaryRoot, IDictionary<string, string> keyList)
        {
            foreach (Item dictionaryItem in dictionaryRoot.Children)
            {
                if (dictionaryItem.TemplateID == Constants.DictionaryFolderTemplateId)
                {
                    ReloadWholeDictionaryCache(dictionaryItem, keyList);
                }

                if (dictionaryItem.TemplateID == Constants.DictionaryEntryTemplateId)
                {
                    var key = dictionaryItem[Constants.DictionaryEntryKeyFieldId];
                    if (!string.IsNullOrEmpty(key))
                    {
                        if (keyList.ContainsKey(key))
                        {
                            var duplicatedKeyItem = GetCache(key);

                            Logger.Warn(string.Format(
                                    "Dictionary contains entries with duplicated keys. Duplicated key name: '{0}', duplicated key items: '{1}' ({2}), '{3}' ({4}). The last found item's value will be used.",
                                    key,
                                    dictionaryItem.Paths.Path,
                                    dictionaryItem.ID.Guid,
                                    duplicatedKeyItem == Guid.Empty ? Guid.Empty.ToString() : dictionaryRoot.Database.GetItem(new ID(duplicatedKeyItem)).Paths.Path,
                                    duplicatedKeyItem == Guid.Empty ? Guid.Empty.ToString() : dictionaryRoot.Database.GetItem(new ID(duplicatedKeyItem)).ID.Guid.ToString()),
                                this);
                        }
                        else
                        {
                            keyList.Add(key, key);
                        }

                        SetCache(key, dictionaryItem.ID.Guid);
                    }
                }
            }
        }

        protected virtual void EnsureCacheIsNotEmpty()
        {
            if (IsCacheEmpty)
            {
                ReloadWholeDictionaryCache();
            }
        }

        #endregion
    }
}
