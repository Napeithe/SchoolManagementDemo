using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using FluentAssertions;
using Model;
using Model.Domain;
using SchoolManagement.Features.Pass.UsePass;
using SchoolManagement.Infrastructure;
using Xunit;

namespace SchoolManagementTest.Feature.PassesTest
{
    public class UsingPassTest
    {
        [Fact]
        public async Task ExecuteShouldGetPassEntryAndAssignToPresence()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();

            GroupClass groupClass = new GroupClassBuilder(context)
                .WithStartClasses(DateTime.Now)
                .WithTimeDurationInMinutes(90)
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, new TimeSpan(18, 0, 0)))
                .AddParticipant(userBuilder => userBuilder.WithName("Piotr", "Kowalski"), ParticipantRole.Follower,
                    passBuilder => passBuilder.WithNumberOfEntry(2).AsActive())
                .WithName("Boogie woogie")
                .WithNumberOfClasses(5)
                .CreateSchedule().BuildAndSave();

            User participant = context.Users.First();
            ClassTime classTime = context.ClassTimes.First();
            ParticipantClassTime participantClassTime = classTime.PresenceParticipants.First();
            participantClassTime.WasPresence = true;
            participantClassTime.PresenceType = PresenceType.Member;

            Command command = new Command
            {
                ParticipantClassTimeId = participantClassTime.Id
            };

            //Act
            DataResult<PassMessage> dataResult = await new Handler(context).Handle(command, CancellationToken.None);

            //Assert
            participantClassTime.WasPresence.Should().BeTrue("pass can be use only when presence is true");
            participantClassTime.PassId.Should().NotBeNull("all presence member have to contains pass entry");
            Pass pass = context.Passes.First();
            pass.Status.Should().Be(Pass.PassStatus.Active);
            pass.Used.Should().Be(1);
            pass.ParticipantClassTimes.Should().HaveCount(1);
            dataResult.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteShouldSetPassAsNotActiveWhenPassWasUsed()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();

            GroupClass groupClass = new GroupClassBuilder(context)
                .WithStartClasses(DateTime.Now)
                .WithTimeDurationInMinutes(90)
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, new TimeSpan(18, 0, 0)))
                .AddParticipant(userBuilder => userBuilder.WithName("Piotr", "Kowalski"), ParticipantRole.Follower,
                    passBuilder => passBuilder.WithNumberOfEntry(1).AsActive())
                .WithName("Boogie woogie")
                .WithNumberOfClasses(1)
                .CreateSchedule().BuildAndSave();

            User participant = context.Users.First();
            ClassTime classTime = context.ClassTimes.First();
            ParticipantClassTime participantClassTime = classTime.PresenceParticipants.First();
            participantClassTime.WasPresence = true;
            participantClassTime.PresenceType = PresenceType.Member;
            Command command = new Command
            {
                ParticipantClassTimeId = participantClassTime.Id
            };


            //Act
            DataResult<PassMessage> dataResult = await new Handler(context).Handle(command, CancellationToken.None);
            //Assert
            participantClassTime.PassId.Should().NotBeNull("all presence member have to contains pass entry");
            Pass pass = context.Passes.First();
            pass.Status.Should().Be(Pass.PassStatus.NotActive,"we used all entry");
            pass.Used.Should().Be(1);
            pass.ParticipantClassTimes.Should().HaveCount(1);
            dataResult.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteShouldGetSecondPassWhenFirstIsUsed()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();

            GroupClass groupClass = new GroupClassBuilder(context)
                .WithStartClasses(DateTime.Now)
                .WithTimeDurationInMinutes(90)
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, new TimeSpan(18, 0, 0)))
                .AddParticipant(userBuilder => userBuilder.WithName("Piotr", "Kowalski"), ParticipantRole.Follower,
                    passBuilder => passBuilder.WithNumberOfEntry(2).AsActive())
                .WithName("Boogie woogie")
                .WithNumberOfClasses(1)
                .CreateSchedule().BuildAndSave();
            User participant = context.Users.First();
            ParticipantGroupClass participantGroupClass = context.GroupClassMembers.First();

             new PassBuilder(context)
                .WithNumberOfEntry(5)
                .WithStartDate(groupClass.StartClasses)
                .With(x => x.Used = 5)
                .With(x=>x.Id = 4)
                .WithParticipant(participant)
                .WithParticipantGroupClass(participantGroupClass).BuildAndSave();

            ClassTime classTime = context.ClassTimes.First();
            ParticipantClassTime participantClassTime = classTime.PresenceParticipants.First();
            participantClassTime.WasPresence = true;
            participantClassTime.PresenceType = PresenceType.Member;
            Command command = new Command
            {
                ParticipantClassTimeId = participantClassTime.Id
            };


            //Act
            DataResult<PassMessage> dataResult = await new Handler(context).Handle(command, CancellationToken.None);
            //Assert
            participantClassTime.PassId.Should().NotBeNull("all presence member have to contains pass entry");
            Pass activePass = context.Passes.First(x => x.Status == Pass.PassStatus.Active);
            activePass.Used.Should().Be(1);
            activePass.ParticipantClassTimes.Should().HaveCount(1);
            Pass noActivePass = context.Passes.First(x => x.Status == Pass.PassStatus.NotActive);
            noActivePass.Used.Should().Be(5);
            dataResult.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task WhenPassIsOutThenDuplicateAndMarkAsNotPaidAndLogEntryFromNew()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();

            GroupClass groupClass = new GroupClassBuilder(context)
                .WithStartClasses(DateTime.Now)
                .WithTimeDurationInMinutes(90)
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, new TimeSpan(18, 0, 0)))
                .AddParticipant(userBuilder => userBuilder.WithName("Piotr", "Kowalski"), ParticipantRole.Follower,
                    passBuilder => passBuilder.WithNumberOfEntry(2).AsActive())
                .WithName("Boogie woogie")
                .WithNumberOfClasses(5)
                .CreateSchedule().BuildAndSave();

            User participant = context.Users.First();
            ClassTime classTime = context.ClassTimes.First();
            ParticipantClassTime participantClassTime = classTime.PresenceParticipants.First();
            participantClassTime.WasPresence = true;
            participantClassTime.PresenceType = PresenceType.Member;

            Command command = new Command
            {
                ParticipantClassTimeId = participantClassTime.Id
            };


            Pass oldPass = context.Passes.OrderBy(x => x.Id).First();
            oldPass.NumberOfEntry = 10;
            oldPass.Status = Pass.PassStatus.NotActive;
            oldPass.Used = 10;

            //Act
            DataResult<PassMessage> dataResult = await new Handler(context).Handle(command, CancellationToken.None);
            //Assert
            participantClassTime.PassId.Should().NotBeNull("presence for make up have to contain pass ");
            context.Passes.Should().HaveCount(2);
            Pass newPass = context.Passes.OrderBy(x => x.Id).Last();
            newPass.WasGenerateAutomatically.Should().BeTrue("we generate this pass");
            newPass.Status.Should().Be(Pass.PassStatus.Active);
            newPass.Used.Should().Be(1);
            newPass.Start.Should().Be(participantClassTime.ClassTime.StartDate);
            newPass.ParticipantClassTimes.Should().HaveCount(1);
            dataResult.Data.Should().NotBeNull();
        }
    }
}
