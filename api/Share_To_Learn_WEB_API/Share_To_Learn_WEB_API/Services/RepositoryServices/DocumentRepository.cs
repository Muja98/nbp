using Neo4jClient;
using Neo4jClient.Cypher;
using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;
using Share_To_Learn_WEB_API.RedisConnection;
using Share_To_Learn_WEB_API.Services.RepositoryContracts;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Services.RepositoryServices
{
    public class DocumentRepository: IDocumentRepository
    {
        private readonly IGraphClient _client;
        private readonly IConnectionMultiplexer _redisConnection;

        public DocumentRepository(IGraphClient client, IRedisConnectionBuilder builder)
        {
            _client = client;
            _redisConnection = builder.Connection;
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

        public async Task<IEnumerable<DocumentDTO>> GetDocuments(int groupId, string filter)
        {
            var process = _client.Cypher
                .Match("(group: Group)-[:CONTAINS]->(document: Document)")
                .Where("ID(group) = $groupId")
                .WithParam("groupId", groupId);

            if (!string.IsNullOrEmpty(filter))
                process = process.AndWhere(filter);

            var res = await process.Return(() => new DocumentDTO
            {
                Id = Return.As<int>("ID(document)"),
                Name = Return.As<string>("document.Name"),
                Level = Return.As<string>("document.Level"),
                Description = Return.As<string>("document.Description"),
            })
                .OrderBy("ID(document) desc")
                .ResultsAsync;

            return res;
        }

        public async Task<string> GetDocumentsPath(int documentId)
        {
            var res = await _client.Cypher
                .Match("(document: Document)")
                .Where("ID(document) = $documentId")
                .WithParam("documentId", documentId)
                .Return<string>("document.DocumentPath")
                .ResultsAsync;

            return res.Single();
        }

        public async Task<string> GetNextDocumentPathId()
        {
            IDatabase db = _redisConnection.GetDatabase();
            long result = await db.StringIncrementAsync("next.document.id");

            return result.ToString();
        }
    }
}
