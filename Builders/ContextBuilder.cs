using System;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Builders
{
    public class ContextBuilder
    {
        public SchoolManagementContext BuildClean()
        {
            DbContextOptionsBuilder<SchoolManagementContext> dbContextOptionsBuilder =
                new DbContextOptionsBuilder<SchoolManagementContext>();

            dbContextOptionsBuilder
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var databaseContext =
                new SchoolManagementContext(dbContextOptionsBuilder.Options);
            databaseContext.Database.EnsureCreated();

            return databaseContext;
        }
    }
}
