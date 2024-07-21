using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Blabber.Api.Models
{
    public static class BlabMapper
    {
        public static BlabView ToView(this Blab blab)
        {
            return new BlabView
            {
                Id = blab.Id,
                Body = !blab.IsDeleted ? blab.Body : "[Removed]",
                CreatedAt = blab.CreatedAt,
                UpdatedAt = blab.UpdatedAt,
                Author = !blab.IsDeleted ? blab.Author?.ToView() : null,
                Liked = !blab.IsDeleted ? blab.Liked.Select(author => author.ToView()).ToList() : [],
                Comments = !blab.IsDeleted ? blab.Comments.Select(comment => comment.ToView()).ToList() : [],
                IsDeleted = blab.IsDeleted
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

        public static BlabFeed ToFeed(this IEnumerable<Blab> blabs, int totalCount, int pageNumber, int pageSize)
        {
            return new BlabFeed
            {
                Blabs = blabs.Select(blab => blab.ToFeedItem()).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public static BlabFeedItem ToFeedItem(this Blab blab)
        {
            return new BlabFeedItem
            {
                Id = blab.Id,
                Body = blab.Body,
                CreatedAt = blab.CreatedAt,
                UpdatedAt = blab.UpdatedAt,
                Author = blab.Author?.ToView(),
                LikedCount = blab.Liked.Count,
                CommentsCount = blab.Comments.Count
            };
        }

        public static BlabUpdateRequest ToBlabEditRequest(this Blab blab)
        {
            return new BlabUpdateRequest
            {
                Id = blab.Id,
                Body = blab.Body
            };
        }

        public static void UpdateBlab(this Blab blab, BlabUpdateRequest request)
        {
            blab.Body = request.Body;
            blab.UpdatedAt = DateTime.UtcNow;
        }

        public static void DeleteBlab(this Blab blab)
        {
            blab.IsDeleted = true;
            blab.UpdatedAt = DateTime.UtcNow;

            foreach (var comment in blab.Comments)
            {
                comment.DeleteComment();
            }
        }
    }
}