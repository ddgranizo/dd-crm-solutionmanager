using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Models
{



    public class MergedInSolutionComponent : SolutionComponentBase
    {
        public enum ReasonWhyIsNotType
        {
            OverComponent = 1,
            ParentIsNot = 2,
        }
        public bool IsIn { get; set; }

        public MergedInSolutionComponent RemovedByComponent { get; set; }
        public List<MergedInSolutionComponent> HasRemovedComponents { get; set; }

        public List<MergedInSolutionComponent> ChildMergedComponents { get; set; }

        public ReasonWhyIsNotType ReasonWhyIsNot { get; set; }

        public MergedInSolutionComponent(SolutionComponentBase solutionComponentBase)
            : base()
        {
            HasRemovedComponents = new List<MergedInSolutionComponent>();
            ChildMergedComponents = new List<MergedInSolutionComponent>();
            this.CreatedBy = solutionComponentBase.CreatedBy;
            this.CreatedOn = solutionComponentBase.CreatedOn;
            this.Id = solutionComponentBase.Id;
            this.IsMetadata = solutionComponentBase.IsMetadata;
            this.ModifiedBy = solutionComponentBase.ModifiedBy;
            this.ModifiedOn = solutionComponentBase.ModifiedOn;
            this.ObjectDefinition = solutionComponentBase.ObjectDefinition;
            this.ObjectId = solutionComponentBase.ObjectId;
            this.RootComponentBehavior = solutionComponentBase.RootComponentBehavior;
            this.RootSolutionComponentId = solutionComponentBase.RootSolutionComponentId;
            this.SolutionId = solutionComponentBase.SolutionId;
            this.Type = solutionComponentBase.Type;
            this.ParentSolutionComponent = solutionComponentBase.ParentSolutionComponent;
        }


    }
}
