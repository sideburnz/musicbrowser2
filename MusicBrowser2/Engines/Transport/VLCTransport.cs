using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MusicBrowser.Engines.Logging;
using MusicBrowser.Interfaces;

namespace MusicBrowser.Engines.Transport
{
    class VLCTransport : ITransportEngine
    {
        private const int BATCH_SIZE = 10;
        public double playRate = 1;

        public void Play(bool queue, string file)
        {
            StringBuilder sb = new StringBuilder();
            playRate = 1;
            paused = false;

            if (queue)
            {
                sb.Append("--playlist-enqueue ");
            }
            else
            {
                Close();
                System.Threading.Thread.Sleep(500);
            }

            sb.Append("--rate=1.0 ");
            sb.Append("--qt-start-minimized ");
            sb.Append("\"" + file + "\"  ");

            ExecuteCommand(sb.ToString());
        }

        public void Play(bool queue, IEnumerable<string> files)
        {
            // batch up large requests
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
            playRate = 1;
            paused = false;

            if (queue)
            {
                sb.Append("--playlist-enqueue ");
            }
            else
            {
                Close();
                System.Threading.Thread.Sleep(500);
            }
            sb.Append("--rate=1.0 ");
            sb.Append("--qt-start-minimized ");

            foreach (string file in files)
            {
                sb.Append("\"" + file + "\"  ");
            }
            ExecuteCommand(sb.ToString());
        }

        public void PlayDisc(string drive)
        {
            throw new NotImplementedException();
        }

        static bool paused = false;

        public void PlayPause()
        {
            // we can't pause forever so we pause for 24hours
            if (paused)
            {
                paused = false;
                ExecuteCommand("vlc://pause:0");
            }
            else
            {
                paused = true;
                ExecuteCommand("vlc://pause:86400");
            }
        }

        public void Stop()
        {
            Close();
        }

        public void Next()
        {

        }

        public void Previous()
        {

        }

        public void Close()
        {
            ExecuteCommand("vlc://quit");
        }

        public PlayState State
        {
            get { return PlayState.Undefined; }
        }

        private string VLCPath
        {
            get { return Util.Config.GetInstance().GetStringSetting("Player.Paths.VLC"); }
        }

        private void ExecuteCommand(string command)
        {
            LoggerEngineFactory.Debug(command);

            ProcessStartInfo externalProc = new ProcessStartInfo();
            externalProc.FileName = VLCPath;
            externalProc.Arguments = "--one-instance " + command;
            externalProc.UseShellExecute = false;
            externalProc.LoadUserProfile = false;
            externalProc.CreateNoWindow = true;
            externalProc.RedirectStandardInput = true;
            externalProc.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(externalProc);
        }


        public void FastForward()
        {
            playRate = playRate * 1.5;
            if (playRate > 4) { playRate = 4; }
            ExecuteCommand(String.Format("--rate={0:0.00}", playRate));
        }

        public void FastReverse()
        {
            playRate = playRate * 0.666666666;
            if (playRate < 0.25) { playRate = 0.25; }
            ExecuteCommand(String.Format("--rate={0:0.00}", playRate));
        }
    }
}
