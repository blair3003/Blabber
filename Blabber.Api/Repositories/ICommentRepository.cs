using Blabber.Api.Models;

namespace Blabber.Api.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment?> AddAsync(Comment comment);
    }
}