using Microsoft.AspNetCore.Identity;
using Blabber.Api.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Blabber.Api.Data
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Blab> Blabs { get; set; } = [];
        [InverseProperty("Liked")]
        public ICollection<Blab> Likes { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
        [InverseProperty("Followers")]
        public ICollection<ApplicationUser> Following { get; set; } = [];
        [InverseProperty("Following")]
        public ICollection<ApplicationUser> Followers { get; set; } = [];
    }
}
