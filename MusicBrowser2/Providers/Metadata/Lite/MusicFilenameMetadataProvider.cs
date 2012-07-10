using System;
using System.IO;
using System.Text.RegularExpressions;
using MusicBrowser.Entities;

namespace MusicBrowser.Providers.Metadata.Lite
{
    static class MusicFilenameMetadataProvider
    {
        private static readonly Regex[] TrackExpressions = new[] {
            new Regex(@"^(?<tracknumber>\d{1,3})[:\-\W]+(?<artist>.*)\W[:\-]+\W(?<trackname>.*)\.(?:.*)"), // 00T - Artist - Track Name
            new Regex(@"^(?<tracknumber>\d{1,3})[:\-\W]+(?<trackname>.*)\.(?:.*)"), // 00T - Track Name
            new Regex(@"^(?<discnumber>\d{2,2})(?<tracknumber>\d{2,2})[:\-\W]+(?<artist>.*)\W[:\-]+\W(?<trackname>.*)\.(?:.*)"), // 0D0T - Artist - Track Name
            new Regex(@"^(?<discnumber>\d{2,2})(?<tracknumber>\d{2,2})[:\-\W]+(?<trackname>.*)\.(?:.*)"), // 0D0T - Track Name
            new Regex(@"^(?<discnumber>\d{1,2})?\.(?<tracknumber>\d{1,2})[:\-\W]+(?<trackname>.*)\.(?:.*)")  // 0D.0T - Track Name
        };

        public static void FetchLite(baseEntity entity)
        {
            #region killer questions
            if (!entity.InheritsFrom<Track>()) { return; }
            #endregion

            Track track = (Track)entity;
            string filename = Path.GetFileName(track.Path);


            foreach (Regex r in TrackExpressions)
            {
                Match m = r.Match(filename);
                if (m.Success)
                {
                    int i;
                    if (int.TryParse(m.Groups["discnumber"].Value, out i))
                    {
                        track.DiscNumber = i;
                    }
                    if (int.TryParse(m.Groups["tracknumber"].Value, out i))
                    {
                        track.TrackNumber = i;
                    }
                    track.Artist = m.Groups["artist"].Value.Trim();
                    track.Title = m.Groups["trackname"].Value.Trim();
                    if (String.IsNullOrEmpty(track.Title))
                    {
                        track.Title = Path.GetFileNameWithoutExtension(filename);
                    }
                    return;
                }
            }
        }
    }
}
