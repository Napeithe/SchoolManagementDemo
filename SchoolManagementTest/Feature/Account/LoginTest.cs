using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Auth;
using Moq;
using SchoolManagement.Features.Account.Login;
using SchoolManagementTest.Moq;
using Xunit;
using Model.Domain;
using SchoolManagement.Infrastructure;

namespace SchoolManagementTest.Feature.Account
{
    public class LoginTest
    {
        [Fact]
        public async Task ShouldInvoqueSigninManager()
        {
            //Arrange
            Mock<SignInManager<User>> signInManager = SignInManagerMoq.Get();
            SchoolManagementContext context = new ContextBuilder().BuildClean();

            signInManager.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<Model.Domain.User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);
            Model.Domain.User user = new Model.Domain.User()
            {
                EmailConfirmed = true
            };
            signInManager.Setup(x => x.SignInAsync(It.IsAny<Model.Domain.User>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(Task.FromResult(user));
            signInManager.Setup(x => x.CreateUserPrincipalAsync(It.IsAny<User>())).ReturnsAsync(new ClaimsPrincipal());
            Mock<UserManager<User>> userManger = UserManagerMoq.Get();

            userManger.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            Claim adminClaim = new Claim(CustomClaimTypes.Permission, Permissions.Users.SeeAllUser);

            signInManager.Object.UserManager = userManger.Object;

            Command cmd = new Command
            {
                Password = "Qwerty123_",
                Email = "administrator@mojasprawa.com"
            };
            //Act
            var result = await new Handler(signInManager.Object, context).Handle(cmd, CancellationToken.None);
            //Assert
            userManger.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            signInManager.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            signInManager.Verify(x => x.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
            result.Status.Should().Be(DataResult.ResultStatus.Success);
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldThrowExceptionWhenUserHasNotEmailConfirmedAccountIsNotActivated()
        {
            //Arrange
            Mock<SignInManager<User>> signInManager = SignInManagerMoq.Get();
            SchoolManagementContext context = new ContextBuilder().BuildClean();

            signInManager.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<Model.Domain.User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);
            Model.Domain.User user = new Model.Domain.User()
            {
                EmailConfirmed = false
            };
            signInManager.Setup(x => x.SignInAsync(It.IsAny<Model.Domain.User>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(Task.FromResult(user));
            Mock<UserManager<User>> userManger = UserManagerMoq.Get();

            userManger.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            signInManager.Object.UserManager = userManger.Object;

            Command cmd = new Command
            {
                Password = "Qwerty123_",
                Email = "administrator@mojasprawa.com"
            };
            //Act
            var result = await new Handler(signInManager.Object, context).Handle(cmd, CancellationToken.None);
            //Assert
            userManger.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            signInManager.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            signInManager.Verify(x => x.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
            result.Status.Should().Be(DataResult.ResultStatus.Error);
            Assert.Equal("Konto jest nieaktywne.", result.Message);
        }
        [Fact]
        public async Task ShouldThrowExceptionWhenPasswordIsInvalid()
        {
            //Arrange
            Mock<SignInManager<User>> signInManager = SignInManagerMoq.Get();
            SchoolManagementContext context = new ContextBuilder().BuildClean();

            signInManager.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);
            User user = new User()
            {
            };
            signInManager.Setup(x => x.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(Task.FromResult(user));
            Mock<UserManager<User>> userManger = UserManagerMoq.Get();

            userManger.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            signInManager.Object.UserManager = userManger.Object;

            Command cmd = new Command
            {
                Password = "Qwerty123_",
                Email = "administrator@mojasprawa.com"
            };
            //Act
            var result = await new Handler(signInManager.Object, context).Handle(cmd, CancellationToken.None);
            //Assert
            userManger.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            signInManager.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            signInManager.Verify(x => x.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
            result.Status.Should().Be(DataResult.ResultStatus.Error);
            Assert.Equal("Błędny login lub hasło.", result.Message);
        }

        [Fact]
        public async Task ShouldThrowInvalidPasswordOrUserNameWhenUserNotExist()
        {
            //Arrange
            Mock<SignInManager<User>> signInManager = SignInManagerMoq.Get();

            SchoolManagementContext context = new ContextBuilder().BuildClean();
            Mock<UserManager<User>> userManger = UserManagerMoq.Get();
            userManger.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            signInManager.Object.UserManager = userManger.Object;
            Command cmd = new Command
            {
                Password = "Qwerty123_",
                Email = "administrator@mojasprawa.com"
            };
            //Act
            var result = await new Handler(signInManager.Object, context).Handle(cmd, CancellationToken.None);
            //Assert
            signInManager.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<Model.Domain.User>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
            signInManager.Verify(x => x.SignInAsync(It.IsAny<Model.Domain.User>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
            result.Status.Should().Be(DataResult.ResultStatus.Error);
            Assert.Equal("Błędny login lub hasło.", result.Message);
        }
    }
}