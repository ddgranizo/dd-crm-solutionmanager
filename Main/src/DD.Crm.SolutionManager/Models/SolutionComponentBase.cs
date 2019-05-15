using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Models
{
    public class SolutionComponentBase
    {
        public enum RootComponentBehaviorType
        {
            IncludeSubComponents = 0,
            DoNotIncludeSubcomponents = 1,
            IncludeAsShellOnly = 2,
        }

        public enum SolutionComponentType
        {
            Entity = 1,
            Field = 2,
            OptionSet = 9,
            Relationship = 10,
            EntityKey = 14,
            Role = 20,
            RolePrivilege = 21,
            View = 26,
            Workflow = 29,
            EmailTemplate = 36,
            Ribbon = 50,
            Chart = 59,
            Form = 60,
            WebResource = 61,
            Sitemap = 62,
            ConnectionRole = 63,
            HierarchyRule = 65,
            App = 80,
            PluginAssembly = 91,
            PluginStep = 92,
            RoutingRule = 150,
            convertRule = 154,
        }

        public SolutionComponentType Type { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid SolutionComponentId { get; set; }
        public DateTime CreatedOn { get; set; }
        public RootComponentBehaviorType RootComponentBehavior { get; set; }
        public bool IsMetadata { get; set; }
        public Guid ObjectId { get; set; }
        public Guid SolutionId { get; set; }
        public Guid RootSolutionComponentId { get; set; }
        public Guid CreatedBy { get; set; }

        public SolutionComponentBase()
        {

        }

        
       
    }
}
