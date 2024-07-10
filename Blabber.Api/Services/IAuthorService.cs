using Blabber.Api.Models;

namespace Blabber.Api.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorView>> GetAllAuthorsAsync();
        Task<AuthorView?> AddAuthorAsync(AuthorCreateRequest request, string applicationUserId);
    }
}