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
 * + rewritten using Sleep and Interrupt - 04-SEP-2011
 * ***************************************************************************/

namespace MusicBrowser.Providers.Background
{ 
    public class BackgroundTaskQueueProvider : IDisposable
    { 
        private readonly List<IBackgroundTaskable> _queue; 
        private Thread[] _threadPool;
        private bool[] _threadAwake;
        private readonly object _obj = new object();

        private int _activeThreads; 
        private readonly int _maximumThreads;

        public BackgroundTaskQueueProvider(ThreadPriority priority, int poolSize)
        {
            lock (_obj)
            {
                // set up the task queue 
                _queue = new List<IBackgroundTaskable>();

                _activeThreads = poolSize;

                // spin up the threads 
                _maximumThreads = poolSize;
                _threadPool = new Thread[poolSize];
                _threadAwake = new bool[poolSize];

                for (int i = 0; i < poolSize; i++)
                {
                    _threadPool[i] = new Thread(Processor) { Priority = priority, IsBackground = true, Name = i.ToString() };
                    _threadAwake[i] = true;
                    _threadPool[i].Start();
                }
            }
        }

        public void Enqueue(IBackgroundTaskable task) 
        { 
            Enqueue(task, false); 
        }

        public void Enqueue(IBackgroundTaskable task, bool highPriority)
        {
            if (highPriority) { _queue.Insert(0, task); }
            else { _queue.Add(task); }

            lock (_obj)
            {
                // if there's more than two times as many pending tasks as there is active threads 
                // and we have spare active threads, start another oone up to process tasks 
                if (((_activeThreads < _maximumThreads) && ((_activeThreads * 2) < _queue.Count)) || _activeThreads == 0)
                {
                    for (int i = 0; i < _maximumThreads; i++)
                    {
                        if (!_threadAwake[i])
                        {
                            _threadPool[i].Interrupt();
                            break;
                        }
                    }
                }
            }
        }

        private void Processor()
        {
            int id = int.Parse(Thread.CurrentThread.Name);

            // the first thing we do is make the thread sleep until it has work
            try
            {
                lock (_obj)
                {
                    _threadAwake[id] = false;
                    _activeThreads--;
                }
                if (id == 0) { Thread.Sleep(1000); }
                else { Thread.Sleep(60000); }
                lock (_obj)
                {
                    _threadAwake[id] = true;
                    _activeThreads++;
#if DEBUG
                    Logging.Logger.Verbose("Thread " + id.ToString() + " awakened - initial sleep timeout", "thread awake");
#endif
                }
            }
            catch (ThreadInterruptedException e)
            {
                lock (_obj)
                {
                    _threadAwake[id] = true;
                    _activeThreads++;
#if DEBUG
                    Logging.Logger.Verbose("Thread " + id.ToString() + " awakened - first time", "thread awake");
#endif
                }
            }

            while (true)
            {
                IBackgroundTaskable task = null;
                if (_queue.Count > 0)
                {
                    try
                    {
                        lock (_obj)
                        {
                            task = _queue[0];
                            _queue.RemoveAt(0);
                        }
#if DEBUG
                        Logging.Logger.Verbose("Thread " + id.ToString() + " " + task.Title, "thread start");
#endif
                        task.Execute();
                        // there's a problem with the UI thread saying it's not responding
                        Thread.Sleep(25);
                    }
                    catch (Exception e)
                    {
                        Logging.Logger.Error(new Exception(string.Format("Thread {0} failed whilst running {1}\r", id, task.Title), e));
                    }
#if DEBUG
                    Logging.Logger.Verbose(String.Format("Thread {0} finished {1}. {2} threads alive, {3} jobs pending", id, task.Title, _activeThreads, _queue.Count), "thread finish");
#endif
                }

                if (_queue.Count == 0)
                {
                    try
                    {
                        lock (_obj)
                        {
                            _threadAwake[id] = false;
                            _activeThreads--;
#if DEBUG
                            Logging.Logger.Verbose(String.Format("Thread {0} is being suspended, {1} threads alive, {2} jobs pending", id, _activeThreads, _queue.Count), "thread asleep");
#endif
                        }
                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch (ThreadInterruptedException e)
                    {
                        lock (_obj)
                        {
                            _threadAwake[id] = true;
                            _activeThreads++;
#if DEBUG
                            Logging.Logger.Verbose("Thread " + id.ToString() + " awakened", "thread awake");
#endif
                        }
                    }
                }
            }
        }

        // kill the threads
        public void Dispose()
        {
            for (int i = 0; i < _maximumThreads; i++)
            {
                _threadPool[i].Abort();
            }
        }
    } 
}