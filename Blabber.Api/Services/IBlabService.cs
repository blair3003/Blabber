using Blabber.Api.Models;

namespace Blabber.Api.Services
{
    public interface IBlabService
    {
        Task<BlabFeed> GetBlabFeedAsync(int pageNumber, int pageSize);
    }
}
