using System;
using System.IO;
using System.Text;
using System.Xml;
using MusicBrowser.Util;
using MusicBrowser.Entities.Kinds;

namespace MusicBrowser.Entities
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

            writer.WriteElementString("Title", data.Title);
            writer.WriteElementString("Summary", data.Summary);
            writer.WriteElementString("Duration", data.Duration.ToString());
            writer.WriteElementString("MusicBrainzID", data.MusicBrainzID);

            writer.WriteStartElement("Images");
            writer.WriteElementString("Background", data.BackgroundPath);
            writer.WriteElementString("Icon", data.IconPath);
            writer.WriteEndElement(); //Images

            writer.WriteStartElement("Properties");
            foreach (string key in data.Properties.Keys)
            {
                writer.WriteStartElement("Item");
                writer.WriteAttributeString("key", key);
                writer.WriteAttributeString("value", data.Properties[key]);
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); //EntityXML 
            writer.WriteEndElement(); //Properties 
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

                // complex reads
                int i;
                int.TryParse(Helper.ReadXmlNode(xml, "EntityXML/Duration"), out i);
                entity.Duration = i;
                string ver = Helper.ReadXmlNode(xml, "EntityXML/@version");
                if (!String.IsNullOrEmpty(ver)) { entity.Version = Helper.ParseVersion(ver); }

                // simple reads
                entity.Title = Helper.ReadXmlNode(xml, "EntityXML/Title");
                entity.Summary = Helper.ReadXmlNode(xml, "EntityXML/Summary");
                entity.MusicBrainzID = Helper.ReadXmlNode(xml, "EntityXML/MusicBrainzID");

                // conditional reads
                string bg = Helper.ReadXmlNode(xml, "EntityXML/Images/Background");
                if (File.Exists(bg)) { entity.BackgroundPath = bg; }
                string ip = Helper.ReadXmlNode(xml, "EntityXML/Images/Icon");
                if (File.Exists(ip)) { entity.IconPath = ip; }

                // compound reads
                foreach (XmlNode node in xml.SelectNodes("/EntityXML/Properties/Item"))
                {
                    entity.SetProperty(node.Attributes["key"].InnerText, node.Attributes["value"].InnerText, true);
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