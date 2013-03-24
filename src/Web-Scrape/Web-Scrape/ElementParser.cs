using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Web_Scrape
{
    public class ElementParser
    {
        public IEnumerable<T> FindElements<T>(string elementName, string html) where T : IElement
        {
            var list = new List<T>();

            var properties = typeof (T).GetProperties();
            var elementMatches = Regex.Matches(html, string.Format(@"(<{0}.*?>.*?</{0}>)", elementName), RegexOptions.Singleline);
            foreach (Match elementMatch in elementMatches)
            {
                var elementMatchValue = elementMatch.Groups[1].Value;
                var t = (T) Activator.CreateInstance(typeof (T));
                foreach (var property in properties)
                {
                    if (property.Name == "Text")
                    {
                        var textValue = Regex.Replace(elementMatchValue, @"\s*<.*?>\s*", string.Empty, RegexOptions.Singleline);
                        property.SetValue(t, textValue, null);
                        continue;
                    }
                    var attributeMatches = Regex.Match(elementMatchValue, string.Format(@"{0}=((\""(.*?)\"")|('(.*?)'))", property.Name), RegexOptions.Singleline);
                    if (attributeMatches.Success)
                    {
                        foreach (Group g in attributeMatches.Groups)
                        {
                            if (!g.Success ||
                                g.Value.StartsWith("'") ||
                                g.Value.StartsWith(("\"")))
                                continue;
                            property.SetValue(t, g.Value, null);
                        }
                    }
                }
                list.Add(t);
            }

            return list;
        }
    }
}
