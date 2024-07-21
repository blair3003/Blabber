namespace Blabber.Api.Models
{
    public static class CommentMapper
    {
        public static CommentView ToView(this Comment comment)
        {
            return new CommentView
            {
                Id = comment.Id,
                ParentId = comment.ParentId,
                Body = !comment.IsDeleted ? comment.Body : "[Removed]",
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                Author = !comment.IsDeleted ? comment.Author?.ToView() : null,
                IsDeleted = comment.IsDeleted
            };
        }

        public static Comment ToComment(this CommentCreateRequest request)
        {
            return new Comment
            {
                BlabId = request.BlabId,
                AuthorId = request.AuthorId,
                ParentId = request.ParentId,
                Body = request.Body
            };
        }

        public static void UpdateComment(this Comment comment, CommentUpdateRequest request)
        {
            comment.Body = request.Body;
            comment.UpdatedAt = DateTime.UtcNow;
        }

        public static void DeleteComment(this Comment comment)
        {
            comment.IsDeleted = true;
            comment.UpdatedAt = DateTime.UtcNow;
        }
    }
}