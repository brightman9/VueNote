using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VueNote.Core.Note;
using VueNote.Core.UserManagement;
using VueNote.WebApi.ViewModels;
using VueNote.WebApi.Extensions;
using VueNote.Core.Util;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VueNote.WebApi.Controllers
{
    public class UserController : Controller
    {
        public async Task<IActionResult> IsAuthenticated()
        {
            var currentUser = await this.GetCurrentUser();

            bool isUserAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            return Ok(new { isUserAuthenticated });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginForm loginUser)
        {
            bool isValidUser = await UserService.ValidUser(loginUser.UserName, loginUser.Password);
            if (isValidUser)
            {
                var user = await UserService.GetUser(loginUser.UserName);
                var userDto = new { Id = user.Id, Name = user.Name, DisplayName = user.DisplayName };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigHelper.JwtSettings.SigningKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    issuer: ConfigHelper.JwtSettings.Issuer,
                    audience: ConfigHelper.JwtSettings.Audience,
                    claims: new List<Claim> { new Claim("Id", user.Id.ToString()) },
                    expires: DateTime.Now.AddMinutes(60 * 24),
                    signingCredentials: credentials
                );
                var tokenString = "Bearer " + new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                var permissions = await UserService.GetUserPermissions(user.Id);
                var permissionsDto = permissions.Select(t => t.Name);

                return Ok(new
                {
                    isLogin = true,
                    token = tokenString,
                    user = userDto,
                    permissions = permissionsDto
                });
            }
            else
            {
                return Ok(new { IsLogin = false, Message = "用户名或密码错误" });
            }
        }
    }
}
