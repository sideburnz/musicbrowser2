using MusicBrowser.Entities;
using ServiceStack.Text;
using System;

namespace MusicBrowser.Engines.Cache
{
    public static class EntityPersistance
    {

        public static string Serialize(Entity data)
        {
            return data.ToXml();
        }

        public static Entity Deserialize(string data)
        {
            if (String.IsNullOrEmpty(data)) { return null; }

            try
            {
                return XmlSerializer.DeserializeFromString<Entity>(data);
            }
            catch
            {
                return null;
            }
        }
    }
}