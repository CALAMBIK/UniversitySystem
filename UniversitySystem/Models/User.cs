using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }

        [Required]
        [StringLength(50)]
        public string Login { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; } // "Admin", "Teacher", "Student"

        public int? IdStudent { get; set; }
        public int? IdTeacher { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastLogin { get; set; } = DateTime.Now;

        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }
    }
}