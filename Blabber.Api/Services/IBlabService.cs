using Blabber.Api.Models;

namespace Blabber.Api.Services
{
    public interface IBlabService
    {
        Task<BlabPage> GetBlabPageAsync(int pageNumber, int pageSize);
    }
}
