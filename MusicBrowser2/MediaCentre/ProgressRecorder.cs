using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter;
using MusicBrowser.Entities;
using MusicBrowser.Engines.Cache;

namespace MusicBrowser.MediaCentre
{
    static class ProgressRecorder
    {
        private static MediaCenterEnvironment _mce = null;
        private static List<baseEntity> _registered = new List<baseEntity>();

        public static void Register(baseEntity e)
        {
            if (_mce == null)
            {
                _mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
                _mce.MediaExperience.Transport.PropertyChanged += new PropertyChangedEventHandler(TransportPropertyChanged);
            }
            _registered.Add(e);

            Engines.Logging.LoggerEngineFactory.Debug("PROGRESS: registered " + e.Title);

        }

        //not perfect
        private static bool ComparePathToURI(string path, string uri)
        {
            Engines.Logging.LoggerEngineFactory.Debug("PROGRESS: testing " + path + " with " + uri);
                        
            string comparerpath = path.Replace('\\', '/');
            return uri.EndsWith(comparerpath);
        }

        private static void TransportPropertyChanged(IPropertyObject sender, string property)
        {
            if (property.ToLower() == "playstate")
            {
                MediaTransport transport = (MediaTransport)sender;
                if (transport.PlayState == Microsoft.MediaCenter.PlayState.Stopped)
                {
                    foreach(baseEntity e in _registered)
                    {
                        if (ComparePathToURI(e.Path, (string)_mce.MediaExperience.MediaMetadata["Uri"]))
                        {
                            e.SetProgress((int)transport.Position.TotalSeconds);
                            _registered.Remove(e);
                        }
                    }
                }
                else if(transport.PlayState == Microsoft.MediaCenter.PlayState.Finished)
                {
                    foreach (baseEntity e in _registered)
                    {
                        if (ComparePathToURI(e.Path, (string)_mce.MediaExperience.MediaMetadata["Uri"]))
                        {
                            e.SetProgress(0);
                            _registered.Remove(e);
                        }
                    } 
                }
            }
        }
    }
}
