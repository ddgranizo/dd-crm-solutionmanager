﻿using Executer.Utilities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executer
{
    class Program
    {
        static void Main(string[] args)
        {
            var userName = SecretManager.GetSecret("username", "YourUsernameHere");
            var password = SecretManager.GetSecret("password", "YourPasswordHere");
            var endpoint = ConfigManager.GetAppConfig("crmLab");
            var strConnection = string.Format(endpoint, userName, password);
            IOrganizationService service = GetService(strConnection);

        }


        static IOrganizationService GetService(string stringConnection)
        {
            CrmServiceClient crmService = new CrmServiceClient(stringConnection);
            IOrganizationService serviceProxy = crmService.OrganizationWebProxyClient != null ?
                                                        crmService.OrganizationWebProxyClient :
                                                        (IOrganizationService)crmService.OrganizationServiceProxy;
            return serviceProxy;
        }
    }
}