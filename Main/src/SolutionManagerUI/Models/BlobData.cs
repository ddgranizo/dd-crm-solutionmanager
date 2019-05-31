using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Models
{
    public class BlobData
    {
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnString { get { return CreatedOn.ToString();  } }
    }
}
