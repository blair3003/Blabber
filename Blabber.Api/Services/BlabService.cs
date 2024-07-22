using Blabber.Api.Models;
using Blabber.Api.Repositories;

namespace Blabber.Api.Services
{
    public class BlabService(IBlabRepository repository) : IBlabService
    {
        private readonly IBlabRepository _repository = repository;

        public async Task<BlabFeed> GetBlabFeedAsync(int pageNumber, int pageSize, int? authorId = null)
        {
            var (blabs, totalCount) = await _repository.GetAsync(pageNumber, pageSize, authorId);

            return blabs.ToFeed(totalCount, pageNumber, pageSize);
        }

        public async Task<BlabView?> GetBlabByIdAsync(int id)
        {
            var blab = await _repository.GetByIdAsync(id);

            return blab?.ToView();
        }

        public async Task<BlabView?> AddBlabAsync(BlabCreateRequest request)
        {
            var newBlab = await _repository.AddAsync(request);

            return newBlab?.ToView();
        }

        public async Task<BlabView?> UpdateBlabAsync(int id, BlabUpdateRequest request)
        {
            var updatedBlab = await _repository.UpdateAsync(id, request);

            return updatedBlab?.ToView();
        }

        public async Task<BlabView?> DeleteBlabAsync(int id)
        {
            var deletedBlab = await _repository.DeleteAsync(id);

            return deletedBlab?.ToView();
        }

        public async Task<bool> AddBlabLikeAsync(int blabId, int authorId)
        {
            var liked = await _repository.AddLikeAsync(blabId, authorId);

            return liked;
        }

        public async Task<bool> RemoveBlabLikeAsync(int blabId, int authorId)
        {
            var unliked = await _repository.RemoveLikeAsync(blabId, authorId);

            return unliked;
        }
    }
}