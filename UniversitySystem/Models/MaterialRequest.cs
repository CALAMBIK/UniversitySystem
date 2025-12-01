using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class MaterialRequest
    {
        [Key]
        public int IdRequest { get; set; }

        [Required]
        public int IdUser { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [StringLength(50)]
        public string MaterialType { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected", "Completed"

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ProcessedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        [StringLength(500)]
        public string? AdminComment { get; set; }

        [StringLength(200)]
        public string? FileUrl { get; set; }

        public User? User { get; set; }
    }
}