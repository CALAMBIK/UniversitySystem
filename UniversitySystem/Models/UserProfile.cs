using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class UserProfile
    {
        [Key]
        public int IdProfile { get; set; }

        [Required]
        public int IdUser { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(500)]
        public string? About { get; set; }

        [StringLength(200)]
        public string? AvatarUrl { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public User? User { get; set; }
    }
}