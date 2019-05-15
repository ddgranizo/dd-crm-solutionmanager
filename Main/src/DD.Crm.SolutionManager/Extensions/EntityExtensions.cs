using DD.Crm.SolutionManager.Models;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Extensions
{
    public static class EntityExtensions
    {
        public static Solution ToSolution(this Entity e)
        {
            if (e.LogicalName != Solution.EntityLogicalName)
            {
                throw new InvalidCastException();
            }
            Solution s = new Solution();
            s.Id = e.GetParameter<Guid>(Solution.AttributeDefinitions.Id);
            s.CreatedBy = e.GetParameter<EntityReference>(Solution.AttributeDefinitions.CreatedBy);
            s.CreatedOn = e.GetParameter<DateTime>(Solution.AttributeDefinitions.CreatedOn);
            s.Description = e.GetParameter<string>(Solution.AttributeDefinitions.Description);
            s.DisplayName = e.GetParameter<string>(Solution.AttributeDefinitions.DisplayName);
            s.InstalledOn = e.GetParameter<DateTime>(Solution.AttributeDefinitions.InstalledOn);
            s.IsManaged = e.GetParameter<bool>(Solution.AttributeDefinitions.IsManaged);
            s.ModifiedBy = e.GetParameter<EntityReference>(Solution.AttributeDefinitions.ModifiedBy);
            s.ModifiedOn = e.GetParameter<DateTime>(Solution.AttributeDefinitions.ModifiedOn);
            s.Publisher = e.GetParameter<EntityReference>(Solution.AttributeDefinitions.Publisher);
            s.UniqueName = e.GetParameter<string>(Solution.AttributeDefinitions.UniqueName);
            s.Version = e.GetParameter<string>(Solution.AttributeDefinitions.Version);
            return s;
        }



        public static T GetParameter<T>(this Entity e, string parameter, T defaultValue = default(T))
        {
            if (e.Attributes.Contains(parameter))
            {
                return e.GetAttributeValue<T>(parameter);
            }
            return defaultValue;
        }

    }
}
