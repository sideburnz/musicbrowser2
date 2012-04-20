using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using MusicBrowser.Engines.PlayState;

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class Container : baseEntity
    {
        [DataMember]
        public Dictionary<string, int> Children
        {
            get
            {
                return _children;
            }
            set
            {
                if (value != _children)
                {
                    _children = value;
                    DataChanged("Children");
                }
            }
        }
        private Dictionary<string, int> _children = new Dictionary<string, int>();

        protected override string DefaultView
        {
            get { return "List"; }
        }

        protected override string DefaultSort
        {
            get { return "[Title:sort]"; }
        }

        public override bool Playable
        {
            get { return false; }
        }

        protected string CalculateInformation(string header, params string[] fields)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string field in fields)
            {
                if (Children.ContainsKey(field))
                {
                    if (Children[field] == 1)
                    {
                        sb.Append(String.Format("{0} {1}  ", Children[field], field));
                    }
                    else
                    {
                        sb.Append(String.Format("{0} {1}s  ", Children[field], field));
                    }
                }
            }

            if (sb.Length == 0)
            {
                if (String.IsNullOrEmpty(header)) 
                { 
                    return Kind; 
                }
                return header;
            }
            if (Duration > 0)
            {
                if (String.IsNullOrEmpty(header))
                {
                    return sb + "(" + TokenSubstitution("[Duration]") + ")";
                }
                return header + "  " + sb + "(" + TokenSubstitution("[Duration]") + ")";
            }
            if (String.IsNullOrEmpty(header))
            {
                return sb.ToString();
            }
            return header + "  " + sb;

        }

        public override IPlayState PlayState
        {
            get { return new BlankPlayState(); }
        }
    }
}
