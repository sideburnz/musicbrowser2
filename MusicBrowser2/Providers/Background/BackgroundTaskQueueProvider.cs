using System;
using System.Collections.Generic;
using System.Threading;
using MusicBrowser.Engines.Logging;

/******************************************************************************
 * 
 * This is a manager for setting and forgetting asynchronous tasks
 * 
 * ----------------------------------------------------------------------------
 * 03-FEB-2011 - JJ
 * ----------------------------------------------------------------------------
 * + rewritten using Sleep and Interrupt - 04-SEP-2011
 * + major changes to simplify
 * ***************************************************************************/

namespace MusicBrowser.Providers.Background
{
    public class BackgroundTaskQueueProvider : IDisposable
    {
        private readonly List<IBackgroundTaskable> _queue;
        private Thread[] _threadPool;
        private readonly object _obj = new object();
        private readonly int _maximumThreads;
        private readonly bool[] _threadStates;

        public BackgroundTaskQueueProvider(ThreadPriority priority)
        {
            // set up the task queue 
            _queue = new List<IBackgroundTaskable>();

            // spin up the threads - two per logical CPU
            _maximumThreads = System.Environment.ProcessorCount * 2;
#if DEBUG
            // if we're debugging, multithreading is a pain in the ass
            _maximumThreads = 1;
#endif
            _threadPool = new Thread[_maximumThreads];
            _threadStates = new bool[_maximumThreads];
            for (int i = 0; i < _maximumThreads; i++) { _threadStates[i] = false; }

            for (int i = 0; i < _maximumThreads; i++)
            {
                _threadPool[i] = new Thread(Processor) { Priority = priority, IsBackground = true, Name = i.ToString() };
                _threadPool[i].Start();
            }
        }

        public void Enqueue(IBackgroundTaskable task)
        {
            lock (_obj)
            {
                AddTask(task, false);
            }
        }

        public void Enqueue(IBackgroundTaskable task, bool urgent)
        {
            lock (_obj)
            {
                AddTask(task, urgent);
            }
        }

        private void AddTask(IBackgroundTaskable task, bool urgent)
        {
            if (urgent) { _queue.Insert(0, task); }
            else { _queue.Add(task); }

            // if we have a spare thread, spin one up
            for (int i = 0; i < _maximumThreads; i++)
            {
                if (!_threadStates[i] & Sleeping(i))
                {
                    if (AllAsleep() && !(OnStateChanged == null)) { OnStateChanged(true); }
                    _threadStates[i] = true;
                    _threadPool[i].Interrupt();
                    break;
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
                    try
                    {
                        lock (_obj)
                        {
                            task = _queue[0];
                            _queue.RemoveAt(0);
                        }
#if DEBUG
                        Engines.Logging.LoggerEngineFactory.Verbose("Thread " + id.ToString() + " " + task.Title, "thread start");
#endif
                        System.Threading.Thread.Sleep(100);
                        task.Execute();
                    }
                    catch (Exception e)
                    {
                        LoggerEngineFactory.Error(new Exception(string.Format("Thread {0} failed whilst running {1}\r", id, task.Title), e));
                    }
#if DEBUG
                    Engines.Logging.LoggerEngineFactory.Verbose(String.Format("Thread {0} finished {1}. {2} jobs pending", id, task.Title, _queue.Count), "thread finish");
#endif
                }
                else
                {
                    try
                    {
#if DEBUG
                        Engines.Logging.LoggerEngineFactory.Verbose(String.Format("Thread {0} is being suspended, {1} jobs pending", id, _queue.Count), "thread asleep");
#endif
                        _threadStates[id] = false;
                        if (AllAsleep() && !(OnStateChanged == null)) { OnStateChanged(false); }
                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch (ThreadInterruptedException e)
                    {
#if DEBUG
                        Engines.Logging.LoggerEngineFactory.Verbose("Thread " + id.ToString() + " awakened", "thread awake");
#endif
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

        private bool Sleeping(int threadID)
        {
            ThreadState ts = _threadPool[threadID].ThreadState;

            return ((ts & ThreadState.WaitSleepJoin) == ThreadState.WaitSleepJoin);
        }

        public bool AllAsleep()
        {
            foreach (bool a in _threadStates)
            {
                if (a) { return false; }
            }
            return true;
        }

        public delegate void ChangedEventHandler(bool busy);
        public event ChangedEventHandler OnStateChanged;
    }
}