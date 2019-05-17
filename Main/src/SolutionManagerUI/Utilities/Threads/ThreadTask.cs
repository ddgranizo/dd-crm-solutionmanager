using SolutionManagerUI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Utilities.Threads
{
    public delegate void ThreadTaskHandler(object sender, ThreadTaskEventArgs args);

    public class ThreadTask
    {


        public event ThreadTaskHandler OnStartedTask;
        public event ThreadTaskHandler OnCompletedTask;
        public event ThreadTaskHandler OnErrorTask;
        public Guid Id { get; set; }
        public Action TaskToComplete { get; set; }
        public DateTime ScheduledOn { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime CompletedOn { get; set; }
        public bool IsError { get; set; }
        public Exception Exception { get; set; }
        public int DelayMilliseconds { get; set; }
        public string Description { get; set; }
        public ThreadTask()
        {
            Id = Guid.NewGuid();
        }

        public ThreadTask(Guid id)
        {
            this.Id = id;
            ScheduledOn = DateTime.Now;
        }

        public void Run()
        {
            StartedOn = DateTime.Now;
            OnStartedTask?.Invoke(this, new ThreadTaskEventArgs());
            try
            {
                TaskToComplete.Invoke();
            }
            catch (Exception e)
            {
                IsError = true;
                Exception = e;
                OnErrorTask?.Invoke(this, new ThreadTaskEventArgs(e));
            }
            CompletedOn = DateTime.Now;
            OnCompletedTask?.Invoke(this, new ThreadTaskEventArgs());
        }
    }
}
