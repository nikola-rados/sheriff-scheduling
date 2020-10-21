using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SS.Api.Helpers.Extensions;
using SS.Api.infrastructure.exceptions;
using SS.Api.models.dto;
using SS.Api.Models.Dto;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.sheriff;

namespace SS.Api.services
{
    public class UserService
    {
        private readonly SheriffDbContext _db;
        public UserService(SheriffDbContext sheriffDbContext)
        {
            _db = sheriffDbContext;
        }
        
        public async Task<User> DisableUser(Guid id)
        {
            var user = await _db.User.FindAsync(id);
            user.ThrowBusinessExceptionIfNull($"User with the id: {id} could not be found. ");

            user.IsEnabled = false;
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User> EnableUser(Guid id)
        {
            var user = await _db.User.FindAsync(id);
            user.ThrowBusinessExceptionIfNull($"User with the id: {id} could not be found. ");

            user.IsEnabled = true;
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task AssignRolesToUser(List<UserRole> assignRoles)
        {
            foreach (var assignRole in assignRoles)
            {
                var user = await _db.User.FindAsync(assignRole.UserId);
                user.ThrowBusinessExceptionIfNull($"User with id {assignRole.UserId} does not exist.");

                var role = await _db.Role.AsSingleQuery().Include(r => r.UserRoles).FirstOrDefaultAsync(r => r.Id == assignRole.RoleId);
                role.ThrowBusinessExceptionIfNull($"Role with id {assignRole.RoleId} does not exist.");

                var savedUserRole = user.UserRoles.FirstOrDefault(ur =>
                    ur.UserId == assignRole.UserId &&
                    ur.RoleId == assignRole.RoleId);

                //Update if exists.
                if (savedUserRole != null)
                {
                    savedUserRole.Role = role;
                    savedUserRole.User = user;
                    savedUserRole.ExpiryDate = assignRole.ExpiryDate;
                    savedUserRole.EffectiveDate = assignRole.EffectiveDate;
                    savedUserRole.ExpiryReason = assignRole.ExpiryReason;
                }
                else
                {
                    user.UserRoles.Add(new UserRole
                    {
                        Role = role,
                        User = user,
                        ExpiryDate = assignRole.ExpiryDate,
                        EffectiveDate = assignRole.EffectiveDate
                    });
                }
            }
            await _db.SaveChangesAsync();
        }

        public async Task UnassignRoleFromUser(List<UserRole> unassignRoles)
        {
            foreach (var unassignRole in unassignRoles)
            {
                var user = await _db.User.AsSingleQuery().Include(r => r.UserRoles).FirstOrDefaultAsync(u => u.Id == unassignRole.UserId);
                user.ThrowBusinessExceptionIfNull($"User with id {unassignRole.UserId} does not exist.");

                var userRole = user.UserRoles.FirstOrDefault(r => r.UserId == unassignRole.UserId && r.RoleId == unassignRole.RoleId);
                if (userRole != null)
                {
                    userRole.ExpiryDate = DateTime.UtcNow;
                    userRole.ExpiryReason = unassignRole.ExpiryReason;
                }
            }
            await _db.SaveChangesAsync();
        }
    }
}
