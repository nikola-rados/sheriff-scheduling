using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.lookupcodes;

namespace SS.Db.configuration
{
    public class LookupSortOrderConfiguration : BaseEntityConfiguration<LookupSortOrder>
    {
        public override void Configure(EntityTypeBuilder<LookupSortOrder> builder)
        {
            builder.HasOne(b => b.Location).WithMany().HasForeignKey(m => m.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.LookupCode).WithMany().HasForeignKey(m => m.LookupCodeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(b => b.LookupType);

            base.Configure(builder);
        }
    }
}
