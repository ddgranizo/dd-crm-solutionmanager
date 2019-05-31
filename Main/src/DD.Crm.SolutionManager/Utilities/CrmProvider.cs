using DD.Crm.SolutionManager.Extensions;
using DD.Crm.SolutionManager.Models;
using DD.Crm.SolutionManager.Models.Data;
using Microsoft.Crm.Sdk.Messages;
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
using Newtonsoft;
using System.IO;
using System.ServiceModel;
using static DD.Crm.SolutionManager.Models.SolutionComponentBase;

namespace DD.Crm.SolutionManager.Utilities
{
    public static class CrmProvider
    {



        public static void UpdateAggregatedSolutionStatus(IOrganizationService service, Guid workSolutionid, AggregatedSolution.AggregatedSolutionStatus status)
        {
            Entity e = new Entity(AggregatedSolution.EntityLogicalName);
            e.Id = workSolutionid;
            e[AggregatedSolution.AttributeDefinitions.Status] = new OptionSetValue((int)status);
            service.Update(e);
        }

        public static void UpdateAggregatedSolutionMergedWithSupersolutionFlag(IOrganizationService service, Guid aggratedSolutionId, bool merged)
        {
            Entity e = new Entity(AggregatedSolution.EntityLogicalName);
            e.Id = aggratedSolutionId;
            e[AggregatedSolution.AttributeDefinitions.IsMergedWithSupersolution] = merged;
            service.Update(e);
        }


        public static void RemoveAggregatedSolution(IOrganizationService service, Guid workSolutionid)
        {
            service.Delete(AggregatedSolution.EntityLogicalName, workSolutionid);
        }

        public static void RemoveWorkSolution(IOrganizationService service, Guid workSolutionid)
        {
            service.Delete(WorkSolution.EntityLogicalName, workSolutionid);
        }

        public static void UpdateWorkSolutionStatus(IOrganizationService service, Guid workSolutionid, WorkSolution.WorkSolutionStatus status)
        {
            Entity e = new Entity(WorkSolution.EntityLogicalName);
            e.Id = workSolutionid;
            e[WorkSolution.AttributeDefinitions.Status] = new OptionSetValue((int)status);
            service.Update(e);
        }


        public static void AssignWorkSolutionToAggregatedSolution(IOrganizationService service, Guid aggregatedSolutionId, Guid workSolutionId)
        {
            service.Associate(
                AggregatedSolution.EntityLogicalName,
                aggregatedSolutionId,
                new Relationship(WorkSolution.IntersectionEntitySchemaName),
                new EntityReferenceCollection() { new EntityReference(WorkSolution.EntityLogicalName, workSolutionId) });
        }

        public static Guid CreateWorkSolution(IOrganizationService service, string name, string jira)
        {
            Entity e = new Entity(WorkSolution.EntityLogicalName);
            e[WorkSolution.AttributeDefinitions.Name] = name;
            e[WorkSolution.AttributeDefinitions.Jira] = jira;
            return service.Create(e);
        }

        public static Guid CreateAggregatedSolution(IOrganizationService service, string name, AggregatedSolution.AggregatedSolutionType type)
        {
            Entity e = new Entity(AggregatedSolution.EntityLogicalName);
            e[AggregatedSolution.AttributeDefinitions.Name] = name;
            e[AggregatedSolution.AttributeDefinitions.Type] = new OptionSetValue((int)type);
            return service.Create(e);
        }

        public static List<SolutionComponentBase> SearchComponent
            (IOrganizationService service, SolutionComponentType type, string value)
        {
            List<SolutionComponentBase> components = new List<SolutionComponentBase>();
            List<Guid> componentsId = new List<Guid>();
            if (type == SolutionComponentType.App)
            {
                componentsId = GetAppsWithName(service, value);
            }
            else if (type == SolutionComponentType.Chart)
            {
                componentsId = GetChartWithName(service, value);
            }
            else if (type == SolutionComponentType.ConnectionRole)
            {
                componentsId = GetConnectionRoleWithName(service, value);
            }
            else if (type == SolutionComponentType.ConvertRule)
            {
                componentsId = GetConvertRuleWithName(service, value);
            }
            else if (type == SolutionComponentType.EmailTemplate)
            {
                componentsId = GetEmailTemplateWithName(service, value);
            }
            else if (type == SolutionComponentType.Form)
            {
                componentsId = GetFormWithName(service, value);
            }
            else if (type == SolutionComponentType.HierarchyRule)
            {
                componentsId = GetHierarchyRuleWithName(service, value);
            }
            else if (type == SolutionComponentType.PluginAssembly)
            {
                componentsId = GetPlugginAssemblyWithName(service, value);
            }
            else if (type == SolutionComponentType.PluginStep)
            {
                componentsId = GetPlugginStepWithName(service, value);
            }
            else if (type == SolutionComponentType.Ribbon)
            {
                componentsId = GetRibbonWithName(service, value);
            }
            else if (type == SolutionComponentType.Role)
            {
                componentsId = GetRoleWithName(service, value);
            }
            else if (type == SolutionComponentType.RolePrivilege)
            {
                componentsId = GetRolePrivilegeWithName(service, value);
            }
            else if (type == SolutionComponentType.RoutingRule)
            {
                componentsId = GetRoutingRuleWithName(service, value);
            }
            else if (type == SolutionComponentType.Sitemap)
            {
                componentsId = GetSitemapWithName(service, value);
            }
            else if (type == SolutionComponentType.View)
            {
                componentsId = GetViewWithName(service, value);
            }
            else if (type == SolutionComponentType.WebResource)
            {
                componentsId = GetWebResourceWithName(service, value);
            }
            else if (type == SolutionComponentType.Workflow)
            {
                componentsId = GetWorkflowWithName(service, value);
            }
            return componentsId.Count>0 ? GetComponentsWithId(service, componentsId) : new List<SolutionComponentBase>();
        }





        private static List<Guid> GetWorkflowWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new WorkflowData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }


        private static List<Guid> GetWebResourceWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new WebResourceData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetViewWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new ViewData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetSitemapWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new SiteMapData().EntityLogicalName,
                new List<string>() { "sitemapname", "sitemapnameunique" },
                value);
        }


        private static List<Guid> GetRoutingRuleWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new RoutingRuleData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetRolePrivilegeWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new RolePrivilegeData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }


        private static List<Guid> GetRoleWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new RoleData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetRibbonWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new RibbonData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetPlugginStepWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new PluginStepData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetPlugginAssemblyWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new PluginAssemblyData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetHierarchyRuleWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new HierarchyRuleData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetFormWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new FormData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetEmailTemplateWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new EmailTemplateData().EntityLogicalName,
                new List<string>() { "template" },
                value);
        }


        private static List<Guid> GetConvertRuleWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new ConvertRuleData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetConnectionRoleWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new ConnectionRoleData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetChartWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new ChartData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetAppsWithName(IOrganizationService service, string value)
        {
            return GetComponentWithName(
                service,
                new AppData().EntityLogicalName,
                new List<string>() { "name" },
                value);
        }

        private static List<Guid> GetComponentWithName(IOrganizationService service, string entityLogicalName, List<string> attributes, string value)
        {
            QueryExpression qe = new QueryExpression(entityLogicalName);
            qe.ColumnSet = new ColumnSet(true);
            FilterExpression fe = new FilterExpression(LogicalOperator.Or);
            foreach (var attribute in attributes)
            {
                fe.AddCondition(new ConditionExpression(attribute, ConditionOperator.BeginsWith, value));
            }
            qe.Criteria = fe;
            var entities = service.RetrieveMultiple(qe)
                                 .Entities;
            return entities
                .Select(k => { return k.Id; })
                .ToList();
        }

        public static void PublishAll(IOrganizationService service)
        {
            PublishAllXmlRequest req = new PublishAllXmlRequest();
            service.Execute(req);
        }

        public static void ImportSolution(
                IOrganizationService service,
                byte[] data,
                bool overwriteUnmanagedCustomizations = true,
                bool migrateAsHold = false,
                bool publishWorkflows = true)
        {
            ImportSolutionRequest importRequest = new ImportSolutionRequest()
            {
                CustomizationFile = data,
                OverwriteUnmanagedCustomizations = overwriteUnmanagedCustomizations,
                HoldingSolution = migrateAsHold,
                PublishWorkflows = publishWorkflows,
            };
            service.Execute(importRequest);
        }


        public static Guid ImportSolutionAsync(
                IOrganizationService service,
                byte[] data,
                bool overwriteUnmanagedCustomizations = true,
                bool migrateAsHold = false,
                bool publishWorkflows = true)
        {
            ImportSolutionRequest importRequest = new ImportSolutionRequest()
            {
                CustomizationFile = data,
                OverwriteUnmanagedCustomizations = overwriteUnmanagedCustomizations,
                HoldingSolution = migrateAsHold,
                PublishWorkflows = publishWorkflows,
            };

            ExecuteAsyncRequest asyncRequest = new ExecuteAsyncRequest()
            {
                Request = importRequest,

            };
            var asyncResponse = (ExecuteAsyncResponse)service.Execute(asyncRequest);
            var asyncJobId = asyncResponse.AsyncJobId;
            return asyncJobId;
        }


        public static void ExportSolution(IOrganizationService service, string uniqueName, string path, bool managed)
        {
            ExportSolutionRequest req = new ExportSolutionRequest()
            {
                Managed = managed,
                SolutionName = uniqueName,
            };
            var response = (ExportSolutionResponse)service.Execute(req);
            File.WriteAllBytes(path, response.ExportSolutionFile);
        }

        public static List<Solution> FindEmptySolutions(IOrganizationService service)
        {
            QueryExpression qe = new QueryExpression(Solution.EntityLogicalName);
            qe.ColumnSet = new ColumnSet(true);
            var solutionList = service.RetrieveMultiple(qe)
                                 .Entities
                                 .Select(k => { return k.ToSolution(); });

            List<Solution> emptySolutions = new List<Solution>();
            foreach (var solution in solutionList)
            {
                var components = GetSolutionComponents(service, solution.Id, false);
                if (components.Count == 0)
                {
                    emptySolutions.Add(solution);
                }
            }
            return emptySolutions;
        }


        public static void UpdateSolutionVersion(IOrganizationService service, Guid solutionId, string newVersion)
        {
            Entity e = new Entity(Solution.EntityLogicalName);
            e.Id = solutionId;
            e[Solution.AttributeDefinitions.Version] = newVersion;
            service.Update(e);
        }

        public static void CleanSolution(IOrganizationService service, string uniqueName)
        {
            var solution = GetSolution(service, uniqueName);
            if (solution != null)
            {
                var solutionComponents = GetSolutionComponents(service, solution.Id)
                   .OrderBy(k => k.GetOrderWeight())
                   .Reverse();

                foreach (var item in solutionComponents)
                {
                    RemoveSolutionComponentRequest req = new RemoveSolutionComponentRequest()
                    {
                        ComponentId = item.ObjectId,
                        ComponentType = (int)item.Type,
                        SolutionUniqueName = uniqueName,
                    };
                    service.Execute(req);
                }
            }
        }


        public static void RemoveSolution(IOrganizationService service, Guid solutionId)
        {
            service.Delete(Solution.EntityLogicalName, solutionId);
        }



        public static Guid CloneSolution(
            IOrganizationService service,
            Guid sourceId,
            string newName = null,
            string newUniqueName = null,
            string newVersion = null,
            string newDescription = null)
        {

            var solution = GetSolution(service, sourceId);
            var name = string.IsNullOrEmpty(newName)
                         ? string.Format("{0}_Copy", solution.DisplayName)
                         : newName;
            var uniqueName = string.IsNullOrEmpty(newUniqueName)
                         ? string.Format("{0}_copy_rand_{1}", solution.UniqueName, RandomString(3))
                         : newUniqueName;

            var version = string.IsNullOrEmpty(newVersion) ? solution.Version : newVersion;
            var description = string.IsNullOrEmpty(newDescription) ? solution.Description : newDescription;

            var newSolution = CreateSolution(service, name, uniqueName, solution.Publisher, version, description);

            var componentsInSourceSolution =
                GetSolutionComponents(service, sourceId, false)
                .OrderBy(k => k.GetOrderWeight());
            foreach (var component in componentsInSourceSolution)
            {
                AddComponentToSolution(service, newSolution.Id, component);
            }
            return newSolution.Id;
        }

        public static List<Solution> GetSolutionsWhereComponentIs(IOrganizationService service, List<Guid> componentIds)
        {
            var solutionList = GetComponentsWithId(service, componentIds);

            List<Guid> solutionIds = new List<Guid>();
            foreach (var item in solutionList)
            {
                var id = item.SolutionId.Id;
                solutionIds.Add(id);
            }
            return GetSolutions(service, solutionIds);
        }


        public static List<SolutionComponentBase> GetComponentsWithId(IOrganizationService service, List<Guid> componentIds)
        {
            QueryExpression qe = new QueryExpression(SolutionComponentBase.EntityLogicalName);
            FilterExpression fe = new FilterExpression(LogicalOperator.Or);
            foreach (var id in componentIds)
            {
                fe.AddCondition(SolutionComponentBase.AttributeDefinitions.ObjectId, ConditionOperator.Equal, id);
            }
            qe.Criteria = fe;
            qe.ColumnSet = new ColumnSet(true);
            return service.RetrieveMultiple(qe)
                                 .Entities
                                 .Select(k => k.ToSolutionComponent())
                                 .ToList();
        }

        public static List<Solution> GetSolutionsWhereComponentIs(IOrganizationService service, Guid componentId)
        {
            return GetSolutionsWhereComponentIs(service, new List<Guid>() { componentId });
        }

        public static Solution CreateSolution(
            IOrganizationService service,
            string name,
            string uniqueName,
            EntityReference publisher,
            string version,
            string description)
        {
            Entity e = new Entity(Solution.EntityLogicalName);
            e[Solution.AttributeDefinitions.DisplayName] = name;
            e[Solution.AttributeDefinitions.UniqueName] = uniqueName;
            e[Solution.AttributeDefinitions.Publisher] = publisher;
            e[Solution.AttributeDefinitions.Description] = description;
            var ver = string.IsNullOrEmpty(version) ? "1.0.0.0" : version;
            e[Solution.AttributeDefinitions.Version] = ver;
            var id = service.Create(e);
            return service.Retrieve(Solution.EntityLogicalName, id, new ColumnSet(true)).ToSolution();
        }

        public static void AddComponentToSolution(
            IOrganizationService service,
            Guid solutionId,
            SolutionComponentBase component)
        {
            var solution = service.Retrieve(Solution.EntityLogicalName, solutionId, new ColumnSet(true))
                .ToSolution();
            AddComponentToSolution(service, solution.UniqueName, component);
        }
        public static void AddComponentToSolution(
            IOrganizationService service,
            string solutionUniqueName,
            SolutionComponentBase component)
        {
            AddSolutionComponentRequest addReq = new AddSolutionComponentRequest()
            {
                ComponentType = (int)component.Type,
                ComponentId = component.ObjectId,
                SolutionUniqueName = solutionUniqueName,
                AddRequiredComponents = false
            };

            if (component.Type == SolutionComponentType.Entity)
            {
                addReq.DoNotIncludeSubcomponents =
                    component.RootComponentBehavior == SolutionComponentBase.RootComponentBehaviorType.DoNotIncludeSubcomponents
                    || component.RootComponentBehavior == SolutionComponentBase.RootComponentBehaviorType.IncludeAsShellOnly;
            }

            service.Execute(addReq);
        }

        public static List<WorkSolution> GetAllWorkSolutions(IOrganizationService service)
        {
            QueryExpression qe = new QueryExpression(WorkSolution.EntityLogicalName);
            qe.ColumnSet = new ColumnSet(true);
            var entities = service.RetrieveMultiple(qe)
                    .Entities;
            return entities
                    .Select(k => { return k.ToWorkSolution(); })
                    .ToList();
        }


        public static List<WorkSolution> GetAllOpenWorkSolutions(IOrganizationService service)
        {
            QueryByAttribute qe = new QueryByAttribute(WorkSolution.EntityLogicalName);
            qe.ColumnSet = new ColumnSet(true);
            qe.AddAttributeValue(WorkSolution.AttributeDefinitions.Status, (int)WorkSolution.WorkSolutionStatus.Development);
            var entities = service.RetrieveMultiple(qe)
                    .Entities;
            return entities
                    .Select(k => { return k.ToWorkSolution(); })
                    .ToList();
        }

        public static List<WorkSolution> GetWorkSolutions(IOrganizationService service, Guid agregatedSolutionId)
        {
            var qe = new QueryExpression()
            {
                EntityName = WorkSolution.EntityLogicalName,
                ColumnSet = new ColumnSet(true),
                LinkEntities =
                        {
                            new LinkEntity
                            {
                                LinkFromEntityName = WorkSolution.EntityLogicalName,
                                LinkFromAttributeName = WorkSolution.AttributeDefinitions.Id,
                                LinkToEntityName = "alm_workersolution_solutionaggregator",
                                LinkToAttributeName = WorkSolution.AttributeDefinitions.Id,
                                LinkCriteria = new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions =
                                    {
                                        new ConditionExpression
                                        {
                                            AttributeName = AggregatedSolution.AttributeDefinitions.Id,
                                            Operator = ConditionOperator.Equal,
                                            Values = { agregatedSolutionId }
                                        }
                                    }
                                }
                            }
                        }
            };

            var entities = service.RetrieveMultiple(qe)
                    .Entities;
            return entities
                    .Select(k => { return k.ToWorkSolution(); })
                    .ToList();
        }

        public static List<AggregatedSolution> GetAgregatedSolutions(IOrganizationService service)
        {
            QueryExpression qe = new QueryExpression(AggregatedSolution.EntityLogicalName);
            qe.ColumnSet = new ColumnSet(true);
            qe.AddOrder(AggregatedSolution.AttributeDefinitions.CreatedOn, OrderType.Descending);
            return service.RetrieveMultiple(qe)
                    .Entities
                    .Select(k => { return k.ToAgreatedSolution(); })
                    .ToList();
        }


        public static Entity GetRibbonData(IOrganizationService service,  Guid objectId)
        {
            QueryByAttribute qe = new QueryByAttribute(new RibbonData().EntityLogicalName);
            qe.ColumnSet = new ColumnSet(true);
            qe.AddAttributeValue("ribboncustomizationid", objectId);
            var entities = service.RetrieveMultiple(qe).Entities;
            if (entities.Count>0)
            {
                return entities[0];
            }
            return null;
            //return service.Retrieve(entityLogicalName, objectId, new ColumnSet(true));
        }


        public static Entity GetGenericData(IOrganizationService service, string entityLogicalName, Guid objectId)
        {
            return service.Retrieve(entityLogicalName, objectId, new ColumnSet(true));
        }

        public static EntityMetadata GetEntityMetadata(IOrganizationService service, Guid objectId)
        {
            RetrieveEntityRequest attributeRequest = new RetrieveEntityRequest
            {
                MetadataId = objectId,
                RetrieveAsIfPublished = true,
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


        public static Solution GetSolution(IOrganizationService service, Guid solutionId)
        {
            return service.Retrieve(Solution.EntityLogicalName, solutionId, new ColumnSet(true)).ToSolution();
        }

        public static Solution GetSolution(IOrganizationService service, string uniqueName)
        {
            QueryByAttribute qe = new QueryByAttribute(Solution.EntityLogicalName);
            qe.AddAttributeValue(Solution.AttributeDefinitions.UniqueName, uniqueName);
            qe.ColumnSet = new ColumnSet(true);
            return service.RetrieveMultiple(qe)
                    .Entities
                    .Select(k => { return k.ToSolution(); })
                    .FirstOrDefault();

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

        public static List<Solution> GetSolutions(IOrganizationService service, List<Guid> solutionsId)
        {
            QueryExpression qe = new QueryExpression(Solution.EntityLogicalName);
            qe.ColumnSet = new ColumnSet(true);
            FilterExpression fe = new FilterExpression(LogicalOperator.Or);
            foreach (var id in solutionsId)
            {
                fe.AddCondition(Solution.AttributeDefinitions.Id, ConditionOperator.Equal, id);
            }
            qe.Criteria = fe;
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
                //items
                //    .OrderBy(k => k.GetOrderWeight())
                //    .ToList()
                //    .ForEach(k => { k.ObjectDefinition = RetrieveObjectDefinition(service, k); });
                UpdateComponentsDefinition(service, items);
            }
            return items;
        }


        public static void UpdateComponentsDefinition(IOrganizationService service, List<SolutionComponentBase> components)
        {
            components
                .OrderBy(k => k.GetOrderWeight())
                .ToList()
                .ForEach(k => { k.ObjectDefinition = RetrieveObjectDefinition(service, k); });
        }

        public static object RetrieveObjectDefinition(IOrganizationService service, SolutionComponentBase component)
        {
            var componentId = component.ObjectId;
            if (component.Type == SolutionComponentType.Entity)
            {
                return GetEntityMetadata(service, componentId);
            }
            else if (component.Type == SolutionComponentType.Field)
            {
                return GetAttributeMetadata(service, component.ParentSolutionComponent.LogicalName, componentId);
            }
            else if (component.Type == SolutionComponentType.Relationship)
            {
                return GetRelationshipMetadata(service, componentId);
            }
            else if (component.Type == SolutionComponentType.OptionSet)
            {
                return GetOptionSetMetadata(service, componentId);
            }
            else if (component.Type == SolutionComponentType.EntityKey)
            {
                return GetEntityKeyMetadata(service, component.ParentSolutionComponent.LogicalName, componentId);
            }
            else if (component.Type == SolutionComponentType.Role)
            {
                var logicalName = new RoleData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToRoleData();
            }
            else if (component.Type == SolutionComponentType.RolePrivilege)
            {
                var logicalName = new RolePrivilegeData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToRolePrivilegeData();
            }
            else if (component.Type == SolutionComponentType.View)
            {
                var logicalName = new ViewData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToViewData();
            }
            else if (component.Type == SolutionComponentType.Workflow)
            {
                var logicalName = new WorkflowData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToWorkflowData();
            }
            else if (component.Type == SolutionComponentType.EmailTemplate)
            {
                var logicalName = new EmailTemplateData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToEmailTemplateData();
            }
            else if (component.Type == SolutionComponentType.Ribbon)
            {
                var logicalName = new RibbonData().EntityLogicalName;
                var data = GetRibbonData(service, componentId);
                return data.ToRibbonData();
            }
            else if (component.Type == SolutionComponentType.Chart)
            {
                var logicalName = new ChartData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToChartData();
            }
            else if (component.Type == SolutionComponentType.Form)
            {
                var logicalName = new FormData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToFormData();
            }
            else if (component.Type == SolutionComponentType.WebResource)
            {
                var logicalName = new WebResourceData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToWebResourceData();
            }
            else if (component.Type == SolutionComponentType.Sitemap)
            {
                var logicalName = new SiteMapData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToSiteMap();
            }
            else if (component.Type == SolutionComponentType.ConnectionRole)
            {
                var logicalName = new ConnectionRoleData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToConnectionRoleData();
            }
            else if (component.Type == SolutionComponentType.HierarchyRule)
            {
                var logicalName = new HierarchyRuleData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToHierarchyRuleData();
            }
            else if (component.Type == SolutionComponentType.App)
            {
                var logicalName = new AppData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToAppData();
            }
            else if (component.Type == SolutionComponentType.PluginAssembly)
            {
                var logicalName = new PluginAssemblyData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToPluginAssemblyData();
            }
            else if (component.Type == SolutionComponentType.PluginStep)
            {
                var logicalName = new PluginStepData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToPluginStepData();
            }
            else if (component.Type == SolutionComponentType.RoutingRule)
            {
                var logicalName = new RoutingRuleData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToRoutingRuleData();
            }
            else if (component.Type == SolutionComponentType.ConvertRule)
            {
                var logicalName = new ConvertRuleData().EntityLogicalName;
                return GetGenericData(service, logicalName, componentId)
                        .ToConvertRuleData();
            }
            return null;
        }


        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static IOrganizationService GetService(string stringConnection)
        {
            CrmServiceClient crmService = new CrmServiceClient(stringConnection);
            IOrganizationService serviceProxy = crmService.OrganizationWebProxyClient != null ?
                                                        crmService.OrganizationWebProxyClient :
                                                        (IOrganizationService)crmService.OrganizationServiceProxy;
            return serviceProxy;
        }



        public static void WaitAsnycOperation(IOrganizationService service, Guid jobId)
        {
            int timeMaxForTimeOut = 1000 * 60 * 200;
            DateTime end = DateTime.Now.AddMilliseconds(timeMaxForTimeOut);
            bool completed = false;
            while (!completed && end >= DateTime.Now)
            {
                System.Threading.Thread.Sleep(200);
                try
                {
                    Entity asyncOperation = service.Retrieve("asyncoperation", jobId, new ColumnSet(true));
                    var statusCode = asyncOperation.GetAttributeValue<OptionSetValue>("statuscode").Value;
                    if (statusCode == 30)
                    {
                        completed = true;
                    }
                    else if (statusCode == 21
                            || statusCode == 22
                            || statusCode == 31
                            || statusCode == 32)
                    {
                        throw new Exception(
                                string.Format(
                                    "Solution Import Failed: {0} {1}",
                                    statusCode,
                                    asyncOperation.GetAttributeValue<string>("message")));
                    }
                }
                catch (TimeoutException)
                {
                    //do nothign
                }
                catch (FaultException)
                {
                    //Do nothing
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

    }
}
