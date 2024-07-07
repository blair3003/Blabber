using System.ComponentModel.DataAnnotations;

namespace Blabber.Api.Models
{
    public class BlabCreateRequest
    {
        [Display(Name = "Author ID")]
        [Required(ErrorMessage = "The Author ID field is required.")]
        public int AuthorId { get; set; }

        [Display(Name = "Body")]
        [Required(ErrorMessage = "The Body field is required.")]
        [StringLength(255, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string? Body { get; set; }
    }
}
