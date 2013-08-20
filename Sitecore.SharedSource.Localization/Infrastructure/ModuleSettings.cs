namespace Sitecore.SharedSource.Localization.Infrastructure
{
    public class ModuleSettings
    {
        /// <summary>
        /// Gets the sitecore setting size for the max cache size.
        /// </summary>
        /// <value>The sitecore setting size for the max cache size.</value>
        public static long LocalizationMaxCacheSize
        {
            get { return StringUtil.ParseSizeString(Configuration.Settings.GetSetting("Localization.MaxCacheSize", "10MB")); }
        }

        /// <summary>
        /// Gets the sitecore setting for format params delimiter.
        /// </summary>
        /// <value>The sitecore setting for format params delimiter.</value>
        public static string LocalizationFormatParamsDelimiter
        {
            get { return Configuration.Settings.GetSetting("Localization.FormatParamsDelimiter", "|"); }
        }

        /// <summary>
        /// Gets a value indicating whether extra logging enabled.
        /// </summary>
        /// <value><c>true</c> if extra logging enabled; otherwise, <c>false</c>.</value>
        public static bool LocalizationEnableExtraLogging
        {
            get { return System.Convert.ToBoolean(Configuration.Settings.GetSetting("Localization.EnableExtraLogging", "False")); }
        }

        /// <summary>
        /// The localization create items with default values
        /// </summary>
        public static bool CreateItemsWithDefaultValues
        {
            get { return System.Convert.ToBoolean(Configuration.Settings.GetSetting("Localization.CreateItemsWithDefaultValues", "True")); }
        }

        /// <summary>
        /// Gets the global dictionary folder.
        /// </summary>
        /// <value>
        /// The global dictionary folder.
        /// </value>
        public static string GlobalDictionaryFolder
        {
            get { return Configuration.Settings.GetSetting("Localization.GlobalDictionaryFolder", "/sitecore/system/Dictionary"); }
        }

        /// <summary>
        /// Gets the site specific dictionary folder.
        /// </summary>
        /// <param name="siteName">Name of the site.</param>
        /// <returns></returns>
        public static string GetSiteSpecificDictionaryFolder(string siteName)
        {
            return Configuration.Settings.GetSetting(string.Format("Localization.{0}.DictionaryFolder", siteName), null);
        }

		/// <summary>
		/// Determines whether to publish automatically created items with default values.
		/// </summary>
		/// <value>
		/// <c>true</c> if [auto publish created items]; otherwise, <c>false</c>.
		/// </value>
		public static bool AutoPublishCreatedItems
	    {
			get { return Configuration.Settings.GetBoolSetting("Localization.AutoPublishCreatedItems", true); }
	    }
    }
}
