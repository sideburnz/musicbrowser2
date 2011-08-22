using System;
using System.Collections.Generic;
using System.Drawing;

/******************************************************************************
 * 
 *  Contains all of the interfaces, enums and structs for DataProoviders
 * 
 * ***************************************************************************/


namespace MusicBrowser.Interfaces
{

    public enum  DataProviderOutcome
    {
        Success,        // everything was as expected, data is available
        SystemError,    // something went wrong, more info should be in the Errors collection
        NoData,         // the query worked but there's no data or no new data
        NotFound,       // the query worked but there item couldn't be found (e.g. an artist was looked for that doesn't exist)
        InvalidInput
    }

    public enum DataTypes
    {
        Song,
        Album,
        Artist,
        Genre,
        Playlist,
        Disc
    }

    /// <summary>
    /// DTO for passing data to and from data providers
    /// </summary>
    public struct DataProviderDTO
    {
        // in
        public DataTypes DataType;
        public string Path;
        public string DiscId;

        // out
        public DataProviderOutcome Outcome;
        public List<string> Errors;

        // in and out
        public string TrackName;
        public string ArtistName;
        public string AlbumArtist;
        public string AlbumName;

        public string Title;

        public int TrackNumber;
        public int DiscNumber;
        public DateTime ReleaseDate;
        public string Label;

        public int Children;
        public int TrackCount;

        public string Codec;
        public string Channels;
        public int Duration;
        public string SampleRate;
        public string Resolution;

        public int PlayCount;
        public int Rating;
        public int Listeners;
        public int TotalPlays;
        public bool Favorite;

        public Bitmap ThumbImage;
        public Bitmap BackImage;

        public string MusicBrainzId;

        public List<string> Performers;
        public List<string> Genres;

        public string Lyrics;

        public string Summary;
    }

    /// <summary>
    /// Provides a common interface for data providers to provide.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Populates the DTO with data
        /// </summary>
        /// <param name="dto">DTO</param>
        /// <returns>Populated DTO</returns>
        DataProviderDTO Fetch(DataProviderDTO dto);

        string FriendlyName();

        bool CompatibleWith(string type);

        bool isStale(DateTime lastAccess);
    }
}
