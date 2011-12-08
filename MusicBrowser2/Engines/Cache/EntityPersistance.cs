using MusicBrowser.Entities;
using ServiceStack.Text;

namespace MusicBrowser.Engines.Cache
{
    public static class EntityPersistance
    {
        public static baseEntity Deserialize(string typename, string data)
        {
            switch (typename)
            {
                 case "Album":
                    return JsonSerializer.DeserializeFromString<Album>(data);
                case "Artist":
                    return JsonSerializer.DeserializeFromString<Artist>(data); 
                case "Folder":
                    return JsonSerializer.DeserializeFromString<Folder>(data); 
                case "Genre":
                    return JsonSerializer.DeserializeFromString<Genre>(data);
                case "PhotoAlbum":
                    return JsonSerializer.DeserializeFromString<Gallery>(data); 
                case "Season":
                    return JsonSerializer.DeserializeFromString<Season>(data);
                case "Show":
                    return JsonSerializer.DeserializeFromString<Show>(data);

                case "Collection":
                    return JsonSerializer.DeserializeFromString<Collection>(data);
                case "VideoCollection":
                    return JsonSerializer.DeserializeFromString<VideoCollection>(data);
                case "MusicCollection":
                    return JsonSerializer.DeserializeFromString<MusicCollection>(data);
                case "PhotoCollection":
                    return JsonSerializer.DeserializeFromString<PhotoCollection>(data);

                case "Playlist":
                    return JsonSerializer.DeserializeFromString<Playlist>(data);
                case "Track":
                    return JsonSerializer.DeserializeFromString<Track>(data);

                case "Episode":
                    return JsonSerializer.DeserializeFromString<Episode>(data); 
                case "Movie":
                    return JsonSerializer.DeserializeFromString<Movie>(data); 

                case "Photo":
                    return JsonSerializer.DeserializeFromString<Photo>(data);

            }
            return null;
        }

        public static string Serialize(baseEntity graph)
        {
            return graph.ToJson();
        }
    }
}