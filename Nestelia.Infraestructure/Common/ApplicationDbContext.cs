using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nestelia.Domain.Entities;
using Nestelia.Domain.Entities.Audit;
using Nestelia.Domain.Entities.Auth;
using Nestelia.Domain.Entities.Bot;
using Nestelia.Domain.Entities.Wiki.Categories;
using Nestelia.Domain.Entities.Wiki.Entries;
using Nestelia.Domain.Entities.Wiki.Posts;

namespace Nestelia.Infraestructure.Common
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<ApplicationUser> AppUsers { get; set; }        
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Configuration> BotConfigurations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<WikiEntry> WikiEntries { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<New> News { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<WikiEntry>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Post>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Comment>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<New>().HasQueryFilter(x => !x.IsDeleted);

            var entitiesAssembly = typeof(BaseEntity).Assembly;
            modelBuilder.RegisterAllEntities<BaseEntity>(entitiesAssembly);
        }
        
    }
}
