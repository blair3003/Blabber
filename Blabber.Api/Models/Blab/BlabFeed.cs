namespace Blabber.Api.Models
{
    public class BlabFeed
    {
        public IEnumerable<BlabFeedItem> Blabs { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}