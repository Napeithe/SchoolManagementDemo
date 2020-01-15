using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Domain;

namespace Model.DataSeeders
{
    public class DataSeeder
    {
        private List<DataSeederCore> Seeders { get; set; }
        private readonly IConfiguration _configuration;

        public DataSeeder(IServiceProvider serviceProvider)
        {
            SchoolManagementContext context = serviceProvider.GetService<SchoolManagementContext>();

            if (!context.Database.IsInMemory())
            {
                context.Database.Migrate();
            }

            _configuration = serviceProvider.GetService<IConfiguration>(); 
            RoleManager<Role> roleManager = serviceProvider.GetService<RoleManager<Role>>(); 
            UserManager<User> userManager = serviceProvider.GetService<UserManager<User>>(); 

            Seeders = new List<DataSeederCore>();

            if (_configuration.GetValue<bool>("DesirableDataSeeders:Role")) { Seeders.Add(new RoleDataSeeder(context, roleManager)); }
            if (_configuration.GetValue<bool>("DesirableDataSeeders:User")) { Seeders.Add(new UserDataSeeder(context, userManager)); }

            if (_configuration.GetValue<bool>("DesirableDataSeeders:GroupLevel"))
            {
                Seeders.Add(new GroupLevelDataSeeder(context));
            }

        }

        public async Task SeedDataAsync()
        {
            foreach (var item in Seeders)
            {
                if (!item.DataExists())
                {
                    await item.SeedData();
                }
            }
        }
    }
}
