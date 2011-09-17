using System.Collections.Generic; 
using Microsoft.MediaCenter.Hosting;
using MusicBrowser.Entities;
using MusicBrowser.Models;
using MusicBrowser.Providers.Background;
using MusicBrowser.Providers;
 
// taken from the SDK example, I've changed the namespace and classname

namespace MusicBrowser 
{     
    public class EntryPoint : IAddInModule, IAddInEntryPoint     
    {         
        private static HistoryOrientedPageSession _sSession; 
         
        public void Initialize(Dictionary<string, object> appInfo, Dictionary<string, object> entryPointInfo)         
        { 
        }          
        
        public void Uninitialize()         
        {
            Providers.Transport.Transport.GetTransport().Close();
            if (Util.Config.GetInstance().GetBooleanSetting("LogStatsOnClose"))
            {
                Logging.Logger.Stats(Providers.Statistics.GetInstance());
#if DEBUG
                Logging.Logger.Verbose(Util.Helper.outputTypes(), "stats");
#endif
            }
            CacheEngine.NearLineCache.GetInstance().Save();
        }
        
        public void Launch(AddInHost host)         
        {
            // Set up a reference to "home"
            Entity home = new Entity() { Kind = EntityKind.Home };
            home.UpdateValues();

            // Load the NearLine cache
            CacheEngine.NearLineCache.GetInstance().Load();

            if (host != null && host.ApplicationContext != null)
            {
                host.ApplicationContext.SingleInstance = true;
            }
            _sSession = new HistoryOrientedPageSession();
            Application app = new Application(_sSession, host);

            // Go to the initial screen
            app.Navigate(home, new Breadcrumbs());

            // Trigger the background caching tasks
            foreach (string path in Providers.FolderItems.HomePathProvider.Paths)
            {
                CommonTaskQueue.Enqueue(new BackgroundCacheProvider(path));
            }

            // run the scavenger task
            CommonTaskQueue.Enqueue(CacheEngine.NearLineCache.GetInstance());
        }     
    } 
} 
