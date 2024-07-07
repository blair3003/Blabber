using Blabber.Api.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blabber.Api.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? ApplicationUserId { get; set; }

        [Required]
        public string? Handle { get; set; }

        [Required]
        public string? DisplayName { get; set; }

        public string? DisplayPic { get; set; }


        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<Blab> Blabs { get; set; } = [];
        [InverseProperty("Liked")]
        public ICollection<Blab> Likes { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
        [InverseProperty("Followers")]
        public ICollection<Author> Following { get; set; } = [];
        [InverseProperty("Following")]
        public ICollection<Author> Followers { get; set; } = [];

    }
}
