using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using FluentAssertions;
using Model;
using Model.Domain;
using SchoolManagement.Features.GroupClass.Remove;
using SchoolManagement.Infrastructure;
using Xunit;

namespace SchoolManagementTest.Feature.GroupClassTest
{
    public class RemoveTest
    {
        [Fact]
        public async Task ExecuteShouldRemoveGroup()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            new RoleBuilder(context).WithName(Roles.Anchor).BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();
            var groupClass = new GroupClassBuilder(context)
                .WithName("Stara grupa")
                .WithRoom(builder => builder.WithName("Old room"))
                .WithGroupLevel(x => x.With(z => z.Name = "Beginner"))
                .AddAnchor(user => user.WithEmail("anchor1@gmail.com").WithName("Jan", "Kowalski"))
                .AddParticipant(user => user.WithEmail("participant1@gmail.com").WithName("Jan", "Kowalski"), ParticipantRole.Follower)
                .AddParticipant(user => user.WithEmail("participant3@gmail.com").WithName("Jan", "Nowak"), ParticipantRole.Follower).BuildAndSave();

            Command cmd = new Command
            {
                Id = groupClass.Id
            };

            DataResult dataResult = await new Handler(context).Handle(cmd, CancellationToken.None);


            dataResult.Status.Should().Be(DataResult.ResultStatus.Success);
            context.GroupClass.Should().BeEmpty("we remove this group");
        }
        [Fact]
        public async Task ExecuteShouldReturnErrorNotFound()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();

            Command cmd = new Command
            {
                Id = 2
            };

            DataResult dataResult = await new Handler(context).Handle(cmd, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Error);
            dataResult.Message.Should().Be(PolishReadableMessage.GroupClass.NotFound);
        }
    }
}
