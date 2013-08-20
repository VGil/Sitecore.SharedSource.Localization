using Sitecore.Globalization;
using Sitecore.SharedSource.Localization.Domain;

namespace Sitecore.SharedSource.Localization
{
    /// <summary>
    /// Class Translator
    /// </summary>
    public class Translator : ITranslator
    {
        internal TranslatorService _translatorService = new TranslatorService();

	    /// <summary>
	    /// Gets translated phrase for context language by specified translation key.
	    /// Dictionary entry item will be created if it doesn't exist with default phrase value equals to specified key.
	    /// </summary>
	    /// <param name="key">The translation key.</param>
	    /// <param name="language"></param>
	    /// <returns>System.String.</returns>
	    public virtual string Text(string key, Language language = null)
        {
			return _translatorService.Text(key, null, language, null);
        }

	    /// <summary>
	    /// Gets translated phrase for context language by specified translation key.
	    /// Dictionary entry item will be created if it doesn't exist with default phrase value
	    /// equals to passed parameter (or it will be equal to translation key in case default value is null or empty).
	    /// </summary>
	    /// <param name="key">The key.</param>
	    /// <param name="defaultValue">The default value.</param>
	    /// <param name="language"></param>
	    /// <returns>System.String.</returns>
	    public virtual string Text(string key, string defaultValue, Language language = null)
        {
			return _translatorService.Text(key, defaultValue, language, null);
        }

        /// <summary>
        /// Gets translated phrase for context language by specified translation key.
        /// With option of formatting like string.Format() method functionality.
        /// Dictionary entry item will be created if it doesn't exist with default phrase value equals to specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="formatParams">The format params.</param>
        /// <returns>System.String.</returns>
        public virtual string TextF(string key, params object[] formatParams)
        {
			return _translatorService.Text(key, null, null, formatParams);
        }

	    /// <summary>
	    /// Gets translated phrase for context language by specified translation key.
	    /// With option of formatting like string.Format() method functionality.
	    /// Dictionary entry item will be created if it doesn't exist with default phrase value equals to specified key.
	    /// </summary>
	    /// <param name="key">The key.</param>
	    /// <param name="language"></param>
	    /// <param name="formatParams">The format params.</param>
	    /// <returns>System.String.</returns>
	    public virtual string TextF(string key, Language language, params object[] formatParams)
		{
			return _translatorService.Text(key, null, language, formatParams);
		}

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
        public virtual string TextF(string key, string defaultValue, object[] formatParams)
        {
            return _translatorService.Text(key, defaultValue, null, formatParams);
        }

	    /// <summary>
	    /// Gets translated phrase for context language by specified translation key.
	    /// With option of formatting like string.Format() method functionality.
	    /// Dictionary entry item will be created if it doesn't exist with default phrase value
	    /// equals to passed parameter (or it will be equal to translation key in case default value is null or empty).
	    /// </summary>
	    /// <param name="key">The key.</param>
	    /// <param name="language"></param>
	    /// <param name="defaultValue">The default value.</param>
	    /// <param name="formatParams">The format params.</param>
	    /// <returns>System.String.</returns>
		public virtual string TextF(string key, string defaultValue, Language language, object[] formatParams)
		{
			return _translatorService.Text(key, defaultValue, language, formatParams);
		}
    }
}
