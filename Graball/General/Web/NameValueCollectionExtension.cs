using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Graball.General.Web
{
    /// <summary>
    /// Extensão para: NameValueCollection
    /// </summary>
    public static class NameValueCollectionExtension
    {
        /// <summary>
        /// Retorna o conteúdo como query string.
        /// </summary>
        public static string GetQuerystring(this NameValueCollection values)
        {
            var result = new StringBuilder();
            foreach (string key in values)
            {
                if (values[key] != null && values[key].Contains(","))
                {
                    foreach (var value in values[key].Split(','))
                    {
                        if (result.Length > 0) { result.Append("&"); }
                        result.Append(string.Format("{0}={1}", key, value));
                    }                        
                }
                else
                {
                    if (result.Length > 0) { result.Append("&"); }
                    result.Append(string.Format("{0}={1}", key, values[key]));
                }                    
            };
            return result.ToString();
        }
    }
}
