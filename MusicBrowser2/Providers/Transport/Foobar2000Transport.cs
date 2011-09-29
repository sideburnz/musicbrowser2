﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MusicBrowser.Providers.Transport
{
    class Foobar2000Transport : ITransport
    {

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
            //pause
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

        public TrackInfo CurrentTrackInfo
        {
            get { throw new NotImplementedException(); }
        }

        public int Progress
        {
            get { return -1; }
        }

        public PlayState State
        {
            get { return PlayState.Undefined; }
        }

        public event EventHandler TrackChanged;
        public event EventHandler TrackProgress;

        #endregion

        private string FooPath
        {
            get { return Util.Config.GetInstance().GetStringSetting("foobar2000"); }
        }

        private void HideFoobar()
        {
            ExecuteCommand("/hide");
        }

        private void ExecuteCommand(string command)
        {
            Logging.Logger.Debug(command);

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
