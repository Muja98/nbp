using Share_To_Learn_WEB_API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.DTOs
{
    public class GroupStatisticsDTO
    {
        public Group Group { get; set; }
        public int CountOfComments { get; set; }
        public int CountOfPosts { get; set; }
        public int CountOfMembers { get; set; }
    }
}
