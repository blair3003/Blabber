using Blabber.Api.Models;

namespace Blabber.Api.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorView>> GetAllAuthorsAsync();
        Task<AuthorView?> GetAuthorByIdAsync(int id);
        Task<string?> GetApplicationUserIdByAuthorIdAsync(int id);
        Task<int?> GetAuthorIdByApplicationUserIdAsync(string id);
        Task<AuthorView?> AddAuthorAsync(AuthorCreateRequest request);
        Task<AuthorView?> UpdateAuthorAsync(int id, AuthorUpdateRequest request);
    }
}