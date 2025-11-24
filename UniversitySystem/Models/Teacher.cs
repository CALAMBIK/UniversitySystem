using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class Teacher
    {
        [Key]
        public int IdTeacher { get; set; }
        public int? IdDepartament { get; set; }
        [StringLength(50)] public string? Name { get; set; }
        [StringLength(50)] public string? SecondName { get; set; }
        [StringLength(50)] public string? Patronymic { get; set; }
        [StringLength(50)] public string? PhoneNumber { get; set; }
        [StringLength(50)] public string? Login { get; set; }
        [StringLength(50)] public string? Password { get; set; }

        public Departament? Departament { get; set; }
    }
}
