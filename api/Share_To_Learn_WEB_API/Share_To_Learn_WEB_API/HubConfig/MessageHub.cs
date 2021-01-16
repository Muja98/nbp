using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Share_To_Learn_WEB_API.Entities;

namespace Share_To_Learn_WEB_API.HubConfig
{
    public class MessageHub : Hub
    {
        public void sendToAll(Message mess)
        {
            Clients.All.SendAsync("sendToAll", mess);
        }
    }
}
