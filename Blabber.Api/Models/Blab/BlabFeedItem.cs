namespace Blabber.Api.Models
{
    public class BlabFeedItem
    {
        public int Id { get; set; }
        public string? Body { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public AuthorView? Author { get; set; }
        public int LikedCount { get; set; }
        public int CommentsCount { get; set; }
    }
}