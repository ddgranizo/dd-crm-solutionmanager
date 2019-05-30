using SolutionManagerUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Models
{

    public enum CrmColor
    {
        Black = 1,
        Brown = 2,
        Orange = 3,
        Green = 4,
        Blue = 5
    }
    public class CrmConnection
    {

        public CrmColor Color { get; set; }
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public CrmConnection()
        {

        }

        public string GetStringConnetion()
        {
            return string.Format(@"ServiceUri={0}; Username={1}; Password={2}; authtype=Office365; RequireNewInstance=True; Timeout=00:30:00;",
                                    Endpoint,
                                    Username,
                                    Crypto.Decrypt(Password));
        }
    }
}
