using MusicBrowser.Entities;
using ServiceStack.Text;

namespace MusicBrowser.Engines.Cache
{
    public static class EntityPersistance
    {
        public static baseEntity Deserialize(string typename, string data)
        {
            switch (typename.ToLower())
            {
                case "virtual":
                    return JsonSerializer.DeserializeFromString<Virtual>(data);
            }
            return null;
        }

        public static string Serialize(baseEntity graph)
        {
            return graph.ToJson();
        }
    }
}