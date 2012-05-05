using System.Collections.Generic;
using System.Runtime.Serialization;
using MusicBrowser.Engines.ViewState;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Home : Virtual
    {
        public Home()
        {
            Path = "home";
        }

        public override string Title
        {
            get
            {
                string title = Util.Config.GetInstance().GetStringSetting("Entity.Home.Format");
                if (string.IsNullOrEmpty(title))
                {
                    return "MusicBrowser 2";
                }
                return TokenSubstitution(title);
            }
            set { }
        }

        public override IViewState ViewState
        {
            get
            {
                IViewState view = base.ViewState;
                view.DefaultSort = "[SortOrder:sort]";
                view.DefaultView = "Thumb";
                return view;
            }
        }

        public override List<string> SortFields
        {
            get
            {
                return new List<string>
                           {
                               "SortOrder",
                               "Title",
                               "Filename"
                           };
            }
        }
    }
}
