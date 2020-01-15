using System.Threading;
using System.Threading.Tasks;
using Builders;
using Model;
using Model.Domain;
using SchoolManagement.Features.Anchors.Detail;
using Xunit;

namespace SchoolManagementTest.Feature.AnchorsTest
{
    public class DetailTest
    {
        [Theory]
        [InlineData("")]
        [InlineData("3232323")]
        [InlineData(null)]
        public async Task WhenUserIdIsEmptyShouldThrowExceptionUserNotFound(string id)
        {
            //Arrange
                SchoolManagementContext context = new ContextBuilder().BuildClean();
            Query query = new Query
            {
                Id = id
            };
            //Act
            AnchorDetailException anchorDetailException = await Assert.ThrowsAsync<AnchorDetailException>(async ()=>
                await new Handler(context).Handle(query, CancellationToken.None));
            //Assert
            Assert.Equal(PolishReadableMessage.Anchors.NotFound, anchorDetailException.Message);
        }

        [Fact]
        public async Task ShouldReturnUserDetail()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            User user = new UserBuilder(context)
                .WithName("Jan", "Kowalski")
                .With(x=>x.PhoneNumber = "123123123")
                .WithEmail("test@test.pl").BuildAndSave();

            Query query = new Query
            {
                Id = user.Id
            };
            //Act
            AnchorDetail result = await new Handler(context).Handle(query, CancellationToken.None);
            //Assert
            Assert.NotNull(result);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.PhoneNumber, result.PhoneNumber);
        }
    }
}
