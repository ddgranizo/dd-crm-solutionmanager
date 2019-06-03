using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Models
{
    public class AggregatedSolution
    {
        public const string EntityLogicalName = "alm_solutionaggregator";

        public enum AggregatedSolutionStatus
        {
            Development = 1,
            ClosedDevelopment = 2,
            StagingAndIntegration = 3,
            Preproduction = 4,
            Production = 5
        }


        public enum AggregatedSolutionType
        {
            Sprint = 0,
            MidnightExpress = 1,
            OnDemand = 2,
            Mantenance = 3,
        }

        public struct AttributeDefinitions
        {
            public const string Id = "alm_solutionaggregatorid";
            public const string CreatedOn = "createdon";
            public const string ModifiedOn = "modifiedon";
            public const string CreatedBy = "createdby";
            public const string ModifiedBy = "modifiedby";
            public const string Name = "alm_name";
            public const string Type = "alm_aggregatortypecode";
            public const string Status = "statuscode";
            public const string State = "statecode";
            public const string IsMergedWithSupersolution = "alm_ismerged";
        }

        public AggregatedSolutionStatus Status { get; set; }
        public AggregatedSolutionType Type { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public EntityReference CreatedBy { get; set; }
        public List<WorkSolution> WorkSolutions { get; set; }

        public bool IsMergedWithSupersolution { get; set; }
        public string StatusString { get { return Status.ToString(); } }
        public AggregatedSolution()
        {
            WorkSolutions = new List<WorkSolution>();
        }
    }
}
