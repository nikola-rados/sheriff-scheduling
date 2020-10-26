using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using SS.Db.models.scheduling;

namespace SS.Db.configuration
{
    public class ShiftSheriffConfiguration : BaseEntityConfiguration<ShiftSheriff>
    {
        public override void Configure(EntityTypeBuilder<ShiftSheriff> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            builder.HasOne(m => m.Shift).WithMany(m => m.AssignedSheriffs).HasForeignKey(m => m.ShiftId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(s => s.Sheriff).WithOne();

            base.Configure(builder);
        }
    }
}
    