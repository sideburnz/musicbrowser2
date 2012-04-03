using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Entities;

namespace MusicBrowser.MediaCentre
{
    static class ProgressRecorder
    {
        private static MediaCenterEnvironment _mce;
        private static readonly List<Video> Registered = new List<Video>();

        public static void Register(Video e)
        {
            if (_mce == null)
            {
                _mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
                _mce.MediaExperience.Transport.PropertyChanged += TransportPropertyChanged;
            }
            Registered.Add(e);

            Engines.Logging.LoggerEngineFactory.Debug("PROGRESS: registered " + e.Title);

        }

        //not perfect
        private static bool ComparePathToURI(string path, string uri)
        {
            string comparerpath = path.Replace('\\', '/');
            string mediapath = WebServices.Helper.Externals.DecodeURL(uri);
            bool res = mediapath.EndsWith(comparerpath) || mediapath.EndsWith(comparerpath);
            Engines.Logging.LoggerEngineFactory.Debug("PROGRESS: testing " + comparerpath + " with " + uri + " : " + res);
            return res;
        }

        private static void TransportPropertyChanged(IPropertyObject sender, string property)
        {
            if (property.ToLower() == "playstate")
            {
                MediaTransport transport = (MediaTransport)sender;
                if (transport.PlayState == PlayState.Stopped)
                {
                    foreach(Video e in Registered)
                    {
                        if (ComparePathToURI(e.Path, (string)_mce.MediaExperience.MediaMetadata["Uri"]))
                        {                     
                            // avoid divide by 0
                            if (e.Duration == 0) { return; }

                            int pos = (int)transport.Position.TotalSeconds;
                            int per = (pos * 20) / e.Duration;
                            if (per > 1 && per < 19)
                            {
                                e.SetProgress(pos);
                                return;
                            }
                            e.SetProgress(0);
                        }
                    }
                }
                else if(transport.PlayState == PlayState.Finished)
                {
                    foreach (Video e in Registered)
                    {
                        if (ComparePathToURI(e.Path, (string)_mce.MediaExperience.MediaMetadata["Uri"]))
                        {
#if (Util.Helper.IsDVD(e.Path))
                            {
                                Application.GetReference().Session().BackPage();
                            }
                            e.SetProgress(0);
                        }
                    } 
                }
            }
        }
    }
}
