using Blabber.Api.Models;
using Blabber.Api.Repositories;

namespace Blabber.Api.Services
{
    public class CommentService(ICommentRepository repository) : ICommentService
    {
        private readonly ICommentRepository _repository = repository;

        public async Task<CommentView?> GetCommentByIdAsync(int id)
        {
            var comment = await _repository.GetByIdAsync(id);

            return comment?.ToView();
        }

        public async Task<CommentView?> AddCommentAsync(CommentCreateRequest request)
        {
            var newComment = await _repository.AddAsync(request);

            return newComment?.ToView();
        }

        public async Task<CommentView?> UpdateCommentAsync(int id, CommentUpdateRequest request)
        {
            var updatedComment = await _repository.UpdateAsync(id, request);

            return updatedComment?.ToView();
        }
    }

}