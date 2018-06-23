using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts;

namespace PostUserActivity.BL
{
    public static class TasksQueue
    {
        private static ConcurrentQueue<AnalyzeImgArgs> Tasks;
        static TasksQueue()
        {
            Tasks = new ConcurrentQueue<AnalyzeImgArgs>();
        }

        public static void Enqueue(AnalyzeImgArgs item)
        {
            Tasks.Enqueue(item);
        }

        public static AnalyzeImgArgs Dequeue()
        {
            return Tasks.Dequeue();
        }

        public static bool IsQueuNotEmpty { get { return Tasks.Count > 0; } }

        public static int Length { get { return Tasks.Count; } }
    }
}
