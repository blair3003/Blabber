using Blabber.Api.Models;

namespace Blabber.Api.Repositories
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAsync();
        Task<Author?> AddAsync(Author author);
    }
}