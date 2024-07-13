using Blabber.Api.Models;

namespace Blabber.Api.Services
{
    public interface IBlabService
    {
        Task<BlabFeed> GetBlabFeedAsync(int pageNumber, int pageSize);
        Task<BlabView?> GetBlabByIdAsync(int id);
        Task<BlabView?> AddBlabAsync(BlabCreateRequest request);
        Task<BlabView?> UpdateBlabAsync(int id, BlabUpdateRequest request);
    }
}