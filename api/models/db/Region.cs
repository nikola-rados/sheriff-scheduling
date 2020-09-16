using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace SS.Api.Models.DB
{
    public partial class Region
    {
        public Region()
        {
            LocationNavigation = new HashSet<Location>();
        }

        public Guid RegionId { get; set; }
        public string RegionCd { get; set; }
        public string RegionName { get; set; }
        public NpgsqlPolygon? Location { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDtm { get; set; }
        public DateTime UpdatedDtm { get; set; }
        public decimal RevisionCount { get; set; }

        public virtual ICollection<Location> LocationNavigation { get; set; }
    }
}
