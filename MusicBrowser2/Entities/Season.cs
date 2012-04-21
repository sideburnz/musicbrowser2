using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MusicBrowser.Engines.ViewState;
using ServiceStack.Text;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Season : Folder
    {
        public override IViewState ViewState
        {
            get
            {
                IViewState view = base.ViewState;
                view.DefaultSort = "[Episode#:sort]";
                return view;
            }
        }

        public override string Information
        {
            get
            {
                string head = String.Empty;
                if (!String.IsNullOrEmpty(Show))
                {
                    head = Show + ",";
                }
                return CalculateInformation(head, "Episode");
            }
        }

        [DataMember]
        public string Show { get; set; }

        public override string Serialize()
        {
            return this.ToJson();
        }

        public override List<string> SortFields
        {
            get
            {
                return new List<string>
                           {
                               "Episode#",
                               "Title",
                               "Filename",
                               "Added",
                               "Duration"
                           };
            }
        }
    }
}
