using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using FluentAssertions;
using Model;
using Model.Domain;
using SchoolManagement.Features.Presences.RemoveNewUserFromClassTime.Help;
using SchoolManagement.Infrastructure;
using Xunit;

namespace SchoolManagementTest.Feature.RemoveNewUserFromClassTimeTest
{
    public class HelpCommandTest
    {
        [Fact]
        public async Task ExecuteShouldThrowExceptionParticipantNotFound()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();
            GroupClass makeUpClass = new GroupClassBuilder(context)
                .WithStartClasses(DateTime.Now.Subtract(TimeSpan.FromDays(30)))
                .WithNumberOfClasses(14)
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, TimeSpan.FromHours(18)))
                .AddParticipant(x => x.WithEmail("tes@test2.pl"), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                .WithTimeDurationInMinutes(90)
                .CreateSchedule()
                .BuildAndSave();

            ClassTime classTime = makeUpClass.Schedule.First();

            Command command = new Command
            {
                ParticipantId = "dsad",
                ClassTimeId = classTime.Id
            };

            DataResult dataResult = await new Handler(context).Handle(command, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Error);
            dataResult.Message.Should().Be(PolishReadableMessage.Presence.ParticipantNotFound);
        }

        [Fact]
        public async Task ExecuteShouldThrowExceptionClassNotFound()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            User user = new UserBuilder(context).BuildAndSave();
            Command command = new Command
            {
                ParticipantId = user.Id,
                ClassTimeId = 32 
            };

            DataResult dataResult = await new Handler(context).Handle(command, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Error);
            dataResult.Message.Should().Be(PolishReadableMessage.Presence.ClassNotFound);
        }


        [Fact]
        public async Task ExecuteShouldThrowWrongType()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            User user = new UserBuilder(context).BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();

            GroupClass makeUpClass = new GroupClassBuilder(context)
                    .WithStartClasses(DateTime.Now.Subtract(TimeSpan.FromDays(30)))
                    .WithNumberOfClasses(14)
                    .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, TimeSpan.FromHours(18)))
                    .AddParticipant(x => x.WithEmail("tes@test2.pl"), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                    .AddParticipant(x => x.WithUser(user), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                    .WithTimeDurationInMinutes(90)
                    .CreateSchedule()
                    .BuildAndSave();

            ClassTime classTime = context.ClassTimes.First(x => x.GroupClassId == makeUpClass.Id);
            ParticipantClassTime additionUser = classTime.PresenceParticipants.First(x => x.ParticipantId == user.Id);
            additionUser.PresenceType = PresenceType.MakeUp;
            additionUser.WasPresence = true;
            context.Update(additionUser);
            context.SaveChanges();

            Command command = new Command
            {
                ParticipantId = additionUser.ParticipantId,
                ClassTimeId = classTime.Id 
            };

            DataResult dataResult = await new Handler(context).Handle(command, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Error); 
            dataResult.Message.Should().Be(PolishReadableMessage.Presence.RemoveWrongType);

            classTime.PresenceParticipants.Should().HaveCount(2);

        }

        [Fact]
        public async Task ExecuteShouldRemoveAdditionalUser()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            User user = new UserBuilder(context).BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();

            GroupClass makeUpClass = new GroupClassBuilder(context)
                    .WithStartClasses(DateTime.Now.Subtract(TimeSpan.FromDays(30)))
                    .WithNumberOfClasses(14)
                    .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, TimeSpan.FromHours(18)))
                    .AddParticipant(x => x.WithEmail("tes@test2.pl"), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                    .AddParticipant(x => x.WithUser(user), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                    .WithTimeDurationInMinutes(90)
                    .CreateSchedule()
                    .BuildAndSave();

            ClassTime classTime = context.ClassTimes.First(x => x.GroupClassId == makeUpClass.Id);
            ParticipantClassTime additionUser = classTime.PresenceParticipants.First(x => x.ParticipantId == user.Id);
            additionUser.PresenceType = PresenceType.Help;
            additionUser.WasPresence = true;
            context.Update(additionUser);
            context.SaveChanges();

            Command command = new Command
            {
                ParticipantId = additionUser.ParticipantId,
                ClassTimeId = classTime.Id 
            };

            DataResult dataResult = await new Handler(context).Handle(command, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Success);

            classTime.PresenceParticipants.Should().HaveCount(1);

        }
    }
}
