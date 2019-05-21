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
            Integration = 4,
            Preproduction = 5,
            Production = 6
        }


        public enum AggregatedSolutionType
        {
            Sprint = 0,
            Midnight = 1,
            Lighting = 2,
           
        }

        public struct AttributeDefinitions
        {
            public const string Id = "alm_solutionaggregatorid";
            public const string CreatedOn = "createdon	";
            public const string ModifiedOn = "modifiedon";
            public const string CreatedBy = "createdby";
            public const string ModifiedBy = "modifiedby";
            public const string Name = "alm_name";
            public const string Type = "alm_aggregatortypecode";
            public const string Status = "statuscode";
            public const string State = "statecode";
        }

        public AggregatedSolutionStatus Status { get; set; }
        public AggregatedSolutionType Type { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public List<WorkSolution> WorkSolutions { get; set; }
        public AggregatedSolution()
        {
            WorkSolutions = new List<WorkSolution>();
        }
    }
}
