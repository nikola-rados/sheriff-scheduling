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

            builder.HasOne(b => b.LookupCode).WithMany(a => a.SortOrder).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(lc => new { lc.LookupCodeId, lc.LocationId }).IsUnique();

            builder.HasIndex(b => b.LookupCodeId);

            base.Configure(builder);
        }
    }
}
