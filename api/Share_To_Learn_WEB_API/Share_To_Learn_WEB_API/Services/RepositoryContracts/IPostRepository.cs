using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Services.RepositoryContracts
{
    public interface IPostRepository
    {
        Task<IEnumerable<PostDTO>> GetAllPosts(int groupId);
        Task<PostDTO> CreatePost(int groupId, int studentId, Post newPost);
        Task UpdatePost(int postId, Post post);
        Task DeletePost(int postId);
        Task<IEnumerable<CommentDTO>> GetAllComments(int postId);
        Task<CommentDTO> CreateComment(int postId, int studentId, Comment newComment);
        Task DeleteComment(int commentId);
        Task UpdateComment(int commentId, Comment comment);
    }
}
