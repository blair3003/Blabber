using Blabber.Api.Models;

namespace Blabber.Api.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment?> AddAsync(CommentCreateRequest request);
        Task<Comment?> UpdateAsync(int id, CommentUpdateRequest request);
        Task<Comment?> DeleteAsync(int id);
    }
}