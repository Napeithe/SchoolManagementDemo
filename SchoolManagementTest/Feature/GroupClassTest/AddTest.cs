using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using FluentAssertions;
using Model;
using Model.Domain;
using Model.Dto;
using SchoolManagement.Features.GroupClass.Add;
using SchoolManagement.Infrastructure;
using Xunit;

namespace SchoolManagementTest.Feature.GroupClassTest
{
    public class AddTest
    {
        [Fact]
        public async Task ExecuteShouldAddGroupClass()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            GroupLevel groupLevel = new GroupLevelBuilder(context).With(x => x.Level = 1).With(x => x.Name = "Początkujący").BuildAndSave();
            Room room = new RoomBuilder(context).WithName("Sala biała").BuildAndSave();
            User anchor = new UserBuilder(context).WithEmail("test@test.pl").BuildAndSave();
            Role role = new RoleBuilder(context).WithName(Roles.Anchor).AddUserToRole(anchor).BuildAndSave();
            List<User> participants = new List<User>();
            var participantRoleBuilder = new RoleBuilder(context).WithName(Roles.Participant);
            for (var i = 0; i < 10; i++)
            {
                User user = new UserBuilder(context).WithEmail($"email{i}@gmail.com").BuildAndSave();
                participants.Add(user);
                participantRoleBuilder.AddUserToRole(user);
            }

            participantRoleBuilder.BuildAndSave();

            Command cmd = new Command
            {
                Name = "Groupa zajęciowa",
                Anchors = new List<string> { anchor.Id },
                IsSolo = true,
                PassPrice = 200,
                ParticipantLimit = 20,
                GroupLevelId = groupLevel.Id,
                Participants = participants.Select(x=>new ParticipantDto
                {
                    Id = x.Id,
                    Role = ParticipantRole.Leader
                }).ToList(),
                DayOfWeeks = new List<ClassDayOfWeekDto>(),
                RoomId = room.Id
            };
            //Act
            DataResult result = await new Handler(context).Handle(cmd, CancellationToken.None);
            //Assert
            result
                .Should().NotBeNull();
            result.Status.Should().Be(DataResult.ResultStatus.Success,"all parameters are corrected");
            context.GroupClass.Should().NotBeEmpty("we add new group");
            GroupClass groupClass = context.GroupClass.First();
            groupClass.Anchors.Should().NotBeEmpty();
            groupClass.Room.Should().Be(room);
            groupClass.GroupLevel.Should().Be(groupLevel);
            groupClass.PassPrice.Should().Be(cmd.PassPrice);
            groupClass.Participants.Should().NotBeEmpty().And.HaveCount(participants.Count).And
                .Contain(x => participants.Contains(x.User));
        }

        [Fact]
        public async Task ExecuteShouldAddGroupAndAddScheduleWithOffset()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            GroupLevel groupLevel = new GroupLevelBuilder(context).With(x => x.Level = 1).With(x => x.Name = "Początkujący").BuildAndSave();
            Room room = new RoomBuilder(context).WithName("Sala biała").BuildAndSave();
            User anchor = new UserBuilder(context).WithEmail("test@test.pl").BuildAndSave();
            Role role = new RoleBuilder(context).WithName(Roles.Anchor).AddUserToRole(anchor).BuildAndSave();

            Command cmd = new Command
            {
                Name = "Groupa zajęciowa",
                Anchors = new List<string> { anchor.Id },
                IsSolo = true,
                ParticipantLimit = 20,
                GroupLevelId = groupLevel.Id,
                RoomId = room.Id,
                UtcOffsetInMinutes = 60,
                DayOfWeeks = new List<ClassDayOfWeekDto>
                {
                    new ClassDayOfWeekDto()
                    {
                        DayOfWeek = System.DayOfWeek.Monday,
                        BeginTime = new TimeSpan(18,0,0)
                    },
                    new ClassDayOfWeekDto()
                    {
                        DayOfWeek = System.DayOfWeek.Thursday,
                        BeginTime = new TimeSpan(19,0,0)
                    }
                },
                Start = new DateTime(2019, 09, 01), //Sunday
                DurationTimeInMinutes = 90,
                NumberOfClasses = 20
            };

            DataResult dataResult = await new Handler(context).Handle(cmd, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Success, "all parameters are corrected");
            List<ClassTime> classTimes = context.ClassTimes.ToList();
            classTimes.Should().NotBeEmpty("we set schedule")
                .And.HaveCount(20, "because we have classes 20 number of classes");




            ClassTime classes = classTimes.First();
            classes.Room.Should().Be(room);
            classes.RoomId.Should().Be(cmd.RoomId);
            GroupClass groupClass = context.GroupClass.First();

            groupClass.DurationTimeInMinutes.Should().Be(90);
            groupClass.NumberOfClasses.Should().Be(20);
            groupClass.StartClasses.Should().Be(new DateTime(2019, 09, 01).ToUniversalTime());

            List<ClassDayOfWeek> groupClassClassDaysOfWeek = groupClass.ClassDaysOfWeek;
            groupClassClassDaysOfWeek.Should()
                .NotBeNullOrEmpty()
                .And
                .HaveCount(2);
            groupClassClassDaysOfWeek.First().Hour.Should().Be(new TimeSpan(18, 0, 0));


            classes.GroupClass.Should().Be(groupClass);
            classes.GroupClassId.Should().Be(groupClass.Id);

            DateTime expectedStartTimeForOdd = new DateTime(2019, 09, 02, 18, 0, 0);
            DateTime expectedEndTimeForOdd = new DateTime(2019, 09, 02, 19, 30, 0);
            DateTime expectedStartTimeForEven = new DateTime(2019, 09, 05, 19, 0, 0);
            DateTime expectedEndTimeForEven = new DateTime(2019, 09, 05, 20, 30, 0);
            int index = 1;
            foreach (ClassTime classTime in classTimes)
            {
                if (index % 2 == 1)
                {
                    classTime.StartDate.Should().Be(expectedStartTimeForOdd.ToUniversalTime().AddMinutes(cmd.UtcOffsetInMinutes));
                    classTime.StartDate.DayOfWeek.Should().Be(System.DayOfWeek.Monday);
                    classTime.EndDate.Should().Be(expectedEndTimeForOdd.ToUniversalTime().AddMinutes(cmd.UtcOffsetInMinutes), "because classes during 90 minutes");

                    expectedStartTimeForOdd = expectedStartTimeForOdd.AddDays(7);
                    expectedEndTimeForOdd = expectedEndTimeForOdd.AddDays(7);
                }
                else
                {
                    classTime.StartDate.Should().Be(expectedStartTimeForEven.ToUniversalTime().AddMinutes(cmd.UtcOffsetInMinutes));
                    classTime.StartDate.DayOfWeek.Should().Be(System.DayOfWeek.Thursday);
                    classTime.EndDate.Should().Be(expectedEndTimeForEven.ToUniversalTime().AddMinutes(cmd.UtcOffsetInMinutes), "because classes during 90 minutes");

                    expectedStartTimeForEven = expectedStartTimeForEven.AddDays(7);
                    expectedEndTimeForEven = expectedEndTimeForEven.AddDays(7);
                }

                index++;
            }
         
        }

        [Fact]
        public async Task ExecuteShouldAddGroupAndAddSchedule()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            GroupLevel groupLevel = new GroupLevelBuilder(context).With(x => x.Level = 1).With(x => x.Name = "Początkujący").BuildAndSave();
            Room room = new RoomBuilder(context).WithName("Sala biała").BuildAndSave();
            User anchor = new UserBuilder(context).WithEmail("test@test.pl").BuildAndSave();
            Role role = new RoleBuilder(context).WithName(Roles.Anchor).AddUserToRole(anchor).BuildAndSave();

            Command cmd = new Command
            {
                Name = "Groupa zajęciowa",
                Anchors = new List<string> { anchor.Id },
                IsSolo = true,
                ParticipantLimit = 20,
                GroupLevelId = groupLevel.Id,
                RoomId = room.Id,
                UtcOffsetInMinutes = 0,
                DayOfWeeks = new List<ClassDayOfWeekDto>
                {
                    new ClassDayOfWeekDto()
                    {
                        DayOfWeek = System.DayOfWeek.Monday,
                        BeginTime = new TimeSpan(18,0,0)
                    },
                    new ClassDayOfWeekDto()
                    {
                        DayOfWeek = System.DayOfWeek.Thursday,
                        BeginTime = new TimeSpan(19,0,0)
                    }
                },
                Start = new DateTime(2019, 09, 01), //Sunday
                DurationTimeInMinutes = 90,
                NumberOfClasses = 20
            };

            DataResult dataResult = await new Handler(context).Handle(cmd, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Success, "all parameters are corrected");
            List<ClassTime> classTimes = context.ClassTimes.ToList();
            classTimes.Should().NotBeEmpty("we set schedule")
                .And.HaveCount(20, "because we have classes 20 number of classes");




            ClassTime classes = classTimes.First();
            classes.Room.Should().Be(room);
            classes.RoomId.Should().Be(cmd.RoomId);
            GroupClass groupClass = context.GroupClass.First();

            groupClass.DurationTimeInMinutes.Should().Be(90);
            groupClass.NumberOfClasses.Should().Be(20);
            groupClass.StartClasses.Should().Be(new DateTime(2019, 09, 01).ToUniversalTime());

            List<ClassDayOfWeek> groupClassClassDaysOfWeek = groupClass.ClassDaysOfWeek;
            groupClassClassDaysOfWeek.Should()
                .NotBeNullOrEmpty()
                .And
                .HaveCount(2);
            groupClassClassDaysOfWeek.First().Hour.Should().Be(new TimeSpan(18, 0, 0));


            classes.GroupClass.Should().Be(groupClass);
            classes.GroupClassId.Should().Be(groupClass.Id);

            DateTime expectedStartTimeForOdd = new DateTime(2019, 09, 02, 18, 0, 0);
            DateTime expectedEndTimeForOdd = new DateTime(2019, 09, 02, 19, 30, 0);
            DateTime expectedStartTimeForEven = new DateTime(2019, 09, 05, 19, 0, 0);
            DateTime expectedEndTimeForEven = new DateTime(2019, 09, 05, 20, 30, 0);
            int index = 1;
            foreach (ClassTime classTime in classTimes)
            {
                if (index % 2 == 1)
                {
                    classTime.StartDate.Should().Be(expectedStartTimeForOdd.ToUniversalTime());
                    classTime.StartDate.DayOfWeek.Should().Be(System.DayOfWeek.Monday);
                    classTime.EndDate.Should().Be(expectedEndTimeForOdd.ToUniversalTime(), "because classes during 90 minutes");

                    expectedStartTimeForOdd = expectedStartTimeForOdd.AddDays(7);
                    expectedEndTimeForOdd = expectedEndTimeForOdd.AddDays(7);
                }
                else
                {
                    classTime.StartDate.Should().Be(expectedStartTimeForEven.ToUniversalTime());
                    classTime.StartDate.DayOfWeek.Should().Be(System.DayOfWeek.Thursday);
                    classTime.EndDate.Should().Be(expectedEndTimeForEven.ToUniversalTime(), "because classes during 90 minutes");

                    expectedStartTimeForEven = expectedStartTimeForEven.AddDays(7);
                    expectedEndTimeForEven = expectedEndTimeForEven.AddDays(7);
                }

                index++;
            }
         
        }

        [Fact]
        public async Task ExecuteShouldAddGroupAndAddPresence()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            GroupLevel groupLevel = new GroupLevelBuilder(context).With(x => x.Level = 1).With(x => x.Name = "Początkujący").BuildAndSave();
            Room room = new RoomBuilder(context).WithName("Sala biała").BuildAndSave();
            User anchor = new UserBuilder(context).WithEmail("test@test.pl").BuildAndSave();
            Role role = new RoleBuilder(context).WithName(Roles.Anchor).AddUserToRole(anchor).BuildAndSave();
            List<User> participants = new List<User>();
            var participantRoleBuilder = new RoleBuilder(context).WithName(Roles.Participant);
            for (var i = 0; i < 10; i++)
            {
                User user = new UserBuilder(context).WithEmail($"email{i}@gmail.com").BuildAndSave();
                participants.Add(user);
                participantRoleBuilder.AddUserToRole(user);
            }

            participantRoleBuilder.BuildAndSave();
            Command cmd = new Command
            {
                Name = "Groupa zajęciowa",
                Anchors = new List<string> { anchor.Id },
                IsSolo = true,
                ParticipantLimit = 20,
                PassPrice = 200,
                GroupLevelId = groupLevel.Id,
                UtcOffsetInMinutes = 0,
                Participants = participants.Select(x => new ParticipantDto
                {
                    Id = x.Id,
                    Role = ParticipantRole.Leader
                }).ToList(),
                RoomId = room.Id,
                DayOfWeeks = new List<ClassDayOfWeekDto>
                {
                    new ClassDayOfWeekDto()
                    {
                        DayOfWeek = System.DayOfWeek.Monday,
                        BeginTime = new TimeSpan(18,0,0)
                    },
                    new ClassDayOfWeekDto()
                    {
                        DayOfWeek = System.DayOfWeek.Thursday,
                        BeginTime = new TimeSpan(19,0,0)
                    }
                },
                Start = new DateTime(2019, 09, 01), //Sunday
                DurationTimeInMinutes = 90,
                NumberOfClasses = 20
            };

            DataResult dataResult = await new Handler(context).Handle(cmd, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Success, "all parameters are corrected");


            context.ParticipantPresences.Should().NotBeNullOrEmpty()
                .And.HaveCount(cmd.NumberOfClasses * cmd.Participants.Count)
                .And.NotContainNulls(x=>x.ClassTime)
                .And.NotContainNulls(x=>x.Participant);

            context.ClassTimes.Should().NotBeEmpty().And.HaveCount(cmd.NumberOfClasses).And
                .NotContainNulls(x => x.PresenceParticipants);

            ClassTime classTime = context.ClassTimes.First();
            classTime.PresenceParticipants.Should().HaveCount(10);

            ParticipantClassTime participantClassTime = context.ParticipantPresences.First();

            AssertPass(context, participants, cmd);
        } 
      

        private static void AssertPass(SchoolManagementContext context, List<User> participants, Command cmd)
        {
            List<Pass> passes = context.Passes.ToList();
            passes.Should().NotBeNullOrEmpty("each member should have pass").And.HaveCount(participants.Count);
            Pass firstPass = passes.First();
            firstPass.Status.Should().Be(Pass.PassStatus.Active,"new pass is active because used is less than number of entry");
            firstPass.NumberOfEntry.Should().Be(cmd.NumberOfClasses);
            firstPass.Paid.Should().BeFalse("new pass is not paid yet");
            firstPass.Participant.Should().NotBeNull().And.Be(participants.First());
            firstPass.ParticipantGroupClass.Should().Be(context.GroupClass.First().Participants.First());
        }
    }
}
