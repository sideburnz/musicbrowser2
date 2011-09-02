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
            string smaxthreads = Util.Config.GetInstance().GetSetting("ThreadPoolSize");
            int maxthreads;
            if (!int.TryParse(smaxthreads, out maxthreads)) { maxthreads = 1; }
            if (maxthreads < 0) { maxthreads = 0; }
            if (maxthreads > 8) { maxthreads = 8; }
            Util.Config.GetInstance().SetSetting("ThreadPoolSize", maxthreads.ToString());
            return new BackgroundTaskQueueProvider(ThreadPriority.Lowest, maxthreads);
        }
    }
}
