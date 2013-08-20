using Sitecore.Caching;

namespace Sitecore.SharedSource.Localization.Infrastructure.Caching
{
    internal class SitecoreDictionaryCache : CustomCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreDictionaryCache" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="maxSize">Size of the max.</param>
        public SitecoreDictionaryCache(string name, long maxSize) : base(name, maxSize)
        {
        }

        /// <summary>
        /// Sets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public new void SetString(string key, string value)
        {
            base.SetString(key, value);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public new string GetString(string key)
        {
            return base.GetString(key);
        }

        #region Singleton

        private static readonly object m_SyncRoot = new object();
        private static SitecoreDictionaryCache m_Instance;

        public static SitecoreDictionaryCache Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (m_SyncRoot)
                    {
                        if (m_Instance == null)
                        {
                            m_Instance = new SitecoreDictionaryCache(Constants.DICTIONARY_CACHE_NAME, ModuleSettings.LocalizationMaxCacheSize);
                        }
                    }
                }

                return m_Instance;
            }
        }

        #endregion
    }
}
