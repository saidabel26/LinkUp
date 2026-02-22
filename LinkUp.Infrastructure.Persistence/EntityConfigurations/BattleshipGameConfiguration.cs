using LinkUp.Core.Domain.Battleship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.EntityConfigurations
{
    public class BattleshipGameConfiguration : IEntityTypeConfiguration<BattleshipGame>
    {
        public void Configure(EntityTypeBuilder<BattleshipGame> builder)
        {
            builder.ToTable("BattleshipGames");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.CreatorUserId).IsRequired().HasMaxLength(450);
            builder.Property(g => g.OpponentUserId).IsRequired().HasMaxLength(450);
            builder.Property(g => g.Status).HasConversion<int>();

            builder.HasMany(g => g.Placements)
                   .WithOne(p => p.Game!)
                   .HasForeignKey(p => p.GameId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(g => g.Attacks)
                   .WithOne(a => a.Game!)
                   .HasForeignKey(a => a.GameId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(g => new { g.CreatorUserId, g.OpponentUserId, g.Status });
        }
    }
}
