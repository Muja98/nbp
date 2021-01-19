using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;
using Share_To_Learn_WEB_API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.HubConfig;
using Microsoft.AspNetCore.SignalR;

namespace Share_To_Learn_WEB_API.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ISTLRepository _repository;
        private readonly IHubContext<MessageHub> _hub;
        public MessageController(ISTLRepository repository, IHubContext<MessageHub> hub)
        {
            _repository = repository;
            _hub = hub;
        }

        [HttpPost]
        [Route("send")]
        public async Task<ActionResult> SendMessage([FromBody]Message message)
        {
            await _repository.SendMessage(message);
            return Ok();
        }

        [HttpGet]
        [Route("receive")]
        public async Task<ActionResult> ReceiveMessage([FromQuery]int senderId, [FromQuery]int receiverId, [FromQuery]string from, [FromQuery]int count)
        {
            var messages =  await _repository.ReceiveMessage(senderId, receiverId, from, count);
            return Ok(messages);
        }

        [HttpPost]
        [Route("add-conversation/temp")]
        public async Task<ActionResult> StartConversation([FromBody] ConversationDTO participants)
        {
            try
            { 
                await _repository.StartConversation(participants);
                var message = new Message
                {
                    Sender = $"{participants.Sender.Student.FirstName} {participants.Sender.Student.LastName}",
                    SenderId = participants.Sender.Id,
                    Receiver = $"{participants.Receiver.Student.FirstName} {participants.Receiver.Student.LastName}",
                    ReceiverId = participants.Receiver.Id,
                    Content = participants.FirstMessage
                };

                await _repository.SendMessage(message);
                await _repository.SetTimeToLiveForStream(participants.Sender.Id, participants.Receiver.Id);

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("chats/student/{studentId}")]
        public async Task<ActionResult> GetStudentsInChatWith(int studentId)
        {
            var students = await _repository.GetStudentsInChatWith(studentId);
            return Ok(students);
        }

        [HttpGet]
        [Route("chat-ids/student/{studentId}")]
        public async Task<ActionResult> GetIdsStudentsInChatWith(int studentId)
        {
            var ids = await _repository.GetIdsStudentsInChatWith(studentId);
            return Ok(ids);
        }

        [HttpDelete]
        [Route("deleteConversation/user/{biggerId}/{smallerId}")]
        
        public async Task<IActionResult> DeleteConversation(int biggerId, int smallerId)
        {
            await _repository.DeleteConversation(biggerId, smallerId);
            return Ok();
        }
    }
}