using Caviar.Infrastructure.API.BaseApi;
using System.ComponentModel;
using Caviar.SharedKernel.Entities;
using Caviar.SharedKernel.Entities.View;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;


namespace Caviar.Infrastructure.API
{
    public partial class ApplicationUserController : EasyBaseApiController<ApplicationUserView,ApplicationUser>
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ApplicationUserController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Register(ApplicationUserView model)
        {
            var newUser = model.Entity;

            var result = await _userManager.CreateAsync(newUser);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);

                return BadRequest(errors);

            }
            return Ok("账号注册成功");
        }

        [HttpPost]
        public async Task<IActionResult> Login(ApplicationUserView login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Entity.UserName, login.Entity.PasswordHash, false, false);

            if (!result.Succeeded) return BadRequest("Username and password are invalid.");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, login.Entity.UserName)
            };
            var jwt = Configure.CaviarConfig.JWTOptions;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.JwtSecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(jwt.JwtExpiryInDays));

            var token = new JwtSecurityToken(
                jwt.JwtIssuer,
                jwt.JwtAudience,
                claims,
                expires: expiry,
                signingCredentials: creds
            );

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

    }
}