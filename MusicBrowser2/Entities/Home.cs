using System.Runtime.Serialization;

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

        protected override string DefaultView
        {
            get { return "Thumb"; }
        }

        protected override string DefaultSort
        {
            get { return "[SortOrder:sort]"; }
        }
    }
}
