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
        private static MediaExperience _mediaExperience;

        public static void Start()
        {
            if (_mediaExperience == null)
            {
                _mediaExperience = AddInHost.Current.MediaCenterEnvironment.MediaExperience;
                if (_mediaExperience != null)
                {
                    _mediaExperience.Transport.PropertyChanged += TransportPropertyChanged;
                }
            }
        }

        // clumsy but works for everything I've thrown at it
        private static baseEntity GetEntityFromPath(string uri)
        {
            string mediapath = WebServices.Helper.Externals.DecodeUrl(uri);
            mediapath = mediapath.Replace("/", @"\");
            if (mediapath.StartsWith("file:"))
            {
                mediapath = mediapath.Substring(5);
            }
            if (mediapath.StartsWith("dvd:"))
            {
                mediapath = mediapath.Substring(7);
            }
            return InMemoryCache.GetInstance().Fetch(Util.Helper.GetCacheKey(mediapath));
        }

        private static void TransportPropertyChanged(IPropertyObject sender, string property)
        {
            if (property.ToLower() == "playstate")
            {
                var transport = (MediaTransport) sender;
                var media = (string)_mediaExperience.MediaMetadata["Uri"];

                switch (transport.PlayState)
                {
                    case PlayState.Stopped:
                        {
                            // show the app, otherwise DVDs show a blank screen when they end
                            if (media.StartsWith("dvd:"))
                            {
                                AddInHost.Current.ApplicationContext.ReturnToApplication();
                            }

                            baseEntity entity = GetEntityFromPath(media);

                            // avoid divide by 0
                            if (entity.Duration == 0)
                            {
                                entity.Duration = 1;
                            }

                            var pos = (int) transport.Position.TotalSeconds;
                            int per = (pos*100)/entity.Duration;

                            // don't set the progress if it's near the start or finish
                            if (per > 5 && per < 95)
                            {
                                entity.SetProgress(pos);
                                return;
                            }
                            entity.SetProgress(0);

                            break;
                        }
                    case PlayState.Finished:
                        {
                            // show the app, otherwise DVDs show a blank screen when they end
                            if (media.StartsWith("dvd:"))
                            {
                                AddInHost.Current.ApplicationContext.ReturnToApplication();
                            }

                            baseEntity entity = GetEntityFromPath(media);
                            if (entity != null)
                            {
                                // remove the progress indicator
                                entity.SetProgress(0);
                            }
                            break;
                        }
                }
            }
        }
    }
}
