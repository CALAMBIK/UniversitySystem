using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class Departament
    {
        [Key]
        public int IdDepartament { get; set; }
        [StringLength(50)] public string? Name { get; set; }

        public ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}
