using Blabber.Api.Models;

namespace Blabber.Api.Services
{
    public interface ICommentService
    {
        Task<CommentView?> AddCommentAsync(CommentCreateRequest request);
    }
}