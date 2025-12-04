using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class CourseMaterial
    {
        [Key]
        public int IdMaterial { get; set; }

        [Required]
        public int IdTeacher { get; set; }

        [Required]
        public int IdGroup { get; set; }

        [Required]
        public int IdDiscipline { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string? FileUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public Teacher? Teacher { get; set; }
        public StudentGroup? Group { get; set; }
        public Discipline? Discipline { get; set; }
    }
}