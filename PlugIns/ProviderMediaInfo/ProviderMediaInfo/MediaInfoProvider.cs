﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaInfoLib;
using System.Runtime.InteropServices;
using System.IO;
using MusicBrowser.Util;
using MusicBrowser.Entities;

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

        public override bool CompatibleWith(string type)
        {
            switch (type.ToLower())
            {
                case "track":
                case "episode":
                case "video":
                    return true;
                default:
                    return false;
            }
        }

        public override bool AskKillerQuestions(Entities.baseEntity dto)
        {
            if (!CompatibleWith(dto.Kind)) { return false; }
            if (!System.IO.File.Exists(dto.Path)) { return false; }
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
                    string audioSampleRate = mediaInfo.Get(StreamKind.Audio, 0, "SamplingRate/String");
                    dto.SampleRate = audioSampleRate;

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
                    string audioResolution = mediaInfo.Get(StreamKind.Audio, 0, "Resolution/String");
                    dto.Resolution = audioResolution;

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

        private bool DoWorkVideo(baseEntity dto)
        {
            return true;
        }
    }
}