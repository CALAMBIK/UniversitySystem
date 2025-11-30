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
        public DbSet<MaterialRequest> MaterialRequests { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка Student
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.IdStudent);
                entity.HasOne(s => s.Group)
                    .WithMany(g => g.Students)
                    .HasForeignKey(s => s.IdGroup);
            });

            // Настройка StudentGroup
            modelBuilder.Entity<StudentGroup>(entity =>
            {
                entity.HasKey(e => e.IdGroup);
                entity.HasOne(g => g.Departament)
                    .WithMany(d => d.StudentGroups)
                    .HasForeignKey(g => g.IdDepartament);
            });

            // Настройка Teacher
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.IdTeacher);
                entity.HasOne(t => t.Departament)
                    .WithMany(d => d.Teachers)
                    .HasForeignKey(t => t.IdDepartament);
            });

            // Настройка User
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

            // Настройка MaterialRequest
            modelBuilder.Entity<MaterialRequest>(entity =>
            {
                entity.HasKey(e => e.IdRequest);
                entity.HasOne(m => m.User)
                    .WithMany()
                    .HasForeignKey(m => m.IdUser);
            });

            // Настройка UserProfile
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