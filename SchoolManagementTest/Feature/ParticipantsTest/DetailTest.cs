using System.Threading;
using System.Threading.Tasks;
using Builders;
using FluentAssertions;
using Model;
using Model.Domain;
using SchoolManagement.Features.Participants.Detail;
using SchoolManagement.Infrastructure;
using Xunit;

namespace SchoolManagementTest.Feature.ParticipantsTest
{
    public class DetailTest
    {
        [Theory]
        [InlineData("")]
        [InlineData("3232323")]
        [InlineData(null)]
        public async Task WhenUserIdIsEmptyShouldReturnUserNotFound(string id)
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            Query query = new Query
            {
                Id = id
            };
            //Act
            var result = await new Handler(context).Handle(query, CancellationToken.None);
            //Assert
            result.Status.Should().Be(DataResult.ResultStatus.Error);
            result.Message.Should().Be(PolishReadableMessage.Anchors.NotFound);
        }

        [Fact]
        public async Task ShouldReturnUserDetail()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            User user = new UserBuilder(context)
                .WithName("Jan", "Kowalski")
                .With(x => x.PhoneNumber = "123123123")
                .WithEmail("test@test.pl").BuildAndSave();

            Query query = new Query
            {
                Id = user.Id
            };
            //Act
            var resultData = await new Handler(context).Handle(query, CancellationToken.None);
            var result = resultData.Data;
            //Assert
            Assert.NotNull(result);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.PhoneNumber, result.PhoneNumber);
        }
    }
}
