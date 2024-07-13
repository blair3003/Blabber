using System.ComponentModel.DataAnnotations;

namespace Blabber.Api.Models
{
    public class AuthorCreateRequest
    {
        [Required]
        public string? ApplicationUserId { get; set; }

        [Display(Name = "Handle")]
        [Required(ErrorMessage = "The Handle field is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "The {0} must be between {2} and {1} characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "The {0} must contain only alphanumeric characters and underscores.")]
        public string? Handle { get; set; }

        [Display(Name = "Display Name")]
        [Required(ErrorMessage = "The Display Name field is required.")]
        [StringLength(50, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string? DisplayName { get; set; }

        [Display(Name = "Display Pic")]
        [Url(ErrorMessage = "The {0} field is not a valid URL.")]
        public string? DisplayPic { get; set; }
    }
}