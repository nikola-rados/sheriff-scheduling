using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using db.models;
using db.models.auth;

namespace SS.Db.models.auth
{
    public class User
    {
        public User()
        {
            CreatedOn = DateTime.Now;
        }
        [Key]
        public Guid Id { get; set; }
        public bool IsDisabled { get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string BadgeNumber { get; set;}
        public string Rank { get; set; }
        public List<Role> Roles { get; set; }
        public List<Permission> Permissions { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedById { get; set; }
        public User UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
