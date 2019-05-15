using DD.Crm.SolutionManager.Extensions;
using DD.Crm.SolutionManager.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.Crm.SolutionManager.Utilities
{
    public static class CrmProvider
    {



        public static List<Solution> GetSolutions(IOrganizationService service)
        {
            QueryExpression qe = new QueryExpression(Solution.EntityLogicalName);
            qe.ColumnSet = new ColumnSet(true);
            return service.RetrieveMultiple(qe)
                    .Entities
                    .Select(k => { return k.ToSolution(); })
                    .ToList();
        }


        public static List<SolutionComponentBase> GetSolutionComponents(IOrganizationService service, Guid solutionId)
        {
            QueryExpression qe = new QueryExpression(SolutionComponentBase.EntityLogicalName);
            FilterExpression fe = new FilterExpression();
            fe.AddCondition(SolutionComponentBase.AttributeDefinitions.SolutionId, ConditionOperator.Equal, solutionId);
            qe.Criteria = fe;
            qe.ColumnSet = new ColumnSet(true);
            return service.RetrieveMultiple(qe)
                     .Entities
                     .Select(k => { return k.ToSolutionComponent(); })
                     .ToList();
        }


        public static IOrganizationService GetService(string stringConnection)
        {
            CrmServiceClient crmService = new CrmServiceClient(stringConnection);
            IOrganizationService serviceProxy = crmService.OrganizationWebProxyClient != null ?
                                                        crmService.OrganizationWebProxyClient :
                                                        (IOrganizationService)crmService.OrganizationServiceProxy;
            return serviceProxy;
        }

    }
}
