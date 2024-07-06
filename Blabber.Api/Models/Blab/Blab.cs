using Blabber.Api.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blabber.Api.Models
{
    public class Blab
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? AuthorId { get; set; }
        [Required]
        public string? Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser? Author { get; set; }
        [InverseProperty("Likes")]
        public ICollection<ApplicationUser> Liked { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}
