using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace VueNote.Core.UserManagement
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        [Write(false)]
        public List<Permission> Permissions { get; set; }
    }
}
