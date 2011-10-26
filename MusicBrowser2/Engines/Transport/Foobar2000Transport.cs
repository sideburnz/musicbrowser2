using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Engines.Transport
{
    class Foobar2000Transport : ITransportEngine
    {

        private const int BATCH_SIZE = 10;

        #region ITransport Members

        public void PlayPause()
        {
            ExecuteCommand("/playpause");
            HideFoobar();
        }

        public void Play(bool queue, string file)
        {
            StringBuilder sb = new StringBuilder();
            if (queue)
            {
                sb.Append(" /add");
            }
            else
            {
                sb.Append(" /play /immediate");
            }
            sb.Append(" \"" + file + "\"");

            ExecuteCommand(sb.ToString());
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
                sb.Append(" /add");
            }
            else
            {
                sb.Append(" /play /immediate");
            }
            foreach (string file in files)
            {
                sb.Append(" \"" + file + "\"");
            }

            ExecuteCommand(sb.ToString());
            //TODO: why does this pause?
            System.Threading.Thread.Sleep(100);
            HideFoobar();
        }

        public void PlayDisc(string drive)
        {
            ExecuteCommand("/play " + drive);
            System.Threading.Thread.Sleep(100);
            HideFoobar();
        }

        public void Stop()
        {
            ExecuteCommand("/stop");
            HideFoobar();
        }

        public void Next()
        {
            ExecuteCommand("/next");
            HideFoobar();
        }

        public void Previous()
        {
            // there appears to be a bug in this, so rather than stop playback, do nothing
            //ExecuteCommand("/prev");
            //HideFoobar();
        }

        public void Close()
        {
            ExecuteCommand("/exit");
        }

        public PlayState State
        {
            get { return PlayState.Undefined; }
        }

        #endregion

        private string FooPath
        {
            get { return Util.Config.GetInstance().GetStringSetting("Player.Path.foobar2000"); }
        }

        private void HideFoobar()
        {
            ExecuteCommand("/hide");
        }

        private void ExecuteCommand(string command)
        {
            LoggerEngineFactory.Debug(command);

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
