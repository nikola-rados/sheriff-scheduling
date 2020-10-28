using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.scheduling;

namespace SS.Db.configuration
{
    public class AssignmentConfiguration : BaseEntityConfiguration<Assignment>
    {
        public override void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            base.Configure(builder);
        }
    }
}
