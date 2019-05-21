using DD.Crm.SolutionManager.Models.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Models
{
    public class SolutionComponentBase
    {

        public const string EntityLogicalName = "solutioncomponent";

        public struct AttributeDefinitions
        {
            public const string Id = "solutioncomponentid";

            public const string Type = "componenttype";
            public const string SolutionId = "solutionid";
            public const string ModifiedBy = "modifiedby";
            public const string ObjectId = "objectid";
            public const string RootSolutionComponentId = "rootsolutioncomponentid";
            public const string ModifiedOn = "modifiedon";
            public const string CreatedOn = "createdon";
            public const string CreatedBy = "createdby";
            public const string IsMetadata = "ismetadata";
            public const string RootComponentBehavior = "rootcomponentbehavior";
        }



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
            ConvertRule = 154,
        }




        public SolutionComponentBase ParentSolutionComponent { get; set; }
        public SolutionComponentType Type { get; set; }
        public EntityReference ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public RootComponentBehaviorType? RootComponentBehavior { get; set; }
        public bool IsMetadata { get; set; }
        public Guid ObjectId { get; set; }
        public EntityReference SolutionId { get; set; }
        public Guid RootSolutionComponentId { get; set; }
        public EntityReference CreatedBy { get; set; }

        public string TypeString { get { return Type.ToString(); } }

        public object ObjectDefinition { get; set; }
        public string DisplayName
        {
            get
            {
                if (ObjectDefinition != null)
                {
                    return GetDisplayName();
                }
                return null;
            }
        }

        public string LogicalName
        {
            get
            {
                if (ObjectDefinition != null)
                {
                    return GetLogicalName();
                }
                return null;
            }
        }


        public int GetOrderWeight()
        {
            if (Type == SolutionComponentType.Entity)
            {
                if (RootComponentBehavior != null)
                {
                    if (RootComponentBehavior.Value == RootComponentBehaviorType.IncludeSubComponents)
                    {
                        return 1;
                    }
                    else if (RootComponentBehavior.Value == RootComponentBehaviorType.DoNotIncludeSubcomponents)
                    {
                        return 2;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else
                {
                    return 99;
                }
            }
            else
            {
                return 100 + (int)Type;
            }
        }

        
        public bool IsChild { get { return IsChildComponent(); } }

        public bool IsChildComponent()
        {
            return Type == SolutionComponentType.Chart ||
                    Type == SolutionComponentType.Form ||
                    Type == SolutionComponentType.View ||
                    Type == SolutionComponentType.Field ||
                    Type == SolutionComponentType.EntityKey ||
                    Type == SolutionComponentType.Relationship ||
                    Type == SolutionComponentType.HierarchyRule;
        }

        public SolutionComponentBase()
        {

        }


        private string GetDisplayName()
        {
            if (Type == SolutionComponentType.Entity)
            {
                return ((EntityMetadata)ObjectDefinition).DisplayName.UserLocalizedLabel.Label;
            }
            else if (Type == SolutionComponentType.Field)
            {
                return ((AttributeMetadata)ObjectDefinition).DisplayName.UserLocalizedLabel.Label;
            }
            else if (Type == SolutionComponentType.Relationship)
            {
                return ((RelationshipMetadataBase)ObjectDefinition).SchemaName;
            }
            else if (Type == SolutionComponentType.OptionSet)
            {
                return ((OptionSetMetadata)ObjectDefinition).DisplayName.UserLocalizedLabel.Label;
            }
            else if (Type == SolutionComponentType.EntityKey)
            {
                return ((EntityKeyMetadata)ObjectDefinition).DisplayName.UserLocalizedLabel.Label;
            }
            else return ((BaseEntity)ObjectDefinition).DisplayName;
        }

        private string GetLogicalName()
        {
            if (Type == SolutionComponentType.Entity)
            {
                return ((EntityMetadata)ObjectDefinition).LogicalName;
            }
            else if (Type == SolutionComponentType.Field)
            {
                return ((AttributeMetadata)ObjectDefinition).LogicalName;
            }
            else if (Type == SolutionComponentType.Relationship)
            {
                return ((RelationshipMetadataBase)ObjectDefinition).SchemaName;
            }
            else if (Type == SolutionComponentType.OptionSet)
            {
                return ((OptionSetMetadata)ObjectDefinition).Name;
            }
            else if (Type == SolutionComponentType.EntityKey)
            {
                return ((EntityKeyMetadata)ObjectDefinition).LogicalName;
            }
            else return ((BaseEntity)ObjectDefinition).DisplayName;
        }


        public static string GetTypeColor(SolutionComponentType type)
        {
            if (type == SolutionComponentType.Entity)
            {
                return "#BAE1FF";
            }
            else if (type == SolutionComponentType.Field)
            {
                return "#ceeaff";
            }
            else if (type == SolutionComponentType.Relationship)
            {
                return "#e3f3ff";
            }
            else if (type == SolutionComponentType.Form)
            {
                return "#94b4cc";
            }
            else if (type == SolutionComponentType.View)
            {
                return "#9ebbd1";
            }
            else if (type == SolutionComponentType.Ribbon)
            {
                return "#52abf0";
            }
            else if (type == SolutionComponentType.Role)
            {
                return "#ffffba";
            }
            else if (type == SolutionComponentType.ConnectionRole)
            {
                return "#e5e5a7";
            }
            else if (type == SolutionComponentType.Workflow)
            {
                return "#ffb3ba";
            }
            else if (type == SolutionComponentType.PluginAssembly)
            {
                return "#E5A1A7";
            }
            else if (type == SolutionComponentType.PluginStep)
            {
                return "#c78d92";
            }
            else if (type == SolutionComponentType.WebResource)
            {
                return "#baffc9";
            }
            else if (type == SolutionComponentType.App)
            {
                return "#CC94B4";
            }
            else if (type == SolutionComponentType.OptionSet)
            {
                return "#FFDDBA";
            }
            else if (type == SolutionComponentType.Sitemap)
            {
                return "#ffb76e";
            }
            else if (type == SolutionComponentType.ConvertRule)
            {
                return "#E1FFBA";
            }
            else if (type == SolutionComponentType.RoutingRule)
            {
                return "#cae5a7";
            }
            else if (type == SolutionComponentType.Chart)
            {
                return "#f767bb";
            }
            else if (type == SolutionComponentType.EntityKey)
            {
                return "#f748ae";
            }
            else if (type == SolutionComponentType.HierarchyRule)
            {
                return "#f82ba3";
            }
            else if (type == SolutionComponentType.EmailTemplate)
            {
                return "#33a3ee";
            }
            return "#FFFFFF";
        }
    }
}
