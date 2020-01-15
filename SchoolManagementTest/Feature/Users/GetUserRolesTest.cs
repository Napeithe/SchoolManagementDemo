using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model;
using Model.Domain; 
using SchoolManagement.Features.Users.UserRoles;
using Xunit;

namespace SchoolManagementTest.Feature.Users
{
    public class GetUserRolesTest
    {
        [Fact]
        public async Task ExecuteShouldReturnListOfRoles()
        {
            //Arrange
            var roles = new List<Role>
            {
                new Role("Admin", "Administrator"),
                new Role("Participant", "Uczestnik"),
            };
            SchoolManagementContext db = new ContextBuilder().BuildClean();
            await db.Roles.AddRangeAsync(roles);
            await db.SaveChangesAsync();
            Query query = new Query()
            {
                SelectedId = new List<string> { roles.First().Id}
            };
            //Act
            List<SelectListItem> result = await new Handler(db).Handle(query, CancellationToken.None);
            //Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(roles.Count, result.Count);
        }
    }
}
