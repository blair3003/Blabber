using System.ComponentModel.DataAnnotations;

namespace Blabber.Api.Models
{
    public class CommentCreateRequest
    {
        [Display(Name = "Blab ID")]
        [Required(ErrorMessage = "The Blab ID field is required.")]
        public int BlabId { get; set; }

        [Display(Name = "Author ID")]
        [Required(ErrorMessage = "The Author ID field is required.")]
        public int AuthorId { get; set; }

        [Display(Name = "Parent ID")]
        public int? ParentId { get; set; }

        [Display(Name = "Body")]
        [Required(ErrorMessage = "The Body field is required.")]
        [StringLength(255, ErrorMessage = "The {0} must be at max {1} characters long.")]
        public string? Body { get; set; }
    }
}