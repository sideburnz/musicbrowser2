using System.Collections.Generic; 
using Microsoft.MediaCenter.Hosting;
using MusicBrowser.Entities;
using MusicBrowser.Entities.Kinds;
using MusicBrowser.Models;
 
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
            MusicBrowser.Providers.Transport.Transport.getTransport().Close();
            if (Util.Config.getInstance().getBooleanSetting("LogStatsOnClose"))
            {
                Logging.Logger.Stats(MusicBrowser.Providers.Statistics.GetInstance());
            }
        }
        
        public void Launch(AddInHost host)         
        {
            if (host != null && host.ApplicationContext != null)
            {                 
                host.ApplicationContext.SingleInstance = true;
            }             
            _sSession = new HistoryOrientedPageSession();
            Application app = new Application(_sSession, host);
            IEntity home = new Entities.Kinds.Home();
            app.Navigate(home, new Breadcrumbs());
        }     
    } 
} 
