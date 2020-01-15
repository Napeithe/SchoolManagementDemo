using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using FluentAssertions;
using Model;
using Model.Domain;
using SchoolManagement.Features.Presences.ChangePresence;
using SchoolManagement.Infrastructure;
using Xunit;

namespace SchoolManagementTest.Feature.Presence
{
    public class ChangePresenceCommandTest
    {
        [Fact]
        public async Task ExecuteShouldChangePresenceForMemberAndGetPassEntry()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();

            GroupClass groupClass = new GroupClassBuilder(context)
                .WithStartClasses(DateTime.Now)
                .WithTimeDurationInMinutes(90)
                .AddClassDayOfWeek(x=>x.WithDate(DayOfWeek.Monday, new TimeSpan(18,0,0)))
                .AddParticipant(userBuilder => userBuilder.WithName("Piotr", "Kowalski"), ParticipantRole.Follower,
                    passBuilder => passBuilder.WithNumberOfEntry(2))
                .WithName("Boogie woogie")
                .WithNumberOfClasses(5)
                .CreateSchedule().BuildAndSave();

            User participant = context.Users.First();
            ClassTime classTime = context.ClassTimes.First();
            Command command = new Command
            {
                ClassTimeId = classTime.Id,
                ParticipantId = participant.Id,
                IsPresence = true,
                PresenceType = PresenceType.Member
            };

            //Act
            DataResult dataResult = await new Handler(context).Handle(command, CancellationToken.None);
            //Assert
            ParticipantClassTime participantClassTime = classTime.PresenceParticipants.First();
            participantClassTime.WasPresence.Should().BeTrue("we set presence");
        }

      

        
       

     
    }
}
