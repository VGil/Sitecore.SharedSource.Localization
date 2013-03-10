using System;
using System.Xml;
using Sitecore.SharedSource.Localization.Infrastructure;
using Sitecore.Web.UI.XslControls;
using Sitecore.Xml;
using Sitecore.Xml.Xsl;

namespace Sitecore.SharedSource.Localization.Domain.Xsl.Controls
{
    public class LocalizationXslControl : XslControl
    {
        private const string DictionaryKeyAttributeName = "key";
        private const string DictionaryDefaultValueAttributeName = "defaultValue";
        private const string DictionaryFormatParamsAttributeName = "formatParams";

        public override XmlNode PreprocessControl(XmlNode controlNode, IXslControlContainer container)
        {
            if (controlNode.Attributes != null)
            {
                var key          = GetAttributeValue(DictionaryKeyAttributeName, controlNode);
                var defaultValue = GetAttributeValue(DictionaryDefaultValueAttributeName, controlNode);
                var formatParams = GetAttributeValue(DictionaryFormatParamsAttributeName, controlNode);

                if (string.IsNullOrEmpty(key))
                {
                    return null;
                }

                var element = XmlUtil.CreateElement("value-of", "xsl", "http://www.w3.org/1999/XSL/Transform", controlNode.OwnerDocument);

                var selectStatement = BuildSelectStatement(key, formatParams, defaultValue);

                XmlUtil.SetAttribute("select", selectStatement, element);
                XmlUtil.SetAttribute("disable-output-escaping", "yes", element);

                if (controlNode.ParentNode != null)
                {
                    controlNode.ParentNode.ReplaceChild(element, controlNode);
                }

                if (element == null)
                {
                    return null;
                }

                return element;
            }

            return null;
        }

        private static string BuildSelectStatement(string key, string formatParams, string defaultValue)
        {
            var function = "Text";
            var functionParams = "'" + key + "'";

            if (!string.IsNullOrEmpty(formatParams))
            {
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    function = "TextDF";
                    functionParams += ", " + "'" + defaultValue + "'";
                }
                else
                {
                    function = "TextF";
                }

                var splittedFormatParams = formatParams.Split(
                    new [] {ModuleSettings.LocalizationFormatParamsDelimiter}, 
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (var formatParam in splittedFormatParams)
                {
                    functionParams += ", " + "'" + formatParam + "'";
                }
            }
            else if (!string.IsNullOrEmpty(defaultValue))
            {
                functionParams += ", " + "'" + defaultValue + "'";
            }

            return "sc:" + function + "(" + functionParams + ")";
        }

        private static string GetAttributeValue(string attrName, XmlNode controlNode)
        {
            return XmlUtil.GetAttribute(attrName, controlNode).Trim('\'').Trim('"').Trim('\'');
        }
    }
}