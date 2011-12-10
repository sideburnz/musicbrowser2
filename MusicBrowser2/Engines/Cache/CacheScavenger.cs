using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers.Background;

namespace MusicBrowser.Engines.Cache
{
    class CacheScavenger : IBackgroundTaskable
    {
        public string Title
        {
            get { return "Cache Scavenger"; }
        }

        public void Execute()
        {
            CacheEngineFactory.GetEngine().Scavenge();
        }
    }
}
