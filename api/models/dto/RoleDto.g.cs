using System;

namespace SS.Api.Models.Dto
{
    public partial class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}