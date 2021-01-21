using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Services.RepositoryContracts
{
    public interface IDocumentRepository
    {
        Task CreateDocument(int studentId, Document newDocument);
        Task RelateDocumentAndGroup(int groupId, string documentPath);
        Task<IEnumerable<DocumentDTO>> GetDocuments(int groupId, string filter);
        Task<string> GetDocumentsPath(int documentId);
        Task<string> GetNextDocumentPathId();
    }
}
