using DD.Crm.SolutionManager.Models;
using DD.Crm.SolutionManager.Models.Data;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Extensions
{
    public static class EntityExtensions
    {

        public static WorkSolution ToWorkSolution(this Entity e)
        {
            if (e.LogicalName != WorkSolution.EntityLogicalName)
            {
                throw new InvalidCastException();
            }
            WorkSolution s = new WorkSolution();
            s.Id = e.GetParameter<Guid>(WorkSolution.AttributeDefinitions.Id);
            s.CreatedOn = e.GetParameter<DateTime>(WorkSolution.AttributeDefinitions.CreatedOn);
            s.ModifiedOn = e.GetParameter<DateTime>(WorkSolution.AttributeDefinitions.ModifiedOn);
            s.Status = (WorkSolution.WorkSolutionStatus)
                            e.GetParameter<OptionSetValue>(WorkSolution.AttributeDefinitions.Status).Value;
            s.Name = e.GetParameter<string>(WorkSolution.AttributeDefinitions.Name);
            s.Jira = e.GetParameter<string>(WorkSolution.AttributeDefinitions.Jira);
            s.JiraUrl = e.GetParameter<string>(WorkSolution.AttributeDefinitions.JiraUrl);
            s.SolutionUrl = e.GetParameter<string>(WorkSolution.AttributeDefinitions.SolutionUrl);
            s.SolutionId = new Guid(e.GetParameter<string>(WorkSolution.AttributeDefinitions.SolutionId));
            return s;


        }
        public static AggregatedSolution ToAgreatedSolution(this Entity e)
        {
            if (e.LogicalName != AggregatedSolution.EntityLogicalName)
            {
                throw new InvalidCastException();
            }
            AggregatedSolution s = new AggregatedSolution();
            s.Id = e.GetParameter<Guid>(AggregatedSolution.AttributeDefinitions.Id);
            s.CreatedOn = e.GetParameter<DateTime>(AggregatedSolution.AttributeDefinitions.CreatedOn);
            s.ModifiedOn = e.GetParameter<DateTime>(AggregatedSolution.AttributeDefinitions.ModifiedOn);
            s.Status = (AggregatedSolution.AggregatedSolutionStatus)
                            e.GetParameter<OptionSetValue>(AggregatedSolution.AttributeDefinitions.Status).Value;
            s.Name = e.GetParameter<string>(AggregatedSolution.AttributeDefinitions.Name);
            s.Type = (AggregatedSolution.AggregatedSolutionType)
                            e.GetParameter<OptionSetValue>(AggregatedSolution.AttributeDefinitions.Type).Value;

            s.IsMergedWithSupersolution = e.GetParameter<bool>(AggregatedSolution.AttributeDefinitions.IsMergedWithSupersolution);
            return s;
        }

        public static AppData ToAppData(this Entity e)
        {
            return GetGenericData<AppData>(e);
        }

        public static ChartData ToChartData(this Entity e)
        {
            return GetGenericData<ChartData>(e);
        }

        public static ConnectionRoleData ToConnectionRoleData(this Entity e)
        {
            return GetGenericData<ConnectionRoleData>(e);
        }

        public static ConvertRuleData ToConvertRuleData(this Entity e)
        {
            return GetGenericData<ConvertRuleData>(e);
        }

        public static EmailTemplateData ToEmailTemplateData(this Entity e)
        {
            return GetGenericData<EmailTemplateData>(e);
        }

        public static FormData ToFormData(this Entity e)
        {
            return GetGenericData<FormData>(e, true);
        }

        public static HierarchyRuleData ToHierarchyRuleData(this Entity e)
        {
            return GetGenericData<HierarchyRuleData>(e);
        }


        public static PluginAssemblyData ToPluginAssemblyData(this Entity e)
        {
            return GetGenericData<PluginAssemblyData>(e);
        }

        public static PluginStepData ToPluginStepData(this Entity e)
        {
            return GetGenericData<PluginStepData>(e);
        }

        public static RibbonData ToRibbonData(this Entity e)
        {
            return GetGenericData<RibbonData>(e);
        }

        public static RoleData ToRoleData(this Entity e)
        {
            return GetGenericData<RoleData>(e);
        }

        public static RolePrivilegeData ToRolePrivilegeData(this Entity e)
        {
            return GetGenericData<RolePrivilegeData>(e);
        }

        public static RoutingRuleData ToRoutingRuleData(this Entity e)
        {
            return GetGenericData<RoutingRuleData>(e);
        }

        public static SiteMapData ToSiteMap(this Entity e)
        {
            SiteMapData sitemap = new SiteMapData();

            bool isAppWare = e.GetAttributeValue<bool>("isappaware");
            if (isAppWare)
            {
                sitemap.DisplayName = e.GetAttributeValue<string>("sitemapname");
                sitemap.Name = e.GetAttributeValue<string>("sitemapnameunique");
            }
            else
            {
                sitemap.DisplayName = "## CRM Sitemap";
            }
            
            return sitemap;
        }

        public static ViewData ToViewData(this Entity e)
        {
            return GetGenericData<ViewData>(e);
        }

        public static WebResourceData ToWebResourceData(this Entity e)
        {
            WebResourceData webResource = GetGenericData<WebResourceData>(e);
            webResource.WebResourceType = e.GetAttributeValue<OptionSetValue>("webresourcetype").Value;
            return webResource;
        }

        public static WorkflowData ToWorkflowData(this Entity e)
        {
            return GetGenericData<WorkflowData>(e);
        }

        public static T GetGenericData<T>(Entity e, bool includeObjectTypeCode = false) where T : BaseEntity, new()
        {
            T a = new T();
            a.Id = e.Id;
            if (e.LogicalName != a.EntityLogicalName)
            {
                throw new InvalidCastException();
            }
            a.Name = null;
            a.DisplayName = e.GetParameter<string>("name");
            if (includeObjectTypeCode)
            {
                a.ObjectTypeCode = e.GetAttributeValue<string>("objecttypecode");
            }
            return a;
        }
        public static MergedInSolutionComponent ToMergedSolutionComponent(this Entity e)
        {
            var component = e.ToSolutionComponent();
            return new MergedInSolutionComponent(component);
        }

        public static SolutionComponentBase ToSolutionComponent(this Entity e)
        {
            if (e.LogicalName != SolutionComponentBase.EntityLogicalName)
            {
                throw new InvalidCastException();
            }
            SolutionComponentBase s = new SolutionComponentBase();
            s.Id = e.GetParameter<Guid>(SolutionComponentBase.AttributeDefinitions.Id);
            s.CreatedBy = e.GetParameter<EntityReference>(SolutionComponentBase.AttributeDefinitions.CreatedBy);
            s.CreatedOn = e.GetParameter<DateTime>(SolutionComponentBase.AttributeDefinitions.CreatedOn);
            s.IsMetadata = e.GetParameter<bool>(SolutionComponentBase.AttributeDefinitions.IsMetadata);
            s.ModifiedBy = e.GetParameter<EntityReference>(SolutionComponentBase.AttributeDefinitions.ModifiedBy);
            s.ModifiedOn = e.GetParameter<DateTime>(SolutionComponentBase.AttributeDefinitions.ModifiedOn);
            s.ObjectId = e.GetParameter<Guid>(SolutionComponentBase.AttributeDefinitions.ObjectId);
            s.RootSolutionComponentId = e.GetParameter<Guid>(SolutionComponentBase.AttributeDefinitions.RootSolutionComponentId);
            s.SolutionId = e.GetParameter<EntityReference>(SolutionComponentBase.AttributeDefinitions.SolutionId);
            s.Type = (SolutionComponentType)e.GetParameter<OptionSetValue>(SolutionComponentBase.AttributeDefinitions.Type).Value;
            if (e.Attributes.Contains(SolutionComponentBase.AttributeDefinitions.RootComponentBehavior))
            {
                s.RootComponentBehavior = (SolutionComponentBase.RootComponentBehaviorType)e.GetParameter<OptionSetValue>(SolutionComponentBase.AttributeDefinitions.RootComponentBehavior).Value;
            }
            return s;
        }


        public static Solution ToSolution(this Entity e)
        {
            if (e.LogicalName != Solution.EntityLogicalName)
            {
                throw new InvalidCastException();
            }
            Solution s = new Solution();
            s.Id = e.GetParameter<Guid>(Solution.AttributeDefinitions.Id);
            s.CreatedBy = e.GetParameter<EntityReference>(Solution.AttributeDefinitions.CreatedBy);
            s.CreatedOn = e.GetParameter<DateTime>(Solution.AttributeDefinitions.CreatedOn);
            s.Description = e.GetParameter<string>(Solution.AttributeDefinitions.Description);
            s.DisplayName = e.GetParameter<string>(Solution.AttributeDefinitions.DisplayName);
            s.InstalledOn = e.GetParameter<DateTime>(Solution.AttributeDefinitions.InstalledOn);
            s.IsManaged = e.GetParameter<bool>(Solution.AttributeDefinitions.IsManaged);
            s.ModifiedBy = e.GetParameter<EntityReference>(Solution.AttributeDefinitions.ModifiedBy);
            s.ModifiedOn = e.GetParameter<DateTime>(Solution.AttributeDefinitions.ModifiedOn);
            s.Publisher = e.GetParameter<EntityReference>(Solution.AttributeDefinitions.Publisher);
            s.UniqueName = e.GetParameter<string>(Solution.AttributeDefinitions.UniqueName);
            s.Version = e.GetParameter<string>(Solution.AttributeDefinitions.Version);
            return s;
        }



        public static T GetParameter<T>(this Entity e, string parameter, T defaultValue = default(T))
        {
            if (e.Attributes.Contains(parameter))
            {
                return e.GetAttributeValue<T>(parameter);
            }
            return defaultValue;
        }

    }
}
