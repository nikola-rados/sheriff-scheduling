using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [AdaptIgnore]
        public string PreferredUsername { get; set; }
        [AdaptIgnore]
        public Guid IdirId { get; set; }
        public bool IsDisabled { get; set;}
        public Location HomeLocation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public ICollection<UserRole> Roles { get; set; }
        public ICollection<Permission> Permissions { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
