using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using FluentAssertions;
using Model;
using Model.Domain;
using SchoolManagement.Aggregates;
using SchoolManagement.Features.Pass.ReturnPass;
using SchoolManagement.Infrastructure;
using Xunit;

namespace SchoolManagementTest.Feature.PassesTest
{
    public class ReturnPassTest
    {
        [Fact]
        public async Task ExecuteShouldReturnPass()
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

            ParticipantGroupClass participantGroupClass = groupClass.Participants.First();
            ParticipantClassTime participantClassTime = context.ParticipantPresences.First();
            Pass pass = new PassBuilder(context).AsActive()
                .WithParticipant(participantGroupClass.User)
                .WithParticipantGroupClass(participantGroupClass)
                .WithNumberOfEntry(2)
                .With(x=>x.ParticipantClassTimes.Add(participantClassTime))
                .With(x=>x.Used =0).BuildAndSave();

            PassAggregate.FromState(pass).UsePass(participantClassTime);
            context.Update(participantClassTime);
            await context.SaveChangesAsync();

            Command command = new Command
            {
                ParticipantId = participantGroupClass.UserId,
                ClassTimeId = participantClassTime.ClassTimeId
            };
            pass.Used.Should().Be(1);
            //Act
            DataResult dataResult = await new Handler(context).Handle(command, CancellationToken.None);
            //Assert
            pass.Used.Should().Be(0);
        }

        [Fact]
        public async Task ExecuteShouldReturnPassAndRemoveWhenItWasGenerated()
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

            ParticipantGroupClass participantGroupClass = groupClass.Participants.First();
            ParticipantClassTime participantClassTime = context.ParticipantPresences.First();
            Pass pass = new PassBuilder(context)
                .WithParticipant(participantGroupClass.User)
                .WithParticipantGroupClass(participantGroupClass)
                .WithNumberOfEntry(2)
                .With(x=>x.ParticipantClassTimes.Add(participantClassTime))
                .With(x=>x.Used =0).BuildAndSave();
            Pass generatedPass = new PassBuilder(context).AsActive()
                .WithParticipant(participantGroupClass.User)
                .WithParticipantGroupClass(participantGroupClass)
                .With(x=>x.WasGenerateAutomatically = true)
                .WithNumberOfEntry(2)
                .With(x=>x.ParticipantClassTimes.Add(participantClassTime))
                .With(x=>x.Used =0).BuildAndSave();

            PassAggregate.FromState(generatedPass).UsePass(participantClassTime);
            context.Update(participantClassTime);
            await context.SaveChangesAsync();

            Command command = new Command
            {
                ParticipantId = participantGroupClass.UserId,
                ClassTimeId = participantClassTime.ClassTimeId
            };
            //Act
            DataResult dataResult = await new Handler(context).Handle(command, CancellationToken.None);
            //Assert
            context.Passes.Should().NotContain(generatedPass);
            context.Passes.Should().Contain(pass);
            pass.Used.Should().Be(0);

        }
    }
}
