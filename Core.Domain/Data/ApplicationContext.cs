using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Domain.Data
{
    public sealed class ApplicationContext(DbContextOptions<ApplicationContext> options, IConfiguration configuration) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<AuthenticationCodes> AuthCodes => Set<AuthenticationCodes>();
        public DbSet<Essay> Essays => Set<Essay>();
        public DbSet<RepetaitorGroup> Groups => Set<RepetaitorGroup>();
        public DbSet<Assignment> Assignments => Set<Assignment>();
        public DbSet<AssignmentStatus> AssignmentStatuses => Set<AssignmentStatus>();
        public DbSet<EvaluationCommentsStatus> EvaluationCommentsStatuses => Set<EvaluationCommentsStatus>();
        public DbSet<EvaluationTextComment> EvaluationTextComments => Set<EvaluationTextComment>();
        public DbSet<GeneralComment> GeneralComments => Set<GeneralComment>();
        public DbSet<UserAssignment> UserAssignments => Set<UserAssignment>();
        public DbSet<UserGroups> UserGroups => Set<UserGroups>();
        public DbSet<AssignmentImagesStore> AssignmentImagesStores => Set<AssignmentImagesStore>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignment>().Property(x => x.Instructions)
                .HasColumnType("nvarchar(MAX)");
            modelBuilder.Entity<GeneralComment>().Property(x => x.Comment)
                .HasColumnType("nvarchar(MAX)");
            modelBuilder.Entity<UserAssignment>().Property(x => x.Text)
                .HasColumnType("nvarchar(MAX)");
            modelBuilder.Entity<AuthenticationCodes>()
                .HasIndex(e => e.Email);
            modelBuilder.Entity<EvaluationTextComment>().HasOne(u => u.UserAssignment)
                .WithMany(a => a.EvaluationTextComments).HasForeignKey(u => u.UserAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EvaluationTextComment>().HasOne(u => u.Status)
                .WithMany(a => a.EvaluationTextComments).HasForeignKey(u => u.StatusId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GeneralComment>().HasOne(u => u.UserAssignment)
                .WithMany(a => a.GeneralComments).HasForeignKey(u => u.UserAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Essay>().HasOne(u => u.Creator)
                .WithMany(a => a.CreatedEssays).HasForeignKey(u => u.CreatorId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RepetaitorGroup>().HasOne(u => u.Owner).WithMany(u => u.RepetaitorGroups)
                .HasForeignKey(u => u.OwnerId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Assignment>().HasOne(u => u.Creator).WithMany(u => u.CreatedAssignments)
                .HasForeignKey(u => u.CreatorId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Assignment>().HasOne(u => u.Group).WithMany(u => u.Assignments)
                .HasForeignKey(u => u.GroupId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Assignment>().HasOne(u => u.Essay).WithMany(u => u.Assignments)
                .HasForeignKey(u => u.EssayId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserAssignment>().HasOne(u => u.User).WithMany(u => u.AssignedAssignments)
                .HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Restrict);;
            modelBuilder.Entity<UserAssignment>().HasOne(u => u.Status).WithMany(u => u.UserAssignments)
                .HasForeignKey(u => u.StatusId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserAssignment>().HasOne(u => u.Assignment).WithMany(u => u.UserAssignments)
                .HasForeignKey(u => u.AssignmentId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserAssignment>().HasIndex(u => new { u.UserId, u.AssignmentId })
                .HasDatabaseName("UserAssignments_AssignmentId").IsUnique();
            modelBuilder.Entity<UserGroups>().HasOne(u => u.Group).WithMany(u => u.UserGroups)
                .HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.Cascade);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration["Mode"]?.ToLower() == "test"
                    ? configuration["ConnectionStrings:TestConnection"]
                    : configuration["ConnectionStrings:ProductionConnection"]);
            }
        }
    }
}