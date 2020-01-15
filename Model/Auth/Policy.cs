using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Auth
{
    public static class Policy
    {
        public static List<string> GetAllPermissions()
        {
            return typeof(Permissions).GetNestedTypes()
                .SelectMany(x => x.GetFields())
                .Select(x => x.GetValue(null))
                .Cast<string>()
                .ToList();
        }
    }
}
