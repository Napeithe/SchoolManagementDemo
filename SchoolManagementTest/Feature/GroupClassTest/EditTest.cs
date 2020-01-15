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
using Model.Extensions;
using SchoolManagement.Features.GroupClass.Add;
using SchoolManagement.Infrastructure;
using Xunit;
using Command = SchoolManagement.Features.GroupClass.Edit.Command;
using DayOfWeek = System.DayOfWeek;
using Handler = SchoolManagement.Features.GroupClass.Edit.Handler;
using Query = SchoolManagement.Features.GroupClass.Edit.Query;
using QueryHandler = SchoolManagement.Features.GroupClass.Edit.QueryHandler;

namespace SchoolManagementTest.Feature.GroupClassTest
{
    public class EditTest
    {
        [Fact]
        public async Task ExecuteShouldEditGroupClass()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            GroupLevel groupLevel = new GroupLevelBuilder(context).With(x => x.Level = 1).With(x => x.Name = "Początkujący").BuildAndSave();
            Room room = new RoomBuilder(context).WithName("Sala biała").BuildAndSave();
            User anchor = new UserBuilder(context).WithEmail("test@test.pl").BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Anchor).AddUserToRole(anchor).BuildAndSave();
            List<User> participants = new List<User>();
            var participantRoleBuilder = new RoleBuilder(context).WithName(Roles.Participant);
            for (var i = 0; i < 10; i++)
            {
                User user = new UserBuilder(context).WithEmail($"email{i}@gmail.com").BuildAndSave();
                participants.Add(user);
                participantRoleBuilder.AddUserToRole(user);
            }

            participantRoleBuilder.BuildAndSave();

            string expectedAnchorEmail = "anchor2@gmail.com";
            var groupClass = CreateGroupClass(context, expectedAnchorEmail);
            string expectedAnchorId = groupClass.Anchors.Where(x => x.User.Email == expectedAnchorEmail).Select(x => x.UserId).First();

            Command cmd = new Command
            {
                GroupClassId = groupClass.Id,
                Name = "Groupa zajęciowa",
                Anchors = new List<string> { anchor.Id, expectedAnchorId },
                IsSolo = true,
                ParticipantLimit = 20,
                GroupLevelId = groupLevel.Id,
                Participants = participants.Select(x => new ParticipantDto
                {
                    Id = x.Id,
                    Role = ParticipantRole.Leader
                }).ToList(),
                RoomId = room.Id
            };

            var expectedParticipant = groupClass.Participants.Select(x => x.User).First();
            ParticipantDto expectedRole = new ParticipantDto()
            {
                Id = expectedParticipant.Id,
                Role = ParticipantRole.Leader
            };
            cmd.Participants.Add(expectedRole);
            //Act
            DataResult result = await new Handler(context).Handle(cmd, CancellationToken.None);
            //Assert
            result
                .Should().NotBeNull();
            result.Status.Should().Be(DataResult.ResultStatus.Success,"all parameters are corrected");
            context.GroupClass.Should().NotBeEmpty("we add new group");
            groupClass.Anchors.Should().NotBeEmpty().And.HaveCount(cmd.Anchors.Count);
            groupClass.Room.Should().Be(room);
            groupClass.IsSolo.Should().BeTrue();
            groupClass.StartClasses.Should().Be(new DateTime());
            groupClass.ClassDaysOfWeek.Should().BeEmpty("we dont send days");
            groupClass.Schedule.Should().BeEmpty("we dont send days");
            groupClass.Name.Should().Be(cmd.Name);
            groupClass.GroupLevel.Should().Be(groupLevel);
            groupClass.Participants.Should().NotBeEmpty().And.HaveCount(cmd.Participants.Count).And
                .Contain(x => participants.Contains(x.User));
            groupClass.Participants.Where(x => x.User.Email == expectedParticipant.Email).Select(x => x.Role).First()
                .Should().Be(expectedRole.Role, "we changed role");
        }

        [Fact]
        public async Task ExecuteShouldEditScheduleGroupClass()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            GroupLevel groupLevel = new GroupLevelBuilder(context).With(x => x.Level = 1).With(x => x.Name = "Początkujący").BuildAndSave();
            Room room = new RoomBuilder(context).WithName("Sala biała").BuildAndSave();
            User anchor = new UserBuilder(context).WithEmail("test@test.pl").BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Anchor).AddUserToRole(anchor).BuildAndSave();
            List<User> participants = new List<User>();
            var participantRoleBuilder = new RoleBuilder(context).WithName(Roles.Participant);
            for (var i = 0; i < 10; i++)
            {
                User user = new UserBuilder(context).WithEmail($"email{i}@gmail.com").BuildAndSave();
                participants.Add(user);
                participantRoleBuilder.AddUserToRole(user);
            }

            participantRoleBuilder.BuildAndSave();

            string expectedAnchorEmail = "anchor2@gmail.com";
            var groupClass = CreateGroupClass(context, expectedAnchorEmail);
            string expectedAnchorId = groupClass.Anchors.Where(x => x.User.Email == expectedAnchorEmail).Select(x => x.UserId).First();

            Command cmd = new Command
            {
                GroupClassId = groupClass.Id,
                Name = "Groupa zajęciowa",
                Anchors = new List<string> { anchor.Id, expectedAnchorId },
                IsSolo = true,
                ParticipantLimit = 20,
                UtcOffsetInMinutes = 0,
                GroupLevelId = groupLevel.Id,
                Participants = participants.Select(x => new ParticipantDto
                {
                    Id = x.Id,
                    Role = ParticipantRole.Leader
                }).ToList(),
                DayOfWeeks = new List<ClassDayOfWeekDto>
                {
                    new ClassDayOfWeekDto()
                    {
                        BeginTime = new TimeSpan(19,0,0),
                        DayOfWeek = DayOfWeek.Tuesday
                    },
                    new ClassDayOfWeekDto()
                    {
                        BeginTime = new TimeSpan(18,0,0),
                        DayOfWeek = DayOfWeek.Wednesday
                    },
                    new ClassDayOfWeekDto()
                    {
                        BeginTime = new TimeSpan(19,0,0),
                        DayOfWeek = DayOfWeek.Friday
                    },
                },
                DurationTimeInMinutes = 90,
                NumberOfClasses = 24,
                RoomId = room.Id
            };

            var expectedParticipant = groupClass.Participants.Select(x => x.User).First();
            ParticipantDto expectedRole = new ParticipantDto()
            {
                Id = expectedParticipant.Id,
                Role = ParticipantRole.Leader
            };
            cmd.Participants.Add(expectedRole);
            //Act
            DataResult result = await new Handler(context).Handle(cmd, CancellationToken.None);
            //Assert
            result
                .Should().NotBeNull();
            result.Status.Should().Be(DataResult.ResultStatus.Success, "all parameters are corrected");
            context.GroupClass.Should().NotBeEmpty("we add new group");
            groupClass.Anchors.Should().NotBeEmpty().And.HaveCount(cmd.Anchors.Count);
            groupClass.Room.Should().Be(room);
            groupClass.IsSolo.Should().BeTrue();
            groupClass.Name.Should().Be(cmd.Name);
            groupClass.GroupLevel.Should().Be(groupLevel);
            groupClass.Participants.Should().NotBeEmpty().And.HaveCount(cmd.Participants.Count).And
                .Contain(x => participants.Contains(x.User));
            groupClass.Participants.Where(x => x.User.Email == expectedParticipant.Email).Select(x => x.Role).First()
                .Should().Be(expectedRole.Role, "we changed role");

            ValidateClassDayOfWeek(groupClass);

            groupClass.Schedule.Should().NotBeNullOrEmpty().And.HaveCount(24).And.OnlyContain(x =>
                x.StartDate.DayOfWeek == DayOfWeek.Tuesday || x.StartDate.DayOfWeek == DayOfWeek.Friday ||
                x.StartDate.DayOfWeek == DayOfWeek.Wednesday);
        }
        [Fact]
        public async Task ExecuteShouldEditScheduleGroupClassWithOffset()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            GroupLevel groupLevel = new GroupLevelBuilder(context).With(x => x.Level = 1).With(x => x.Name = "Początkujący").BuildAndSave();
            Room room = new RoomBuilder(context).WithName("Sala biała").BuildAndSave();
            User anchor = new UserBuilder(context).WithEmail("test@test.pl").BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Anchor).AddUserToRole(anchor).BuildAndSave();
            List<User> participants = new List<User>();
            var participantRoleBuilder = new RoleBuilder(context).WithName(Roles.Participant);
            for (var i = 0; i < 10; i++)
            {
                User user = new UserBuilder(context).WithEmail($"email{i}@gmail.com").BuildAndSave();
                participants.Add(user);
                participantRoleBuilder.AddUserToRole(user);
            }

            participantRoleBuilder.BuildAndSave();

            string expectedAnchorEmail = "anchor2@gmail.com";
            var groupClass = CreateGroupClass(context, expectedAnchorEmail);
            string expectedAnchorId = groupClass.Anchors.Where(x => x.User.Email == expectedAnchorEmail).Select(x => x.UserId).First();

            Command cmd = new Command
            {
                GroupClassId = groupClass.Id,
                Name = "Groupa zajęciowa",
                Anchors = new List<string> { anchor.Id, expectedAnchorId },
                IsSolo = true,
                ParticipantLimit = 20,
                UtcOffsetInMinutes = -60,
                GroupLevelId = groupLevel.Id,
                Participants = participants.Select(x => new ParticipantDto
                {
                    Id = x.Id,
                    Role = ParticipantRole.Leader
                }).ToList(),
                DayOfWeeks = new List<ClassDayOfWeekDto>
                {
                    new ClassDayOfWeekDto()
                    {
                        BeginTime = new TimeSpan(19,0,0),
                        DayOfWeek = DayOfWeek.Tuesday
                    },
                    new ClassDayOfWeekDto()
                    {
                        BeginTime = new TimeSpan(18,0,0),
                        DayOfWeek = DayOfWeek.Wednesday
                    },
                    new ClassDayOfWeekDto()
                    {
                        BeginTime = new TimeSpan(19,0,0),
                        DayOfWeek = DayOfWeek.Friday
                    },
                },
                DurationTimeInMinutes = 90,
                NumberOfClasses = 24,
                RoomId = room.Id
            };

            var expectedParticipant = groupClass.Participants.Select(x => x.User).First();
            ParticipantDto expectedRole = new ParticipantDto()
            {
                Id = expectedParticipant.Id,
                Role = ParticipantRole.Leader
            };
            cmd.Participants.Add(expectedRole);
            //Act
            DataResult result = await new Handler(context).Handle(cmd, CancellationToken.None);
            //Assert
            result
                .Should().NotBeNull();
            result.Status.Should().Be(DataResult.ResultStatus.Success, "all parameters are corrected");
            context.GroupClass.Should().NotBeEmpty("we add new group");
            groupClass.Anchors.Should().NotBeEmpty().And.HaveCount(cmd.Anchors.Count);
            groupClass.Room.Should().Be(room);
            groupClass.IsSolo.Should().BeTrue();
            groupClass.Name.Should().Be(cmd.Name);
            groupClass.GroupLevel.Should().Be(groupLevel);
            groupClass.Participants.Should().NotBeEmpty().And.HaveCount(cmd.Participants.Count).And
                .Contain(x => participants.Contains(x.User));
            groupClass.Participants.Where(x => x.User.Email == expectedParticipant.Email).Select(x => x.Role).First()
                .Should().Be(expectedRole.Role, "we changed role");

            ValidateClassDayOfWeek(groupClass);

            groupClass.Schedule.Should().NotBeNullOrEmpty().And.HaveCount(24).And.OnlyContain(x =>
                x.StartDate.DayOfWeek == DayOfWeek.Tuesday || x.StartDate.DayOfWeek == DayOfWeek.Friday ||
                x.StartDate.DayOfWeek == DayOfWeek.Wednesday);
        }

        [Fact]
        public async Task ExecuteShouldEditPresenceGroupClass()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            GroupLevel groupLevel = new GroupLevelBuilder(context).With(x => x.Level = 1).With(x => x.Name = "Początkujący").BuildAndSave();
            Room room = new RoomBuilder(context).WithName("Sala biała").BuildAndSave();
            User anchor = new UserBuilder(context).WithEmail("test@test.pl").BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Anchor).AddUserToRole(anchor).BuildAndSave();
            List<User> participants = new List<User>();
            var participantRoleBuilder = new RoleBuilder(context).WithName(Roles.Participant);
            for (var i = 0; i < 2; i++)
            {
                User user = new UserBuilder(context).WithEmail($"email{i}@gmail.com").BuildAndSave();
                participants.Add(user);
                participantRoleBuilder.AddUserToRole(user);
            }

            participantRoleBuilder.BuildAndSave();

            string expectedAnchorEmail = "anchor2@gmail.com";
            var groupClass = CreateGroupClass(context, expectedAnchorEmail);

            participants.AddRange(groupClass.Participants.Select(x => x.User).ToList());
            string expectedAnchorId = groupClass.Anchors.Where(x => x.User.Email == expectedAnchorEmail).Select(x => x.UserId).First();

            Command cmd = new Command
            {
                GroupClassId = groupClass.Id,
                Name = "Groupa zajęciowa",
                Anchors = new List<string> { anchor.Id, expectedAnchorId },
                IsSolo = true,
                ParticipantLimit = 20,
                UtcOffsetInMinutes = 0,
                GroupLevelId = groupLevel.Id,
                Participants = participants.Select(x => new ParticipantDto
                {
                    Id = x.Id,
                    Role = ParticipantRole.Leader
                }).ToList(),
                DayOfWeeks = new List<ClassDayOfWeekDto>
                {
                    new ClassDayOfWeekDto()
                    {
                        BeginTime = new TimeSpan(19,0,0),
                        DayOfWeek = DayOfWeek.Tuesday
                    },
                    new ClassDayOfWeekDto()
                    {
                        BeginTime = new TimeSpan(18,0,0),
                        DayOfWeek = DayOfWeek.Wednesday
                    }
                },
                DurationTimeInMinutes = 90,
                NumberOfClasses = 10,
                RoomId = room.Id
            };


            //Act
            DataResult result = await new Handler(context).Handle(cmd, CancellationToken.None);
            //Assert
            result
                .Should().NotBeNull();
            result.Status.Should().Be(DataResult.ResultStatus.Success, "all parameters are corrected");


            context.ClassTimes.Should().HaveCount(cmd.NumberOfClasses);
            context.ParticipantPresences.Should().HaveCount(cmd.NumberOfClasses * cmd.Participants.Count);
        }


        [Fact]
        public async Task ExecuteShouldEditAddNewDayOfWeekWithNewUser()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            GroupLevel groupLevel = new GroupLevelBuilder(context).With(x => x.Level = 1).With(x => x.Name = "Początkujący").BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Anchor).BuildAndSave();
            List<User> participants = new List<User>();
            var participantRoleBuilder = new RoleBuilder(context).WithName(Roles.Participant);

            User user = new UserBuilder(context).WithEmail($"email{5}@gmail.com").BuildAndSave();
            participants.Add(user);
            participantRoleBuilder.AddUserToRole(user).BuildAndSave();

            var groupClass = new GroupClassBuilder(context)
                .WithName("Stara grupa")
                .WithRoom(builder => builder.WithName("Old room"))
                .WithGroupLevel(x => x.With(z => z.Name = "Beginner"))
                .AddAnchor(anchor => anchor.WithEmail("anchor1@gmail.com").WithName("Jan", "Kowalski"))
                .AddParticipant(aprticipant => aprticipant.WithEmail("participant1@gmail.com").WithName("Jan", "Kowalski"), ParticipantRole.Follower)
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, new TimeSpan(18, 0, 0)))
                .WithStartClasses(new DateTime(2019, 09, 02, 0, 0, 0))
                .WithTimeDurationInMinutes(90)
                .WithNumberOfClasses(10)
                .CreateSchedule().BuildAndSave();
            participants.Add(groupClass.Participants.First().User);

            Command cmd = new Command
            {
                GroupClassId = groupClass.Id,
                Name = "Groupa zajęciowa",
                Anchors = new List<string> { groupClass.Anchors.First().UserId },
                IsSolo = true,
                ParticipantLimit = 20,
                Start = groupClass.StartClasses,
                GroupLevelId = groupLevel.Id,
                Participants = participants.Select(x => new ParticipantDto
                {
                    Id = x.Id,
                    Role = ParticipantRole.Leader
                }).ToList(),
                DayOfWeeks = new List<ClassDayOfWeekDto>
                {
                    new ClassDayOfWeekDto()
                    {
                        BeginTime = new TimeSpan(18,0,0),
                        DayOfWeek = DayOfWeek.Monday
                    },
                    new ClassDayOfWeekDto()
                    {
                        BeginTime = new TimeSpan(20,0,0),
                        DayOfWeek = DayOfWeek.Wednesday
                    }
                },
                DurationTimeInMinutes = 90,
                UtcOffsetInMinutes = 0,
                NumberOfClasses = 10,
                RoomId = groupClass.Room.Id
            };


            //Act
            DataResult result = await new Handler(context).Handle(cmd, CancellationToken.None);
            //Assert
            result
                .Should().NotBeNull();
            result.Status.Should().Be(DataResult.ResultStatus.Success, "all parameters are corrected");


            context.ClassTimes.Should().HaveCount(cmd.NumberOfClasses);
            context.ParticipantPresences.Should().HaveCount(cmd.NumberOfClasses * cmd.Participants.Count);
            foreach (var contextClassTime in context.ClassTimes)
            {
                contextClassTime.PresenceParticipants.Should().HaveCount(2);
            }
        }

        private static void ValidateClassDayOfWeek(GroupClass groupClass)
        {
            groupClass.ClassDaysOfWeek.Should().NotBeNullOrEmpty().And.HaveCount(3);
            groupClass.ClassDaysOfWeek[0].DayOfWeek.Should().Be(DayOfWeek.Tuesday);
            groupClass.ClassDaysOfWeek[1].DayOfWeek.Should().Be(DayOfWeek.Wednesday);
            groupClass.ClassDaysOfWeek[2].Hour.Should().Be(new TimeSpan(19, 0, 0));
        }

        private static GroupClass CreateGroupClass(SchoolManagementContext context, string expectedAnchorEmail)
        {
            var groupClassBuilder = new GroupClassBuilder(context)
                .WithName("Stara grupa")
                .WithRoom(builder => builder.WithName("Old room"))
                .WithGroupLevel(x => x.With(z => z.Name = "Beginner"))
                .AddAnchor(user => user.WithEmail("anchor1@gmail.com").WithName("Jan", "Kowalski"))
                .AddAnchor(user => user.WithEmail(expectedAnchorEmail).WithName("Kamil", "Kowalski"))
                .AddAnchor(user => user.WithEmail("anchor3@gmail.com").WithName("Jan", "Nowak"))
                .AddParticipant(user => user.WithEmail("participant1@gmail.com").WithName("Jan", "Kowalski"), ParticipantRole.Follower)
                .AddParticipant(user => user.WithEmail("participant2@gmail.com").WithName("Kamil", "Kowalski"), ParticipantRole.Leader)
                .AddParticipant(user => user.WithEmail("participant3@gmail.com").WithName("Jan", "Nowak"), ParticipantRole.Follower)
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Monday, new TimeSpan(18, 0, 0)))
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Wednesday, new TimeSpan(18, 0, 0)))
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Friday, new TimeSpan(18, 0, 0)))
                .AddClassDayOfWeek(x => x.WithDate(DayOfWeek.Saturday, new TimeSpan(18, 0, 0)))
                .WithStartClasses(new DateTime(2019, 09, 02, 0, 0, 0))
                .WithTimeDurationInMinutes(90)
                .WithNumberOfClasses(24)
                .CreateSchedule();



            var groupClass = groupClassBuilder.BuildAndSave();
            return groupClass;
        }

        [Fact]
        public async Task ExecuteShouldReturnUpdateViewModelForEditWithOffset()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();

            new RoleBuilder(context).WithName(Roles.Anchor).BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();

            GroupClass groupClass = CreateGroupClass(context, "anchor@email.com");


            var expectedAnchors = groupClass.Anchors.Select(x => x.UserId).ToList();
            var expectedParticipant = groupClass.Participants.Select(x => new ParticipantDto()
            {
                Id = x.UserId,
                Name = $"{x.User.FirstName} {x.User.LastName}",
                Role = x.Role,
            }).OrderBy(x => x.Name);

            var cmd = new Query()
            {
                GroupId = groupClass.Id,
                UtcOffsetInMinutes = -60
            };

            DataResult<UpdateViewModel> result = await new QueryHandler(context).Handle(cmd, CancellationToken.None);


            result.Status.Should().Be(DataResult.ResultStatus.Success, "all parameters are corrected");
            UpdateViewModel updateViewModel = result.Data;
            updateViewModel.Should().NotBeNull();
            updateViewModel.IsEdit.Should().BeTrue();
            updateViewModel.Anchors.Should().NotBeEmpty()
                .And.Contain(expectedAnchors);
            updateViewModel.Participants.Should().NotBeEmpty()
                .And
                .Contain(x => expectedParticipant.Contains(x));

            updateViewModel.DayOfWeeks.Should().NotBeNullOrEmpty().And.HaveCount(4);
            updateViewModel.DayOfWeeks[0].BeginTime.Should().Be(new TimeSpan(18, 0, 0).UTCTimeSpanToLocal(cmd.UtcOffsetInMinutes));
            updateViewModel.DayOfWeeks[0].DayOfWeek.Should().Be(DayOfWeek.Monday);
            updateViewModel.Start.Should().Be(new DateTime(2019, 09, 02, 0, 0, 0));
            updateViewModel.DurationTimeInMinutes.Should().Be(90);
            updateViewModel.NumberOfClasses.Should().Be(24);
        }

        [Fact]
        public async Task ExecuteShouldReturnUpdateViewModelForEdit()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();

            new RoleBuilder(context).WithName(Roles.Anchor).BuildAndSave();
            new RoleBuilder(context).WithName(Roles.Participant).BuildAndSave();

            GroupClass groupClass = CreateGroupClass(context, "anchor@email.com");


            var expectedAnchors = groupClass.Anchors.Select(x => x.UserId).ToList();
            var expectedParticipant = groupClass.Participants.Select(x => new ParticipantDto()
            {
                Id = x.UserId,
                Name = $"{x.User.FirstName} {x.User.LastName}",
                Role = x.Role,
            }).OrderBy(x => x.Name);

            var cmd = new Query()
            {
                GroupId = groupClass.Id,
                UtcOffsetInMinutes = 0
            };

            DataResult<UpdateViewModel> result = await new QueryHandler(context).Handle(cmd, CancellationToken.None);


            result.Status.Should().Be(DataResult.ResultStatus.Success, "all parameters are corrected");
            UpdateViewModel updateViewModel = result.Data;
            updateViewModel.Should().NotBeNull();
            updateViewModel.IsEdit.Should().BeTrue();
            updateViewModel.Anchors.Should().NotBeEmpty()
                .And.Contain(expectedAnchors);
            updateViewModel.Participants.Should().NotBeEmpty()
                .And
                .Contain(x => expectedParticipant.Contains(x));

            updateViewModel.DayOfWeeks.Should().NotBeNullOrEmpty().And.HaveCount(4);
            updateViewModel.DayOfWeeks[0].BeginTime.Should().Be(new TimeSpan(18, 0, 0));
            updateViewModel.DayOfWeeks[0].DayOfWeek.Should().Be(DayOfWeek.Monday);
            updateViewModel.Start.Should().Be(new DateTime(2019, 09, 02, 0, 0, 0));
            updateViewModel.DurationTimeInMinutes.Should().Be(90);
            updateViewModel.NumberOfClasses.Should().Be(24);
        }

        [Fact]
        public async Task ExecuteShouldReturnErrorMessageGroupNotFound()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            Query query = new Query
            {
                GroupId = 2
            };
            DataResult<UpdateViewModel> dataResult = await new QueryHandler(context).Handle(query, CancellationToken.None);

            dataResult.Status.Should().Be(DataResult.ResultStatus.Error, "all parameters are corrected");
            dataResult.Message.Should().Be(PolishReadableMessage.GroupClass.NotFound);
        }
    }
}
