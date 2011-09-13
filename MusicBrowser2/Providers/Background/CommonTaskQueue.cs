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
                string smaxthreads = Util.Config.GetInstance().GetSetting("ThreadPoolSize");
                int maxthreads;
                if (!int.TryParse(smaxthreads, out maxthreads)) { maxthreads = 1; }
                if (maxthreads < 1) { maxthreads = 1; }
                if (maxthreads > 8) { maxthreads = 8; }
                return new BackgroundTaskQueueProvider(ThreadPriority.Lowest, maxthreads);
            }
        }
    }
}
