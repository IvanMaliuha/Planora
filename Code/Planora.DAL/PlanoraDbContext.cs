// PlanoraDbContext.cs
using Microsoft.EntityFrameworkCore;
using Planora.DAL.Models;

namespace Planora.DAL
{
    public class PlanoraDbContext : DbContext
    {
        public PlanoraDbContext(DbContextOptions<PlanoraDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TeachingAssignment> TeachingAssignments { get; set; }
        public DbSet<GroupDisciplineList> GroupDisciplineLists { get; set; }
        public DbSet<Workload> Workloads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Administrator>().ToTable("Administrator");
            modelBuilder.Entity<Teacher>().ToTable("Teachers");
            modelBuilder.Entity<Student>().ToTable("Students");

            modelBuilder.Entity<GroupDisciplineList>()
                .HasKey(g => g.ListId);
                
            modelBuilder.Entity<TeachingAssignment>()
                .HasKey(t => t.AssignmentId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.DisciplineLists)
                .WithOne(gd => gd.Group)
                .HasForeignKey(gd => gd.GroupId);

            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.TeachingAssignments)
                .WithOne(ta => ta.Teacher)
                .HasForeignKey(ta => ta.UserId);

            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Workloads)
                .WithOne(w => w.Teacher)
                .HasForeignKey(w => w.UserId);
        }
    }
}