using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Models
{
    public class SolutionComponent<T> : SolutionComponentBase
    {
        public T RegardingObject { get; set; }
        public SolutionComponent(T regardingObject)
        {
            RegardingObject = regardingObject;
        }

        
    }
}
