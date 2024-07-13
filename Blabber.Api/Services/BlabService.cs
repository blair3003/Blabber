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

        public async Task<BlabView?> GetBlabByIdAsync(int id)
        {
            var blab = await _repository.GetByIdAsync(id);

            return blab?.ToView();
        }

        public async Task<BlabView?> AddBlabAsync(BlabCreateRequest request)
        {
            var newBlab = await _repository.AddAsync(request.ToBlab());

            return newBlab?.ToView();
        }

        public async Task<BlabView?> UpdateBlabAsync(int id, BlabUpdateRequest request)
        {
            var updatedBlab = await _repository.UpdateAsync(id, request);

            return updatedBlab?.ToView();
        }

    }
}