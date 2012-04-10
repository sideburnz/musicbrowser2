using System.Collections.Generic;
using System.Linq;

namespace MusicBrowser.Util
{
    public class Localization
    {
        public Localization()
        {
        }

        private static readonly Dictionary<string, string> Langs = new Dictionary<string, string>
                                                             { 
                { "en", "English" },
                { "fr", "French" },
                { "de", "German" },
                { "it", "Italian" },
                { "jp", "Japanese" },
                { "ru", "Russian" },
                { "es", "Spanish" }
            };

        public List<string> LanguageNames
        {
            get
            {
                return Langs.Values.ToList();
            }            
        }

        static public string LanguageCodeToName(string code)
        {
            if (Langs.ContainsKey(code))
            {
                return Langs[code];
            }
            return string.Empty;
        }

        static public string LanguageNameToCode(string name)
        {
            if (Langs.ContainsValue(name))
            {
                foreach (string key in Langs.Keys)
                {
                    if (Langs[key] == name)
                    {
                        return key;
                    }
                }
            }
            return string.Empty;
        }
    }
}
