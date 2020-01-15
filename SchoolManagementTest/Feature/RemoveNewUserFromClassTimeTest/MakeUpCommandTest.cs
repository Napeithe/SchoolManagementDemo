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
using SchoolManagement.Features.Presences.RemoveNewUserFromClassTime.MakeUp;
using SchoolManagement.Infrastructure;
using Xunit;

namespace SchoolManagementTest.Feature.RemoveNewUserFromClassTimeTest
{
    public class CommandTest
    {
        [Fact]
        public async Task ExecuteShouldThrowExceptionParticipantNotFound()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();
            GroupClass groupClassToMakeUp = new GroupClassBuilder(context)
                .WithStartClasses(DateTime.Now.Subtract(TimeSpan.FromDays(30)))
                .WithNumberOfClasses(14)
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, TimeSpan.FromHours(18)))
                .AddParticipant(x => x.WithEmail("ds@ds.pl"), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                .WithTimeDurationInMinutes(90)
                .CreateSchedule()
                .BuildAndSave();
            ClassTime classTime = groupClassToMakeUp.Schedule.First();
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

            ParticipantClassTime makeUpParticipantClassTime = participantClassTimes[participantClassTimes.Count - 5];
            makeUpParticipantClassTime.PresenceType = PresenceType.MakeUp;
            context.UpdateRange(participantClassTimes);
            context.SaveChanges();

            GroupClass makeUpClass = new GroupClassBuilder(context)
                    .WithStartClasses(DateTime.Now.Subtract(TimeSpan.FromDays(30)))
                    .WithNumberOfClasses(14)
                    .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, TimeSpan.FromHours(18)))
                    .AddParticipant(x => x.WithEmail("tes@test.pl"), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                    .AddParticipant(x=>x.WithUser(user), ParticipantRole.Follower)
                    .WithTimeDurationInMinutes(90)
                    .CreateSchedule()
                    .BuildAndSave();

            ClassTime classTime = context.ClassTimes.Last(x => x.GroupClassId == makeUpClass.Id);
            ParticipantClassTime newUserToRemove = classTime.PresenceParticipants.Last();
            newUserToRemove.MakeUpParticipant = makeUpParticipantClassTime;
            newUserToRemove.MakeUpParticipantId = makeUpParticipantClassTime.Id;
            newUserToRemove.PresenceType = PresenceType.Help;
            context.Update(newUserToRemove);
            context.SaveChanges();
            Command command = new Command
            {
                ParticipantId = user.Id,
                ClassTimeId = classTime.Id 
            };

            DataResult dataResult = await new Handler(context).Handle(command, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Error);
            dataResult.Message.Should().Be(PolishReadableMessage.Presence.RemoveWrongType);

            classTime.PresenceParticipants.Should().HaveCount(2);
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

            ParticipantClassTime makeUpParticipantClassTime = participantClassTimes[participantClassTimes.Count - 5];
            makeUpParticipantClassTime.PresenceType = PresenceType.MakeUp;
            context.UpdateRange(participantClassTimes);
            context.SaveChanges();

            GroupClass makeUpClass = new GroupClassBuilder(context)
                    .WithStartClasses(DateTime.Now.Subtract(TimeSpan.FromDays(30)))
                    .WithNumberOfClasses(14)
                    .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, TimeSpan.FromHours(18)))
                    .AddParticipant(x => x.WithEmail("tes@test.pl"), ParticipantRole.Follower, passBuilder => passBuilder.AsActive())
                    .AddParticipant(x=>x.WithUser(user), ParticipantRole.Follower)
                    .WithTimeDurationInMinutes(90)
                    .CreateSchedule()
                    .BuildAndSave();

            ClassTime classTime = context.ClassTimes.Last(x => x.GroupClassId == makeUpClass.Id);
            ParticipantClassTime newUserToRemove = classTime.PresenceParticipants.Last();
            newUserToRemove.MakeUpParticipant = makeUpParticipantClassTime;
            newUserToRemove.MakeUpParticipantId = makeUpParticipantClassTime.Id;
            newUserToRemove.PresenceType = PresenceType.MakeUp;
            context.Update(newUserToRemove);
            context.SaveChanges();
            Command command = new Command
            {
                ParticipantId = user.Id,
                ClassTimeId = classTime.Id 
            };

            DataResult dataResult = await new Handler(context).Handle(command, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Success);

            classTime.PresenceParticipants.Should().HaveCount(1);
            makeUpParticipantClassTime.PresenceType.Should().Be(PresenceType.None);
        }
    }
}
