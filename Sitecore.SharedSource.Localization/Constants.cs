using System;
using Sitecore.Data;

namespace Sitecore.SharedSource.Localization
{
    internal static class Constants
    {
        public static TemplateID DictionaryFolderTemplateId = new TemplateID(new ID(new Guid("{267D9AC7-5D85-4E9D-AF89-99AB296CC218}")));
        public static TemplateID DictionaryEntryTemplateId = new TemplateID(new ID(new Guid("{6D1CD897-1936-4A3A-A511-289A94C2A7B1}")));
        public static TemplateID DictionaryRootTemplateId = new TemplateID(new ID(new Guid("{239F9CF4-E5A0-44E0-B342-0F32CD4C6D8B}")));

        public const string DictionaryEntryPhraseFieldName = "Phrase";
        public const string DictionaryEntryKeyFieldName = "Key";

        public const string DictionaryCacheName = "Sitecore.SharedSource.Localization";
    }
}
