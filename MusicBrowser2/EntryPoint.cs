using System.Collections.Generic;
using Microsoft.MediaCenter.Hosting;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Engines.Transport;
using MusicBrowser.Entities;
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
            TransportEngineFactory.GetEngine().Close();
            if (Util.Config.GetInstance().GetBooleanSetting("Telemetry.Participate"))
            {
                Telemetry.Send();
            }
        }
        
        public void Launch(AddInHost host)         
        {
#if DEBUG
            AddInHost.Current.MediaCenterEnvironment.Dialog("Attach debugger and hit ok", "debug", Microsoft.MediaCenter.DialogButtons.Ok, 100, true);
#endif
            if (host != null && host.ApplicationContext != null)
            {
                host.ApplicationContext.SingleInstance = true;
            }
            _sSession = new HistoryOrientedPageSession();
            Application app = new Application(_sSession, host);

            Engines.PlugIns.PlugInLoader.Execute();

            FirstRun.Initialize();

            // Go to the initial screen
            baseEntity firstScreen = new Home();
            firstScreen.Path = "home";
            app.Navigate(firstScreen);

        }     
    } 
} 
