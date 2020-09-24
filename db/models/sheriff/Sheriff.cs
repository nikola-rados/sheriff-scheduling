using System;
using System.Collections.Generic;
using db.models;
using Mapster;
using SS.Db.models.auth;

namespace SS.Db.models.sheriff
{
    [AdaptTo("[name]Dto")]
    public class Sheriff : User
    {
        public Gender Gender { get; set; }
        public string BadgeNumber { get; set; }
        public string Rank { get; set; }
        public List<SheriffAwayLocation> AwayLocations { get; set; }
        public List<SheriffLeave> Leaves { get; set; }
        public List<SheriffTraining> Training { get; set; }
        public byte[] Photo { get; set; }
    }
}
