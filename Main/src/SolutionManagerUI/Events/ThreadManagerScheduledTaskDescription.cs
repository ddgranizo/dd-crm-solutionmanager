using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Events
{
    public class ThreadManagerScheduledTaskDescription : EventArgs
    {
        public string Description { get; set; }
        public ThreadManagerScheduledTaskDescription(string description)
        {
            this.Description = description;
        }
    }
}
