using LinkUp.Core.Domain.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.EntityConfigurations
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.ToTable("Friendships");
            builder.HasKey(f => f.Id);

            builder.Property(f => f.UserId).IsRequired().HasMaxLength(450);
            builder.Property(f => f.FriendId).IsRequired().HasMaxLength(450);

            builder.HasIndex(f => new { f.UserId, f.FriendId }).IsUnique();
        }
    }
}
