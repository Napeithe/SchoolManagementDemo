using System.Threading;
using System.Threading.Tasks;
using Builders;
using Model.Domain;
using SchoolManagement.Features.Users.Remove;
using Xunit;

namespace SchoolManagementTest.Feature.Users
{
    public class RemoveTest
    {
        [Fact]
        public async Task ShouldRemoveUser()
        {
            //Arrange
            var context = new ContextBuilder().BuildClean();
            User user = new UserBuilder(context).WithEmail("useremail@gmail.com")
                .WithName("FirstName", "LastName")
                .BuildAndSave();
            new RoleBuilder(context)
                .AddUserToRole(user)
                .BuildAndSave();
            Command cmd = new Command
            {
                Id = user.Id
            };
            //Act
            await new Handler(context).Handle(cmd, CancellationToken.None);
            //Assert
            Assert.Empty(context.Users);
            Assert.Empty(context.UserRoles);
        }
    }
}
