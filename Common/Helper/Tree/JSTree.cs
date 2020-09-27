using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class JSTree
    {
        public string id { get; set; }
        public string text { get; set; }
        public List<JSTree> children { get; set; }
    }
}
