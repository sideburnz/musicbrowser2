using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;
using PlayState = Microsoft.MediaCenter.PlayState;

namespace MusicBrowser.MediaCentre
{
    static class ProgressRecorder
    {
        private static MediaCenterEnvironment _mce;
        private static readonly List<baseEntity> WatchList = new List<baseEntity>();

        public static void Register(baseEntity e)
        {
            // killer questions
            if (!e.InheritsFrom<Video>()) { return; }
            if (WatchList.Contains(e)) { return; }

            // add the item to the watch list
            WatchList.Add(e);

            // make sure we're listening for stop events
            if (_mce == null)
            {
                _mce = AddInHost.Current.MediaCenterEnvironment;
                _mce.MediaExperience.Transport.PropertyChanged += TransportPropertyChanged;
            }

            Engines.Logging.LoggerEngineFactory.Debug("ProgressRecorder", "registered " + e.Title);
        }

        // clumsy but works for everything I've thrown at it
        private static bool ComparePathToURI(string path, string uri)
        {
            string comparerpath = path.Replace('\\', '/');
            string mediapath = WebServices.Helper.Externals.DecodeURL(uri);
            bool res = mediapath.EndsWith(comparerpath) || mediapath.EndsWith(comparerpath);
            Engines.Logging.LoggerEngineFactory.Debug("ProgressRecorder", "testing " + comparerpath + " with " + uri + " : " + res);
            return res;
        }

        private static void TransportPropertyChanged(IPropertyObject sender, string property)
        {
            if (property.ToLower() == "playstate")
            {
                MediaTransport transport = (MediaTransport)sender;
                string media = (string) _mce.MediaExperience.MediaMetadata["Uri"];

                switch (transport.PlayState)
                {
                    case PlayState.Stopped:

                        // show the app, otherwise DVDs show a blank screen when they end
                        if (Util.Helper.IsDVD(media))
                        {
                            AddInHost.Current.ApplicationContext.ReturnToApplication();
                        }

                        foreach(Video e in WatchList)
                        {
                            if (ComparePathToURI(e.Path, media))
                            {                     
                                // avoid divide by 0
                                if (e.Duration == 0) { return; }

                                int pos = (int)transport.Position.TotalSeconds;
                                int per = (pos * 20) / e.Duration;
                            
                                // don't set the progress if it's the first or last 5% of the video
                                if (per > 1 && per < 19)
                                {
                                    e.SetProgress(pos);
                                    return;
                                }
                                e.SetProgress(0);
                            }
                        }
                        break;
                    case PlayState.Finished:

                        // show the app, otherwise DVDs show a blank screen when they end
                        if (Util.Helper.IsDVD(media))
                        {
                            AddInHost.Current.ApplicationContext.ReturnToApplication();
                        }

                        foreach (Video e in WatchList)
                        {
                            if (ComparePathToURI(e.Path, media))
                            {
                                // remove the progress indicator
                                e.SetProgress(0);
                            }
                        }
                        break;
                }
            }
        }
    }
}
