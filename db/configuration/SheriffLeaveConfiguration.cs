using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.sheriff;

namespace SS.Db.configuration
{
    public class SheriffLeaveConfiguration : BaseEntityConfiguration<SheriffLeave>
    {
        public override void Configure(EntityTypeBuilder<SheriffLeave> builder)
        {
            builder.HasIndex(b => new { b.StartDate, b.EndDate });

            base.Configure(builder);
        }
    }
}
