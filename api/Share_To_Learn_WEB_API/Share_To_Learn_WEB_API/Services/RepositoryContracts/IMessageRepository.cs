using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Services.RepositoryContracts
{
    public interface IMessageRepository
    {
        Task SendMessage(Message message);
        Task<IEnumerable<MessageDTO>> ReceiveMessage(int senderId, int receiverId, string from, int count);
        Task StartConversation(ConversationDTO participants);
        Task SetTimeToLiveForStream(int senderId, int receiverId);
        Task<IEnumerable<StudentDTO>> GetStudentsInChatWith(int studentId);
        Task<IEnumerable<int>> GetIdsStudentsInChatWith(int studentId);
        Task DeleteConversation(int biggerId, int smallerId);
    }
}
