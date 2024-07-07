namespace Blabber.Api.Models
{
    public static class BlabMapper
    {
        public static BlabView ToView(this Blab blab)
        {
            return new BlabView
            {
                Id = blab.Id,
                Body = blab.Body,
                CreatedAt = blab.CreatedAt,
                UpdatedAt = blab.UpdatedAt,
                Author = blab.Author?.ToView(),
                Liked = blab.Liked.Select(author => author.ToView()).ToList(),
                Comments = blab.Comments.Select(comment => comment.ToView()).ToList()
            };
        }

        public static Blab ToBlab(this BlabCreateRequest request)
        {
            return new Blab
            {
                AuthorId = request.AuthorId,
                Body = request.Body
            };
        }

        public static BlabEditRequest ToBlabEditRequest(this Blab blab)
        {
            return new BlabEditRequest
            {
                Body = blab.Body
            };
        }

        public static void EditBlab(this Blab blab, BlabEditRequest request)
        {
            blab.Body = request.Body;
            blab.UpdatedAt = DateTime.UtcNow;
        }
    }
}
