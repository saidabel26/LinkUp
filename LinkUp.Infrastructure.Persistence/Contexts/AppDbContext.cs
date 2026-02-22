using LinkUp.Core.Domain.Common;
using LinkUp.Core.Domain.Social;
using LinkUp.Core.Domain.Battleship;
using LinkUp.Infrastructure.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Reaction> Reactions => Set<Reaction>();
        public DbSet<Friendship> Friendships => Set<Friendship>();
        public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
        public DbSet<BattleshipGame> BattleshipGames => Set<BattleshipGame>();
        public DbSet<ShipPlacement> ShipPlacements => Set<ShipPlacement>();
        public DbSet<Attack> Attacks => Set<Attack>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new ReactionConfiguration());
            modelBuilder.ApplyConfiguration(new FriendshipConfiguration());
            modelBuilder.ApplyConfiguration(new FriendRequestConfiguration());
            modelBuilder.ApplyConfiguration(new BattleshipGameConfiguration());
            modelBuilder.ApplyConfiguration(new ShipPlacementConfiguration());
            modelBuilder.ApplyConfiguration(new AttackConfiguration());

            // Soft delete global filter
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(AppDbContext).GetMethod(nameof(SetIsDeletedFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                        .MakeGenericMethod(entityType.ClrType);
                    method.Invoke(null, new object[] { modelBuilder });
                }
            }
        }

        private static void SetIsDeletedFilter<TEntity>(ModelBuilder builder) where TEntity : BaseEntity
        {
            builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
