using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VueNote.Core.Util;

namespace VueNote.Core.UserManagement
{
    public static class UserService
    {
        public static async Task<User> GetUser(int userId)
        {
            var user = await DbHelper.Get<User>(userId);
            return user;
        }
        public static async Task<User> GetUser(string username)
        {
            string sql = "select * from User where Name = @Name";
            var user = await DbHelper.QueryFirst<User>(sql, new { Name = username });
            return user;
        }

        public static async Task<bool> ValidUser(string username, string password)
        {
            var user = await DbHelper.QueryFirst<User>(
                "select * from User where Name = @Name",
                new { Name = username });

            if (user == null)
                return false;

            return CryptoHelper.VerifyPassword(password, user.HashedPassword);
        }

        public static async Task<bool> HasPermission(int userId, string permission)
        {
            var permissions = await GetUserPermissions(userId);
            bool hasPermission = permissions.Count(t => t.Name == permission) > 0;
            return hasPermission;
        }

        public static async Task<IEnumerable<Permission>> GetUserPermissions(int userId)
        {
            var permissions = await DbHelper.Query<Permission>(
                @"select permission.* 
                from User as user
                left join Role as role on role.Id = user.RoleId
                left join Role_Permission_Relation as relation on relation.RoleId = role.Id
                left join Permission as permission on permission.Id = relation.PermissionId
                where user.Id = @UserId",
                new { UserId = userId });

            return permissions;
        }
    }
}
