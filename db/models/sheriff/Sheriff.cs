using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public virtual List<SheriffAwayLocation> AwayLocation { get; set; } = new List<SheriffAwayLocation>();
        public virtual List<SheriffLeave> Leave { get; set; } = new List<SheriffLeave>();
        public virtual List<SheriffTraining> Training { get; set; } = new List<SheriffTraining>();
        public byte[] Photo { get; set; }
    }
}
