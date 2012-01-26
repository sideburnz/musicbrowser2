using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using MusicBrowser.Interfaces;
using MusicBrowser.Util;
using System.Text.RegularExpressions;

namespace MusicBrowser.Providers.Metadata.Lite
{
    class VideoFilenameMetadataProvider
    {
        private static readonly Regex[] episodeExpressions = new Regex[] {
            new Regex(@"(?:.*[^\\])\\(?<seriesname>.*[^\\])\\(?:.*[^\\])\\[s|S](?<seasonnumber>\d{1,2})x?[e|E](?<episodenumber>\d{1,3})\W*(?<episodename>.*)\.(?:.*)"), // S01E02 blah.avi, S01xE01 blah.avi
            new Regex(@"(?:.*[^\\])\\(?<seriesname>.*[^\\])\\(?:.*[^\\])\\[s|S](?<seasonnumber>\d{1,2})x?[e|E](?<episodenumber>\d{1,3})\W*(?<episodename>.*)")          // DVDs
        };

        public static void FetchLite(baseEntity entity)
        {
            #region killer questions
            if (!entity.InheritsFrom<Episode>()) { return; }
            #endregion

            Episode episode = (Episode)entity;

            foreach (Regex r in episodeExpressions)
            {
                Match m = r.Match(entity.Path);
                if (m.Success)
                {
                    int i = 0;
                    if (int.TryParse(m.Groups["episodenumber"].Value, out i))
                    {
                        episode.EpisodeNumber = i;
                    }
                    if (int.TryParse(m.Groups["seasonnumber"].Value, out i))
                    {
                        episode.SeasonNumber = i;
                    }
                    episode.Title = m.Groups["episodename"].Value.Trim();
                    if (String.IsNullOrEmpty(episode.Title))
                    {
                        episode.Title = System.IO.Path.GetFileNameWithoutExtension(episode.Path);
                    }
                    episode.ShowName = m.Groups["seriesname"].Value.Trim();

                    entity = episode;
                    return;
                }
            }
        }
    }
}
