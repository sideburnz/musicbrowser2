using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Collection : Virtual
    {
        public string SortOrder { get; set; }

        public override string TokenSubstitution(string input)
        {
            string output = input;

            Regex regex = new Regex("\\[.*?\\]");
            foreach (Match matches in regex.Matches(input))
            {
                string token = matches.Value.Substring(1, matches.Value.Length - 2);
                switch (token)
                {
                    case "sortorder":
                    case "SortOrder":
                    case "sortorder:sort":
                    case "SortOrder:sort":
                        output = output.Replace("[" + token + "]", SortOrder); break;
                }
            }

            return base.TokenSubstitution(output);
        }
    }
}
