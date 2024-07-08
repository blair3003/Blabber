using Blabber.Api.Models;

namespace Blabber.Api.Services
{
    public interface IBlabService
    {
        Task<BlabPage> GetBlabsAsync(int pageNumber, int pageSize);
    }
}
