using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBrowser.Providers;
using MusicBrowser.Entities;
using MusicBrowser.Providers.Background;
using MusicBrowser.Engines.Cache;
using MusicBrowser.Actions;

namespace MusicBrowser.Engines.PlugIns.Actions
{
    class ActionPlayForgottenTracks : baseActionCommand
    {
//=====================================
//How does the "Forgotten Music" Work?
//We find all your "old favorite" artists based on your Last.fm overall, annual, and 6-monthly charts. We then remove any that are listed on your 3 month chart, weekly chart, or recent scrobbles.
//What remains are your favorite artists which have not been played recently. Maybe it is time to pull that CD out from the bottom of the pile. Or maybe you can laugh at how your taste has changed.
//=====================================

        private const string LABEL = "Play Forgotten Tracks (Last.fm)";
        private const string ICON_PATH = "resx://LastFMProvider/LastFMProvider.Resources/IconLastFM"; 

        public ActionPlayForgottenTracks(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
            Available = MusicBrowser.Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders") &&
                !String.IsNullOrEmpty(Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
        }

        public ActionPlayForgottenTracks()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Available = MusicBrowser.Util.Config.GetInstance().GetBooleanSetting("Internet.UseProviders") &&
                !String.IsNullOrEmpty(Util.Config.GetInstance().GetStringSetting("Internet.LastFMUserName"));
        }
        public override void DoAction(baseEntity entity)
        {
            throw new NotImplementedException();
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionPlayForgottenTracks(entity);
        }
    }
}
