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
            var totalCount = await _context.Blabs.CountAsync();

            var blabs = await _context.Blabs
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
                .Include(b => b.Author)
                .Include(b => b.Liked)
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(b => id == b.Id);

            return blab;
        }

        public async Task<Blab?> AddAsync(Blab blab)
        {
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

            if (existingBlab == null)
            {
                return null;
            }

            existingBlab.UpdateBlab(request);

            await _context.SaveChangesAsync();

            return existingBlab;
        }

        public async Task AddLikeAsync(int blabId, int authorId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var blabTask = _context.Blabs
                    .Include(b => b.Liked)
                    .FirstOrDefaultAsync(b => b.Id == blabId);

                var authorTask = _context.Authors
                    .FirstOrDefaultAsync(a => a.Id == authorId);

                await Task.WhenAll(blabTask, authorTask);

                var blab = await blabTask;
                var author = await authorTask;

                if (blab == null)
                {
                    throw new KeyNotFoundException("Blab not found");
                }

                if (author == null)
                {
                    throw new KeyNotFoundException("Author not found");
                }

                if (blab.Liked.Contains(author))
                {
                    throw new InvalidOperationException("Author already likes Blab!");
                }

                blab.Liked.Add(author);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveLikeAsync(int blabId, int authorId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var blabTask = _context.Blabs
                    .Include(b => b.Liked)
                    .FirstOrDefaultAsync(b => b.Id == blabId);

                var authorTask = _context.Authors
                    .FirstOrDefaultAsync(a => a.Id == authorId);

                await Task.WhenAll(blabTask, authorTask);

                var blab = await blabTask;
                var author = await authorTask;

                if (blab == null)
                {
                    throw new KeyNotFoundException("Blab not found");
                }

                if (author == null)
                {
                    throw new KeyNotFoundException("Author not found");
                }

                if (!blab.Liked.Contains(author))
                {
                    throw new InvalidOperationException("Author does not already like Blab!");
                }

                blab.Liked.Remove(author);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}