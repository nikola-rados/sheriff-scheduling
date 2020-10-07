using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SS.DB.Configuration;
using ss.db.models;
using SS.Db.models.auth;
using SS.Db.models.lookupcodes;

namespace SS.Db.configuration
{
    public class LookupCodeOrderConfiguration : BaseEntityConfiguration<LookupCode>
    {
        public override void Configure(EntityTypeBuilder<LookupCode> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 200);

            builder.HasIndex(lc => new {lc.Type, lc.Code, lc.LocationId}).IsUnique();

            builder.HasData(
                new LookupCode { Id = 1, Description = "Chief Sheriff", Type = LookupTypes.SheriffRank },
                new LookupCode { Id = 2, Description = "Superintendent", Type = LookupTypes.SheriffRank },
                new LookupCode { Id = 3, Description = "Staff Inspector", Type = LookupTypes.SheriffRank },
                new LookupCode { Id = 4, Description = "Inspector", Type = LookupTypes.SheriffRank },
                new LookupCode { Id = 5, Description = "Staff Sergeant", Type = LookupTypes.SheriffRank },
                new LookupCode { Id = 6, Description = "Sergeant", Type = LookupTypes.SheriffRank },
                new LookupCode { Id = 7, Description = "Deputy Sheriff", Type = LookupTypes.SheriffRank }
            );

            base.Configure(builder);
        }
    }
}
