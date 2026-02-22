using LinkUp.Core.Domain.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.EntityConfigurations
{
    public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
    {
        public void Configure(EntityTypeBuilder<Reaction> builder)
        {
            builder.ToTable("Reactions");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.UserId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(r => r.Type)
                   .HasConversion<int>();

            builder.HasOne(r => r.Post)
                   .WithMany(p => p.Reactions)
                   .HasForeignKey(r => r.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => new { r.PostId, r.UserId }).IsUnique();
        }
    }
}
