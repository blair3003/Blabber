using Microsoft.AspNetCore.Identity;
using Blabber.Api.Models;

namespace Blabber.Api.Data
{
    public class ApplicationUser : IdentityUser
    {
        public Author? Author { get; set; }
    }
}
