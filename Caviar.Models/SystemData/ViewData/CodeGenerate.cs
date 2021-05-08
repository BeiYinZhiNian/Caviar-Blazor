using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.Models.SystemData
{
    public class CodeGenerateData
    {
        public string EntityName { get; set; }

        public string OutName { get; set; }

        public string[] Page { get; set; }

        public string[] Button { get; set; }

        public string[] Config { get; set; }
    }
}
