using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VueNote.Core.UserManagement;

namespace VueNote.WebApi.Extensions
{
    public static partial class ControllerExtension
    {
        public static async Task<User> GetCurrentUser(this Controller controller)
        {
            string userIdString = controller.User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return null;
            }

            int userId = int.Parse(userIdString);
            var user = await UserService.GetUser(userId);
            return user;
        }
    }
}
