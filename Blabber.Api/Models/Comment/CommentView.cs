using Blabber.Api.Data;
using System.ComponentModel.DataAnnotations;

namespace Blabber.Api.Models
{
    public class CommentView
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string? Body { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public AuthorView? Author { get; set; }

        public bool IsDeleted { get; set; }
    }
}