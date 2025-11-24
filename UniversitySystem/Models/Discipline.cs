using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class Discipline
    {
        [Key]
        public int IdDiscipline { get; set; }
        [StringLength(50)] public string? Name { get; set; }
    }
}
