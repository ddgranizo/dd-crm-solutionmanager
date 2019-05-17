using SolutionManagerUI.Utilities.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Events
{
    public class ThreadManagerScheduledTaskEventArgs : EventArgs
    {
        public ThreadTask Task { get; set; }
        public ThreadManagerScheduledTaskEventArgs(ThreadTask task)
        {
            this.Task = task;
        }
    }
}
