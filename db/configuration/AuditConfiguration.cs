using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.audit;

namespace SS.Db.configuration
{
    public class AuditConfiguration : BaseEntityConfiguration<Audit>
    {
        public override void Configure(EntityTypeBuilder<Audit> builder)
        {
            base.Configure(builder);

            builder.Ignore(a => a.UpdatedBy);
            builder.Ignore(a => a.UpdatedById);
            builder.Ignore(a => a.UpdatedOn);
            builder.Ignore(a => a.ConcurrencyToken);
        }
    }
}
