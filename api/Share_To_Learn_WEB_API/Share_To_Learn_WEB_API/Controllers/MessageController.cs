using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;
using Share_To_Learn_WEB_API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ISTLRepository _repository;

        public MessageController(ISTLRepository repository)
        {
            _repository = repository;
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
        public async Task<ActionResult> AddConversationTemp([FromBody] ConversationParticipantsDTO participants)
        {
            try
            { 
                await _repository.StartConversationTemp(participants);
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
    }
}