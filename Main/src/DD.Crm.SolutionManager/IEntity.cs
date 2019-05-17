using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager
{
    public interface IEntity
    {
        Guid GetId();
        string GetEntityLogicalName();
    }
}
