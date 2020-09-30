using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using db.models;
using Mapster;
using SS.Api.Models.DB;

namespace SS.Db.models.auth
{
    public class User : BaseEntity
    {
        public User()
        {
            CreatedOn = DateTime.Now;
        }
        [Key]
        public Guid Id { get; set; }
        public string IdirName { get; set; }
        [AdaptIgnore]
        public Guid IdirId { get; set; }
        [AdaptIgnore]
        public bool IsEnabled { get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [AdaptIgnore]
        public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
        [AdaptIgnore]
        public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        [AdaptIgnore]
        public DateTime? LastLogin { get; set; }
    }
}
