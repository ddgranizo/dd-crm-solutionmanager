using SolutionManagerUI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Utilities.Threads
{
    public delegate void ThreadManagerTaskDescription(object sender, ThreadManagerScheduledTaskDescription args);
    public delegate void ThreadManagerTask(object sender, ThreadManagerScheduledTaskEventArgs args);
    public class ThreadManager 
    {



        private static readonly object _lockInstanceObj = new object();
        private static readonly object _lockActiveObj = new object();
        private static readonly object _lockCompletedObj = new object();
        private static readonly object _lockQueueingObj = new object();

        public event ThreadManagerTask OnScheduledTask;
        public event ThreadManagerTask OnStartedTask;
        public event ThreadManagerTask OnCompletedTask;
        public event ThreadManagerTask OnErrorTask;
        public event ThreadManagerTaskDescription OnUpdatedCurrentTaskDescription;

        private static ThreadManager _instance = null;
        public static ThreadManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockInstanceObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new ThreadManager();
                        }
                    }
                }
                return _instance;
            }

        }
        public ThreadManager()
        {
            this.OnScheduledTask += ThreadManager_OnScheduledTask;
            this.OnStartedTask += ThreadManager_OnStartedTask;
            this.OnCompletedTask += ThreadManager_OnCompletedTask;
        }

        private void ThreadManager_OnCompletedTask(object sender, ThreadManagerScheduledTaskEventArgs args)
        {

        }

        private void ThreadManager_OnStartedTask(object sender, ThreadManagerScheduledTaskEventArgs args)
        {

        }

        private void ThreadManager_OnScheduledTask(object sender, ThreadManagerScheduledTaskEventArgs args)
        {
            //if (ActiveTasks.Count< MaxPararllelThreads)
            {
                RunTask(args.Task);
            }
        }

        public void UpdateCurrentTaskDescription(string description)
        {
            OnUpdatedCurrentTaskDescription?.Invoke(this, new ThreadManagerScheduledTaskDescription(description));
        }

        public Guid ScheduleTask(Action action, string description, Guid taskId, int delayMilliseconds = 0)
        {
            ThreadTask task = new ThreadTask(taskId);
            task.Description = description;
            task.TaskToComplete = action;
            task.DelayMilliseconds = delayMilliseconds;
            ScheduleTask(task);
            return task.Id;
        }

        private void ScheduleTask(ThreadTask task)
        {
            QueuingTasks.Add(task);
            OnScheduledTask?.Invoke(this, new ThreadManagerScheduledTaskEventArgs(task));
        }

        public bool IsTaskActive(Guid id)
        {
            return ActiveTasks.FirstOrDefault(k => k.Id == id) != null;
        }
        private void RunTask(ThreadTask task)
        {
            task.OnCompletedTask += Task_OnCompletedTask;
            task.OnErrorTask += Task_OnErrorTask;
            ActiveTasks.Add(task);
            QueuingTasks.Remove(task);
            var thread = new System.Threading.Thread(() =>
            {
                System.Threading.Thread.Sleep(task.DelayMilliseconds);
                task.Run();
            });
            thread.Start();
            OnStartedTask?.Invoke(this, new ThreadManagerScheduledTaskEventArgs(task));
        }

        private void CompletedTask(ThreadTask task)
        {
            CompletedTasks.Add(task);
            ActiveTasks.Remove(task);
            if (task.IsError)
            {
                OnErrorTask?.Invoke(this, new ThreadManagerScheduledTaskEventArgs(task));
            }
            else
            {
                OnCompletedTask?.Invoke(this, new ThreadManagerScheduledTaskEventArgs(task));
            }

        }

        private void Task_OnErrorTask(object sender, ThreadTaskEventArgs args)
        {
            ((ThreadTask)sender).OnErrorTask -= Task_OnErrorTask;
            ((ThreadTask)sender).OnCompletedTask -= Task_OnCompletedTask;
            CompletedTask((ThreadTask)sender);
        }

        private void Task_OnCompletedTask(object sender, ThreadTaskEventArgs args)
        {
            ((ThreadTask)sender).OnErrorTask -= Task_OnErrorTask;
            ((ThreadTask)sender).OnCompletedTask -= Task_OnCompletedTask;
            CompletedTask((ThreadTask)sender);
        }

        private readonly List<ThreadTask> _queuingTasks = new List<ThreadTask>();
        public List<ThreadTask> QueuingTasks
        {
            get
            {
                var aux = new List<ThreadTask>();
                lock (_lockQueueingObj)
                {
                    aux = _queuingTasks;
                }
                return aux;
            }
        }

        private readonly List<ThreadTask> _activeTasks = new List<ThreadTask>();
        public List<ThreadTask> ActiveTasks
        {
            get
            {
                var aux = new List<ThreadTask>();
                lock (_lockActiveObj)
                {
                    aux = _activeTasks;
                }
                return aux;
            }
        }

        private readonly List<ThreadTask> _completedTasks = new List<ThreadTask>();
        public List<ThreadTask> CompletedTasks
        {
            get
            {
                var aux = new List<ThreadTask>();
                lock (_lockCompletedObj)
                {
                    aux = _completedTasks;
                }
                return aux;
            }
        }
    }
}
