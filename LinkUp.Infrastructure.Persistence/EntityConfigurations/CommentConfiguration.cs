using LinkUp.Core.Domain.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.EntityConfigurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(c => c.Content)
                   .IsRequired()
                   .HasMaxLength(750);

            builder.HasOne(c => c.Post)
                   .WithMany(p => p.Comments)
                   .HasForeignKey(c => c.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.ParentComment)
                   .WithMany(c => c.Replies)
                   .HasForeignKey(c => c.ParentCommentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => new { c.PostId, c.ParentCommentId });
        }
    }
}
