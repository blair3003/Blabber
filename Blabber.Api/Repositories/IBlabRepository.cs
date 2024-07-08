using Blabber.Api.Models;

namespace Blabber.Api.Repositories
{
    public interface IBlabRepository
    {
        Task<(IEnumerable<Blab> Blabs, int TotalCount)> GetAsync(int pageNumber, int pageSize);
    }
}
