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

    }
}