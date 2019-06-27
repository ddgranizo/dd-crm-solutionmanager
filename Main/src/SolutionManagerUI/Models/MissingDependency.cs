using DD.Crm.SolutionManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Models
{
    class MissingDependency
    {
        public string RequiredDisplayName { get; set; }
        public SolutionComponentType RequiredType { get; set; }


        public string DependantDisplayName { get; set; }
        public SolutionComponentType DependantType { get; set; }
    }
}
