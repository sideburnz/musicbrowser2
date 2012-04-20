using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using MusicBrowser.Engines.PlayState;
using MusicBrowser.Engines.ViewState;

namespace MusicBrowser.Entities
{
    [DataContract]
    public abstract class Container : baseEntity
    {
        private IViewState _viewState;

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

        public override IViewState ViewState
        {
            get
            {
                return _viewState ?? (_viewState = new ContainerViewState(CacheKey)
                                                       {
                                                           DefaultSort = "[Title:sort]",
                                                           DefaultView = "List"
                                                       });
            }
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
