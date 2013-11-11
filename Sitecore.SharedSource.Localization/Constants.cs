using System;
using Sitecore.Data;

namespace Sitecore.SharedSource.Localization
{
    internal static class Constants
    {
        public static TemplateID DictionaryFolderTemplateId = new TemplateID(new ID(new Guid("{267D9AC7-5D85-4E9D-AF89-99AB296CC218}")));
        public static TemplateID DictionaryEntryTemplateId = new TemplateID(new ID(new Guid("{6D1CD897-1936-4A3A-A511-289A94C2A7B1}")));
        public static TemplateID DictionaryRootTemplateId = new TemplateID(new ID(new Guid("{239F9CF4-E5A0-44E0-B342-0F32CD4C6D8B}")));

        public static ID DictionaryEntryPhraseFieldId = new ID(new Guid("{2BA3454A-9A9C-4CDF-A9F8-107FD484EB6E}"));
        public static ID DictionaryEntryKeyFieldId = new ID(new Guid("{580C75A8-C01A-4580-83CB-987776CEB3AF}"));

        public const string DICTIONARY_CACHE_NAME = "Sitecore.SharedSource.Localization";
        public const string DICTIONARY_ENTRY_PHRASE_FIELD_NAME = "Phrase";
    }
}
