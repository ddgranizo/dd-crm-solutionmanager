using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DD.Crm.SolutionManager.Models.SolutionComponentBase;

namespace DD.Crm.SolutionManager.Models
{
    public class Component
    {

        public Guid ObjectId { get; set; }
        public SolutionComponentType Type { get; set; }

        public Component()
        {

        }
    }
}
