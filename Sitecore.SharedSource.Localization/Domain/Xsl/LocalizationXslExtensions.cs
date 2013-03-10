using Sitecore.Xml.Xsl;

namespace Sitecore.SharedSource.Localization.Domain.Xsl
{
    public class LocalizationXslExtensions : XslHelper
    {
        internal ITranslator m_Translator = new Translator();

        #region Implementation of ITranslator

        public string Text(string key)
        {
            return m_Translator.Text(key);
        }

        public string Text(string key, string defaultValue)
        {
            return m_Translator.Text(key, defaultValue);
        }

        public string TextF(string key, string formatParam1)
        {
            return m_Translator.TextF(key, new[] { formatParam1 });
        }

        public string TextF(string key, string formatParam1, string formatParam2)
        {
            return m_Translator.TextF(key, new[] { formatParam1, formatParam2 });
        }

        public string TextF(string key, string formatParam1, string formatParam2, string formatParam3)
        {
            return m_Translator.TextF(key, new[] { formatParam1, formatParam2, formatParam3 });
        }

        public string TextF(string key, string formatParam1, string formatParam2, string formatParam3, string formatParam4)
        {
            return m_Translator.TextF(key, new[] { formatParam1, formatParam2, formatParam3, formatParam4 });
        }

        public string TextF(string key, string formatParam1, string formatParam2, string formatParam3, string formatParam4, string formatParam5)
        {
            return m_Translator.TextF(key, new[] { formatParam1, formatParam2, formatParam3, formatParam4, formatParam5 });
        }
        
        public string TextDF(string key, string defaultValue, string formatParam1)
        {
            return m_Translator.TextF(key, defaultValue, new[] { formatParam1 });
        }

        public string TextDF(string key, string defaultValue, string formatParam1, string formatParam2)
        {
            return m_Translator.TextF(key, defaultValue, new[] { formatParam1, formatParam2 });
        }

        public string TextDF(string key, string defaultValue, string formatParam1, string formatParam2, string formatParam3)
        {
            return m_Translator.TextF(key, defaultValue, new[] { formatParam1, formatParam2, formatParam3 });
        }

        public string TextDF(string key, string defaultValue, string formatParam1, string formatParam2, string formatParam3, string formatParam4)
        {
            return m_Translator.TextF(key, defaultValue, new[] { formatParam1, formatParam2, formatParam3, formatParam4 });
        }

        public string TextDF(string key, string defaultValue, string formatParam1, string formatParam2, string formatParam3, string formatParam4, string formatParam5)
        {
            return m_Translator.TextF(key, defaultValue, new[] { formatParam1, formatParam2, formatParam3, formatParam4, formatParam5 });
        }

        #endregion
    }
}
