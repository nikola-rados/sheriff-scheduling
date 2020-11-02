using Mapster;
using SS.Api.Models.DB;

namespace SS.Db.models.sheriff
{
    [AdaptTo("[name]Dto")]
    public class SheriffAwayLocation : SheriffEvent
    {
        public virtual Location Location { get; set; }
        public int? LocationId { get; set; }
    }
}
