using Blabber.Api.Models;

namespace Blabber.Api.Repositories
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAsync();
        Task<Author?> GetByIdAsync(int id);
        Task<Author?> GetByIdThinAsync(int id);
        Task<Author?> GetByUserIdAsync(string userId);
        Task<Author?> AddAsync(AuthorCreateRequest request, string applicationUserId);
        Task<Author?> UpdateAsync(int id, AuthorUpdateRequest request);
        Task<bool> AddFollowerAsync(int authorId, int followerId);
        Task<bool> RemoveFollowerAsync(int authorId, int followerId);
    }
}