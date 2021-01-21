using Neo4jClient;
using Neo4jClient.Cypher;
using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;
using Share_To_Learn_WEB_API.Services.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Services.RepositoryServices
{
    public class PostRepository: IPostRepository
    {
        private readonly IGraphClient _client;

        public PostRepository(IGraphClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<PostDTO>> GetAllPosts(int groupId)
        {
            var result = await _client.Cypher
                         .Match("(gr:Group),(ps:Post), (st:Student), (ps)-[:BELONG]-(gr), (st)-[:WRITE]-(ps)")
                         .Where("ID(gr) = $groupId")
                         .WithParam("groupId", groupId)
                         .With("ps,{Id: ID(st), Student: st} as pst")
                         .Return((gr, pst) => new PostDTO
                         {
                             Id = Return.As<int>("ID(ps)"),
                             Post = Return.As<Post>("ps"),
                             Student = pst.As<StudentDTO>()
                         })
                        .ResultsAsync;
            return result;
        }

        public async Task<PostDTO> CreatePost(int groupId, int studentId, Post newPost)
        {
            IEnumerable<PostDTO> p = await _client.Cypher
            .Match("(st:Student), (gr:Group)")
            .Where("ID(gr) = $groupId")
            .WithParam("groupId", groupId)
            .AndWhere("ID(st) = $studentId")
            .WithParam("studentId", studentId)
            .Create("(st)-[:WRITE]->(ps:Post $newPost)")
            .WithParam("newPost", newPost)
            .Create("(ps)-[:BELONG]->(gr)")
            .With("ps,{Id: ID(st), Student: st} as pst")
            .Return((gr, pst) => new PostDTO
            {
                Id = Return.As<int>("ID(ps)"),
                Post = Return.As<Post>("ps"),
                Student = pst.As<StudentDTO>()
            })
            .ResultsAsync;
            PostDTO g = p.Single();

            return g;
        }

        public async Task UpdatePost(int postId, Post post)
        {
            await _client.Cypher
                   .Match("(ps:Post)")
                   .Where("ID(ps) = $postId")
                   .WithParam("postId", postId)
                   .Set("ps.Content = $content")
                   .WithParam("content", post.Content)
                   .ExecuteWithoutResultsAsync();
        }

        public async Task DeletePost(int postId)
        {
            await _client.Cypher
                    .Match("(co: Comment), (ps: Post), (co)-[:STOREDIN]->(ps)")
                    .Where("ID(ps) = $postId")
                    .WithParam("postId", postId)
                    .DetachDelete("co, ps")
                    .ExecuteWithoutResultsAsync();

            await _client.Cypher
                    .Match("(ps: Post)")
                    .Where("ID(ps) = $postId")
                    .WithParam("postId", postId)
                    .DetachDelete("ps")
                    .ExecuteWithoutResultsAsync();
        }

        public async Task<IEnumerable<CommentDTO>> GetAllComments(int postId)
        {
            var result = await _client.Cypher
                         .Match("(co:Comment),(ps:Post), (st:Student), (co)-[:STOREDIN]-(ps), (st)-[:WROTECOMMENT]-(co)")
                         .Where("ID(ps) = $postId")
                         .WithParam("postId", postId)
                         .With("co, {Id: ID(st), Student: st} as pst")
                         .Return((co, pst) => new CommentDTO
                         {
                             Id = Return.As<int>("ID(co)"),
                             Comment = Return.As<Comment>("co"),
                             Student = pst.As<StudentDTO>()
                         })
                        .ResultsAsync;
            return result;
        }

        public async Task<CommentDTO> CreateComment(int postId, int studentId, Comment newComment)
        {
            IEnumerable<CommentDTO> c = await _client.Cypher
            .Match("(st:Student), (ps:Post)")
            .Where("ID(ps) = $postId")
            .WithParam("postId", postId)
            .AndWhere("ID(st) = $studentId")
            .WithParam("studentId", studentId)
            .Create("(st)-[:WROTECOMMENT]->(co:Comment $newComment)")
            .WithParam("newComment", newComment)
            .Create("(co)-[:STOREDIN]->(ps)")
            .With("co, {Id: ID(st), Student: st} as pst")
            .Return((co, pst) => new CommentDTO
            {
                Id = Return.As<int>("ID(co)"),
                Comment = Return.As<Comment>("co"),
                Student = pst.As<StudentDTO>()
            })
            .ResultsAsync;

            CommentDTO g = c.Single();

            return g;
        }

        public async Task DeleteComment(int commentId)
        {
            await _client.Cypher
                .Match("(co:Comment)")
                .Where("ID(co) = $commentId")
                .WithParam("commentId", commentId)
                .DetachDelete("co")
                .ExecuteWithoutResultsAsync();
        }

        public async Task UpdateComment(int commentId, Comment comment)
        {
            await _client.Cypher
                 .Match("(co:Comment)")
                 .Where("ID(co) = $commentId")
                 .WithParam("commentId", commentId)
                 .Set("co.Content = $comment")
                 .WithParam("comment", comment.Content)
                 .ExecuteWithoutResultsAsync();
        }
    }
}
