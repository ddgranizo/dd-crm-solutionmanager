using DD.Crm.SolutionManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Models
{
    public class ComponentTypeData
    {
        public SolutionComponentType Type { get; set; }
        public override string ToString()
        {
            return $"[{(int)Type}] {Type.ToString()}";
        }
    }
}
