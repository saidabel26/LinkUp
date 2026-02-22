using LinkUp.Core.Domain.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.EntityConfigurations
{
    public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
    {
        public void Configure(EntityTypeBuilder<FriendRequest> builder)
        {
            builder.ToTable("FriendRequests");
            builder.HasKey(fr => fr.Id);

            builder.Property(fr => fr.FromUserId).IsRequired().HasMaxLength(450);
            builder.Property(fr => fr.ToUserId).IsRequired().HasMaxLength(450);

            builder.Property(fr => fr.Status).HasConversion<int>();

            builder.HasIndex(fr => new { fr.FromUserId, fr.ToUserId }).IsUnique();
        }
    }
}
