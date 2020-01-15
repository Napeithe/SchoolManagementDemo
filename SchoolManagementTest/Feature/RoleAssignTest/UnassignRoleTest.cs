using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Auth;
using Model.Domain;
using Moq;
using Xunit;
using SchoolManagement.Features.RoleAssign.Unassign;
using SchoolManagementTest.Moq;

namespace SchoolManagementTest.Feature.RoleAssignTest
{
    public class UnassignRoleTest
    {
        [Theory]
        [InlineData(Roles.Anchor)]
        [InlineData(Roles.Participant)]
        [InlineData(Roles.Admin)]
        public async Task ShouldThrowExceptionMissingPermission(string roleName)
        {
            Mock<UserManager<User>> userManagerMock = UserManagerMoq.Get();
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipalBuilder().Build();

            Command cmd = new Command
            {
                RoleName = roleName,
                UserId = Guid.NewGuid().ToString(),
                ClaimsPrincipal = claimsPrincipal
            };

            UnassignRoleException unassignRoleException = await Assert.ThrowsAsync<UnassignRoleException>(async () => await new Handler(userManagerMock.Object).Handle(cmd, CancellationToken.None));

            Assert.Equal(PolishReadableMessage.Assign.DontHavePermissionForUnassignThisRole, unassignRoleException.Message);
            userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
            userManagerMock.Verify(x => x.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }
        [Fact]
        public async Task ShouldThrowExceptionUserNotFound()
        {
            Mock<UserManager<User>> userManagerMock = UserManagerMoq.Get();
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipalBuilder().AddPermissionClaim(Permissions.Anchors.Remove).Build();
            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            Command cmd = new Command
            {
                RoleName = Roles.Anchor,
                UserId = Guid.NewGuid().ToString(),
                ClaimsPrincipal = claimsPrincipal
            };

            UnassignRoleException unassignRoleException = await Assert.ThrowsAsync<UnassignRoleException>(async () => await new Handler(userManagerMock.Object).Handle(cmd, CancellationToken.None));

            Assert.Equal(PolishReadableMessage.UserNotFound, unassignRoleException.Message);
            userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(x => x.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ShouldRemoveRoleFromUser()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            Mock<UserManager<User>> userManagerMock = UserManagerMoq.Get();
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipalBuilder().AddPermissionClaim(Permissions.Anchors.Remove).Build();
            var user = new UserBuilder(context).WithEmail("eamil@test.pl").BuildAndSave();
            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            Command cmd = new Command
            {
                RoleName = Roles.Anchor,
                UserId = Guid.NewGuid().ToString(),
                ClaimsPrincipal = claimsPrincipal
            };

            await new Handler(userManagerMock.Object).Handle(cmd, CancellationToken.None);

            userManagerMock.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(x => x.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }
    }
}
