using System;
using System.ComponentModel.DataAnnotations;

namespace SS.Db.models.jc
{
    public class JcSynchronization
    {
        [Key]
        public int Id { get; set; }
        public DateTimeOffset LastSynchronization { get; set; }
    }
}
