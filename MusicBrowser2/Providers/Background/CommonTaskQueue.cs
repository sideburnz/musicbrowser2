using System.Threading;
using System;

/******************************************************************************
 * 
 * This is a manager for setting and forgetting asynchronous tasks
 * 
 * ***************************************************************************/

namespace MusicBrowser.Providers.Background
{
    public static class CommonTaskQueue
    {
        private static readonly object _lock = new object();
        private static readonly BackgroundTaskQueueProvider Queue = CreateQueue();

        public static void Enqueue(IBackgroundTaskable task)
        {
            Queue.Enqueue(task, false);
        }

        public static void Enqueue(IBackgroundTaskable task, bool urgent)
        {
            Queue.Enqueue(task, urgent);
        }

        private static BackgroundTaskQueueProvider CreateQueue()
        {
            lock (_lock)
            {
                BackgroundTaskQueueProvider ret = new BackgroundTaskQueueProvider(ThreadPriority.Lowest);
                ret.OnStateChanged += StateChanged;
                ret.OnStateChanged += StateChanged;
                return ret;
            }
        }

        private static void StateChanged(bool busy)
        {
            if (!(OnStateChanged == null)) { OnStateChanged(busy); }
        }

        public static bool Busy
        {
            get { return !Queue.AllAsleep(); }
        }

        public delegate void ChangedEventHandler(bool busy);
        public static event ChangedEventHandler OnStateChanged;
    }
}
