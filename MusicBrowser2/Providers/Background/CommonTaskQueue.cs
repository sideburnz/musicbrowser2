using System;
using System.Collections.Generic;
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
        private static BackgroundTaskQueueProvider _queue = CreateQueue();

        public static void Enqueue(IBackgroundTaskable task)
        {
            _queue.Enqueue(task, false);
        }

        public static void Enqueue(IBackgroundTaskable task, bool urgent)
        {
            _queue.Enqueue(task, urgent);
        }

        private static BackgroundTaskQueueProvider CreateQueue()
        {
            string smaxthreads = Util.Config.getInstance().getSetting("ThreadPoolSize");
            int maxthreads = 2;
            int.TryParse(smaxthreads, out maxthreads);
            if (maxthreads < 0) { maxthreads = 0; }
            if (maxthreads > 8) { maxthreads = 8; }
            Util.Config.getInstance().setSetting("ThreadPoolSize", maxthreads.ToString());
            return new BackgroundTaskQueueProvider(ThreadPriority.Lowest, maxthreads);
        }
    }
}
