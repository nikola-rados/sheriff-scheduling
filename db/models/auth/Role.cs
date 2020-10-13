﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;

namespace SS.Db.models.auth
{
    [AdaptTo("[name]Dto")]
    public class Role : BaseEntity
    {
        public const string Manager = nameof(Manager);
        public const string Administrator = nameof(Administrator);
        public const string Sheriff = nameof(Sheriff);
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        [AdaptIgnore]
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
