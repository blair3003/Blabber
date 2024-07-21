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
                .Where(c => !c.IsDeleted)
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == id);

            return comment;
        }

        public async Task<Comment?> AddAsync(CommentCreateRequest request)
        {
            var blab = await _context.Blabs
                .Where(b => !b.IsDeleted)
                .FirstOrDefaultAsync(b => b.Id == request.BlabId);

            if (blab == null)
            {
                return null;
            }

            var author = await _context.Authors.FindAsync(request.AuthorId);

            if (author == null)
            {
                return null;
            }

            var parent = request.ParentId.HasValue
                ? await _context.Comments.FindAsync(request.ParentId.Value)
                : null;

            if (request.ParentId.HasValue && parent == null)
            {
                return null;
            }

            var comment = request.ToComment();

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

            if (existingComment == null || existingComment.IsDeleted)
            {
                return null;
            }

            existingComment.UpdateComment(request);
            await _context.SaveChangesAsync();

            return existingComment;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var existingComment = await _context.Comments.FindAsync(id);

            if (existingComment == null)
            {
                return null;
            }

            existingComment.DeleteComment();
            await _context.SaveChangesAsync();

            return existingComment;
        }

    }
}