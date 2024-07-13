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

        public async Task<Author?> GetByIdAsync(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Following)
                .Include(a => a.Followers)
                .FirstOrDefaultAsync(a => id == a.Id);

            return author;
        }

        public async Task<Author?> AddAsync(Author author)
        {
            var applicationUser = await _context.Users.FindAsync(author.ApplicationUserId);

            if (applicationUser == null)
            {
                return null;
            }

            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            return author;
        }

        public async Task<Author?> UpdateAsync(int id, AuthorUpdateRequest request)
        {
            if (id != request.Id)
            {
                return null;
            }

            var existingAuthor = await _context.Authors.FindAsync(id);

            if (existingAuthor == null)
            {
                return null;
            }

            existingAuthor.UpdateAuthor(request);

            await _context.SaveChangesAsync();

            return existingAuthor;
        }
    }
}