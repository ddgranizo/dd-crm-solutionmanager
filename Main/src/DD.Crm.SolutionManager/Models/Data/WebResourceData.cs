using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Models.Data
{
    public class WebResourceData : BaseEntity
    {
        public int WebResourceType { get; set; }

        public WebResourceData()
                   : base()
        {
            EntityLogicalName = "webresource";
        }

    }
}
