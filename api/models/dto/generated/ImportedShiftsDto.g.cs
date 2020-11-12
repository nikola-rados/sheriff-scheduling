using System.Collections.Generic;
using SS.Api.models.dto.generated;

namespace SS.Api.models.dto.generated
{
    public partial class ImportedShiftsDto
    {
        public List<ShiftDto> Shifts { get; set; }
        public List<string> ConflictMessages { get; set; }
    }
}