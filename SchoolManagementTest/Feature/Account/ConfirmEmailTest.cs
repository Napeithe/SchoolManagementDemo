using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Domain;
using Moq;
using SchoolManagement.Features.Account.ConfirmEmail;
using SchoolManagement.Infrastructure;
using SchoolManagement.Infrastructure.Identity;
using SchoolManagementTest.Moq;
using Xunit;

namespace SchoolManagementTest.Feature.Account
{
    public class ConfirmEmailTest
    {
        [Fact]
        public async Task ShouldActivateAccount()
        {
            //Arrange
            Mock<UserManager<User>> userManagerMock = UserManagerMoq.Get();

            User user = new User()
            {
                EmailConfirmed = false
            };
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            string tokenEncoded = HttpUtility.HtmlEncode("przykaldoweasdadsa@#+==32323232");
            Command command = new Command
            {
                Email = "test@test.pl",
                Token = tokenEncoded
            };


            string tokenDecoded = HttpUtility.HtmlDecode(command.Token);
            userManagerMock.Setup(x => x.ConfirmEmailAsync(user, tokenDecoded))
                .ReturnsAsync(IdentityResult.Success);
            //Act
            DataResult result = await new Handler(userManagerMock.Object).Handle(command, CancellationToken.None);
            //Assert
            result.Status.Should().Be(DataResult.ResultStatus.Success);
            userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<String>()), Times.Once);
            userManagerMock.Verify(x => x.ConfirmEmailAsync(user, tokenDecoded), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnErrorWhenUserHasActivatedAccount()
        {
            //Arrange
            Mock<UserManager<User>> userManagerMock = UserManagerMoq.Get();

            User user = new User()
            {
                EmailConfirmed = true
            };
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            string tokenEncoded = HttpUtility.HtmlEncode("przykaldoweasdadsa@#+==32323232");
            Command command = new Command
            {
                Email = "test@test.pl",
                Token = tokenEncoded
            };


            string tokenDecoded = HttpUtility.HtmlDecode(command.Token);
            userManagerMock.Setup(x => x.ConfirmEmailAsync(user, tokenDecoded))
                .ReturnsAsync(IdentityResult.Success);
            //Act
            var result = await new Handler(userManagerMock.Object).Handle(command, CancellationToken.None);
            //Assert
            Assert.Equal(PolishReadableMessage.Account.UserHasActivatedAccountAlready, result.Message);
               result.Status.Should().Be(DataResult.ResultStatus.Error);
            userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<String>()), Times.Once);
            userManagerMock.Verify(x => x.ConfirmEmailAsync(user, tokenDecoded), Times.Never);
        }

        [Fact]
        public async Task ShouldReturnErrorWhenTokenIsInvalid()
        {
            //Arrange
            Mock<UserManager<User>> userManagerMock = UserManagerMoq.Get();

            User user = new User()
            {
                EmailConfirmed = false
            };
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            string tokenEncoded = HttpUtility.HtmlEncode("przykaldoweasdadsa@#+==32323232");
            Command command = new Command
            {
                Email = "test@test.pl",
                Token = tokenEncoded
            };


            string tokenDecoded = HttpUtility.HtmlDecode(command.Token);
            string invalidTokenError = new PolishIdentityErrorDescriber().InvalidToken().Description;
            userManagerMock.Setup(x => x.ConfirmEmailAsync(user, tokenDecoded))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError(){Description = invalidTokenError}));
            //Act
            var result = await new Handler(userManagerMock.Object).Handle(command, CancellationToken.None);
            //Assert
               result.Status.Should().Be(DataResult.ResultStatus.Error);
            Assert.Equal(invalidTokenError, result.Message);
            userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<String>()), Times.Once);
            userManagerMock.Verify(x => x.ConfirmEmailAsync(user, tokenDecoded), Times.Once);
        }

        [Fact]
        public async Task ShouldReturnErrorUserNotExist()
        {
            //Arrange
            Mock<UserManager<User>> userManagerMock = UserManagerMoq.Get();

         
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            string tokenEncoded = HttpUtility.HtmlEncode("przykaldoweasdadsa@#+==32323232");
            Command command = new Command
            {
                Email = "test@test.pl",
                Token = tokenEncoded
            };


            string tokenDecoded = HttpUtility.HtmlDecode(command.Token);
            userManagerMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), tokenDecoded))
                .ReturnsAsync(IdentityResult.Success);
            //Act
            var result= await new Handler(userManagerMock.Object).Handle(command, CancellationToken.None);
            //Assert
               result.Status.Should().Be(DataResult.ResultStatus.Error);
            Assert.Equal(PolishReadableMessage.Account.CannotActivateEmailAccountNotExist, result.Message);
            userManagerMock.Verify(x => x.FindByEmailAsync(It.IsAny<String>()), Times.Once);
            userManagerMock.Verify(x => x.ConfirmEmailAsync(It.IsAny<User>(), tokenDecoded), Times.Never);
        }
    }
}
