using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Cache
{
    public static class EntityPersistance
    {
        public static Entity Deserialize(byte[] data)
        {
            try
            {
                MemoryStream stream = new MemoryStream(data);
                BinaryFormatter formatter = new BinaryFormatter();
                return (Entity)formatter.Deserialize(stream);
            }
            catch
            {
                return null;
            }
        }

        public static byte[] Serialize(Entity graph)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream reader = new MemoryStream();
            formatter.Serialize(reader, graph);
            return reader.ToArray();
        }
    }
}