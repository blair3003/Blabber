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

        public async Task<Author?> GetByIdThinAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            return author;
        }

        public async Task<Author?> GetByUserIdAsync(string userId)
        {
            var author = await _context.Authors
                .FirstOrDefaultAsync(a => userId == a.ApplicationUserId);

            return author;
        }

        public async Task<Author?> AddAsync(AuthorCreateRequest request)
        {
            var author = request.ToAuthor();

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

        public async Task<bool> AddFollowerAsync(int authorId, int followerId)
        {
            if (authorId == followerId)
            {
                return false;
            }

            var author = await _context.Authors
                .Include(a => a.Followers)
                .FirstOrDefaultAsync(a => a.Id == authorId);

            var follower = await _context.Authors.FindAsync(followerId);

            if (author == null || follower == null || author.Followers.Contains(follower))
            {
                return false;
            }

            author.Followers.Add(follower);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveFollowerAsync(int authorId, int followerId)
        {
            if (authorId == followerId)
            {
                return false;
            }

            var author = await _context.Authors
                .Include(a => a.Followers)
                .FirstOrDefaultAsync(a => a.Id == authorId);

            var follower = await _context.Authors.FindAsync(followerId);

            if (author == null || follower == null || !author.Followers.Contains(follower))
            {
                return false;
            }

            author.Followers.Remove(follower);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}