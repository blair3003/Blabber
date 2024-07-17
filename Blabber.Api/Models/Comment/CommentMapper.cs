namespace Blabber.Api.Models
{
    public static class CommentMapper
    {
        public static CommentView ToView(this Comment comment)
        {
            return new CommentView
            {
                Id = comment.Id,
                Body = comment.Body,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                Author = comment.Author?.ToView(),
                Children = comment.Children.Select(child => child.ToView()).ToList()
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

        public static CommentUpdateRequest ToCommentUpdateRequest(this Comment comment)
        {
            return new CommentUpdateRequest
            {
                Id = comment.Id,
                Body = comment.Body
            };
        }

        public static void UpdateComment(this Comment comment, CommentUpdateRequest request)
        {
            comment.Body = request.Body;
            comment.UpdatedAt = DateTime.UtcNow;
        }
    }
}