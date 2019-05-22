using DD.Crm.SolutionManager.Models;
using DD.Crm.SolutionManager.Utilities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager
{
    public class SolutionManager
    {

        private readonly IOrganizationService _service;

        public SolutionManager(string strConnection)
        {
            this._service = CrmProvider.GetService(strConnection);
        }

        public SolutionManager(IOrganizationService service)
        {
            this._service = service;
        }

        public List<Solution> GetSolutions()
        {
            return CrmProvider.GetSolutions(this._service);
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


        public List<AggregatedSolution> GetAggregatedSolutions()
        {
            return CrmProvider.GetAgregatedSolutions(this._service);
        }

        public List<SolutionComponentBase> GetSolutionComponents(Guid solutionId, bool expandDefinition = false)
        {
            return CrmProvider.GetSolutionComponents(this._service, solutionId, expandDefinition);
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

        public List<MergedInSolutionComponent> GetMergedSolutionComponents(List<Guid> solutionIds, bool expandDefinition = false)
        {
            return GetMergedSolutionComponents(GetSolutionsComponents(solutionIds, expandDefinition));
        }

        public List<MergedInSolutionComponent> GetMergedSolutionComponents(List<Solution> solutions, bool expandDefinition = false)
        {
            return GetMergedSolutionComponents(GetSolutionsComponents(solutions, expandDefinition));
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


        private List<MergedInSolutionComponent> GetMergedChildComponents(
            MergedInSolutionComponent component,
            List<MergedInSolutionComponent> mergedComponents)
        {

            return mergedComponents
                .Where(k => k.IsChild)
                .Where(k => k.ParentSolutionComponent.ObjectId == component.ObjectId)
                .ToList();
        }

        private void AddSolutionComponentToMergedSolution(
            SolutionComponentBase component,
            List<MergedInSolutionComponent> mergedComponents)
        {
            MergedInSolutionComponent newItem = new MergedInSolutionComponent(component);
            var reasonWhyIsNotIn = GetReasonWhyIsNotIn(component, mergedComponents);
            newItem.IsIn = reasonWhyIsNotIn == null;
            newItem.RemovedByComponent = reasonWhyIsNotIn;
            if (!newItem.IsIn)
            {
                reasonWhyIsNotIn.HasRemovedComponents.Add(newItem);
            }
            mergedComponents.Add(newItem);
        }





        private MergedInSolutionComponent GetReasonWhyIsNotIn(
            SolutionComponentBase component,
            List<MergedInSolutionComponent> mergedComponents)
        {
            if (component.Type == SolutionComponentBase.SolutionComponentType.Entity
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

        //private bool IsIn(
        //    SolutionComponentBase component,
        //    List<MergedInSolutionComponent> mergedComponents)
        //{
        //    if (component.Type == SolutionComponentBase.SolutionComponentType.Entity
        //        && component.RootComponentBehavior != null)
        //    {
        //        MergedInSolutionComponent sameObjectId =
        //            mergedComponents.FirstOrDefault(k => k.ObjectId == component.ObjectId);
        //        if (sameObjectId == null)
        //        {
        //            return true;
        //        }
        //        return component.RootComponentBehavior.Value < sameObjectId.RootComponentBehavior.Value;
        //    }
        //    else if (component.IsChildComponent())
        //    {
        //        MergedInSolutionComponent sameObjectId =
        //            mergedComponents.FirstOrDefault(k => k.ObjectId == component.ObjectId);
        //        if (sameObjectId == null)
        //        {
        //            var parentComponent =
        //                mergedComponents
        //                    .FirstOrDefault(k => k.Id == component.ParentSolutionComponent?.Id);
        //            if (parentComponent != null)
        //            {
        //                return parentComponent.IsIn;
        //            }
        //            return true;
        //        }
        //        return false;
        //    }
        //    return true;
        //}




    }
}
