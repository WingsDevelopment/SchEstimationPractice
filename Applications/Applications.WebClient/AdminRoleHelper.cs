using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Applications.WebbClient
{
    public static class AdminRoleHelper
    {
        public static string AdminPass { get; set; }

        public static bool IsAdmin(string pass)
        {
            return AdminPass == pass;
        }
    }
}
