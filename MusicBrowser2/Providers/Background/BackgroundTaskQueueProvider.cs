using System;
using System.Collections.Generic;
using System.Threading;

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
        private readonly object _obj = new object();

        private int _activeThreads; 
        private readonly int _maximumThreads;

        public BackgroundTaskQueueProvider(ThreadPriority priority, int poolSize) 
        { 
            // set up the task queue 
            _queue = new List<IBackgroundTaskable>();

            _activeThreads = 0;

            // spin up the threads 
            _maximumThreads = poolSize;
            _threadpool = new Thread[poolSize];
            for (int i = 0; i < poolSize; i++) 
            {
                _threadpool[i] = new Thread(Processor) {Priority = priority, IsBackground = true, Name = i.ToString()};

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
            lock (_obj)
            {
                if (highPriority) { _queue.Insert(0, task); }
                else { _queue.Add(task); }


                // if there's more than four time as many pending tasks as there is active threads 
                // and we have spare active threads, start another oone up to process tasks 
                if ((_activeThreads < _maximumThreads) && ((_activeThreads * 4) < _queue.Count))
                {
                    for (int i = 0; i < _maximumThreads; i++)
                    {
                        if (_threadpool[i].ThreadState == (ThreadState.Suspended | ThreadState.Background))
                        {
                            Logging.Logger.Debug("Thread is being resumed: " + i);
                            _activeThreads++;
                            _threadpool[i].Resume();
                            break;
                        }
                    }
                }
            }

        }

        private void Processor() 
        { 
            int id = int.Parse(Thread.CurrentThread.Name);
            while (true) 
            { 
                IBackgroundTaskable task = null;
                if (_queue.Count > 0)
                {
                    lock (_obj)
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
                else
                {
                    Logging.Logger.Debug("Thread is being suspended: " + id);
                    _activeThreads--;
                    _threadpool[id].Suspend(); 
                }
            }

        } 
    } 
}