﻿using Share_To_Learn_WEB_API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.DTOs
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public Student Student { get; set; }
    }
}
