using LinkUp.Core.Domain.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.EntityConfigurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(p => p.Content)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(p => p.MediaType)
                   .HasConversion<int>();

            builder.Property(p => p.ImageUrl)
                   .HasMaxLength(2048);

            builder.Property(p => p.YouTubeUrl)
                   .HasMaxLength(2048);

            builder.HasMany(p => p.Comments)
                   .WithOne(c => c.Post!)
                   .HasForeignKey(c => c.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Reactions)
                   .WithOne(r => r.Post!)
                   .HasForeignKey(r => r.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => new { p.UserId, p.CreatedAt });
        }
    }
}
