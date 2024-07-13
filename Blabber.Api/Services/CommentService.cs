using Blabber.Api.Models;
using Blabber.Api.Repositories;

namespace Blabber.Api.Services
{
    public class CommentService(ICommentRepository repository) : ICommentService
    {
        private readonly ICommentRepository _repository = repository;
        public async Task<CommentView?> AddCommentAsync(CommentCreateRequest request)
        {
            var newComment = await _repository.AddAsync(request.ToComment());

            return newComment?.ToView();
        }
    }

}