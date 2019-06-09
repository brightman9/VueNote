using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace VueNote.Core.UserManagement
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string HashedPassword { get; set; }
        public int RoleId { get; set; }

        [Write(false)]
        public Role Role { get; set; }
    }
}
