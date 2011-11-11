using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MusicBrowser.Entities;
using MusicBrowser.Util;
using MusicBrowser.Interfaces;
using System.IO;

namespace MusicBrowser.Providers.Metadata
{
    class MetadataFileProvider : IDataProvider
    {
        private const string Name = "MetadataFileProvider";

        private const int MinDaysBetweenHits = 7;
        private const int MaxDaysBetweenHits = 14;
        private const int RefreshPercentage = 25;

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);


        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
#if DEBUG
            Engines.Logging.LoggerEngineFactory.Verbose(Name + ": " + dto.Path, "start");
#endif
            dto.Outcome = DataProviderOutcome.Success;

            #region killer questions

            FileSystemItem metadataFile = MetadataPath(dto.Path);
            if (String.IsNullOrEmpty(metadataFile.Name))
            {
                dto.Outcome = DataProviderOutcome.NoData;
                dto.Errors = new List<string>() { "no metadata file" };
                return dto;
            }
            if (dto.ProviderTimeStamps.ContainsKey(Name))
            {
            if (metadataFile.LastUpdated < dto.ProviderTimeStamps[Name])
            {
                dto.Outcome = DataProviderOutcome.NoData;
                dto.Errors = new List<string>() { "no change to metadata" };
                return dto;
            }
            }

            #endregion

            Statistics.Hit(Name + ".hit");

            // this currently only supports overriding the automatically determined types
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(metadataFile.FullPath);
                switch (Helper.ReadXmlNode(xml, "EntityXML/@type").ToLower())
                {
                    case "album":
                        dto.DataType = DataTypes.Album; break;
                    case "artist":
                        dto.DataType = DataTypes.Artist; break;
                    case "genre":
                        dto.DataType = DataTypes.Genre; break;
                }
            }
            catch { }

            return dto;
        }

        public string FriendlyName()
        {
            return Name;
        }

        public bool CompatibleWith(string type)
        {
            return true;
        }

        public bool isStale(DateTime lastAccess)
        {
            return true;
        }

        public ProviderType Type
        {
            get { return ProviderType.Core; }
        }

        // works out where the metadata file is (if there is one)
        private static FileSystemItem MetadataPath(string item)
        {
            string itemName = Path.GetFileNameWithoutExtension(item);
            string metadataPath = Directory.GetParent(item).FullName;
            FileSystemItem metadataFile;

            string metadataLocal = metadataPath + "\\" + itemName + "\\metadata.xml";
            metadataFile = FileSystemProvider.GetItemDetails(metadataLocal);
            if (!String.IsNullOrEmpty(metadataFile.Name))
            {
                return metadataFile;
            }
            string metadataInParent = metadataPath + "\\metadata\\" + itemName + ".xml";
            metadataFile = FileSystemProvider.GetItemDetails(metadataInParent);
            // this either returns detail or an empty struct which would indicate not found
            return metadataFile;
        }
    }
}
