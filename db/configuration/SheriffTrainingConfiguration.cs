using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.sheriff;

namespace SS.Db.configuration
{
    public class SheriffTrainingConfiguration : BaseEntityConfiguration<SheriffTraining>
    {
        public override void Configure(EntityTypeBuilder<SheriffTraining> builder)
        {
            builder.HasIndex(b => new { b.StartDate, b.EndDate });

            base.Configure(builder);
        }
    }
}
