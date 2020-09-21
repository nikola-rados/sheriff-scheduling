using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using SS.Api.helpers.extensions;
using SS.Api.Models.DB;
using SS.Db.models.auth;

namespace SS.Api.services
{
    public class AuthService
    {
        private readonly SheriffDbContext _db;
        public AuthService(SheriffDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<User> UpsertUserFromClaims(ClaimsIdentity claimsIdentity)
        {
            var claims = claimsIdentity.Claims.ToList();
            var id = Guid.Parse(claims.GetValueByType(ClaimTypes.NameIdentifier)); 
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var user = await _db.AuthUser.FindAsync(id);
                if (user != null)
                {
                    user.LastLogin = DateTime.Now;
                }
                else
                {
                    user = new User
                    {
                        Id = Guid.Parse(claims.GetValueByType(ClaimTypes.NameIdentifier)),
                        FirstName = claims.GetValueByType(ClaimTypes.GivenName),
                        LastName = claims.GetValueByType(ClaimTypes.Name),
                        Email = claims.GetValueByType(ClaimTypes.Email),
                        IsDisabled = true
                    };
                    await _db.AuthUser.AddAsync(user);
                }
                await _db.SaveChangesAsync();
                scope.Complete();
                return user;
            }
        }
    }
}
