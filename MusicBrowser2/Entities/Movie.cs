using System.Runtime.Serialization;
using ServiceStack.Text;

namespace MusicBrowser.Entities
{
    [DataContract]
    public class Movie : Video
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imageVideo"; }
        }

        public override string Information
        {
            get
            {
                return Title + "  " + base.Information;
            }
        }

        public override string Serialize()
        {
            return this.ToJson();
        }
    }
}
