using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class News
    {
        [Key]
        public int IdNews { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? PublishDate { get; set; } = DateTime.Now;

        public bool IsPublished { get; set; } = true;

        public string? Author { get; set; }
    }
}