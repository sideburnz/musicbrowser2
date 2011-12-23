using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Models;


// this controls the playback via Foobar2000

namespace MusicBrowser.Engines.Transport
{
    public class Foobar2000Transport : BaseModel, ITransportEngine
    {
        private const int BATCH_SIZE = 10;

        #region ITransport Members

        public void PlayPause()
        {
            ExecuteCommand("PlayOrPause");
        }

        public void Play(bool queue, string file)
        {
            StringBuilder sb = new StringBuilder();
            if (queue)
            {
                sb.Append(" /add /immediate");
            }
            else
            {
                sb.Append(" /play /immediate");
            }
            sb.Append(" \"" + file + "\"");

            ExecuteCommandLine(sb.ToString());
            // pause
            System.Threading.Thread.Sleep(100);
            HideFoobar();
        }

        public void Play(bool queue, IEnumerable<string> files)
        {
            // batch up the play, large playlists fail
            if (files.Count() > BATCH_SIZE)
            {
                int startEntry = 0;
                while (startEntry < files.Count())
                {
                    Play(startEntry == 0 && queue, files.Skip(startEntry).Take(BATCH_SIZE));
                    startEntry += BATCH_SIZE;
                }
            }

            StringBuilder sb = new StringBuilder();
            if (queue)
            {
                sb.Append(" /add /immediate");
            }
            else
            {
                sb.Append(" /play /immediate");
            }
            foreach (string file in files)
            {
                sb.Append(" \"" + file + "\"");
            }

            ExecuteCommandLine(sb.ToString());
            System.Threading.Thread.Sleep(100);
            HideFoobar();
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
            ExecuteCommand("/hide");
        }

        protected string ExecuteCommand(string command, params string[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(FooURL);
            sb.Append("?" + WebServices.Helper.Externals.EncodeURL(command));

            foreach (string param in parameters)
            {
                sb.Append("&" + WebServices.Helper.Externals.EncodeURL(param));
            }

            WebServices.Helper.HttpProvider h = new WebServices.Helper.HttpProvider();
            h.Method = WebServices.Helper.HttpProvider.HttpMethod.Get;
            h.Url = sb.ToString();
            h.DoService();

            if (h.Status != "200")
            {
                return string.Empty;
            }

            return h.Response;

        }

        protected void ExecuteCommandLine(string command, params string[] parameters)
        {
            ProcessStartInfo externalProc = new ProcessStartInfo();
            externalProc.FileName = FooPath;
            externalProc.Arguments = command;
            externalProc.UseShellExecute = false;
            externalProc.LoadUserProfile = false;
            externalProc.CreateNoWindow = true;
            externalProc.RedirectStandardInput = true;
            externalProc.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(externalProc);
        }
    }
}
