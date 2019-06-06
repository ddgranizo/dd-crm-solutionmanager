

using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Models
{


    public enum DependencyType
    {
        None = 0,
        SolutionInternal = 1,
        Published = 2,
        Unpublished = 3,
    }
    public class Dependency
    {

        public struct AttributeDefinitions
        {
            public const string Id = "dependencyid";
            public const string DependentComponentParentId = "dependentcomponentparentid";
            public const string RequiredComponentType = "requiredcomponenttype";
            public const string DependentComponentObjectId = "dependentcomponentobjectid";
            public const string DependentComponentBaseSolutionid = "dependentcomponentbasesolutionid";
            public const string DependentComponentType = "dependentcomponenttype";
            public const string RequiredComponentBaseSolutionId = "requiredcomponentbasesolutionid";
            public const string RequiredComponentParentId = "requiredcomponentparentid";
            public const string RequiredComponentObjectId = "requiredcomponentobjectid";
            public const string DependencyType = "dependencytype";

        }


        public Guid Id { get; set; }

        public object DependentParentComponentDefinition { get; set; }
        public object DependentComponentDefinition { get; set; }


        public object RequiredParentComponentDefinition { get; set; }
        public object RequiredComponentDefinition { get; set; }

        public Guid DependentComponentParentId { get; set; }
        public SolutionComponentType RequiredComponentType { get; set; }
        public Guid DependentComponentObjectId { get; set; }
        public Guid DependentComponentBaseSolutionid { get; set; }
        public SolutionComponentType DependentComponentType { get; set; }
        public Guid RequiredComponentBaseSolutionId { get; set; }
        public Guid RequiredComponentParentId { get; set; }
        public Guid RequiredComponentObjectId { get; set; }
        public DependencyType DependencyType { get; set; }
        

        public Dependency()
        {

        }
    }
}
