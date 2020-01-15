using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using Model;
using Model.Domain;
using Model.Dto;
using SchoolManagement.Features.Participants.List;
using Xunit;

namespace SchoolManagementTest.Feature.ParticipantsTest
{
    public class ParticipantsListTest
    {
        [Fact]
        public async Task ExecuteShouldReturnOnlyAnchorUsers()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            for (var i = 0; i < 10; i++)
            {
                new UserBuilder(context)
                    .WithEmail($"test@test-{i}.pl")
                    .WithName("Jan", $"Kowalski {i}").BuildAndSave();
            }
            List<User> users = context.Users.ToList();

            var expectedUser = users[0];
            new RoleBuilder(context)
                .WithDescription("Wazna rola")
                .WithName(Roles.Participant)
                .AddUserToRole(expectedUser)
                .AddUserToRole(users[2])
                .AddUserToRole(users[4])
                .BuildAndSave();

            new RoleBuilder(context)
                .WithDescription("Wazna rola")
                .WithName(Roles.Admin)
                .AddUserToRole(users[1])
                .AddUserToRole(users[2])
                .AddUserToRole(users[5])
                .BuildAndSave();

            Query query = new Query();
            //Act
            List<ParticipantItemDto> result = await new Handler(context).Handle(query, CancellationToken.None);
            //Arrange
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            var numberOfUserExpected = 3;

            Assert.Equal(numberOfUserExpected, result.Count);
            ParticipantItemDto anchorDto = result[0];
            Assert.Equal($"{expectedUser.FirstName} {expectedUser.LastName}", anchorDto.Name);
            Assert.Equal(expectedUser.Id, anchorDto.Id);
            Assert.Equal(expectedUser.Email, anchorDto.Email);
        }
    }
}
