using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Entities
{
    public class Post
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateOfPublishing { get; set; }
    }
}
