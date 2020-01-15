using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using FluentAssertions;
using FluentAssertions.Common;
using Model;
using Model.Domain;
using SchoolManagement.Features.Presences.AddNewUserToClassTime.MakeUp;
using SchoolManagement.Infrastructure;
using Xunit;

namespace SchoolManagementTest.Feature.AddNewUserToClassTimeTest
{
    public class CommandTest
    {
        [Fact]
        public async Task ExecuteShouldThrowExceptionParticipantNotFound()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            Command command = new Command
            {
                ParticipantId = "dsad",
                ClassTimeId = 32
            };

            DataResult<int> dataResult = await new Handler(context).Handle(command, CancellationToken.None);

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

            DataResult<int> dataResult = await new Handler(context).Handle(command, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Error);
            dataResult.Message.Should().Be(PolishReadableMessage.Presence.ClassNotFound);
        }

        [Fact]
        public async Task ExecuteShouldThrowNotClassToMakeUp()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            User user = new UserBuilder(context).BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();
            GroupClass groupClassToMakeUp = new GroupClassBuilder(context)
                .WithStartClasses(DateTime.Now.Subtract(TimeSpan.FromDays(30)))
                .WithNumberOfClasses(14)
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, TimeSpan.FromHours(18)))
                .AddParticipant(x => x.WithUser(user), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                .WithTimeDurationInMinutes(90)
                .CreateSchedule()
                .BuildAndSave();

            List<ParticipantClassTime> participantClassTimes = context.ParticipantPresences.ToList();
            for (int i = 0; i < participantClassTimes.Count - 5; i++)
            {
                participantClassTimes[i].WasPresence = true;
                participantClassTimes[i].PresenceType = PresenceType.Member;
            }
            context.UpdateRange(participantClassTimes);
            context.SaveChanges();

            GroupClass makeUpClass = new GroupClassBuilder(context)
                    .WithStartClasses(DateTime.Now.Subtract(TimeSpan.FromDays(30)))
                    .WithNumberOfClasses(14)
                    .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, TimeSpan.FromHours(18)))
                    .AddParticipant(x => x.WithEmail("tes@test.pl"), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                    .WithTimeDurationInMinutes(90)
                    .CreateSchedule()
                    .BuildAndSave();

            ClassTime classTime = context.ClassTimes.First(x => x.GroupClassId == makeUpClass.Id);

            Command command = new Command
            {
                ParticipantId = user.Id,
                ClassTimeId = classTime.Id 
            };

            DataResult<int> dataResult = await new Handler(context).Handle(command, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Error);

            classTime.PresenceParticipants.Should().HaveCount(1);

        }
        [Fact]
        public async Task ExecuteShouldSetPresenceAsMakeUp()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            User user = new UserBuilder(context).BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();
            GroupClass groupClassToMakeUp = new GroupClassBuilder(context)
                .WithStartClasses(DateTime.Now.Subtract(TimeSpan.FromDays(30)))
                .WithNumberOfClasses(14)
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, TimeSpan.FromHours(18)))
                .AddParticipant(x => x.WithUser(user), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                .WithTimeDurationInMinutes(90)
                .CreateSchedule()
                .BuildAndSave();

            List<ParticipantClassTime> participantClassTimes = context.ParticipantPresences.ToList();
            for (int i = 0; i < participantClassTimes.Count - 5; i++)
            {
                participantClassTimes[i].WasPresence = true;
                participantClassTimes[i].PresenceType = PresenceType.Member;
            }
            context.UpdateRange(participantClassTimes);
            context.SaveChanges();

            GroupClass makeUpClass = new GroupClassBuilder(context)
                    .WithStartClasses(DateTime.Now.Subtract(TimeSpan.FromDays(30)))
                    .WithNumberOfClasses(14)
                    .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, TimeSpan.FromHours(18)))
                    .AddParticipant(x => x.WithEmail("tes@test.pl"), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                    .WithTimeDurationInMinutes(90)
                    .CreateSchedule()
                    .BuildAndSave();

            ClassTime classTime = context.ClassTimes.Last(x => x.GroupClassId == makeUpClass.Id);

            Command command = new Command
            {
                ParticipantId = user.Id,
                ClassTimeId = classTime.Id 
            };

            DataResult<int> dataResult = await new Handler(context).Handle(command, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Success);

            classTime.PresenceParticipants.Should().HaveCount(2);
            classTime.PresenceParticipants[1].WasPresence.Should().BeTrue();
            classTime.PresenceParticipants[1].PresenceType = PresenceType.MakeUp;
            classTime.PresenceParticipants[1].MakeUpParticipant.Should()
                .NotBeNull("we have reference to make up classes");
            classTime.PresenceParticipants[1].MakeUpParticipant.Should()
                .Be(participantClassTimes[participantClassTimes.Count - 5]);
            dataResult.Data.Should().Be(participantClassTimes[participantClassTimes.Count - 5].Id);

        }
    }
}
