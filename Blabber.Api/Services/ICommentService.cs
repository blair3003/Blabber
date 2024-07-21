using Blabber.Api.Models;

namespace Blabber.Api.Services
{
    public interface ICommentService
    {
        Task<CommentView?> GetCommentByIdAsync(int id);
        Task<CommentView?> AddCommentAsync(CommentCreateRequest request);
        Task<CommentView?> UpdateCommentAsync(int id, CommentUpdateRequest request);
        Task<CommentView?> DeleteCommentAsync(int id);
    }
}