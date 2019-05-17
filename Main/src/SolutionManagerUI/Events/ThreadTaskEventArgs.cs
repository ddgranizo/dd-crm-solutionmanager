using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Events
{
    public class ThreadTaskEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public bool IsError { get; set; }
        public ThreadTaskEventArgs()
        {
        }

        public ThreadTaskEventArgs(Exception e)
        {
            this.Exception = e;
            this.IsError = true;
        }
    }
}
