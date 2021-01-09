using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.Entities;
using Neo4jClient;
using Share_To_Learn_WEB_API.DTOs;
using Neo4jClient.Cypher;

namespace Share_To_Learn_WEB_API.Services
{
    public class STLRepository : ISTLRepository
    {  
        private readonly IGraphClient _client;

        public STLRepository(IGraphClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<StudentDTO>> GetStudentsPage(string filter, string userFilter, string orderBy, bool descending, int from, int to)
        {
            var a = _client.Cypher
                        .Match("(s:Student)")
                        .Where(userFilter);

            if (!string.IsNullOrEmpty(filter))
                a = a.AndWhere(filter);

            ICypherFluentQuery<StudentDTO> ret = a.Return(() => new StudentDTO
            {
                Id = Return.As<int>("ID(s)"),
                Student = Return.As<Student>("s")
            });

            ret = ret.OrderBy(orderBy);

            return await ret.Skip(from).Limit(to).ResultsAsync;
        }

        public async Task<int> GetStudentsCount(string filter, string userFilter)
        {
            var a = _client.Cypher
                        .Match("(s:Student)")
                        .Where(userFilter);

            if (!string.IsNullOrEmpty(filter))
                a = a.AndWhere(filter);

            var res = await a.Return<int>("count(s)").ResultsAsync;
            return res.Single();
        }

        public async Task<StudentDTO> StudentExists(string email)
        {
            var res = await _client.Cypher
                            .Match("(student:Student)")
                            .Where((Student student) => student.Email == email)
                            .Return(() => new StudentDTO
                            {
                                Id = Return.As<int>("ID(student)"),
                                Student = Return.As<Student>("student")
                            }).ResultsAsync;

            return res.Single();
        }

        public async Task CreateStudent(Student newStudent)
        {
            await _client.Cypher
                .Create("(student:Student $newStudent)")
                .WithParam("newStudent", newStudent)
                .ExecuteWithoutResultsAsync();
        }

        public async Task<bool> CreateNonExistingStudent(StudentRegisterDTO newStudent)
        {
            Student rawStudent = newStudent.Student;
            string password = newStudent.Password;
            var res = await _client.Cypher
                            .Merge("(student:Student {Email: $email})")
                            .WithParam("email", rawStudent.Email)
                            .OnCreate().Set("student = $rawStudent, student.IsNew = true, student.Password = $password")
                            .WithParams(new
                            {
                                rawStudent = rawStudent,
                                password = password
                            })
                            .OnMatch().Set("student.IsNew = false")
                            .Return<bool>("student.IsNew")
                            .ResultsAsync;

            return res.Single();
        }

        public async Task<string> GetPassword(string email)
        {
            var res = await _client.Cypher
                            .Match("(student:Student)")
                            .Where((Student student) => student.Email == email)
                            .Return<string>("student.Password")
                            .ResultsAsync;

            if (res.Count() > 0)
                return res.Single();
            else
                return null;
        }

        public async Task CreateGroup(int ownerId, Group newGroup)
        {
            await _client.Cypher
                    .Match("(owner: Student)")
                    .Where("ID(owner) = $ownerId")
                    .WithParam("ownerId", ownerId)
                    .Create("(owner)-[:OWNER]->(group:Group $newGroup)")
                    .WithParam("newGroup", newGroup)
                    .ExecuteWithoutResultsAsync();
        }

        public async Task<bool> CheckIfStudentIsMemberOfAGroup(int studentId, int groupId)
        {
            var res = await _client.Cypher

                            .Match("(st:Student)-[MEMBER]-(gr:Group)")
                            .Where("ID(st) = $studentId")
                            .WithParam("studentId", studentId)
                            .AndWhere("ID(gr) = $groupId")
                            .WithParam("groupId", groupId)
                            .Return(st => st.As<Student>())
                            .ResultsAsync;
            return res.Count() == 0;
        }

        public async Task AddStudentToGroup(int studentId, int groupId)
        {
            await _client.Cypher
                             .Match("(st:Student),(gr:Group)")
                             .Where("ID(st) = $studentId")
                             .WithParam("studentId", studentId)
                             .AndWhere("ID(gr) = $groupId")
                             .WithParam("groupId", groupId)
                             .Create("(st)-[:MEMBER]->(gr)")
                             .ExecuteWithoutResultsAsync();
        }

        public async Task RemoveStudentFromGroup(int studentId, int groupId)
        {
            await _client.Cypher
                            .Match("(st:Student),(gr:Group)")
                            .Match("(st)-[r:MEMBER]-(gr)")
                            .Where("ID(st) = $studentId")
                            .WithParam("studentId", studentId)
                            .AndWhere("ID(gr) = $groupId")
                            .WithParam("groupId", groupId)
                            .Delete("r")
                            .ExecuteWithoutResultsAsync();
        }

        public async Task UpdateGroup(int groupId, Group updatedGroup)
        {
            await _client.Cypher
               .Match("(group: Group)")
               .Where("ID(group) = $groupId")
               .WithParam("groupId", groupId)
               .Set("group = $updatedGroup")
               .WithParam("updatedGroup", updatedGroup)
               .ExecuteWithoutResultsAsync();
        }

        public async Task UpdateStudent(int studentId, Student updatedStudent)
        {
            await _client.Cypher
                .Match("(student: Student)")
                .Where("ID(student) = $studentId")
                .WithParam("studentId", studentId)
                .Set("student.FirstName = $FirstName")
                .WithParam("FirstName", updatedStudent.FirstName)
                .Set("student.LastName = $LastName")
                .WithParam("LastName", updatedStudent.LastName)
                .Set("student.DateOfBirth = $DateOfBirth")
                .WithParam("DateOfBirth", updatedStudent.DateOfBirth)
                .Set("student.ProfilePicturePath = $ProfilePicturePath")
                .WithParam("ProfilePicturePath", updatedStudent.ProfilePicturePath)
                .ExecuteWithoutResultsAsync();
        }

        public async Task<bool> StudentExists(int studentId)
        {
            var res = await _client.Cypher
                .Match("(student:Student)")
                .Where("ID(student) = $studentId")
                .WithParam("studentId", studentId)
                .Return<int>("count(student)")
                .ResultsAsync;

            return res.Single() > 0;
        }

        public async Task<bool> GroupExists(int groupId)
        {
            var res = await _client.Cypher
                .Match("(group: Group)")
                .Where("ID(group) = $groupId")
                .WithParam("groupId", groupId)
                .Return<int>("count(group)")
                .ResultsAsync;

            return res.Single() > 0;
        }

        public async Task<IEnumerable<GroupDTO>> GetGroupsPage(string filters, string userFilter, string orderBy, bool descending, int from, int to)
        {
            var a = _client.Cypher
                        .Match("(g:Group), (s:Student), (ow)-[:OWNER]-(g)")
                        .Where(userFilter);

            if (!string.IsNullOrEmpty(filters))
                a = a.AndWhere(filters);

            a = a.With("g, {Id: ID(ow), Student: ow} as st");

            ICypherFluentQuery<GroupDTO> ret = a.Return((g, st) => new GroupDTO
            {
                Id = Return.As<int>("ID(g)"),
                Group = Return.As<Group>("g"),
                Student = st.As<StudentDTO>()
            });

            if (descending)
                ret = ret.OrderByDescending(orderBy);
            else
                ret = ret.OrderBy(orderBy);

            return await ret.Skip(from).Limit(to).ResultsAsync;
        }


        public async Task<IEnumerable<GroupDTO>> GetMemberships(int studentId)
        {
            var result = await _client.Cypher
                            .Match("(s)-[r:MEMBER]-(g), (ow)-[rel:OWNER]-(g)")
                            .Where("ID(s) = $studentId")
                            .WithParam("studentId", studentId)
                            .With("g, {Id: ID(ow), Student: ow} as st")
                            .Return((g, st) => new GroupDTO
                            {
                                Id = Return.As<int>("ID(g)"),
                                Group = Return.As<Group>("g"),
                                Student = st.As<StudentDTO>()
                            })
                            .ResultsAsync;

            return result;
        }

        public async Task<IEnumerable<GroupDTO>> GetOwnerships(int studentId)
        {
            var result = await _client.Cypher
                .Match("(ow)-[r:OWNER]-(g)")
                .Where("ID(ow) = $studentId")
                .WithParam("studentId", studentId)
                .With("g, {Id: ID(ow), Student: ow} as st")
                .Return((g, st) => new GroupDTO
                {
                    Id = Return.As<int>("ID(g)"),
                    Group = Return.As<Group>("g"),
                    Student = st.As<StudentDTO>()
                })
                .ResultsAsync;

            return result;
        }
      

        public async Task<int> GetGroupsCount(string filters, string userFilter)
        {
            var a = _client.Cypher
                        .Match("(g:Group), (s:Student), (ow)-[:OWNER]-(g)")
                        .Where(userFilter);

            if (!string.IsNullOrEmpty(filters))
                a = a.AndWhere(filters);

            var res = await a.Return<int>("count(g)").ResultsAsync;
            return res.Single();
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

        public async Task CreatePost(int groupId, int studentId, Post newPost)
        {
             await _client.Cypher
            .Match("(st:Student), (gr:Group)")
            .Where("ID(gr) = $groupId")
            .WithParam("groupId", groupId)
            .AndWhere("ID(st) = $studentId")
            .WithParam("studentId", studentId)
            .Create("(st)-[:WRITE]->(ps:Post $newPost)")
            .WithParam("newPost",newPost)
            .Create("(ps)-[:BELONG]->(gr)")
            .ExecuteWithoutResultsAsync();
        }
        
        public async Task<IEnumerable<CommentDTO>> GetAllComment(int postId)
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

        public async Task CreateComment(int postId, int studentId, Comment newComment)
        {
            await _client.Cypher
            .Match("(st:Student), (ps:Post)")
            .Where("ID(ps) = $postId")
            .WithParam("postId", postId)
            .AndWhere("ID(st) = $studentId")
            .WithParam("studentId", studentId)
            .Create("(st)-[:WROTECOMMENT]->(co:Comment $newComment)")
            .WithParam("newComment", newComment)
            .Create("(co)-[:STOREDIN]->(ps)")
            .ExecuteWithoutResultsAsync();
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
        }

        public async Task<IEnumerable<StudentDTO>> GetGroupMembers(int groupId)
        {
            return await _client.Cypher
                    .Match("(s:Student)-[:MEMBER]-(g:Group)")
                    .Where("ID(g) = $groupId")
                    .WithParam("groupId", groupId)
                    .Return(() => new StudentDTO
                    {
                        Id = Return.As<int>("ID(s)"),
                        Student = Return.As<Student>("s")
                    }).ResultsAsync;
        }

        public async Task<StudentDTO> GetGroupOwner(int groupId)
        {
            var res = await _client.Cypher
                    .Match("(s:Student)-[:OWNER]-(g:Group)")
                    .Where("ID(g) = $groupId")
                    .WithParam("groupId", groupId)
                    .Return(() => new StudentDTO
                    {
                        Id = Return.As<int>("ID(s)"),
                        Student = Return.As<Student>("s")
                    }).ResultsAsync;

            return res.Single();
        }

        public async Task<GroupStatisticsDTO> GetGroupStatistics(int groupId)
        {
            var group = await _client.Cypher
                .Match("(g:Group)")
                .Where("ID(g) = $groupId")
                .WithParam("groupId", groupId)
                .Return(() => new GroupStatisticsDTO
                {
                    Group = Return.As<Group>("g"),
                }).ResultsAsync;

            var countOfPosts = await _client.Cypher
                .Match("(p:Post)-[:BELONG]->(g:Group)")
                .Where("ID(g) = $groupId")
                .WithParam("groupId", groupId)
                .Return<int>("count(distinct p)")
                .ResultsAsync;

            var countOfComments= await _client.Cypher
                .Match("(c:Comment)-[:STOREDIN]->(p:Post)-[:BELONG]->(g:Group)")
                .Where("ID(g) = $groupId")
                .WithParam("groupId", groupId)
                .Return<int>("count(distinct c)")
                .ResultsAsync;

            var countOfMembers = await _client.Cypher
                .Match("(st:Student)-[:Member]-> (g:Group)")
                .Where("ID(g) = $groupId")
                .WithParam("groupId", groupId)
                .Return<int>("count(distinct st)")
                .ResultsAsync;

            group.Single().CountOfMembers = countOfMembers.Single();
            group.Single().CountOfPosts = countOfPosts.Single();
            group.Single().CountOfComments = countOfComments.Single();

            return group.Single();

        }

        public async Task AddFriend(int studentId1, int studentId2)
        {
            await _client.Cypher
                   .Match("(st1:Student), (st2:Student)")
                   .Where("ID(st1) = $studentId1")
                   .WithParam("studentId1", studentId1)
                   .AndWhere("ID(st2) = $studentId2")
                   .WithParam("studentId2", studentId2)
                   .Create("(st1)-[:FRIEND]->(st2)")
                   .Create("(st2)-[:FRIEND]->(st1)")
                   .ExecuteWithoutResultsAsync();
        }

        public async Task RemoveFriend(int studentId1, int studentId2)
        {
            await _client.Cypher
                   .Match("(st1:Student), (st2:Student),(st1)-[f1:FRIEND]->(st2), (st2)-[f2:FRIEND]->(st1)")
                   .Where("ID(st1) = $studentId1")
                   .WithParam("studentId1", studentId1)
                   .AndWhere("ID(st2) = $studentId2")
                   .WithParam("studentId2", studentId2)
                   .Delete("f1, f2")
                   .ExecuteWithoutResultsAsync();
        }

        public async Task CreateDocument(int studentId, Document newDocument)
        {
            await _client.Cypher
                .Match("(creator: Student)")
                .Where("ID(creator) = $studentId")
                .WithParam("studentId", studentId)
                .Create("(creator)-[:CREATED]->(document:Document $newDocument)")
                .WithParam("newDocument", newDocument)
                .ExecuteWithoutResultsAsync();
        }

        public async Task RelateDocumentAndGroup(int groupId, string documentPath)
        {
            await _client.Cypher
                .Match("(group: Group), (document: Document)")
                .Where("ID(group) = $groupId")
                .WithParam("groupId", groupId)
                .AndWhere("document.DocumentPath = $documentPath")
                .WithParam("documentPath", documentPath)
                .Create("(group)-[:CONTAINS]->(document)")
                .ExecuteWithoutResultsAsync();
        }

        public async Task<IEnumerable<DocumentDTO>> GetDocuments(int groupId)
        {
            var res = await _client.Cypher
                    .Match("(group: Group)-[:CONTAINS]->(document: Document)")
                    .Where("ID(group) = $groupId")
                    .WithParam("groupId", groupId)
                    .Return(() => new DocumentDTO
                    {
                         Id = Return.As<int>("ID(document)"),
                         Document = Return.As<Document>("document")
                    }
                    ).ResultsAsync;

            return res;
        }


    }
}