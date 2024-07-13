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

        public async Task<AuthorView?> GetAuthorByIdAsync(int id)
        {
            var author = await _repository.GetByIdAsync(id);

            return author?.ToView();
        }

        public async Task<string?> GetApplicationUserIdByAuthorIdAsync(int id)
        {
            var author = await _repository.GetByIdAsync(id);

            return author?.ApplicationUserId;
        }

        public async Task<AuthorView?> AddAuthorAsync(AuthorCreateRequest request)
        {
            var newAuthor = await _repository.AddAsync(request.ToAuthor());

            return newAuthor?.ToView();
        }

        public async Task<AuthorView?> UpdateAuthorAsync(int id, AuthorUpdateRequest request)
        {
            var updatedAuthor = await _repository.UpdateAsync(id, request);

            return updatedAuthor?.ToView();
        }
    }
}