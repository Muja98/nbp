using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Services.RepositoryContracts
{
    public interface ISharedRepository
    {
        Task<string> GetNextDocumentId();
        Task<string> GetNextImageId();
    }
}
