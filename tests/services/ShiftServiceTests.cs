using SS.Api.services.scheduling;
using tests.api.helpers;

namespace tests.services
{
    public class ShiftServiceTests : WrapInTransactionScope
    {
        public ShiftService ShiftService { get; set; }
        public ShiftServiceTests() : base(true)
        {
            ShiftService = new ShiftService(Db);
        }
    }
}
