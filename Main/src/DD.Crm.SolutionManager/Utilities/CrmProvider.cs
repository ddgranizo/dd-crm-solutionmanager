using DD.Crm.SolutionManager.Extensions;
using DD.Crm.SolutionManager.Models;
using DD.Crm.SolutionManager.Models.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Utilities
{
    public static class CrmProvider
    {

        public static Entity GetGenericData(IOrganizationService service, string entityLogicalName,  Guid objectId)
        {
            return service.Retrieve(entityLogicalName, objectId, new ColumnSet(true));
        }

        public static EntityMetadata GetEntityMetadata(IOrganizationService service, Guid objectId)
        {
            RetrieveEntityRequest attributeRequest = new RetrieveEntityRequest
            {
                MetadataId = objectId,
                RetrieveAsIfPublished = true
            };
            RetrieveEntityResponse attributeResponse =
                (RetrieveEntityResponse)service.Execute(attributeRequest);
            return attributeResponse.EntityMetadata;
        }


        public static AttributeMetadata GetAttributeMetadata(IOrganizationService service, string entityLogicalName, Guid objectId)
        {
            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityLogicalName,
                MetadataId = objectId,
                RetrieveAsIfPublished = true
            };
            RetrieveAttributeResponse attributeResponse =
                (RetrieveAttributeResponse)service.Execute(attributeRequest);
            return attributeResponse.AttributeMetadata;
        }


        public static RelationshipMetadataBase GetRelationshipMetadata(IOrganizationService service, Guid objectId)
        {
            RetrieveRelationshipRequest attributeRequest = new RetrieveRelationshipRequest
            {
                MetadataId = objectId,
                RetrieveAsIfPublished = true
            };
            RetrieveRelationshipResponse attributeResponse =
                (RetrieveRelationshipResponse)service.Execute(attributeRequest);
            return attributeResponse.RelationshipMetadata;
        }

        public static OptionSetMetadataBase GetOptionSetMetadata(IOrganizationService service, Guid objectId)
        {
            RetrieveOptionSetRequest attributeRequest = new RetrieveOptionSetRequest
            {
                MetadataId = objectId,
                RetrieveAsIfPublished = true
            };
            RetrieveOptionSetResponse attributeResponse =
                (RetrieveOptionSetResponse)service.Execute(attributeRequest);
            return attributeResponse.OptionSetMetadata;
        }


        public static EntityKeyMetadata GetEntityKeyMetadata(IOrganizationService service, string entityLogicalName, Guid objectId)
        {
            RetrieveEntityKeyRequest attributeRequest = new RetrieveEntityKeyRequest
            {
                MetadataId = objectId,
                RetrieveAsIfPublished = true,
                EntityLogicalName = entityLogicalName,
            };
            RetrieveEntityKeyResponse attributeResponse =
                (RetrieveEntityKeyResponse)service.Execute(attributeRequest);
            return attributeResponse.EntityKeyMetadata;
        }

        public static List<Solution> GetSolutions(IOrganizationService service)
        {
            QueryExpression qe = new QueryExpression(Solution.EntityLogicalName);
            qe.ColumnSet = new ColumnSet(true);
            return service.RetrieveMultiple(qe)
                    .Entities
                    .Select(k => { return k.ToSolution(); })
                    .ToList();
        }


        public static List<SolutionComponentBase> GetSolutionComponents(IOrganizationService service, Guid solutionId, bool expandDefinition = false)
        {
            QueryExpression qe = new QueryExpression(SolutionComponentBase.EntityLogicalName);
            FilterExpression fe = new FilterExpression();
            fe.AddCondition(SolutionComponentBase.AttributeDefinitions.SolutionId, ConditionOperator.Equal, solutionId);
            qe.Criteria = fe;
            qe.ColumnSet = new ColumnSet(true);

            var items = service.RetrieveMultiple(qe)
                     .Entities
                     .Select(k => { return k.ToSolutionComponent(); })
                     .ToList();

            foreach (var item in items)
            {
                if (item.RootSolutionComponentId != Guid.Empty)
                {
                    var parent = items.FirstOrDefault(k => k.Id == item.RootSolutionComponentId);
                    item.ParentSolutionComponent = parent;
                }
            }
            if (expandDefinition)
            {
                items
                    .OrderBy(k=>k.GetOrderWeight())
                    .ToList()
                    .ForEach(k => { k.ObjectDefinition = RetrieveObjectDefinition(service, k); });
            }
            return items;
        }


        public static object RetrieveObjectDefinition(IOrganizationService service, SolutionComponentBase component)
        {
            var componentId = component.ObjectId;
            if (component.Type == SolutionComponentBase.SolutionComponentType.Entity)
            {
                return GetEntityMetadata(service, componentId);
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.Field)
            {
                return GetAttributeMetadata(service, component.ParentSolutionComponent.LogicalName, componentId);
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.Relationship)
            {
                return GetRelationshipMetadata(service, componentId);
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.OptionSet)
            {
                return GetOptionSetMetadata(service, componentId);
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.EntityKey)
            {
                return GetEntityKeyMetadata(service, component.ParentSolutionComponent.LogicalName, componentId);
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.Role)
            {
                var logicalName = new RoleData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToRoleData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.RolePrivilege)
            {
                var logicalName = new RolePrivilegeData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToRolePrivilegeData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.View)
            {
                var logicalName = new ViewData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToViewData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.Workflow)
            {
                var logicalName = new WorkflowData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToWorkflowData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.EmailTemplate)
            {
                var logicalName = new EmailTemplateData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToEmailTemplateData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.Ribbon)
            {
                var logicalName = new RibbonData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToRibbonData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.Chart)
            {
                var logicalName = new ChartData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToChartData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.Form)
            {
                var logicalName = new FormData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToFormData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.WebResource)
            {
                var logicalName = new WebResourceData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToWebResourceData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.Sitemap)
            {
                var logicalName = new SiteMapData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToSiteMap();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.ConnectionRole)
            {
                var logicalName = new ConnectionRoleData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToConnectionRoleData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.HierarchyRule)
            {
                var logicalName = new HierarchyRuleData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToHierarchyRuleData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.App)
            {
                var logicalName = new AppData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToAppData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.PluginAssembly)
            {
                var logicalName = new PluginAssemblyData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToPluginAssemblyData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.PluginStep)
            {
                var logicalName = new PluginStepData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToPluginStepData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.RoutingRule)
            {
                var logicalName = new RoutingRuleData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToRoutingRuleData();
            }
            else if (component.Type == SolutionComponentBase.SolutionComponentType.ConvertRule)
            {
                var logicalName = new ConvertRuleData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToConvertRuleData();
            }
            return null;
        }




        public static IOrganizationService GetService(string stringConnection)
        {
            CrmServiceClient crmService = new CrmServiceClient(stringConnection);
            IOrganizationService serviceProxy = crmService.OrganizationWebProxyClient != null ?
                                                        crmService.OrganizationWebProxyClient :
                                                        (IOrganizationService)crmService.OrganizationServiceProxy;
            return serviceProxy;
        }

    }
}
