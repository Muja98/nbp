using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Entities
{
    public class Message
    {
        public string Sender { get; set; }
        public int SenderId { get; set; }
        public string Receiver { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
