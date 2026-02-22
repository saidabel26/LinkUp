using LinkUp.Core.Domain.Battleship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.EntityConfigurations
{
    public class ShipPlacementConfiguration : IEntityTypeConfiguration<ShipPlacement>
    {
        public void Configure(EntityTypeBuilder<ShipPlacement> builder)
        {
            builder.ToTable("ShipPlacements");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.UserId).IsRequired().HasMaxLength(450);
            builder.Property(s => s.Direction).HasConversion<int>();
            builder.HasOne(s => s.Game)
                   .WithMany(g => g.Placements)
                   .HasForeignKey(s => s.GameId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(s => new { s.GameId, s.UserId });
        }
    }
}
