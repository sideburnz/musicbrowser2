using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Util;
using System.IO;

namespace MusicBrowser.MediaCentre
{
    class FirstRun
    {

        static void Initialize()
        {
            switch(Config.GetInstance().GetStringSetting("LastRunVersion"))
            {
                case "0.0.0.0":
                    LastVersion0000();
                    break;
            }
        }

        private static void LastVersion0000()
        {
            try
            {
                // delete actions.config
                string actionsFile = Path.Combine(Util.Helper.AppFolder, "actions.config"));
                if (File.Exists(actionsFile))
                {
                    File.Delete(actionsFile);
                }

                // delete musicbrowser.config
                // move musiclibrary to the Collections folder
                // delete the filesystem cache from the dsic

                Config.GetInstance().SetSetting("LastRunVersion", Application.Version);
            }
            catch {}
        }

    }
}
