using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blabber.Api.Models
{
    public class Blab
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public string? Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public Author? Author { get; set; }
        [InverseProperty("Likes")]
        public ICollection<Author> Liked { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}