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

namespace MusicBrowser.Engines.Metadata
{
    public class MediaInfoProvider : baseMetadataProvider
    {
        private static int _state = 0;

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);

        #region abstract class implementation
        public MediaInfoProvider()
        {
            Name = "MediaInfoProvider";
            MinDaysBetweenHits = 60;
            MaxDaysBetweenHits = 365;
            RefreshPercentage = 5;
        }

        public override bool CompatibleWith(baseEntity dto)
        {
            return (Helper.InheritsFrom<Track>(dto) || Helper.InheritsFrom<Video>(dto));
        }

        public override bool AskKillerQuestions(Entities.baseEntity dto)
        {
            if (!CompatibleWith(dto)) { return false; }
            if (!Enabled()) { return false; }

            return true;
        }
        #endregion

        public override ProviderOutcome DoWork(baseEntity dto)
        {
            if (Helper.InheritsFrom<Track>(dto))
            {
                if (DoWorkMusic((Track)dto)) 
                { 
                    return ProviderOutcome.Success; 
                }
                return ProviderOutcome.SystemError;
            }
            if (Helper.InheritsFrom<Video>(dto))
            {
                if (DoWorkVideo((Video)dto)) 
                { 
                    return ProviderOutcome.Success; 
                }
                return ProviderOutcome.SystemError;
            }
            return ProviderOutcome.InvalidInput;
        }

        private bool Is64Bit
        {
            get { return IntPtr.Size == 8; }
        }

        private bool Enabled()
        {
            if (_state == 0) // locking throws errors on plug-ins, this doesn't solve or fully replace that
            {
                _state = -1;
                if (CheckForLib())
                {
                    _state = 1;
                }
            }
            return (_state == 1);
        }

        private bool CheckForLib()
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

        private bool DoWorkMusic(Track dto)
        {
            bool ret = true;

            try
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
                }
                mediaInfo.Close();
            }
            catch 
            {
                ret = false;
            }
            return ret;
        }

        private long FileSize(string filename)
        {
            FileInfo f = new FileInfo(filename);
            return f.Length;
        }

        private bool DoWorkVideo(Video dto)
        {
            bool ret = true;
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

                    Logging.LoggerEngineFactory.Info("DVD path: " + path);

                    if (!File.Exists(path))
                    {
                        path = string.Empty;
                    }
                }
                else
                {
                    // if it's a folder, get the details from the first item we find that's a video file
                    IEnumerable<FileSystemItem> items = FileSystemProvider.GetAllSubPaths(dto.Path);
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
                    return false;
                }
            }

            try
            {
                MediaInfo mediaInfo = new MediaInfo();
                int i = mediaInfo.Open(path);
                if (i != 0)
                {
                    int j = 0;

                    // general
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

                    dto.Subtitles = !String.IsNullOrEmpty(mediaInfo.Get(StreamKind.Text, 0, "Format"));

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
            catch
            {
                ret = false;
            }
            return ret;

        }
    }
}
