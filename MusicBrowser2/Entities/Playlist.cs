using System.Runtime.Serialization;
using ServiceStack.Text;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Playlist : Music
    {
        public override string DefaultThumbPath
        {
            get { return "resx://MusicBrowser/MusicBrowser.Resources/imagePlaylist"; }
        }

        public override string Serialize()
        {
            return this.ToJson();
        }
    }
}
