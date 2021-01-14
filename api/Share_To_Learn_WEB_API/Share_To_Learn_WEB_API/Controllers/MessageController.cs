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
            IEnumerable<MessageDTO> messages =  await _repository.ReceiveMessage(senderId, receiverId, from, count);
            return Ok(messages);
        }
    }
}