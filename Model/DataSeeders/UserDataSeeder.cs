using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Model.Domain;

namespace Model.DataSeeders
{
    public class UserDataSeeder : DataSeederCore
    {
        private readonly UserManager<User> _userManager;

        public UserDataSeeder(SchoolManagementContext context, UserManager<User> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public override bool DataExists()
        {
            return Context.Users.Any();
        }

        public override async Task SeedData()
        {
            var user = new User
            {
                UserName = "administrator",
                Email = "administrator@test.pl",
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(user, "123123");
            await _userManager.AddToRoleAsync(user, Roles.SuperAdmin);

        }
    }
}
