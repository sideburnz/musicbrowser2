using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.MediaCenter;
using MusicBrowser.Models;
using MusicBrowser.Util;

// this controls the playback via Foobar2000

namespace MusicBrowser.Engines.Transport
{
    public class Foobar2000Transport : BaseModel, ITransportEngine
    {
        #region ITransport Members

        // avoid calling this, it's too inefficient for regular calls
        public bool IsPlaying
        {
            get
            {
                string xml = ExecuteCommand("RefreshPlayingInfo");
                if (!String.IsNullOrEmpty(xml))
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml(xml);
                    return (Helper.ReadXmlNode(xmldoc, "/foobar2000/state/IS_PLAYING", "0") == "1");
                }
                return false;
            }
        }

        public void PlayPause()
        {
            ExecuteCommand("PlayOrPause");
        }

        public void Play(bool queue, string file)
        {
            MediaCenterEnvironment mce = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            if (mce != null)
            {
                mce.MediaExperience.Transport.PlayRate = 0;
            }

            if (!queue)
            {
                ExecuteCommand("EmptyPlaylist");
            }
            ExecuteCommand("Browse", file);
            if (!queue)
            {
                System.Threading.Thread.Sleep(100);
                ExecuteCommand("StartNext");
            }
        }

        public void Play(bool queue, IEnumerable<string> files)
        {
            if (!queue)
            {
                ExecuteCommand("EmptyPlaylist");
            }
            foreach (string item in files)
            {
                Play(true, item);
                System.Threading.Thread.Sleep(150);
            }
            if (!queue)
            {
                System.Threading.Thread.Sleep(100);
                ExecuteCommand("StartNext");
            }
        }

        public void Stop()
        {
            ExecuteCommand("Stop");
        }

        public void Next()
        {
            ExecuteCommand("StartNext");
        }

        public void Previous()
        {
            ExecuteCommand("StartPrevious");
        }

        public void Close()
        {
            ExecuteCommandLine("/exit");
        }

        public void Open()
        {
            // delete running file... makes foobar show a pop-up if the app crashed last time it was run
            string running = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), @"foobar2000\running");
            if (File.Exists(running))
            {
                try
                {
                    File.Delete(running);
                }
                catch { }
            }

            // send a request to foobar to open it
            HideFoobar();
        }

        #endregion

        private string FooPath
        {
            get { return Util.Config.GetInstance().GetStringSetting("Player.Paths.foobar2000"); }
        }

        private string FooURL
        {
            get { return Util.Config.GetInstance().GetStringSetting("Player.URLs.foobar2000"); }
        }

        private void HideFoobar()
        {
            ExecuteCommandLine("/hide");
        }

        protected string ExecuteCommand(string command, params string[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(FooURL);
            sb.Append("?cmd=" + WebServices.Helper.Externals.EncodeURL(command));

            int i = 1;
            foreach (string param in parameters)
            {
                sb.Append("&param" + i + "=" + WebServices.Helper.Externals.EncodeURL(param));
                i++;
            }

            WebServices.Helper.HttpProvider h = new WebServices.Helper.HttpProvider
                                                    {
                                                        Method = WebServices.Helper.HttpProvider.HttpMethod.Get,
                                                        Url = sb.ToString()
                                                    };
            h.DoService();

            if (command != "RefreshPlayingInfo")
            {
                Logging.LoggerEngineFactory.Debug("Foobar2000Transport", sb + " => " + h.Status);
            }

            if (h.Status != "200")
            {
                return string.Empty;
            }

            return h.Response;
        }

        protected void ExecuteCommandLine(string command, params string[] parameters)
        {
            ProcessStartInfo externalProc = new ProcessStartInfo
                                                {
                                                    FileName = FooPath,
                                                    Arguments = command,
                                                    UseShellExecute = false,
                                                    LoadUserProfile = false,
                                                    CreateNoWindow = true,
                                                    RedirectStandardInput = true,
                                                    WindowStyle = ProcessWindowStyle.Hidden
                                                };
            Process.Start(externalProc);
        }

        public bool ShowNowPlaying()
        {
            if (!string.IsNullOrEmpty(ExecuteCommand("RefreshPlayingInfo")))
            {
                Application.GetReference().NavigateToFoo();
                return true;
            }
            return false;
        }

        public bool HasBespokeNowPlaying
        {
            get { return true; }
        }

        public void JumpForward()
        {
            ExecuteCommand("SeekDelta", "20");
        }

        public void JumpBack()
        {
            ExecuteCommand("SeekDelta", "-20");
        }
    }
}
