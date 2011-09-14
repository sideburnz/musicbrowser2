using System.Threading;

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
                return new BackgroundTaskQueueProvider(ThreadPriority.Lowest);
            }
        }
    }
}
