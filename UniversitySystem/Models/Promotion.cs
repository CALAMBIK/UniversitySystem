using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class Promotion
    {
        [Key]
        public int IdPromotion { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(30);

        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        [StringLength(50)]
        public string? Discount { get; set; }
    }
}