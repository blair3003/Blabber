using Blabber.Api.Models;
using Blabber.Api.Repositories;

namespace Blabber.Api.Services
{
    public class BlabService(IBlabRepository repository) : IBlabService
    {
        private readonly IBlabRepository _repository = repository;

        public async Task<BlabFeed> GetBlabFeedAsync(int pageNumber, int pageSize)
        {
            var (blabs, totalCount) = await _repository.GetAsync(pageNumber, pageSize);

            return blabs.ToFeed(totalCount, pageNumber, pageSize);
        }

    }
}