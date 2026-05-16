using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Models;

namespace BugTrackingSystem.Data
{
    /// <summary>
    /// Основний контекст бази даних застосунку.
    /// Успадковує <see cref="IdentityDbContext{TUser}"/> для підтримки автентифікації.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        /// <summary>Таблиця багів.</summary>
        public DbSet<Bug> Bugs { get; set; }

        /// <summary>Таблиця проєктів.</summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>Таблиця учасників проєктів (зв'язок User ↔ Project).</summary>
        public DbSet<ProjectMember> ProjectMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Bug → Author: Restrict (автора не можна видалити, якщо є баги)
            builder.Entity<Bug>()
                .HasOne(b => b.Author)
                .WithMany(u => u.CreatedBugs)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bug → AssignedTo: SetNull (видалення користувача знімає призначення)
            builder.Entity<Bug>()
                .HasOne(b => b.AssignedTo)
                .WithMany(u => u.AssignedBugs)
                .HasForeignKey(b => b.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            // ProjectMember: складений первинний ключ (ProjectId + UserId)
            builder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.ProjectId, pm.UserId });

            // ProjectMember → Project: Cascade (видалення проєкту → видалення записів членства)
            builder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProjectMember → User: Cascade (видалення користувача → видалення записів членства)
            builder.Entity<ProjectMember>()
                .HasOne(pm => pm.User)
                .WithMany(u => u.ProjectMemberships)
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}