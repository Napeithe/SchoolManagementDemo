using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Model;
using Model.DataSeeders;

namespace SchoolManagement.Infrastructure
{
    public static class WebHostExtensions
    {
        public static async Task<IWebHost> SeedData(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;

                await new DataSeeder(services).SeedDataAsync();
            }

            return host;
        }
    }
}
