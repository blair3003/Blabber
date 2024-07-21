using Blabber.Api.Data;
using Blabber.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Blabber.Api.Repositories
{
    public class BlabRepository(ApplicationDbContext context) : IBlabRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<(IEnumerable<Blab> Blabs, int TotalCount)> GetAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Blabs.CountAsync(b => !b.IsDeleted);

            var blabs = await _context.Blabs
                .Where(b => !b.IsDeleted)
                .Include(b => b.Author)
                .Include(b => b.Liked)
                .Include(b => b.Comments)
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (blabs, totalCount);
        }

        public async Task<Blab?> GetByIdAsync(int id)
        {
            var blab = await _context.Blabs
                .Where(b => !b.IsDeleted)
                .Include(b => b.Author)
                .Include(b => b.Liked)
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(b => id == b.Id);

            return blab;
        }

        public async Task<Blab?> AddAsync(BlabCreateRequest request)
        {
            var blab = request.ToBlab();

            var author = await _context.Authors.FindAsync(blab.AuthorId);

            if (author == null)
            {
                return null;
            }

            await _context.Blabs.AddAsync(blab);
            await _context.SaveChangesAsync();

            return blab;
        }

        public async Task<Blab?> UpdateAsync(int id, BlabUpdateRequest request)
        {
            if (id != request.Id)
            {
                return null;
            }

            var existingBlab = await _context.Blabs.FindAsync(id);

            if (existingBlab == null || existingBlab.IsDeleted)
            {
                return null;
            }

            existingBlab.UpdateBlab(request);
            await _context.SaveChangesAsync();

            return existingBlab;
        }

        public async Task<Blab?> DeleteAsync(int id)
        {
            var existingBlab = await _context.Blabs
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(b => id == b.Id);

            if (existingBlab == null || existingBlab.IsDeleted)
            {
                return null;
            }

            existingBlab.DeleteBlab();

            //await _context.Database.ExecuteSqlRaw("DELETE FROM Likes WHERE LikesId = {0}", id);

            await _context.SaveChangesAsync();

            return existingBlab;
        }

        public async Task<bool> AddLikeAsync(int blabId, int authorId)
        {
            var blab = await _context.Blabs
                .Include(b => b.Liked)
                .FirstOrDefaultAsync(b => b.Id == blabId);

            var author = await _context.Authors.FindAsync(authorId);

            if (blab == null || author == null || blab.Liked.Contains(author) || blab.IsDeleted)
            {
                return false;
            }

            blab.Liked.Add(author);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveLikeAsync(int blabId, int authorId)
        {
            var blab = await _context.Blabs
                .Include(b => b.Liked)
                .FirstOrDefaultAsync(b => b.Id == blabId);

            var author = await _context.Authors.FindAsync(authorId);

            if (blab == null || author == null || !blab.Liked.Contains(author) || blab.IsDeleted)
            {
                return false;
            }

            blab.Liked.Remove(author);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}