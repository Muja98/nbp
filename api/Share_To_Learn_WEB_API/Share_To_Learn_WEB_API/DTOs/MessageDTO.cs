using Share_To_Learn_WEB_API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.DTOs
{
    public class MessageDTO
    {
        public string Id { get; set; }
        public string Sender { get; set; }
        public int SenderId { get; set; }
        public string Receiver { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
