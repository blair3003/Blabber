using System.ComponentModel.DataAnnotations;

namespace Blabber.Api.Models
{
    public class BlabPage
    {
        [Required]
        public IEnumerable<BlabView> Blabs { get; set; } = [];
        [Required]
        public int PageNumber { get; set; }
        [Required]
        public int PageSize { get; set; }
        [Required]
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
