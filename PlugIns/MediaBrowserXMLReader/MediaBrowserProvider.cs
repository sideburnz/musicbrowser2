using System;
using System.IO;
using System.Linq;
using System.Xml;
using MusicBrowser.Entities;
using MusicBrowser.Util;

namespace MusicBrowser.Engines.Metadata
{
    public class MediaBrowserProvider : baseMetadataProvider
    {
        public MediaBrowserProvider()
        {
            Name = "MediaBrowserProvider";
            MinDaysBetweenHits = 28;
            MaxDaysBetweenHits = 365;
            RefreshPercentage = 5;
        }

        public override bool CompatibleWith(baseEntity dto)
        {
            return (dto.InheritsFrom<Movie>()
                || dto.InheritsFrom<Episode>()
                || dto.InheritsFrom<Show>());
        }

        protected override bool AskKillerQuestions(baseEntity dto)
        {
            if (!CompatibleWith(dto)) { return false; }
            return true;
        }

        protected override ProviderOutcome DoWork(baseEntity dto)
        {
            if (dto.InheritsFrom<Show>())
            {
                if (DoWorkShow((Show)dto))
                {
                    return ProviderOutcome.Success;
                }
                return ProviderOutcome.SystemError;
            }
            if (dto.InheritsFrom<Episode>())
            {
                if (DoWorkEpisode((Episode)dto))
                {
                    return ProviderOutcome.Success;
                }
                return ProviderOutcome.SystemError;
            }
            if (dto.InheritsFrom<Movie>())
            {
                if (DoWorkMovie((Movie)dto))
                {
                    return ProviderOutcome.Success;
                }
                return ProviderOutcome.SystemError;
            }

            return ProviderOutcome.InvalidInput;
        }

        private bool DoWorkShow(Show dto)
        {
            string datafile = Path.Combine(dto.Path, "series.xml");
            if (!File.Exists(datafile)) { return false; }

            try
            {
                XmlDocument data = new XmlDocument();
                data.Load(datafile);

                dto.Overview = Helper.ReadXmlNode(data, @"Series/Overview");
                dto.Title = Helper.ReadXmlNode(data, @"Series/SeriesName");

                double r;
                if (Double.TryParse(Helper.ReadXmlNode(data, @"Series/Rating"), out r))
                {
                    dto.Rating = (int)(r * 10);
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        private bool DoWorkEpisode(Episode dto)
        {
            string datapath;
            string datafile;

            if (File.Exists(dto.Path))
            {
                datapath = Path.Combine(Path.GetDirectoryName(dto.Path), "metadata");
                datafile = Path.Combine(datapath, Path.GetFileNameWithoutExtension(dto.Path) + ".xml");
            }
            else
            {
                datapath = Path.Combine(Directory.GetParent(dto.Path).FullName, "metadata");
                datafile = Path.Combine(datapath, Path.GetFileName(dto.Path) + ".xml");
            }

            if (!File.Exists(datafile)) { return false; }

            try
            {
                XmlDocument data = new XmlDocument();
                data.Load(datafile);

                dto.Overview = Helper.ReadXmlNode(data, @"Item/Overview");
                dto.Title = Helper.ReadXmlNode(data, @"Item/EpisodeName", dto.Title);

                string s = Helper.ReadXmlNode(data, @"Item/EpisodeNumber", dto.EpisodeNumber.ToString());
                int i;
                if (Int32.TryParse(s, out i))
                {
                    dto.EpisodeNumber = i;
                }

                s = Helper.ReadXmlNode(data, @"Item/FirstAired", dto.ReleaseDate.ToString("yyyy-nn-dd"));
                DateTime d;
                if (DateTime.TryParse(s, out d))
                {
                    dto.ReleaseDate = d;
                }

                s = Helper.ReadXmlNode(data, @"Item/filename", dto.ThumbPath);
                if (s.Contains('/'))
                {
                    s = s.Substring(s.LastIndexOf('/') + 1);
                }
                s = Path.Combine(datapath, s);
                if (File.Exists(s))
                {
                    dto.ThumbPath = s;
                }

                double r;
                if (Double.TryParse(Helper.ReadXmlNode(data, @"Item/Rating"), out r))
                {
                    dto.Rating = (int)(r * 10);
                }

                return true;
            }
            catch
            {
            }

            return false;
        }

        private bool DoWorkMovie(Movie dto)
        {
            string datapath = Directory.Exists(dto.Path) ? dto.Path : Path.GetDirectoryName(dto.Path);

            // mymovies.xml is being phased out - look for movie.xml first
            string datafile = Path.Combine(datapath, "movie.xml");
            if (!File.Exists(datafile))
            {
                datafile = Path.Combine(datapath, "mymovies.xml");
            }

            if (!File.Exists(datafile)) { return false; }

            try
            {
                XmlDocument data = new XmlDocument();
                data.Load(datafile);

                dto.Overview = Helper.ReadXmlNode(data, @"Title/Description");
                dto.Title = Helper.ReadXmlNode(data, @"Title/LocalTitle", dto.Title);

                string s = Helper.ReadXmlNode(data, @"Title/ProductionYear", dto.ReleaseDate.Year.ToString()) + "-01-01";
                DateTime d;
                if (DateTime.TryParse(s, out d))
                {
                    dto.ReleaseDate = d;
                }

                double r;
                if (Double.TryParse(Helper.ReadXmlNode(data, @"Title/IMDBrating"), out r))
                {
                    dto.Rating = (int)(r * 10);
                }

                return true;
            }
            catch
            {
            }

            return false;
        }
    }
}
