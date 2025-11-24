using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class StudentGroup
    {
        [Key]
        public int IdGroup { get; set; }
        public int? IdDepartament { get; set; }
        public int? NumberGroup { get; set; }

        public Departament? Departament { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
