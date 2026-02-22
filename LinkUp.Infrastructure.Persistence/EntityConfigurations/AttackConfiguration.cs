using LinkUp.Core.Domain.Battleship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.EntityConfigurations
{
    public class AttackConfiguration : IEntityTypeConfiguration<Attack>
    {
        public void Configure(EntityTypeBuilder<Attack> builder)
        {
            builder.ToTable("Attacks");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.AttackerUserId).IsRequired().HasMaxLength(450);
            builder.Property(a => a.DefenderUserId).IsRequired().HasMaxLength(450);
            builder.HasOne(a => a.Game)
                   .WithMany(g => g.Attacks)
                   .HasForeignKey(a => a.GameId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(a => new { a.GameId, a.AttackerUserId });
        }
    }
}
