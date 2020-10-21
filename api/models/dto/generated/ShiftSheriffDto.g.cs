using System;
using SS.Api.Models.Dto;

namespace SS.Api.Models.Dto
{
    public partial class ShiftSheriffDto
    {
        public int Id { get; set; }
        public SheriffDto Sheriff { get; set; }
        public Guid SheriffId { get; set; }
        public ShiftDto Shift { get; set; }
        public int ShiftId { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}