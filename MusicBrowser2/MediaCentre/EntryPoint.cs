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
        }
        
        public void Launch(AddInHost host)         
        {
#if DEBUG
            Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog("Attach debugger and hit ok", "debug", Microsoft.MediaCenter.DialogButtons.Ok, 100, true);
#endif
            if (host != null && host.ApplicationContext != null)
            {
                host.ApplicationContext.SingleInstance = true;
            }
            _sSession = new HistoryOrientedPageSession();
            Application app = new Application(_sSession, host);

            MusicBrowser.Engines.PlugIns.LoadPlugIns.Execute();

            FirstRun.Initialize();
            SQLiteLoader.Load();

            // Go to the initial screen
            baseEntity firstScreen = new Home();
            firstScreen.Path = "home";
            app.Navigate(firstScreen);

        }     
    } 
} 
