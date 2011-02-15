using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MusicBrowser.Models;

/******************************************************************************
 * 
 * This is a manager for setting and forgetting asynchronous tasks
 * 
 * ----------------------------------------------------------------------------
 * 03-FEB-2011 - JJ
 * ----------------------------------------------------------------------------
 * + a thread pool for multiple thread to be running tasks
 * + improved blocking, threads block each other less 
 * ***************************************************************************/

namespace MusicBrowser.Providers.Background
{ 
    public class BackgroundTaskQueueProvider
    { 
        private readonly List<IBackgroundTaskable> _queue; 
        private Thread[] _threadpool; 
        private object obj = new object();

        private int _activeThreads; 
        private int _maximumThreads;

        public BackgroundTaskQueueProvider(ThreadPriority priority, int poolsize) 
        { 
            // set up the task queue 
            _queue = new List<IBackgroundTaskable>();

            _activeThreads = 0;

            // spin up the threads 
            _maximumThreads = poolsize; 
            _threadpool = new Thread[poolsize]; 
            for (int i = 0; i < poolsize; i++) 
            { 
                _threadpool[i] = new Thread(Processor); 
                _threadpool[i].Priority = priority; 
                _threadpool[i].IsBackground = true; 
                _threadpool[i].Name = i.ToString();

                // there must be a better way to have a thread on suspended state 
                _threadpool[i].Start();
                _threadpool[i].Suspend(); 
            }

        }

        public void Enqueue(IBackgroundTaskable task) 
        { 
            Enqueue(task, false); 
        }

        public void Enqueue(IBackgroundTaskable task, bool highPriority) 
        { 
            lock (obj) 
            { 
                if (highPriority) { _queue.Insert(0, task); } 
                else { _queue.Add(task); } 
            }

            // if there's more than twice as many pending tasks as there is active threads 
            // and we have spare active threads, start another oone up to process tasks 
            if ((_activeThreads < _maximumThreads) && ((_activeThreads * 2) < _queue.Count)) 
            { 
                for (int i = 0; i < _maximumThreads; i++) 
                { 
                    if (_threadpool[i].ThreadState == (ThreadState.Suspended | ThreadState.Background)) 
                    {
                        Logging.Logger.Info("Thread is being resumed: " + i); 
                        _activeThreads++; 
                        _threadpool[i].Resume(); 
                        break; 
                    } 
                } 
            }

        }

        private void Processor() 
        { 
            int id = int.Parse(System.Threading.Thread.CurrentThread.Name);
            while (true) 
            { 
                IBackgroundTaskable task = null; 
                if (_queue.Count > 0) 
                { 
                    lock (obj) 
                    { 
                        task = _queue[0]; 
                        _queue.RemoveAt(0); 
                    } 
                } 
                if (task != null) 
                {
                    try
                    {
                        task.Execute();
                        // there's a problem with the UI thread saying it's not responding
                        Thread.Sleep(10);
                    }
                    catch (Exception e)
                    {
                        Logging.Logger.Error(new Exception(string.Format("Thread {0} failed whilst running {1}\r", id, task.Title), e)); 
                    }
                } 
                if ((_queue.Count + 1) < _activeThreads) 
                { 
                    Logging.Logger.Info("Thread is being suspended: " + id); 
                    _activeThreads--; 
                    _threadpool[id].Suspend(); 
                } 
            }

        } 
    } 
}