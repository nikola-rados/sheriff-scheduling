using Mapster;
using ss.db.models;

namespace SS.Db.models.sheriff
{
    [AdaptTo("[name]Dto")]
    public class SheriffLeave : SheriffEvent
    {
        public virtual LookupCode LeaveType { get; set; }
        public int? LeaveTypeId { get; set; }
    }
}
