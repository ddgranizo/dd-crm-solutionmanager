using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Models
{
    public class Solution
    {
        public struct AttributeDefinitions
        {
            public const string Id = "solutionid";
            public const string CreatedOn = "createdon	";
            public const string InstalledOn = "installedon";
            public const string ModifiedOn = "modifiedon";
            public const string UniqueName = "uniquename";
            public const string IsManaged = "ismanaged";
            public const string CreatedBy = "createdby";
            public const string ModifiedBy = "modifiedby";
            public const string Version = "version";
            public const string Publisher = "publisherid";
            public const string DisplayName = "friendlyname";
            public const string Description = "description";
        }

        public const string EntityLogicalName = "solution";

        public EntityReference Publisher { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime InstalledOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string UniqueName { get; set; }
        public Guid Id { get; set; }
        public bool IsManaged { get; set; }
        public EntityReference CreatedBy { get; set; }
        public EntityReference ModifiedBy { get; set; }
        public string Version { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }


        public List<SolutionComponentBase> SolutionComponent { get; set; }
        public Solution()
        {

        }
    }
}
