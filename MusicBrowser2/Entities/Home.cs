using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBrowser.Entities
{
    [DataContract]
    class Home : Virtual
    {
        public override string  Title
        {
            get
            {
                return "MusicBrowser 2";
            }
            set
            {
            }
        }

        public override string View
        {
            get
            {
                return Util.Config.GetInstance().GetStringSetting("Entity.Home.View");
            }
            set
            {
                Util.Config.GetInstance().SetSetting("Entity.Home.View", value);
                FirePropertyChanged("View");
            }
        }

        public override string CacheKey
        {
            get
            {
                return "home";
            }
        }

    }
}
