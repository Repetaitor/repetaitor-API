using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Domain.Data
{
    public sealed class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthenticationCodes>()
                .HasIndex(e => e.Email)
                .IsUnique();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseSqlServer("Server=sql.bsite.net\\MSSQL2016;Database=repetaitor_DB;User Id=repetaitor_DB;Password=dachidachi1;Trusted_Connection=False;TrustServerCertificate=True;");
            }
        }
    }
}
