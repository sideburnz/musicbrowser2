using System;
using System.Text.RegularExpressions;
using MusicBrowser.Entities;

namespace MusicBrowser.Providers.Metadata.Lite
{
    class MusicFilenameMetadataProvider
    {
        private static readonly Regex[] TrackExpressions = new[] {
            new Regex(@"\\(?<tracknumber>\d{1,2})\-\W*(?<trackname>.*)\.(?:.*)"), // 0T - Track Name
            new Regex(@"\\(?<discnumber>\d{2,2})(?<tracknumber>\d{2,2})\-\W*(?<trackname>.*)\.(?:.*)"), // 0D0T - Track Name
            new Regex(@"\\(?<discnumber>\d{1,2})?\.(?<tracknumber>\d{1,2})\-\W*(?<trackname>.*)\.(?:.*)")  // 0D.0T - Track Name
        };

        public static void FetchLite(baseEntity entity)
        {
            #region killer questions
            if (!entity.InheritsFrom<Track>()) { return; }
            #endregion

            Track track = (Track)entity;

            foreach (Regex r in TrackExpressions)
            {
                Match m = r.Match(entity.Path);
                if (m.Success)
                {
                    int i;
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

                    return;
                }
            }
        }
    }
}
