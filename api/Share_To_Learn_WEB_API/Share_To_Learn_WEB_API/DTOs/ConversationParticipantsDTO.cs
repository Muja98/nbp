﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.DTOs
{
    public class ConversationParticipantsDTO
    {
        public StudentDTO Sender { get; set; }
        public StudentDTO Receiver { get; set; }
    }
}
