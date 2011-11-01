using System.Collections.Generic;
using Microsoft.MediaCenter.Hosting;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Entities;
using MusicBrowser.Providers;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Transport;
 
// taken from the SDK example, I've changed the namespace and classname

namespace MusicBrowser 
{     
    public class EntryPoint : IAddInModule, IAddInEntryPoint     
    {         
        private static HistoryOrientedPageSession _sSession; 
         
        public void Initialize(Dictionary<string, object> appInfo, Dictionary<string, object> entryPointInfo)
        {
#if DEBUG
            host.MediaCenterEnvironment.Dialog("Attach debugger and hit ok", "debug", DialogButtons.Ok, 100, true); 
#endif
        }          
        
        public void Uninitialize()         
        {
            TransportEngineFactory.GetEngine().Close();
            if (Util.Config.GetInstance().GetBooleanSetting("Telemetry.Participate"))
            {
                Statistics.Send();
#if DEBUG
                Engines.Logging.LoggerEngineFactory.Verbose(Util.Helper.outputTypes(), "stats");
#endif
            }
            InMemoryCache.GetInstance().Save();
        }
        
        public void Launch(AddInHost host)         
        {
            // load the fast memory cache
            InMemoryCache.GetInstance();

            // Set up a reference to "home"
            Entity home = new Entity() { Kind = EntityKind.Home };
            home.UpdateValues();

            if (host != null && host.ApplicationContext != null)
            {
                host.ApplicationContext.SingleInstance = true;
            }
            _sSession = new HistoryOrientedPageSession();
            Application app = new Application(_sSession, host);

            // Go to the initial screen
            app.Navigate(home);

            // Trigger the background caching tasks
            foreach (string path in Providers.FolderItems.HomePathProvider.Paths)
            {
                CommonTaskQueue.Enqueue(new BackgroundCacheProvider(path));
            }
        }     
    } 
} 
