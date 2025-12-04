using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class TeacherDiscipline
    {
        [Key]
        public int IdTeacherDiscipline { get; set; }

        [Required]
        public int IdTeacher { get; set; }

        [Required]
        public int IdDiscipline { get; set; }

        public int? IdGroup { get; set; }

        public Teacher? Teacher { get; set; }
        public Discipline? Discipline { get; set; }
        public StudentGroup? Group { get; set; }
    }
}