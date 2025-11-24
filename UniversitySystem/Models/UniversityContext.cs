using Microsoft.EntityFrameworkCore;

namespace UniversitySystem.Models
{
    public class UniversityContext : DbContext
    {
        public UniversityContext(DbContextOptions<UniversityContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<Departament> Departaments { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.IdGroup);

            modelBuilder.Entity<StudentGroup>()
                .HasOne(g => g.Departament)
                .WithMany(d => d.StudentGroups)
                .HasForeignKey(g => g.IdDepartament);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Departament)
                .WithMany(d => d.Teachers)
                .HasForeignKey(t => t.IdDepartament);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Student)
                .WithMany()
                .HasForeignKey(u => u.IdStudent);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Teacher)
                .WithMany()
                .HasForeignKey(u => u.IdTeacher);
        }
    }
}