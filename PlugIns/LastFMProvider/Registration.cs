using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.PlugIns;
using MusicBrowser.Actions;
using MusicBrowser.Entities;
using MusicBrowser.Engines.PlugIns.Actions;

namespace MusicBrowser.Engines.PlugIns
{
    public class Registration : IPlugIn
    {
        public void Register()
        {
            Factory.RegisterAction(new ActionPlayPopularLastFM());
            Factory.RegisterAction(new ActionPlayRandomPopularLastFM());
            Factory.RegisterAction(new ActionPlaySimilarTracks());

            //metadata
        }
    }
}
