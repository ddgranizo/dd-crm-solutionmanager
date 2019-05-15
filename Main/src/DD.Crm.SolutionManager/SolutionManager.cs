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


        public List<SolutionComponentBase> GetSolutionComponents(Guid solutionId)
        {
            return CrmProvider.GetSolutionComponents(this._service, solutionId);
        }

    }
}
