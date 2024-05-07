using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
using Dummy.Domain.Commons;
using Dummy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Dummy.Infrastructure
{
    public class MainDBContext : DbContext
    {
        public const string MemberSchema = "member";
        public const string LocationSchema = "location";

        public DbSet<Member> Members { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public MainDBContext(DbContextOptions<MainDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pgcrypto").HasPostgresExtension("uuid-ossp");
            // Using "AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL" package
            // Auto generate Quartz table
            modelBuilder.AddQuartz(builder => builder.UsePostgreSql());
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainDBContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddAuditInfo()
        {
            var entries = ChangeTracker.Entries()
                                       .Where(e => e.Entity is IAuditEntity &&
                                             (e.State == EntityState.Added || e.State == EntityState.Modified)
                                        );

            foreach (var entry in entries)
            {
                var entity = (IAuditEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = DateTime.UtcNow;
                }
                else
                {
                    Entry(entity).Property(e => e.CreatedDate).IsModified = false;
                }

                entity.UpdatedDate = DateTime.UtcNow;
            }
        }

    }
}
