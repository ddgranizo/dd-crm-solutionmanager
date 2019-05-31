using DD.Crm.SolutionManager.Models;
using DD.Crm.SolutionManager.Models.Data;
using DD.Crm.SolutionManager.Utilities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DD.Crm.SolutionManager.Models.SolutionComponentBase;

namespace DD.Crm.SolutionManager
{
    public class SolutionManager
    {

        private readonly IOrganizationService _service;
        public bool ExpandDefinition { get; set; } = true;
        public SolutionManager(string strConnection)
        {
            this._service = CrmProvider.GetService(strConnection);
        }

        public SolutionManager(IOrganizationService service)
        {
            this._service = service;
        }



        public void SetStatusAggregatedSolution(Guid aggregatedId, AggregatedSolution.AggregatedSolutionStatus status)
        {
            CrmProvider.UpdateAggregatedSolutionStatus(_service, aggregatedId, status);
        }


        public void UnsetMergedWithSupersolutionFlagInAggregatedSolution(Guid aggregatedId)
        {
            CrmProvider.UpdateAggregatedSolutionMergedWithSupersolutionFlag(_service, aggregatedId, false);
        }

        public void SetMergedWithSupersolutionFlagInAggregatedSolution(Guid aggregatedId)
        {
            CrmProvider.UpdateAggregatedSolutionMergedWithSupersolutionFlag(_service, aggregatedId, true);
        }


        public void RemoveAggregatedSolution(Guid id)
        {
            CrmProvider.RemoveAggregatedSolution(_service, id);
        }

        public void RemoveWorkSolution(Guid id)
        {
            CrmProvider.RemoveWorkSolution(_service, id);
        }

        public void SetReadyWorkSolution(Guid id)
        {
            CrmProvider.UpdateWorkSolutionStatus(_service, id, WorkSolution.WorkSolutionStatus.ReadyToInt);
        }

        public void SetNotReadyWorkSolution(Guid id)
        {
            CrmProvider.UpdateWorkSolutionStatus(_service, id, WorkSolution.WorkSolutionStatus.Development);
        }

        public void AssignWorkSolutionToAggregatedSolution(Guid aggregatedSolutionId, Guid workSolutionId)
        {
            CrmProvider.AssignWorkSolutionToAggregatedSolution(_service, aggregatedSolutionId, workSolutionId);
        }

        public Guid CreateAggregatedSolution(string name, AggregatedSolution.AggregatedSolutionType type)
        {
            return CrmProvider.CreateAggregatedSolution(_service, name, type);
        }

        public Guid CreateWorkSolution(string name, string jira)
        {
            return CrmProvider.CreateWorkSolution(_service, name, jira);
        }

        public List<SolutionComponentBase> SearchComponent(SolutionComponentType type, string displayName)
        {
            return CrmProvider.SearchComponent(_service, type, displayName);
        }

        public void PublishAll()
        {
            CrmProvider.PublishAll(_service);
        }

        public void ImportSolution(
           string path,
           bool overwriteUnmanagedCustomizations = true,
           bool migrateAsHold = false,
           bool publishWorkflows = true)
        {
            var data = File.ReadAllBytes(path);
            CrmProvider
                .ImportSolution(
                    _service,
                    data,
                    overwriteUnmanagedCustomizations,
                    migrateAsHold,
                    publishWorkflows);
        }

        public void ImportSolutionAsnyc(
            string path,
            bool overwriteUnmanagedCustomizations = true,
            bool migrateAsHold = false,
            bool publishWorkflows = true)
        {
            var data = File.ReadAllBytes(path);
            var jobId = CrmProvider
                    .ImportSolutionAsync(
                        _service,
                        data,
                        overwriteUnmanagedCustomizations,
                        migrateAsHold,
                        publishWorkflows);
            CrmProvider.WaitAsnycOperation(_service, jobId);
        }

        public void ExportSolution(string uniqueName, string path, bool managed)
        {
            CrmProvider.ExportSolution(_service, uniqueName, path, managed);
        }

        public List<Solution> FindEmptySolutions()
        {
            return CrmProvider.FindEmptySolutions(_service);
        }

        public string GetSolutionVersion(Guid solutionId)
        {
            var solution = CrmProvider.GetSolution(_service, solutionId);
            return solution.Version;
        }


        public void IncreaseSolutionRevisionVersion(Guid solutionId)
        {
            var solution = CrmProvider.GetSolution(_service, solutionId);
            var version = solution.Version;
            var versioned = new Version(version);
            var newVersion = new Version(versioned.Major, versioned.Minor, versioned.Build, versioned.Revision + 1);
            CrmProvider.UpdateSolutionVersion(_service, solutionId, newVersion.ToString());
        }

        public void CleanSolution(Guid solutionId)
        {
            var solution = CrmProvider.GetSolution(_service, solutionId);
            CrmProvider.CleanSolution(_service, solution.UniqueName);
        }


        public void RemoveSolution(Guid solutionId)
        {
            CrmProvider.RemoveSolution(_service, solutionId);
        }

        public void RemoveSolution(string uniqueName)
        {
            var solution = GetSolution(uniqueName);
            if (solution != null)
            {
                CrmProvider.RemoveSolution(_service, solution.Id);
            }

        }

        public Guid CloneSolution(Guid solutionId, string newName = null, string newUniqueName = null, string version = null, string description = null)
        {
            return CrmProvider.CloneSolution(_service, solutionId, newName, newUniqueName, version, description);
        }

        public List<Solution> GetSolutionWhereComponentIs(Guid objectId)
        {
            return CrmProvider.GetSolutionsWhereComponentIs(_service, objectId);
        }


        public void CheckAggregatedSolution(AggregatedSolution aggregatedSolution)
        {
            var workSolutions = GetWorkSolutions(aggregatedSolution);
            var openWorkSolutions = workSolutions.Where(k => k.Status != WorkSolution.WorkSolutionStatus.ReadyToInt);
            if (openWorkSolutions.ToList().Count > 0)
            {
                var openSolutions = string.Join(", ", openWorkSolutions.Select(k => { return k.Name; }).ToList());
                throw new Exception($"Selected aggregated solution contains open work solutions:\r\n{openSolutions}");
            }

            StringBuilder st = new StringBuilder();
            st.AppendLine();
            bool foundOne = false;
            var allOpenSolutions = GetAllOpenWorkSolutions();
            var allOpenComponents = GetMergedSolutionComponents(allOpenSolutions);

            foreach (var workSolution in workSolutions)
            {
                List<WorkSolution> w = new List<WorkSolution>() { workSolution };
                var workComponents = GetMergedSolutionComponents(w);

                List<MergedInSolutionComponent> blockedComponents = new List<MergedInSolutionComponent>();
                foreach (var component in allOpenComponents)
                {
                    var foundSameComponentInOpen = allOpenComponents.FirstOrDefault(k => k.ObjectId == component.ObjectId);
                    if (foundSameComponentInOpen != null)
                    {
                        blockedComponents.Add(foundSameComponentInOpen);
                    }
                }
                if (blockedComponents.Count > 0)
                {
                    foundOne = true;
                    st.AppendLine($"- WorkSolution: {workSolution.Name}");
                    StringBuilder foundComponents = new StringBuilder();

                    CrmProvider.UpdateComponentsDefinition(_service, blockedComponents.Cast<SolutionComponentBase>().ToList());
                    foreach (var component in blockedComponents)
                    {
                        var def = ((BaseEntity)component.ObjectDefinition);
                        foundComponents.AppendLine($"\t- {def.DisplayName} of type {component.TypeString}");
                    }
                    st.AppendLine(foundComponents.ToString());
                }
            }
            if (foundOne)
            {
                throw new Exception($"Next component have been used in other open work solutions and cannot be added to the general solution: {st.ToString()}");
            }
        }

        public Solution CreateSolution(string name, string uniqueName, EntityReference pubisher, string description)
        {
            return CrmProvider.CreateSolution(_service, name, uniqueName, pubisher, null, description);
        }

        public void CreateMergedSolution(Guid solutionId, List<MergedInSolutionComponent> components)
        {
            foreach (var item in components.OrderBy(k => k.GetOrderWeight()))
            {
                CrmProvider.AddComponentToSolution(_service, solutionId, item);
            }
        }

        public void CreateMergedSolution(Solution solution, List<MergedInSolutionComponent> components)
        {
            CreateMergedSolution(solution.Id, components);
        }

        public List<Solution> GetSolutions()
        {
            return CrmProvider.GetSolutions(this._service);
        }


        public Solution GetSolution(string uniqueName)
        {
            return CrmProvider.GetSolution(this._service, uniqueName);
        }

        public List<WorkSolution> GetWorkSolutions(List<AggregatedSolution> aggregatedSolutions)
        {
            return GetWorkSolutions(aggregatedSolutions.Select(k => { return k.Id; }).ToList());
        }

        public List<WorkSolution> GetWorkSolutions(List<Guid> aggregatedSolutionsIds)
        {
            List<WorkSolution> solutions = new List<WorkSolution>();
            foreach (var solutionId in aggregatedSolutionsIds)
            {
                solutions.AddRange(GetWorkSolutions(solutionId));
            }

            return solutions;
        }

        public List<WorkSolution> GetWorkSolutions(AggregatedSolution aggregatedSolution)
        {
            return GetWorkSolutions(aggregatedSolution.Id);
        }

        public List<WorkSolution> GetWorkSolutions(Guid aggregatedSolutionId)
        {
            return CrmProvider.GetWorkSolutions(this._service, aggregatedSolutionId);
        }

        public List<WorkSolution> GetAllWorkSolutions()
        {
            return CrmProvider.GetAllWorkSolutions(this._service);
        }

        public List<WorkSolution> GetAllOpenWorkSolutions()
        {
            return CrmProvider.GetAllOpenWorkSolutions(this._service);
        }

        public List<AggregatedSolution> GetAggregatedSolutions()
        {
            return CrmProvider.GetAgregatedSolutions(this._service);
        }

        public List<SolutionComponentBase> GetSolutionComponents(Guid solutionId, bool expandDefinition = false)
        {
            bool expand = !ExpandDefinition ? ExpandDefinition : expandDefinition;
            return CrmProvider.GetSolutionComponents(this._service, solutionId, expand);
        }

        public List<SolutionComponentBase> GetSolutionsComponents(List<Solution> solutions, bool expandDefinition = false)
        {
            List<SolutionComponentBase> list = new List<SolutionComponentBase>();
            foreach (var solution in solutions)
            {
                list.AddRange(GetSolutionComponents(solution.Id, expandDefinition));
            }
            return list;
        }

        public List<SolutionComponentBase> GetSolutionsComponents(List<Guid> solutionIds, bool expandDefinition = false)
        {
            List<SolutionComponentBase> list = new List<SolutionComponentBase>();
            foreach (var id in solutionIds)
            {
                list.AddRange(GetSolutionComponents(id, expandDefinition));
            }
            return list;
        }

        public List<MergedInSolutionComponent> GetMergedSolutionComponents(List<Guid> solutionIds, List<MergedInSolutionComponent> components, bool expandDefinition = false)
        {
            var componentsFromSolutions = GetSolutionsComponents(solutionIds, expandDefinition);
            componentsFromSolutions.AddRange(components);
            return GetMergedSolutionComponents(componentsFromSolutions);
        }

       

        public List<MergedInSolutionComponent> GetMergedSolutionComponents(List<Guid> solutionIds, bool expandDefinition = false)
        {
            return GetMergedSolutionComponents(GetSolutionsComponents(solutionIds, expandDefinition));
        }

        public List<MergedInSolutionComponent> GetMergedSolutionComponents(List<Solution> solutions, bool expandDefinition = false)
        {
            return GetMergedSolutionComponents(GetSolutionsComponents(solutions, expandDefinition));
        }

        public List<MergedInSolutionComponent> GetMergedSolutionComponents(List<WorkSolution> workSolutions, bool expandDefinition = false)
        {
            var listSolutionsIds = workSolutions.Select(k => { return k.SolutionId; }).ToList();
            return GetMergedSolutionComponents(listSolutionsIds, expandDefinition);
        }

        public List<MergedInSolutionComponent> GetMergedSolutionComponents(List<SolutionComponentBase> components)
        {
            List<MergedInSolutionComponent> list = new List<MergedInSolutionComponent>();
            var sortedComponents = components
                .OrderBy(k => k.GetOrderWeight());

            foreach (var item in sortedComponents)
            {
                AddSolutionComponentToMergedSolution(item, list);
            }

            return list;
        }


        private void AddSolutionComponentToMergedSolution(
            SolutionComponentBase component,
            List<MergedInSolutionComponent> mergedComponents)
        {
            MergedInSolutionComponent newItem = new MergedInSolutionComponent(component);
            MergedInSolutionComponent reasonWhyIsNotIn = null;
            newItem.IsIn = true;
            if (newItem.IsChild && newItem.ParentSolutionComponent == null)
            {
                newItem.ReasonWhyIsNot = MergedInSolutionComponent.ReasonWhyIsNotType.ParentIsNot;
                newItem.IsIn = false;
            }
            else
            {
                reasonWhyIsNotIn = GetReasonWhyIsNotIn(component, mergedComponents);
            }
            if (reasonWhyIsNotIn != null)
            {
                newItem.IsIn = false;
                newItem.RemovedByComponent = reasonWhyIsNotIn;
                newItem.ReasonWhyIsNot = MergedInSolutionComponent.ReasonWhyIsNotType.OverComponent;
                reasonWhyIsNotIn.HasRemovedComponents.Add(newItem);
            }
            mergedComponents.Add(newItem);
        }


        private MergedInSolutionComponent GetReasonWhyIsNotIn(
            SolutionComponentBase component,
            List<MergedInSolutionComponent> mergedComponents)
        {
            if (component.Type == SolutionComponentType.Entity
                && component.RootComponentBehavior != null)
            {
                MergedInSolutionComponent sameObjectId =
                    mergedComponents.FirstOrDefault(k =>
                        k.ObjectId == component.ObjectId
                        && component.RootComponentBehavior.Value > k.RootComponentBehavior.Value);
                return sameObjectId;
            }
            else if (component.IsChild)
            {
                MergedInSolutionComponent sameObjectId =
                    mergedComponents.FirstOrDefault(k => k.ObjectId == component.ObjectId);
                if (sameObjectId == null)
                {
                    var parentComponent =
                       mergedComponents
                           .FirstOrDefault(k => k.Id == component.ParentSolutionComponent?.Id);
                    if (parentComponent != null)
                    {
                        if (!parentComponent.IsIn)
                        {
                            return parentComponent.RemovedByComponent;
                        }
                        return null;
                    }
                    return null;
                }
                return sameObjectId;
            }
            return null;
        }



    }
}
