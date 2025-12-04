using Microsoft.EntityFrameworkCore;
using UniversitySystem.Models;

namespace UniversitySystem.Data
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
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }
        public DbSet<TeacherDiscipline> TeacherDisciplines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.IdStudent);
                entity.HasOne(s => s.Group)
                    .WithMany(g => g.Students)
                    .HasForeignKey(s => s.IdGroup);
            });

            modelBuilder.Entity<StudentGroup>(entity =>
            {
                entity.HasKey(e => e.IdGroup);
                entity.HasOne(g => g.Departament)
                    .WithMany(d => d.StudentGroups)
                    .HasForeignKey(g => g.IdDepartament);
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.IdTeacher);
                entity.HasOne(t => t.Departament)
                    .WithMany(d => d.Teachers)
                    .HasForeignKey(t => t.IdDepartament);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser);
                entity.HasOne(u => u.Student)
                    .WithMany()
                    .HasForeignKey(u => u.IdStudent);
                entity.HasOne(u => u.Teacher)
                    .WithMany()
                    .HasForeignKey(u => u.IdTeacher);
            });

            modelBuilder.Entity<CourseMaterial>(entity =>
            {
                entity.HasKey(e => e.IdMaterial);
                entity.HasOne(m => m.Teacher)
                    .WithMany()
                    .HasForeignKey(m => m.IdTeacher);
                entity.HasOne(m => m.Group)
                    .WithMany()
                    .HasForeignKey(m => m.IdGroup);
                entity.HasOne(m => m.Discipline)
                    .WithMany()
                    .HasForeignKey(m => m.IdDiscipline);
            });

            modelBuilder.Entity<TeacherDiscipline>(entity =>
            {
                entity.HasKey(e => e.IdTeacherDiscipline);
                entity.HasOne(td => td.Teacher)
                    .WithMany()
                    .HasForeignKey(td => td.IdTeacher);
                entity.HasOne(td => td.Discipline)
                    .WithMany()
                    .HasForeignKey(td => td.IdDiscipline);
                entity.HasOne(td => td.Group)
                    .WithMany()
                    .HasForeignKey(td => td.IdGroup);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.IdProfile);
                entity.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.IdUser);
            });
        }
    }
}