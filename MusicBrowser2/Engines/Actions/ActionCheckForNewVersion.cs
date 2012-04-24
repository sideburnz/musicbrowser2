using System;
using MusicBrowser.Entities;

namespace MusicBrowser.Engines.Actions
{
    public class ActionCheckForNewVersion : baseActionCommand
    {
        private const string LABEL = "Check for new Version";
        private const string ICON_PATH = "resx://MusicBrowser/MusicBrowser.Resources/IconNew";

        public ActionCheckForNewVersion(baseEntity entity)
        {
            Label = LABEL;
            IconPath = ICON_PATH;
            Entity = entity;
        }

        public ActionCheckForNewVersion()
        {
            Label = LABEL;
            IconPath = ICON_PATH;
        }

        public override baseActionCommand NewInstance(baseEntity entity)
        {
            return new ActionNoOperation(entity);
        }

        public override void DoAction(baseEntity entity)
        {
            string message = "No new version available";
            WebServices.Helper.HttpProvider h = new WebServices.Helper.HttpProvider();
            h.Method = WebServices.Helper.HttpProvider.HttpMethod.Get;
            h.Url = "http://api.musicbrowser2.com/simple/version.asp?version=" + Application.Version;
            h.DoService();
            if (h.Status != "200")
            {
                message = "Error checking for new version";
            }
            if (!String.IsNullOrEmpty(h.Response.Trim()))
            {
                message = h.Response.Trim();
            }

            Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.Dialog
                        (message,
                        LABEL,
                        Microsoft.MediaCenter.DialogButtons.Ok,
                        30,
                        true);
        }
    }
}
