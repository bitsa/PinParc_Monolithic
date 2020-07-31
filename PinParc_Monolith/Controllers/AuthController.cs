using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PinParc_Monolith.Models.DTO;
using PinParc_Monolith.Services.Authentication.Managers;

namespace PinParc_Monolith.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomSignInManager _signInManager;
        public AuthController(UserManager<IdentityUser> userManager,
            CustomSignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
       

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Token([FromBody]AuthenticateDTO model)
        {
            //var model = new IdentityUser()
            //{
            //    UserName = "TheGod"
            //};

            var pass = model.Password; //"meShevqmenZeca2@";

            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(pass))
                return BadRequest(new { message = "მომხმარებლის სახელი და პაროლი სავალდებულო ველებია" });
            //var tokenJson = await _signInManager.PasswordSignInReturnJWTasync(user.UserName,user.Password,true,false);

            //  var userFromDb = await _userManager.FindByNameAsync(user.UserName);

            //  var signInResult = await _signInManager.PasswordSignInAsync(userFromDb, user.Password, true, false);

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {

                //if (_env.EnvironmentName == "Development")
                //{
                //    var signInResult = await _signInManager.SignInReturnJWTasync(model.UserName, true);

                //    if (signInResult.Item1 == SignInResult.Success)
                //    {
                //        return Ok(signInResult.Item2);
                //    }
                //}


                var signInResultRelease = await _signInManager.PasswordSignInReturnJWTasync(model.UserName, pass, true, false);

                if (signInResultRelease.Item1 == Microsoft.AspNetCore.Identity.SignInResult.Success)
                {
                    //var userClaims = User.Claims;
                    //var res = GenerateJwtToken(userClaims.ToList());
                    //var tokenProp = new JProperty("token", JsonConvert.SerializeObject(res));
                    //var tokenJson = new JObject(tokenProp);
                    return Ok(signInResultRelease.Item2);
                }

                return BadRequest(new { message = signInResultRelease.Item1.ToString() });

            }

            // var tokenJson = new JObject(tokenProp);
            return BadRequest(new { message = "მომხმარებელი ან პაროლი არასწორია" });
        }


        [AllowAnonymous]
        [HttpPost]
        //[Route("Create")]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserDTO model)
        {

            //var newUser = new IdentityUser()
            //{
            //    UserName = "TheGod",
            //    Email = "crazy.bitsa@gmail.com",
            //    Id = Guid.NewGuid().ToString()
            //};

            var newUser = new IdentityUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                Id = Guid.NewGuid().ToString()
            };


            var createStatus = new IdentityResult();

            try
            {
                var test = await _userManager.CreateAsync(newUser, model.Password);
                createStatus = test;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (createStatus == IdentityResult.Success)
            {
                return Ok();
            }

            return BadRequest(createStatus.Errors);

        }

    }
}
