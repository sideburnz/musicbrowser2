using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using MediaInfoLib;
using MusicBrowser.Interfaces;
using MusicBrowser.Util;

namespace MusicBrowser.Providers.Metadata
{
    class MediaInfoProvider : IDataProvider
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);

        private const string Name = "MediaInfo.dll";
        private static int _state = 0;
        private static object obj = new object();

        private const int MinDaysBetweenHits = 180;
        private const int MaxDaysBetweenHits = 360;
        private const int RefreshPercentage = 25;

        private static readonly Random Rnd = new Random(DateTime.Now.Millisecond);

        public string FriendlyName() { return Name; }

        public DataProviderDTO Fetch(DataProviderDTO dto)
        {
#if DEBUG
            Logging.Logger.Verbose(Name + ": " + dto.Path, "start");
#endif
            dto.Outcome = DataProviderOutcome.Success;

            #region killer questions

            if (!Enabled())
            {
                dto.Outcome = DataProviderOutcome.SystemError;
                dto.Errors = new List<string> { "Not Enabled: " + Name };
                return dto;
            }

            if (dto.DataType != DataTypes.Track)
            {
                dto.Outcome = DataProviderOutcome.InvalidInput;
                dto.Errors = new List<string> { "Not a track: " + dto.Path };
                return dto;
            }

            #endregion

            Statistics.Hit(Name + ".hit");

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
                    if (Int32.TryParse(mediaInfo.Get(StreamKind.General, 0, "Rating"), out Rating)) { dto.Rating = Rating; }

                    // label (sony/bmg etc)
                    string label = mediaInfo.Get(StreamKind.General, 0, "Label");
                    dto.Label = label;
                }
                mediaInfo.Close();
            }
            catch (Exception e)
            {
                Logging.Logger.Error(e);
                dto.Outcome = DataProviderOutcome.SystemError;
                dto.Errors = new List<string> { "Error retrieving data" };
            }
            return dto;
        }

        private static bool Is64Bit
        {
            get
            {
                return IntPtr.Size == 8;
            }
        }

        public bool CompatibleWith(string type)
        {
            return (type.ToLower() == "track");
        }

        /// <summary>
        /// refresh requests between the min and max refresh period have 10% chance of refreshing
        /// </summary>
        private static bool RandomlyRefreshData(DateTime stamp)
        {
            // if it's never refreshed, refresh it
            if (stamp < DateTime.Parse("01-JAN-1000")) { return true; }

            // if it's less then the min, don't refresh if it's older than the max then do refresh
            int dataAge = (DateTime.Today.Subtract(stamp)).Days;
            if (dataAge <= MinDaysBetweenHits) { return false; }
            if (dataAge >= MaxDaysBetweenHits) { return true; }

            // otherwise refresh randomly (95% don't refresh each run)
            return (Rnd.Next(100) >= RefreshPercentage);
        }

        public bool isStale(DateTime lastAccess)
        {
            return RandomlyRefreshData(lastAccess);
        }

        private static bool Enabled()
        {
            if (_state == 0)
            {
                lock (obj)
                {
                    _state = -1;
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
            string mediaInfoPath = Path.Combine(Helper.PlugInFolder, "MediaInfo.dll");

            if (!File.Exists(mediaInfoPath))
            {
                string sourceFile = Path.Combine(Helper.PlugInFolder, String.Format("mediainfo.dll.{0}", Is64Bit ? 64 : 32));
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

        public ProviderType Type
        {
            get { return ProviderType.Peripheral; }
        }
    }
}

#region MediaInfoDll
// MediaInfoDLL - All info about media files, for DLL
// Copyright (C) 2002-2006 Jerome Martinez, Zen@MediaArea.net

// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.

// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.

// MediaInfoDLL - All info about media files, for DLL
// Copyright (C) 2002-2006 Jerome Martinez, Zen@MediaArea.net

// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.

// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

// Microsoft Visual C# wrapper for MediaInfo Library
// See MediaInfo.h for help

// To make it working, you must put MediaInfo.Dll
// in the executable folder

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



namespace MediaInfoLib
{
    public enum StreamKind
    {
        General,
        Video,
        Audio,
        Text,
        Chapters,
        Image
    }

    public enum InfoKind
    {
        Name,
        Text,
        Measure,
        Options,
        NameText,
        MeasureText,
        Info,
        HowTo
    }

    public enum InfoOptions
    {
        ShowInInform,
        Support,
        ShowInSupported,
        TypeOfValue
    }

    public enum InfoFileOptions
    {
        FileOptionNothing = 0x00,
        FileOptionRecursive = 0x01,
        FileOptionCloseAll = 0x02,
        FileOptionMax = 0x04
    };


    public class MediaInfo
    {
        //Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)  
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_New();
        [DllImport("MediaInfo.dll")]
        public static extern void MediaInfo_Delete(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);
        [DllImport("MediaInfo.dll")]
        public static extern void MediaInfo_Close(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_Inform(IntPtr Handle, IntPtr Reserved);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_GetI(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_State_Get(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_Count_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber);

        //MediaInfo class
        public MediaInfo() { Handle = MediaInfo_New(); }
        ~MediaInfo() { MediaInfo_Delete(Handle); }
        public int Open(String FileName) { return (int)MediaInfo_Open(Handle, FileName); }
        public void Close() { MediaInfo_Close(Handle); }
        public String Inform() { return Marshal.PtrToStringUni(MediaInfo_Inform(Handle, (IntPtr)0)); }
        public String Get(StreamKind StreamKind, int StreamNumber, String Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch) { return Marshal.PtrToStringUni(MediaInfo_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, Parameter, (IntPtr)KindOfInfo, (IntPtr)KindOfSearch)); }
        public String Get(StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo) { return Marshal.PtrToStringUni(MediaInfo_GetI(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber, (IntPtr)Parameter, (IntPtr)KindOfInfo)); }
        public String Option(String Option, String Value) { return Marshal.PtrToStringUni(MediaInfo_Option(Handle, Option, Value)); }
        public int State_Get() { return (int)MediaInfo_State_Get(Handle); }
        public int Count_Get(StreamKind StreamKind, int StreamNumber) { return (int)MediaInfo_Count_Get(Handle, (IntPtr)StreamKind, (IntPtr)StreamNumber); }
        private IntPtr Handle;

        //Default values, if you know how to set default values in C#, say me
        public String Get(StreamKind StreamKind, int StreamNumber, String Parameter, InfoKind KindOfInfo) { return Get(StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name); }
        public String Get(StreamKind StreamKind, int StreamNumber, String Parameter) { return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name); }
        public String Get(StreamKind StreamKind, int StreamNumber, int Parameter) { return Get(StreamKind, StreamNumber, Parameter, InfoKind.Text); }
        public String Option(String Option_) { return Option(Option_, ""); }
        public int Count_Get(StreamKind StreamKind) { return Count_Get(StreamKind, -1); }
    }

    public class MediaInfoList
    {
        //Import of DLL functions. DO NOT USE until you know what you do (MediaInfo DLL do NOT use CoTaskMemAlloc to allocate memory)  
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoList_New();
        [DllImport("MediaInfo.dll")]
        public static extern void MediaInfoList_Delete(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoList_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName, IntPtr Options);
        [DllImport("MediaInfo.dll")]
        public static extern void MediaInfoList_Close(IntPtr Handle, IntPtr FilePos);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoList_Inform(IntPtr Handle, IntPtr FilePos, IntPtr Reserved);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoList_GetI(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, IntPtr Parameter, IntPtr KindOfInfo);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoList_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoList_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoList_State_Get(IntPtr Handle);
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoList_Count_Get(IntPtr Handle, IntPtr FilePos, IntPtr StreamKind, IntPtr StreamNumber);

        //MediaInfo class
        public MediaInfoList() { Handle = MediaInfoList_New(); }
        ~MediaInfoList() { MediaInfoList_Delete(Handle); }
        public int Open(String FileName, InfoFileOptions Options) { return (int)MediaInfoList_Open(Handle, FileName, (IntPtr)Options); }
        public void Close(int FilePos) { MediaInfoList_Close(Handle, (IntPtr)FilePos); }
        public String Inform(int FilePos) { return Marshal.PtrToStringUni(MediaInfoList_Inform(Handle, (IntPtr)FilePos, (IntPtr)0)); }
        public String Get(int FilePos, StreamKind StreamKind, int StreamNumber, String Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch) { return Marshal.PtrToStringUni(MediaInfoList_Get(Handle, (IntPtr)FilePos, (IntPtr)StreamKind, (IntPtr)StreamNumber, Parameter, (IntPtr)KindOfInfo, (IntPtr)KindOfSearch)); }
        public String Get(int FilePos, StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo) { return Marshal.PtrToStringUni(MediaInfoList_GetI(Handle, (IntPtr)FilePos, (IntPtr)StreamKind, (IntPtr)StreamNumber, (IntPtr)Parameter, (IntPtr)KindOfInfo)); }
        public String Option(String Option, String Value) { return Marshal.PtrToStringUni(MediaInfoList_Option(Handle, Option, Value)); }
        public int State_Get() { return (int)MediaInfoList_State_Get(Handle); }
        public int Count_Get(int FilePos, StreamKind StreamKind, int StreamNumber) { return (int)MediaInfoList_Count_Get(Handle, (IntPtr)FilePos, (IntPtr)StreamKind, (IntPtr)StreamNumber); }
        private IntPtr Handle;

        //Default values, if you know how to set default values in C#, say me
        public void Open(String FileName) { Open(FileName, 0); }
        public void Close() { Close(-1); }
        public String Get(int FilePos, StreamKind StreamKind, int StreamNumber, String Parameter, InfoKind KindOfInfo) { return Get(FilePos, StreamKind, StreamNumber, Parameter, KindOfInfo, InfoKind.Name); }
        public String Get(int FilePos, StreamKind StreamKind, int StreamNumber, String Parameter) { return Get(FilePos, StreamKind, StreamNumber, Parameter, InfoKind.Text, InfoKind.Name); }
        public String Get(int FilePos, StreamKind StreamKind, int StreamNumber, int Parameter) { return Get(FilePos, StreamKind, StreamNumber, Parameter, InfoKind.Text); }
        public String Option(String Option_) { return Option(Option_, ""); }
        public int Count_Get(int FilePos, StreamKind StreamKind) { return Count_Get(FilePos, StreamKind, -1); }
    }

} //NameSpace

#endregion