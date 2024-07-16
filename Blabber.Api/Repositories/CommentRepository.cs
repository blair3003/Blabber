using Blabber.Api.Data;
using Blabber.Api.Models;

namespace Blabber.Api.Repositories
{
    public class CommentRepository(ApplicationDbContext context) : ICommentRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Comment?> AddAsync(Comment comment)
        {
            var blab = await _context.Blabs.FindAsync(comment.BlabId);
            var author = await _context.Authors.FindAsync(comment.AuthorId);
            var parent = comment.ParentId.HasValue
                ? await _context.Comments.FindAsync(comment.ParentId.Value)
                : null;

            if (blab == null || author == null || (comment.ParentId.HasValue && parent == null))
            {
                return null;
            }

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

    }
}