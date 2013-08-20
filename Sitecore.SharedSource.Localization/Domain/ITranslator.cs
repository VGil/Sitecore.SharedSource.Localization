using Sitecore.Globalization;

namespace Sitecore.SharedSource.Localization.Domain
{
    /// <summary>
    /// Basic Interface ITranslator. This should be used for implementation on any 
    /// custom translator provider if you will want to rebuild existing logic.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Gets translated phrase for context language by specified translation key.
        /// Dictionary entry item will be created if it doesn't exist with default phrase value equals to specified key.
        /// </summary>
        /// <param name="key">The translation key.</param>
        /// <returns>System.String.</returns>
        string Text(string key, Language language = null);

        /// <summary>
        /// Gets translated phrase for context language by specified translation key.
        /// Dictionary entry item will be created if it doesn't exist with default phrase value 
        /// equals to passed parameter (or it will be equal to translation key in case default value is null or empty).
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.String.</returns>
		string Text(string key, string defaultValue, Language language = null);


        /// <summary>
        /// Gets translated phrase for context language by specified translation key. 
        /// With option of formatting like string.Format() method functionality.
        /// Dictionary entry item will be created if it doesn't exist with default phrase value equals to specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="formatParams">The format params.</param>
        /// <returns>System.String.</returns>
        string TextF(string key, params object[] formatParams);

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
        string TextF(string key, string defaultValue, object[] formatParams);

	    /// <summary>
	    /// Gets translated phrase for context language by specified translation key. 
	    /// With option of formatting like string.Format() method functionality.
	    /// Dictionary entry item will be created if it doesn't exist with default phrase value equals to specified key.
	    /// </summary>
	    /// <param name="key">The key.</param>
	    /// <param name="language"></param>
	    /// <param name="formatParams">The format params.</param>
	    /// <returns>System.String.</returns>
	    string TextF(string key, Language language, params object[] formatParams);

	    /// <summary>
	    /// Gets translated phrase for context language by specified translation key.
	    /// With option of formatting like string.Format() method functionality.
	    /// Dictionary entry item will be created if it doesn't exist with default phrase value 
	    /// equals to passed parameter (or it will be equal to translation key in case default value is null or empty).
	    /// </summary>
	    /// <param name="key">The key.</param>
	    /// <param name="defaultValue">The default value.</param>
	    /// <param name="language"></param>
	    /// <param name="formatParams">The format params.</param>
	    /// <returns>System.String.</returns>
	    string TextF(string key, string defaultValue, Language language, object[] formatParams);
    }
}
