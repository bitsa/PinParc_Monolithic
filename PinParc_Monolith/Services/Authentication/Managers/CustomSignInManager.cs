using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PinParc_Monolith.Services.Authentication.Managers
{
    public class CustomSignInManager : SignInManager<IdentityUser>
    {
        private readonly IConfiguration _configuration;
        private ApplicationUserClaimsPrincipalFactory _claimsFactory;
        public CustomSignInManager(
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<IdentityUser>> logger,
            IAuthenticationSchemeProvider schemes
            ) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, new DefaultUserConfirmation<IdentityUser>())
        {
            _configuration = configuration;
            _claimsFactory = claimsFactory as ApplicationUserClaimsPrincipalFactory;
        }


        public async Task<(SignInResult, JObject)> PasswordSignInReturnJWTasync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            IdentityUser byNameAsync = await this.UserManager.FindByNameAsync(userName);
            if ((object)byNameAsync == null)
                return (SignInResult.Failed, new JObject());

            var signInres = await this.PasswordSignInAsync(byNameAsync, password, isPersistent, lockoutOnFailure);

            //var signInres = await PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
            if (signInres.Succeeded)
            {
                var userClaims = await _claimsFactory.GetUserClaims(byNameAsync);//await UserManager.GetClaimsAsync(byNameAsync);
                /*ToDo: Generate JTW*/
                var tokenJson = GenerateJwtToken(userClaims.Claims.ToList());
                return (SignInResult.Success, tokenJson);
            }
            return (signInres, new JObject());
        }

        public async Task<(SignInResult, JObject)> SignInReturnJWTasync(string userName, bool isPersistent)
        {
            IdentityUser byNameAsync = await this.UserManager.FindByNameAsync(userName);
            if ((object)byNameAsync == null)
                return (SignInResult.Failed, new JObject());

            try
            {
                await this.SignInAsync(byNameAsync, isPersistent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (SignInResult.Failed, new JObject());
            }

            var userClaims = await _claimsFactory.GetUserClaims(byNameAsync);//await UserManager.GetClaimsAsync(byNameAsync);
            /*ToDo: Generate JTW*/
            var tokenJson = GenerateJwtToken(userClaims.Claims.ToList());
            return (SignInResult.Success, tokenJson);
        }

        private JObject GenerateJwtToken(IList<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                null,
                null,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenObj = new JwtSecurityTokenHandler().WriteToken(token);

            var tokenProp = new JProperty("token", JsonConvert.SerializeObject(tokenObj));


            var tokenJson = new JObject(tokenProp);
            return tokenJson;
        }

    }
}
