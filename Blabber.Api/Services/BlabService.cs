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

        public async Task<bool> AddBlabLikeAsync(int blabId, int authorId)
        {
            var like = await _repository.AddLikeAsync(blabId, authorId);

            return like;
        }

        public async Task<bool> RemoveBlabLikeAsync(int blabId, int authorId)
        {
            var unlike = await _repository.RemoveLikeAsync(blabId, authorId);

            return unlike;
        }
    }
}