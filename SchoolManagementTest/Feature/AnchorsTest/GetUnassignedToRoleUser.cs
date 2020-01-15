using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model;
using Model.Domain;
using SchoolManagement.Features.RoleAssign;
using SchoolManagement.Features.RoleAssign.Assign;
using Xunit;

namespace SchoolManagementTest.Feature.AnchorsTest
{
    public class GetUnassignedToRoleUser
    {
        [Fact]
        public async Task WhenRoleNameIsNotSetShouldThrowException()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            var query = new Query();
            AssignToRoleException assignToRoleException = await Assert.ThrowsAsync<AssignToRoleException>(async () => await new QueryHandler(context).Handle(query, CancellationToken.None));
            Assert.Equal(PolishReadableMessage.Assign.RoleNameIsNotSet, assignToRoleException.Message);
        }

        [Fact]
        public async Task ShouldReturnUserWhichAreNotAssignedToSelectedRole()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            User userNotAssigned = new UserBuilder(context).WithEmail("2154@re.pl").BuildAndSave();
            User userAssigned = new UserBuilder(context).WithEmail("2321@re.pl").BuildAndSave();

            new RoleBuilder(context).WithName(Roles.Admin).AddUserToRole(userNotAssigned).BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Admin).AddUserToRole(userAssigned).BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Anchor).AddUserToRole(userAssigned).BuildAndSave();
            Query query = new Query
            {
                RoleName = Roles.Anchor
            };
            //Act
            List<SelectListItem> result = await new QueryHandler(context).Handle(query, CancellationToken.None);
            //Arrange
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Single(result);
            SelectListItem userItem = result.First();
            Assert.Equal(userNotAssigned.Id, userItem.Value);
            Assert.Contains(userNotAssigned.Email, userItem.Text);
        }
    }
}
