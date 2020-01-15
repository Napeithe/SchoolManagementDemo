using System.Threading;
using System.Threading.Tasks;
using Builders;
using Model;
using Model.Domain;
using SchoolManagement.Features.Users.UserList;
using Xunit;

namespace SchoolManagementTest.Feature.Users
{
    public class UserListTest
    {
        [Fact]
        public async Task ExecuteShouldReturnUsersWithoutAdministrator()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            //Arrange
            new UserBuilder(context).With(x => x.FirstName = "Firs name")
                .With(x => x.LastName = "SuperAdmin")
                .WithRole(x=>x.WithName(Roles.SuperAdmin)).BuildAndSave();
            new UserBuilder(context).With(x => x.FirstName = "Firs name")
                .With(x => x.LastName = "Admin")
                .WithRole(x => x.WithName(Roles.Admin)).BuildAndSave();
            new UserBuilder(context).With(x => x.FirstName = "Firs name")
                .With(x => x.LastName = "Anchor")
                .WithRole(x => x.WithName(Roles.Anchor)).BuildAndSave();
            new UserBuilder(context).With(x => x.FirstName = "Firs name")
                .With(x => x.LastName = "Participant")
                .WithRole(x => x.WithName(Roles.Participant)).BuildAndSave();
            //Act
            Query query = new Query()
            {
                IsIncludeAdministrators = false
            };

            UsersListVierModel result = await new Handler(context).Handle(query, CancellationToken.None);
            //Arrange
            Assert.NotNull(result);
            Assert.NotEmpty(result.UserDtoList);
            Assert.Equal(2, result.UserDtoList.Count);
        }
        [Fact]
        public async Task ExecuteShouldReturnUsersWithAdministrator()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            //Arrange
            new UserBuilder(context).With(x => x.FirstName = "Firs name")
                .With(x => x.LastName = "SuperAdmin")
                .WithRole(x => x.WithName(Roles.SuperAdmin)).BuildAndSave();
            new UserBuilder(context).With(x => x.FirstName = "Firs name")
                .With(x => x.LastName = "Admin")
                .WithRole(x => x.WithName(Roles.Admin)).BuildAndSave();
            new UserBuilder(context).With(x => x.FirstName = "Firs name")
                .With(x => x.LastName = "Anchor")
                .WithRole(x => x.WithName(Roles.Anchor)).BuildAndSave();
            new UserBuilder(context).With(x => x.FirstName = "Firs name")
                .With(x => x.LastName = "Participant")
                .WithRole(x => x.WithName(Roles.Participant)).BuildAndSave();
            //Act
            Query query = new Query()
            {
                IsIncludeAdministrators = true
            };

            UsersListVierModel result = await new Handler(context).Handle(query, CancellationToken.None);
            //Arrange
            Assert.NotNull(result);
            Assert.NotEmpty(result.UserDtoList);
            Assert.Equal(3, result.UserDtoList.Count);
        }
    }
}
