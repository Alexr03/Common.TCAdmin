using TCAdmin.SDK.Database;

namespace Alexr03.Common.TCAdmin.Extensions
{
    public static class XmlFieldExtensions
    {
        public static bool HasValueAndSet(this XmlField xmlField, string key)
        {
            return xmlField.HasValue(key) && !string.IsNullOrEmpty(xmlField[key].ToString());
        }
    }
}