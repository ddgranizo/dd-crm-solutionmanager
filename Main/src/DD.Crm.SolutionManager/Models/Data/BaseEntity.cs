using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Models.Data
{
    public class BaseEntity
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string EntityLogicalName { get; set; }
        public Guid Id { get; set; }
        public BaseEntity(Guid id)
        {
            this.Id = id;
        }
        public BaseEntity()
        {
        }
    }
}
