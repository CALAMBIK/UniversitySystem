using System.ComponentModel.DataAnnotations;

namespace UniversitySystem.Models
{
    public class Student
    {
        [Key]
        public int IdStudent { get; set; }
        public int? IdGroup { get; set; }
        [StringLength(50)] public string? Name { get; set; }
        [StringLength(50)] public string? SecondName { get; set; }
        [StringLength(50)] public string? Patronymic { get; set; }
        [StringLength(50)] public string? PhoneNumber { get; set; }
        public DateTime? DateBirthday { get; set; }
        [StringLength(50)] public string? Login { get; set; }
        [StringLength(50)] public string? Password { get; set; }

        public StudentGroup? Group { get; set; }
    }
}
