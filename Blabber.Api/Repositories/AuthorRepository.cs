using Blabber.Api.Data;
using Blabber.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Blabber.Api.Repositories
{
    public class AuthorRepository(ApplicationDbContext context) : IAuthorRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            var authors = await _context.Authors
                .Include(a => a.Following)
                .Include(a => a.Followers)
                .ToListAsync();

            return authors;
        }

        public async Task<Author?> AddAsync(Author author)
        {
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            return author;
        }
    }
}