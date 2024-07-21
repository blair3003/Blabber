namespace Blabber.Api.Models
{
    public class BlabView
    {
        public int Id { get; set; }

        public string? Body { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public AuthorView? Author { get; set; }

        public ICollection<AuthorView> Liked { get; set; } = [];

        public ICollection<CommentView> Comments { get; set; } = [];

        public bool IsDeleted { get; set; }
    }
}