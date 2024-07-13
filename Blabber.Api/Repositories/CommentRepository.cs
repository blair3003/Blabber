using Blabber.Api.Data;
using Blabber.Api.Models;

namespace Blabber.Api.Repositories
{
    public class CommentRepository(ApplicationDbContext context) : ICommentRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Comment?> AddAsync(Comment comment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var blabTask = _context.Blabs.FindAsync(comment.BlabId);
                var authorTask = _context.Authors.FindAsync(comment.AuthorId);
                var parentTask = ValueTask.FromResult<Comment?>(null);

                if (comment.ParentId.HasValue)
                {
                    parentTask = _context.Comments.FindAsync(comment.ParentId.Value);
                }

                var blab = await blabTask;
                var author = await authorTask;
                var parent = await parentTask;

                if (blab == null)
                {
                    throw new ArgumentException("Invalid BlabId");
                }

                if (author == null)
                {
                    throw new ArgumentException("Invalid AuthorId");
                }

                if (comment.ParentId.HasValue && parent == null)
                {
                    throw new ArgumentException("Invalid ParentId");
                }

                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return comment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}