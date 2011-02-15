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
        private static readonly BackgroundTaskQueueProvider _queue = new BackgroundTaskQueueProvider(ThreadPriority.Lowest, 2);

        public static void Enqueue(IBackgroundTaskable task)
        {
            _queue.Enqueue(task, false);
        }

        public static void Enqueue(IBackgroundTaskable task, bool urgent)
        {
            _queue.Enqueue(task, urgent);
        }
    }
}
