using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace PinParc_Monolith.Services.Authentication
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<IdentityUser> userManager,
            IOptions<IdentityOptions> optionsAccessor,
            IHttpContextAccessor httpContext)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var id = await base.GenerateClaimsAsync(user);
            id.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            id.AddClaim(new Claim("UserName", user.UserName));
            id.AddClaim(new Claim("UserID", user.Id));
            return id;
        }

        public async Task<ClaimsIdentity> GetUserClaims(IdentityUser user)
        {
            var id = await GenerateClaimsAsync(user);
            return id;
        }
    }
}
