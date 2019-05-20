using SolutionManagerUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Models
{
    public class CrmConnection
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CrmConnection()
        {

        }

        public string GetStringConnetion()
        {
            return string.Format(@"ServiceUri={0}; Username={1}; Password={2}; authtype=Office365; RequireNewInstance=True;",
                                    Endpoint,
                                    Username,
                                    Crypto.Decrypt(Password));
        }
    }
}
