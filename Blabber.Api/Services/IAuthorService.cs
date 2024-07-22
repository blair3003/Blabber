using Blabber.Api.Models;

namespace Blabber.Api.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorView>> GetAllAuthorsAsync();
        Task<AuthorView?> GetAuthorByIdAsync(int id);
        Task<string?> GetApplicationUserIdByAuthorIdAsync(int id);
        Task<int?> GetAuthorIdByApplicationUserIdAsync(string id);
        Task<AuthorProfile?> GetAuthorProfileByIdAsync(int id);
        Task<AuthorView?> AddAuthorAsync(AuthorCreateRequest request, string applicationUserId);
        Task<AuthorView?> UpdateAuthorAsync(int id, AuthorUpdateRequest request);
        Task<bool> AddAuthorFollowerAsync(int authorId, int followerId);
        Task<bool> RemoveAuthorFollowerAsync(int authorId, int followerId);
    }
}