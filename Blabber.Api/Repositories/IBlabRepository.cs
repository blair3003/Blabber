using Blabber.Api.Models;

namespace Blabber.Api.Repositories
{
    public interface IBlabRepository
    {
        Task<(IEnumerable<Blab> Blabs, int TotalCount)> GetAsync(int pageNumber, int pageSize);
        Task<Blab?> GetByIdAsync(int id);
        Task<Blab?> AddAsync(BlabCreateRequest request);
        Task<Blab?> UpdateAsync(int id, BlabUpdateRequest request);
        Task<Blab?> DeleteAsync(int id);
        Task<bool> AddLikeAsync(int blabId, int authorId);
        Task<bool> RemoveLikeAsync(int blabId, int authorId);
    }
}