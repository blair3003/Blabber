using Blabber.Api.Models;
using Blabber.Api.Repositories;

namespace Blabber.Api.Services
{
    public class AuthorService(IAuthorRepository repository) : IAuthorService
    {
        private readonly IAuthorRepository _repository = repository;

        public async Task<IEnumerable<AuthorView>> GetAllAuthorsAsync()
        {
            var authors = await _repository.GetAllAsync();

            return authors.ToViewList();
        }

        public async Task<AuthorView?> AddAuthorAsync(AuthorCreateRequest request, string applicationUserId)
        {
            var newAuthor = await _repository.AddAsync(request.ToAuthor(applicationUserId));

            return newAuthor?.ToView();
        }
    }
}