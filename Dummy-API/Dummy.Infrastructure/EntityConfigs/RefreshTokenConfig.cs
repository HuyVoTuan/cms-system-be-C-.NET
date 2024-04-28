using Dummy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dummy.Infrastructure.EntityConfigs
{
    internal class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable(nameof(RefreshToken), MainDBContext.MemberSchema);

            builder.HasIndex(x => x.Token);
            builder.HasIndex(x => x.ExpiredDate);

            builder.Property(x => x.Token).IsRequired();
            builder.Property(x => x.ExpiredDate).IsRequired();
        }
    }
}
