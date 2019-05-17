using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Models
{
    public class MergedInSolutionComponent : SolutionComponentBase
    {
        public bool IsIn { get; set; }

        public SolutionComponentBase RemovedByComponent { get; set; }
        public List<SolutionComponentBase> HasRemovedComponents { get; set; }

        public MergedInSolutionComponent(SolutionComponentBase solutionComponentBase)
            :base()
        {
            HasRemovedComponents = new List<SolutionComponentBase>();
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
        }


    }
}
