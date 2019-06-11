using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using VueNote.Core.UserManagement;

namespace VueNote.WebApi.Common
{
    public class PermissionFilter : Attribute, IAsyncAuthorizationFilter
    {
        public string Permission { get; set; }

        public PermissionFilter(string permission)
        {
            this.Permission = permission;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string userIdString = context.HttpContext.User.FindFirst("Id")?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                var hasPermission = await UserService.HasPermission(userId, this.Permission);
                if (!hasPermission)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
        }
    }
}
