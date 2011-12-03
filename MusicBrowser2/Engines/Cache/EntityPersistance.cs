using MusicBrowser.Entities;
using ServiceStack.Text;

namespace MusicBrowser.Engines.Cache
{
    public static class EntityPersistance
    {
        public static Entity Deserialize(string typename, string data)
        {
            //switch (typename.ToLower())
            //{
            //    case "track":
            //        return JsonSerializer.DeserializeFromString<Track>(data);
            //    case "album":
            //        return JsonSerializer.DeserializeFromString<Album>(data);
            //}
            //return null;
            return XmlSerializer.DeserializeFromString<Entity>(data);
        }

        public static string Serialize(Entity graph)
        {
            //return graph.ToJson();
            return graph.ToXml();
        }
    }
}