using System.ComponentModel.DataAnnotations;

namespace Blabber.Api.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int BlabId { get; set; }
        [Required]
        public int AuthorId { get; set; }
        public int? ParentId { get; set; }
        [Required]
        public string? Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public Blab? Blab { get; set; }
        public Author? Author { get; set; }
        public Comment? Parent { get; set; }
        public ICollection<Comment> Children { get; set; } = [];
    }
}