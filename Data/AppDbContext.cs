#nullable disable warnings
using Microsoft.EntityFrameworkCore;
using BugTracker.Models;

namespace BugTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(c => c.Username)
                .UseCollation("NOCASE");
            
            modelBuilder.Entity<Project>()
                .HasOne(u => u.UserCreated)
                .WithMany(p => p.ProjectsCreated)
                .HasForeignKey(u => u.UserCreatedId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.UserAssigned)
                .WithMany(u => u.TicketsAssigned)
                .HasForeignKey(t => t.UserAssignedId)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.UserCreated)
                .WithMany(u => u.TicketsCreated)
                .HasForeignKey(t => t.UserCreatedId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
}