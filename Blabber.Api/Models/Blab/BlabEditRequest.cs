using System.ComponentModel.DataAnnotations;

namespace Blabber.Api.Models
{
    public class BlabEditRequest
    {
        [Display(Name = "Body")]
        [Required(ErrorMessage = "The Body field is required.")]
        [StringLength(255, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string? Body { get; set; }
    }
}
