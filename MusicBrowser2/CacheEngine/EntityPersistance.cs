using System;
using System.IO;
using System.Text;
using System.Xml;
using MusicBrowser.Entities;
using MusicBrowser.Util;
using MusicBrowser.Entities.Kinds;


namespace MusicBrowser.CacheEngine
{
    public static class EntityPersistance
    {
        private static readonly string AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Serializes an Entity using XmlWriter
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Serialize(IEntity data)
        {
            StringBuilder sb = new StringBuilder();
            TextWriter text = new StringWriter(sb);
            XmlWriter writer = new XmlTextWriter(text);

            writer.WriteStartElement("EntityXML");

            writer.WriteAttributeString("type", data.Kind.ToString());
            writer.WriteAttributeString("version", AppVersion);

            writer.WriteElementString("Path", data.Path);                       // this isn't actually read from the cache
            writer.WriteElementString("Title", data.Title);
            writer.WriteElementString("IconPath", data.IconPath);
            writer.WriteElementString("BackgroundPath", data.BackgroundPath);

            writer.WriteElementString("MusicBrainzID", data.MusicBrainzID);

            writer.WriteElementString("TrackName", data.TrackName);
            writer.WriteElementString("ArtistName", data.ArtistName);
            writer.WriteElementString("AlbumArtist", data.AlbumArtist);
            writer.WriteElementString("AlbumName", data.AlbumName);

            writer.WriteElementString("TrackNumber", data.TrackNumber.ToString());
            writer.WriteElementString("DiscNumber", data.DiscNumber.ToString());
            writer.WriteElementString("DiscId", data.DiscId);
            writer.WriteElementString("ReleaseDate", data.ReleaseDate.ToString("yyyy-MM-dd"));

            writer.WriteElementString("Listeners", data.Listeners.ToString());
            writer.WriteElementString("TotalPlays", data.TotalPlays.ToString());
            writer.WriteElementString("PlayCount", data.PlayCount.ToString());

            writer.WriteElementString("Codec", data.Codec);
            writer.WriteElementString("Channels", data.Channels);
            writer.WriteElementString("SampleRate", data.SampleRate);
            writer.WriteElementString("Resolution", data.Resolution);
            writer.WriteElementString("Duration", data.Duration.ToString());
            writer.WriteElementString("Rating", data.Rating.ToString());

            if (data.Performers != null) { writer.WriteElementString("Performers", String.Join("|", data.Performers.ToArray())); }
            if (data.Genres != null) { writer.WriteElementString("Genres", String.Join("|", data.Genres.ToArray())); }

            writer.WriteElementString("Favorite", data.Favorite.ToString());
            writer.WriteElementString("Summary", data.Summary);
            writer.WriteElementString("Lyrics", data.Lyrics);

            // used to stop providers from fetching data every time they're invoked
            writer.WriteStartElement("ProviderTimeStamps");
            foreach (string key in data.ProviderTimeStamps.Keys)
            {
                writer.WriteStartElement("Provider");
                writer.WriteAttributeString("name", key);
                writer.WriteAttributeString("timestamp", data.ProviderTimeStamps[key].ToString("yyyy-MM-dd"));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndElement(); //EntityXML 
            writer.Close();

            return sb.ToString();
        }

        public static IEntity Deserialize(string data)
        {
            IEntity entity;

            try
            {
                XmlDocument xml = new XmlDocument();

                xml.LoadXml(data);

                EntityKind kind = EntityKindParse(Helper.ReadXmlNode(xml, "EntityXML/@type"));
                switch (kind)
                {
                    case EntityKind.Album:
                        {
                            entity = new Album();
                            break;
                        }
                    case EntityKind.Artist:
                        {
                            entity = new Artist();
                            break;
                        }
                    case EntityKind.Folder:
                        {
                            entity = new Folder();
                            break;
                        }
                    case EntityKind.Home:
                        {
                            entity = new Home();
                            break;
                        }
                    case EntityKind.Playlist:
                        {
                            entity = new Playlist();
                            break;
                        }
                    case EntityKind.Song:
                        {
                            entity = new Song();
                            break;
                        }
                    default:
                        {
                            throw new Exception("unknown type");
                        }
                }

                string ver = Helper.ReadXmlNode(xml, "EntityXML/@version");
                if (!String.IsNullOrEmpty(ver))
                {
                    entity.Version = Helper.ParseVersion(ver);
                }

                // string reads
                entity.Title = Helper.ReadXmlNode(xml, "EntityXML/Title");
                entity.Summary = Helper.ReadXmlNode(xml, "EntityXML/Summary");
                entity.MusicBrainzID = Helper.ReadXmlNode(xml, "EntityXML/MusicBrainzID");
                entity.TrackName = Helper.ReadXmlNode(xml, "EntityXML/TrackName");
                entity.ArtistName = Helper.ReadXmlNode(xml, "EntityXML/ArtistName");
                entity.AlbumArtist = Helper.ReadXmlNode(xml, "EntityXML/AlbumArtist");
                entity.AlbumName = Helper.ReadXmlNode(xml, "EntityXML/AlbumName");
                entity.Lyrics = Helper.ReadXmlNode(xml, "EntityXML/Lyrics");
                entity.Codec = Helper.ReadXmlNode(xml, "EntityXML/Codec");
                entity.Channels = Helper.ReadXmlNode(xml, "EntityXML/Channels");
                entity.SampleRate = Helper.ReadXmlNode(xml, "EntityXML/SampleRate");
                entity.Resolution = Helper.ReadXmlNode(xml, "EntityXML/Resolution");
                entity.DiscId = Helper.ReadXmlNode(xml, "EntityXML/DiscId");

                //  number reads
                int i;
                int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Duration"), out i); entity.Duration = i;
                int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Listeners"), out i); entity.Listeners = i;
                int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/TotalPlays"), out i); entity.TotalPlays = i;
                int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/PlayCount"), out i); entity.PlayCount = i;
                int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Duration"), out i); entity.Duration = i;
                int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Rating"), out i); entity.Rating = i;
                int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/TrackNumber"), out i); entity.TrackNumber = i;
                int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/DiscNumber"), out i); entity.DiscNumber = i;

                // date reads
                DateTime d;
                DateTime.TryParse(Helper.ReadXmlNode(xml, "EntityXML/ReleaseDate"), out d); entity.ReleaseDate = d;

                // path reads
                string backgroundImage = Helper.ReadXmlNode(xml, "EntityXML/BackgroundPath");
                    if (File.Exists(backgroundImage)) { entity.BackgroundPath = backgroundImage; }
                string thumbImage = Helper.ReadXmlNode(xml, "EntityXML/IconPath"); 
                    if (File.Exists(thumbImage)) { entity.IconPath = thumbImage; }

                // boolean reads
                entity.Favorite = Helper.ReadXmlNode(xml, "EntityXML/Favorite").ToLower() == "true";

                // list reads
                entity.Performers.AddRange(Helper.ReadXmlNode(xml, "ExntityXML/Performers").Split('|'));
                entity.Genres.AddRange(Helper.ReadXmlNode(xml, "ExntityXML/Genres").Split('|'));

                // complex reads
                foreach (XmlNode node in xml.SelectNodes("/EntityXML/ProviderTimeStamps/Provider"))
                {
                    entity.ProviderTimeStamps[node.Attributes["name"].InnerText] = Convert.ToDateTime(node.Attributes["timestamp"].InnerText);
                }
            }
            catch
            {
                // there's been a problem
                return new Unknown();
            }
            return entity;
        }

        /// <summary>
        /// EntityKind is a enum, this parse method converts string to an EntityKind
        /// </summary>
        /// <param name="value">value to parse</param>
        /// <returns>EntityKind</returns>
        public static EntityKind EntityKindParse(string value)
        {
            switch (value.ToLower())
            {
                case "album":
                    return EntityKind.Album;
                case "artist":
                    return EntityKind.Artist;
                case "folder":
                    return EntityKind.Folder;
                case "home":
                    return EntityKind.Home;
                case "playlist":
                    return EntityKind.Playlist;
                case "song":
                    return EntityKind.Song;
            }
            return EntityKind.Unknown;
        }
    }
}