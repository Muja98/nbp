﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.Entities;

namespace Share_To_Learn_WEB_API.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public Comment Comment { get; set; }
        public StudentDTO Student { get; set; }
    }
}
