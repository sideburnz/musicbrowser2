//*****************************************************************************
//
//  Basic standing data reader to remove hard coded lists in the application.
//  This data is read-only during runtime.
//
//*****************************************************************************

namespace MusicBrowser.Util
{
    static class StandingData
    {
        public static string[] GetStandingData(string category)
        {
            switch (category.ToLower())
            {
                case "views":
                    {
                        string[] rval = {"List", "Thumbs", "Cover Flow"};
                        return rval;
                    }
                case "languages":
                    {
                        string[] rval = {"English", "French", "German", "Dutch"};
                        return rval;
                    }
                case "images":
                    {
                        string[] rval = {".jpg", ".png", ".jpeg", ".gif"};
                        return rval;
                    }
                case "songs":
                    {
                        string[] rval = {".mp3", ".flac", ".wma", ".ogg", ".mpc", ".wav", ".m4a", ".mp2", ".ape"};
                        return rval;
                    }
                case "playlists":
                    {
                        string[] rval = {".wpl", ".m3u", ".asx"};
                        return rval;
                    }
                case "nonentityextentions":
                    {
                        string[] rval = { ".xml", ".jpg", ".png", ".jpeg", ".gif" };
                        return rval;
                    }
                case "logdestination":
                    {
                        string[] rval = {"file", "event log"};
                        return rval;
                    }
                case "loglevel":
                    {
                        string[] rval = {"debug", "info", "error", "none"};
                        return rval;
                    }
                case "sortorder":
                    {
                        string[] rval = { "Name", "Release", "Track", "Duration" };
                        return rval;
                    }
            }
            return null;
        }
    }
}
