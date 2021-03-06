﻿using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Models
{

    public class WorkSolution
    {
        public const string IntersectionEntitySchemaName = "alm_workersolution_solutionaggregator";
        public const string EntityLogicalName = "alm_workersolution";

        public enum WorkSolutionStatus
        {
            Development = 1,
            ReadyToInt = 3,
        }

        public struct AttributeDefinitions
        {
            public const string Id = "alm_workersolutionid";
            public const string CreatedOn = "createdon";
            public const string ModifiedOn = "modifiedon";
            public const string CreatedBy = "createdby";
            public const string ModifiedBy = "modifiedby";
            public const string Name = "alm_name";
            public const string Jira = "alm_jiracode";
            public const string JiraUrl = "alm_jiraurl";
            public const string SolutionId = "alm_solutionid";
            public const string SolutionUrl = "alm_solutionurl";
            public const string Status = "statuscode";
            public const string State = "statecode";
            public const string CheckedDependenciesOn = "alm_checkeddependencieson";
            public const string AreAllDependencies = "alm_arealldependencies";
            public const string Error = "alm_error";
        }


        public Guid Id { get; set; }
        public WorkSolutionStatus Status { get; set; }

        public string StatusString { get { return Status.ToString(); } }
        public string Name { get; set; }

        public string Jira { get; set; }
        public string JiraUrl { get; set; }
        public string SolutionUrl { get; set; }
        public Guid SolutionId { get; set; }
        public EntityReference CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime CheckedDependenciesOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool AreAllDependencies { get; set; }

        public bool IsDependencyError { get { return !AreAllDependencies && !string.IsNullOrEmpty(Error); } }
        public string Error { get; set; }
        public WorkSolution()
        {

        }
    }
}
