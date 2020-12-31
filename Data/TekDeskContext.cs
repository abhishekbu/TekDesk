using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TekDesk.Models;

namespace TekDesk.Data
{
    public class TekDeskContext : DbContext
    {
        public TekDeskContext(DbContextOptions<TekDeskContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Query> Queries { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<Artifact> Artifacts { get; set; }
        
        public DbSet<EmployeeSubject> EmployeeSubjects { get; set; }
        public DbSet<EmployeeNotification> EmployeeNotifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employee");

            modelBuilder.Entity<Query>().ToTable("Query")
                .Property(a => a.Added).HasDefaultValueSql("getdate()");          

            modelBuilder.Entity<Subject>().ToTable("Subject");


            modelBuilder.Entity<Solution>().ToTable("Solution")
                .HasOne(q => q.Query)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solution>().ToTable("Solution")
                .Property(a => a.Added).HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Solution>().ToTable("Solution")
                .HasOne<Query>(q => q.Query)
                .WithMany(s => s.Solutions)
                .HasForeignKey(q => q.QueryID);

            modelBuilder.Entity<Artifact>().ToTable("Artifact");

            modelBuilder.Entity<EmployeeSubject>().ToTable("EmployeeSubject")
                .HasKey(c => new { c.EmployeeID, c.SubjectID });

            modelBuilder.Entity<EmployeeNotification>().ToTable("EmployeeNotification")
                .HasKey(c => new { c.EmployeeID, c.QueryID });

            modelBuilder.Entity<EmployeeNotification>().ToTable("EmployeeNotification")
                .HasOne(e => e.Employee)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeNotification>()
                .HasOne<Employee>(sc => sc.Employee)
                .WithMany(s => s.EmployeeNotifications)
                .HasForeignKey(sc => sc.EmployeeID);

            modelBuilder.Entity<EmployeeNotification>()
                .HasOne<Query>(sc => sc.Query)
                .WithMany(s => s.EmployeeNotifications)
                .HasForeignKey(sc => sc.QueryID);
        }
    }
}
