using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SS.Api.Helpers.Extensions;
using SS.Api.infrastructure.exceptions;
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

        public async Task AssignRolesToUser(Guid userId, List<int> roleIds)
        {
            var user = await _db.User.FindAsync(userId);
            user.ThrowBusinessExceptionIfNull($"User with id {userId} does not exist.");

            foreach (var roleId in roleIds)
            {
                var role = await _db.Role.Include(r => r.UserRoles).FirstOrDefaultAsync(r => r.Id == roleId);
                role.ThrowBusinessExceptionIfNull($"Role with id {roleId} does not exist.");

                if (user.UserRoles.Any(r => r.UserId == userId && r.RoleId == roleId))
                    return;

                user.UserRoles.Add(new UserRole
                {
                    Role = role,
                    User = user
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task UnassignRoleFromUser(Guid userId, List<int> roleIds)
        {
            var user = await _db.User.Include(r => r.UserRoles).FirstOrDefaultAsync(u => u.Id == userId);
            user.ThrowBusinessExceptionIfNull($"User with id {userId} does not exist.");

            _db.RemoveRange(user.UserRoles.Where(r => r.UserId == userId && roleIds.Contains(r.RoleId)));

            await _db.SaveChangesAsync();
        }
    }
}
