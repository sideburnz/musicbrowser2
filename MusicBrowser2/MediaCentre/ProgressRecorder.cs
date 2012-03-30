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
        private static readonly List<baseEntity> Registered = new List<baseEntity>();

        public static void Register(baseEntity e)
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
                    foreach(baseEntity e in Registered)
                    {
                        if (ComparePathToURI(e.Path, (string)_mce.MediaExperience.MediaMetadata["Uri"]))
                        {
                            e.SetProgress((int)transport.Position.TotalSeconds);
                        }
                    }
                }
                else if(transport.PlayState == PlayState.Finished)
                {
                    foreach (baseEntity e in Registered)
                    {
                        if (ComparePathToURI(e.Path, (string)_mce.MediaExperience.MediaMetadata["Uri"]))
                        {
                            e.SetProgress(0);
                            e.UpdateCache();
                        }
                    } 
                }
            }
        }
    }
}
