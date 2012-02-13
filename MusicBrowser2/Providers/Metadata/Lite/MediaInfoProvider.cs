using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaInfoLib;
using System.Runtime.InteropServices;
using System.IO;
using MusicBrowser.Util;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Engines.Logging;

namespace MusicBrowser.Providers.Metadata.Lite
{
    public class MediaInfoProvider
    {
        private static int _state = 0;
        private static object _lock = new object();

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);

        public static void FetchLite(baseEntity dto)
        {
            try
            {
                if (!Enabled()) { throw new NotSupportedException("MediaInfo.dll missing"); }
                if (dto.InheritsFrom<Track>())
                {
                    DoWorkMusic((Track)dto);
                }
                if (dto.InheritsFrom<Video>())
                {
                    DoWorkVideo((Video)dto);
                }
            }
            catch (Exception ex)
            {
                LoggerEngineFactory.Error(ex);
            }
        }

        private static bool Is64Bit
        {
            get { return IntPtr.Size == 8; }
        }

        private static bool Enabled()
        {
            if (_state == 0)
            {
                _state = -1;
                lock (_lock)
                {
                    if (CheckForLib())
                    {
                        _state = 1;
                    }
                }
            }
            return (_state == 1);
        }

        private static bool CheckForLib()
        {
            string mediaInfoPath = Path.Combine(Helper.ComponentFolder, "MediaInfo.dll");

            if (!File.Exists(mediaInfoPath))
            {
                string sourceFile = Path.Combine(Helper.ComponentFolder, String.Format("mediainfo.dll.{0}", Is64Bit ? 64 : 32));
                if (!File.Exists(sourceFile)) { return false; }
                File.Copy(sourceFile, mediaInfoPath);
            }

            if (File.Exists(mediaInfoPath))
            {
                var handle = LoadLibrary(mediaInfoPath);
                return handle != IntPtr.Zero;
            }
            return false;
        }

        private static void DoWorkMusic(Track dto)
        {
            MediaInfo mediaInfo = new MediaInfo();
            int i = mediaInfo.Open(dto.Path);
            if (i != 0)
            {
                // sample rate
                dto.SampleRate = mediaInfo.Get(StreamKind.Audio, 0, "SamplingRate/String");

                // channels
                int audioChannels;
                if (Int32.TryParse(mediaInfo.Get(StreamKind.Audio, 0, "Channel(s)"), out audioChannels))
                {
                    switch (audioChannels)
                    {
                        case 1: dto.Channels = "Mono"; break;
                        case 2: dto.Channels = "Stereo"; break;
                        default: dto.Channels = audioChannels + " channels"; break;
                    }
                }

                // sample resolution
                dto.Resolution = mediaInfo.Get(StreamKind.Audio, 0, "Resolution/String");

                // track rating
                int Rating;
                if (Int32.TryParse(mediaInfo.Get(StreamKind.General, 0, "Rating"), out Rating))
                {
                    dto.Rating = Rating;
                }

                dto.Title = mediaInfo.Get(StreamKind.General, 0, "Track");
                dto.Album = mediaInfo.Get(StreamKind.General, 0, "Album");
                //TODO: if accompaniment is blank use performer
                dto.Artist = mediaInfo.Get(StreamKind.General, 0, "Accompaniment");
                dto.Genre = mediaInfo.Get(StreamKind.General, 0, "Genre");
                dto.Codec = mediaInfo.Get(StreamKind.Audio, 0, "Format").ToLower();
                int pos;
                if (Int32.TryParse(mediaInfo.Get(StreamKind.General, 0, "Track/Position"), out pos))
                {
                    dto.TrackNumber = pos;
                }
                int duration;
                if (Int32.TryParse(mediaInfo.Get(StreamKind.General, 0, "Duration"), out duration))
                {
                    if (duration > 0)
                    {
                        dto.Duration = (int)(duration / 1000.00);
                    }
                    else
                    {
                        dto.Duration = duration;
                    }
                }
                else
                {
                    dto.Duration = 0;
                }


                //track.AlbumArtist = fileTag.Tag.FirstAlbumArtist; // albumartistsort
                //track.ReleaseDate = Convert.ToDateTime("01-JAN-" + fileTag.Tag.Year); //  Recorded_Date
                //track.DiscNumber = Convert.ToInt32(fileTag.Tag.Disc);   // Part
                //track.MusicBrainzID = fileTag.Tag.MusicBrainzTrackId;  // musicbrainz_trackid
                //track.ThumbPath = iconPath;


            }
            mediaInfo.Close();
        }

        private static long FileSize(string filename)
        {
            FileInfo f = new FileInfo(filename);
            return f.Length;
        }

        private static void DoWorkVideo(Video dto)
        {
            string path = string.Empty;

            if (Directory.Exists(dto.Path))
            {
                if (Helper.IsDVD(dto.Path))
                {
                    IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(dto.Path);
                    FileSystemItem selected = items
                        .Where(item => item.Name.ToLower().EndsWith(".vob"))
                        .OrderBy(item => FileSize(item.FullPath))
                        .Reverse()
                        .FirstOrDefault();
                    path = selected.FullPath.Replace(".vob", ".ifo");

                    LoggerEngineFactory.Info("DVD path: " + path);

                    if (!File.Exists(path))
                    {
                        path = string.Empty;
                    }
                }
                else
                {
                    // if it's a folder, get the details from the first item we find that's a video file
                    IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(dto.Path).FilterDVDFiles();
                    foreach (FileSystemItem item in items)
                    {
                        if (Helper.getKnownType(item) == Helper.knownType.Video)
                        {
                            path = item.FullPath;
                            continue;
                        }
                    }
                }

                if (string.IsNullOrEmpty(path))
                {
                    return;
                }
            }
            else
            {
                path = dto.Path;
            }

            MediaInfo mediaInfo = new MediaInfo();
            int i = mediaInfo.Open(path);
            if (i != 0)
            {
                int j = 0;

                // general
                dto.Subtitles = !String.IsNullOrEmpty(mediaInfo.Get(StreamKind.Text, 0, "Format"));
                dto.Container = mediaInfo.Get(StreamKind.General, 0, "Format");
                if (Int32.TryParse(mediaInfo.Get(StreamKind.General, 0, "Duration"), out j))
                {
                    if (j > 0)
                    {
                        dto.Duration = (int)(j / 1000.00);
                    }
                    else
                    {
                        dto.Duration = j;
                    }
                }
                else
                {
                    dto.Duration = 0;
                }

                // video
                dto.VideoCodec = mediaInfo.Get(StreamKind.Video, 0, "Format");
                dto.AspectRatio = mediaInfo.Get(StreamKind.Video, 0, "DisplayAspectRatio/String");

                j = 0;
                if (Int32.TryParse(mediaInfo.Get(StreamKind.Video, 0, "Height"), out j))
                {
                    dto.HiDef = j > 719;
                }

                //audio
                dto.AudioCodec = mediaInfo.Get(StreamKind.Audio, 0, "Format");
                if (Int32.TryParse(mediaInfo.Get(StreamKind.Audio, 0, "Channel(s)"), out j))
                {
                    dto.AudioChannels = j;
                }
                else
                {
                    dto.AudioChannels = 0;
                }

                // track rating
                if (Int32.TryParse(mediaInfo.Get(StreamKind.General, 0, "Rating"), out j))
                {
                    dto.Rating = j;
                }
                else
                {
                    dto.Rating = 0;
                }
            }
            mediaInfo.Close();
        }
    }
}
