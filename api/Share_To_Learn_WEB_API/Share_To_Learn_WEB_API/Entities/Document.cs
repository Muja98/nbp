using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Entities
{
    public class Document
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public string Description { get; set; }
        public string DocumentPath { get; set; }
    }
}
