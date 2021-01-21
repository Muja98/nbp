using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.DTOs
{
    public class FriendRequestNotificationDTO
    {
        public int ReceiverId { get; set; }
        public RequestDTO RequestDTO { get; set; }
    }
}