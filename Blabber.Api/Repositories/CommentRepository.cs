using Blabber.Api.Data;
using Blabber.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Blabber.Api.Repositories
{
    public class CommentRepository(ApplicationDbContext context) : ICommentRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Comment?> GetByIdAsync(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == id);

            return comment;
        }

        public async Task<Comment?> AddAsync(CommentCreateRequest request)
        {
            var comment = request.ToComment();
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

        public async Task<Comment?> UpdateAsync(int id, CommentUpdateRequest request)
        {
            if (id != request.Id)
            {
                return null;
            }

            var existingComment = await _context.Comments.FindAsync(id);

            if (existingComment == null)
            {
                return null;
            }

            existingComment.UpdateComment(request);
            await _context.SaveChangesAsync();

            return existingComment;
        }

    }
}