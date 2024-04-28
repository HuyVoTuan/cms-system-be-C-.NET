using Dummy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dummy.Infrastructure.EntityConfigs
{
    internal class MemberConfig : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.ToTable(nameof(Member), MainDBContext.MemberSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Slug).IsRequired();
            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();
            builder.Property(x => x.Email).IsRequired();

            builder.HasIndex(x => x.Slug);

            builder.HasMany(x => x.Locations)
                   .WithOne(y => y.Member)
                   .HasForeignKey(y => y.MemberId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.RefreshToken)
                   .WithOne(y => y.Member)
                   .HasForeignKey(y => y.MemberId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
