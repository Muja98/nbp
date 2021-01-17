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

        public  async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }

        //public async Task SendMessage(Message mess, string groupName)
        //{
        //    //Clients.Group(groupName).SendAsync("Send",mess);
        //    await Clients.Group(groupName).SendAsync("ReceiveMessage", mess);
        //}







    }
}
