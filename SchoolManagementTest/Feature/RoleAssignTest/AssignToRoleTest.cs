using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Domain;
using Moq;
using SchoolManagement.Features.RoleAssign;
using SchoolManagement.Features.RoleAssign.Assign;
using SchoolManagementTest.Moq;
using Xunit;

namespace SchoolManagementTest.Feature.RoleAssignTest
{
    public class AssignToRoleTest
    {
        [Fact]
        public async Task WhenUserNotFoundShouldThrowException()
        {
            //Arrange
            Mock<UserManager<User>> userManagerMock = UserManagerMoq.Get();
            var cmd = new Command()
            {
                UserId = Guid.NewGuid().ToString()
            };
            //Act
            AssignToRoleException exception = await Assert.ThrowsAsync<AssignToRoleException>(async () =>
                await new Handler(userManagerMock.Object).Handle(cmd, CancellationToken.None));
            //Assert
            Assert.Equal(PolishReadableMessage.Assign.UserNotFound, exception.Message);
            userManagerMock.Verify(x=>x.FindByIdAsync(cmd.UserId),Times.Once);
            userManagerMock.Verify(x=>x.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>()),Times.Never);
            userManagerMock.Verify(x=>x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()),Times.Never);
        }

        [Fact]
        public async Task WhenUserIsAssignedToRoleAlreadyShouldThrowException()
        {
            //Arrange
            var context = new ContextBuilder().BuildClean();
            Mock<UserManager<User>> userManagerMock = UserManagerMoq.Get();
            User user = new UserBuilder(context).WithEmail("email@email.com").BuildAndSave();
            var cmd = new Command()
            {
                UserId = user.Id,
                RoleName = Roles.Admin
            };
            userManagerMock.Setup(x => x.FindByIdAsync(cmd.UserId)).ReturnsAsync(user);
            userManagerMock.Setup(x => x.IsInRoleAsync(user, cmd.RoleName)).ReturnsAsync(true);
            //Act
            AssignToRoleException exception = await Assert.ThrowsAsync<AssignToRoleException>(async () =>
                await new Handler(userManagerMock.Object).Handle(cmd, CancellationToken.None));
            //Assert
            Assert.Equal(PolishReadableMessage.Assign.UserIsAssignedToRoleAlready, exception.Message);
            userManagerMock.Verify(x => x.FindByIdAsync(cmd.UserId), Times.Once);
            userManagerMock.Verify(x => x.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ShouldAssignToRole()
        {
            //Arrange
            var context = new ContextBuilder().BuildClean();
            Mock<UserManager<User>> userManagerMock = UserManagerMoq.Get();
            User user = new UserBuilder(context).WithEmail("email@email.com").BuildAndSave();
            var cmd = new Command()
            {
                UserId = user.Id,
                RoleName = Roles.Admin
            };
            userManagerMock.Setup(x => x.FindByIdAsync(cmd.UserId)).ReturnsAsync(user);
            userManagerMock.Setup(x => x.IsInRoleAsync(user, cmd.RoleName)).ReturnsAsync(false);
            //Act
            await new Handler(userManagerMock.Object).Handle(cmd, CancellationToken.None);
            //Assert
            userManagerMock.Verify(x => x.FindByIdAsync(cmd.UserId), Times.Once);
            userManagerMock.Verify(x => x.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }
    }
}
