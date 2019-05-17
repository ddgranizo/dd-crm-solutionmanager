using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Models
{
    public class Setting
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Setting()
        {

        }
        public Setting(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
