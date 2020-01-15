using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Domain;
using Model.Dto;
using SchoolManagement.Aggregates;

namespace Builders
{
    public class GroupClassBuilder : Builder<GroupClassBuilder, GroupClass>
    {
        public GroupClassBuilder(SchoolManagementContext context):base(context)
        {
            
        }

        public GroupClassBuilder WithName(string name)
        {
            State.Name = name;
            return this;
        }

        public GroupClassBuilder WithRoom(Action<RoomBuilder> actionBuilder)
        {
            RoomBuilder roomBuilder = new RoomBuilder(Context);
            actionBuilder.Invoke(roomBuilder);
            State.Room = roomBuilder.BuildAndSave();
            return this;
        }

        public GroupClassBuilder WithGroupLevel(Action<GroupLevelBuilder> actionBuilder)
        {
            GroupLevelBuilder builder = new GroupLevelBuilder(Context);
            actionBuilder.Invoke(builder);
            State.GroupLevel = builder.BuildAndSave();
            return this;
        }

        public GroupClassBuilder AddAnchor(Action<UserBuilder> actionBuilder)
        {
            var user = BuildUserWithRole(actionBuilder, Roles.Anchor);

            AnchorGroupClass anchorGroupClass = new AnchorGroupClass()
            {
                UserId = user.Id,
                GroupClass = State
            };
            State.Anchors.Add(anchorGroupClass);
            return this;
        }

        public GroupClassBuilder AddParticipant(Action<UserBuilder> userBuilder, ParticipantRole participantRole)
        {
            return AddParticipant(userBuilder, participantRole, passBuilder => passBuilder.WithNumberOfEntry(10));
        }
        public GroupClassBuilder AddParticipant(Action<UserBuilder> userBuilder, ParticipantRole participantRole, Action<PassBuilder> passBuilder)
        {
            var roleName = Roles.Participant;

            var user = BuildUserWithRole(userBuilder, roleName);

            PassBuilder pass = new PassBuilder(Context);
            passBuilder.Invoke(pass);
            pass.WithStartDate(State.StartClasses)
                .WithParticipant(user);

            ParticipantGroupClass participantGroupClass = new ParticipantGroupClass()
            {
                UserId = user.Id,
                User = user,
                GroupClass = State,
                Role = participantRole
            };
            pass.WithParticipantGroupClass(participantGroupClass);
            participantGroupClass.Passes.Add(pass.Build());
            State.Participants.Add(participantGroupClass);
            return this;
        }

        private User BuildUserWithRole(Action<UserBuilder> actionBuilder, string roleName)
        {
            UserBuilder builder = new UserBuilder(Context);
            actionBuilder.Invoke(builder);
            Role role = Context.Roles.FirstOrDefault(x => x.Name == roleName);
            if (role is null)
            {
                throw new BuilderException($"{roleName} role is not exist");
            }

            if (!Context.UserRoles.Any(x => x.RoleId == role.Id && x.UserId == builder.Get().Id))
            {
                builder.WithRole(role);
            }

            User user = builder.BuildAndSave();
            
            return user;
        }

        public GroupClassBuilder AddClassDayOfWeek(Action<ClassDayOfWeekBuilder> actionBuilder)
        {
            ClassDayOfWeekBuilder builder = new ClassDayOfWeekBuilder(Context);
            actionBuilder.Invoke(builder);
            builder.WithGroupClass(State);
            ClassDayOfWeek classDayOfWeek = builder.Build();
            State.ClassDaysOfWeek.Add(classDayOfWeek);
            return this;
        }

        public GroupClassBuilder WithStartClasses(DateTime startClassesDate)
        {
            State.StartClasses = startClassesDate;
            return this;
        }
        public GroupClassBuilder WithTimeDurationInMinutes(int timeDuration)
        {
            State.DurationTimeInMinutes = timeDuration;
            return this;
        }
        public GroupClassBuilder WithNumberOfClasses(int numberOfClasses)
        {
            State.NumberOfClasses = numberOfClasses;
            return this;
        }

        public GroupClassBuilder CreateSchedule()
        {
            if (State.DurationTimeInMinutes == 0)
            {
                throw new BuilderException("Duration time is not set");
            }

            if (!State.ClassDaysOfWeek.Any())
            {
                throw new BuilderException("Class day of week is not set");
            }

            if (State.NumberOfClasses == 0)
            {
                throw new BuilderException("Number of classes is not set");
            }

            DateTime startClasses = State.StartClasses;

            if (startClasses ==  new DateTime())
            {
                throw new BuilderException("Start date is not set");
            }

            var numberOfWeeks = State.NumberOfClasses / State.ClassDaysOfWeek.Count;
            List<ClassTime> classTimes = new List<ClassTime>();
            int numberOfClasses = 1;
            for (int i = 0; i < numberOfWeeks; i++)
            {
                foreach (ClassDayOfWeek requestDayOfWeek in State.ClassDaysOfWeek)
                {
                    DateTime nextDayOfWeeks = FindNearestOfDayOfWeek(startClasses, requestDayOfWeek);
                    nextDayOfWeeks = nextDayOfWeeks.Add(requestDayOfWeek.Hour);
                    ClassTime classTime = ClassTimeAggregate.New()
                        .AddParticipant(State.Participants.Select(x=>x.User).ToList(), PresenceType.None)
                        .FromGroupClass(State)
                        .WithNumberOfClass(numberOfClasses++)
                        .WithDate(nextDayOfWeeks, State.DurationTimeInMinutes).State;
                    classTimes.Add(classTime);
                }

                startClasses = startClasses.AddDays(7);
            }

            State.Schedule.AddRange(classTimes);
            return this;

        } 

        private DateTime FindNearestOfDayOfWeek(in DateTime startDate, in ClassDayOfWeek classDayOfWeekDto)
        {
            System.DayOfWeek startDateDayOfWeek = startDate.DayOfWeek;
            int difference = (int)classDayOfWeekDto.DayOfWeek - (int)startDateDayOfWeek;
            var startedDay = startDate.AddDays(difference);
            return startedDay;
        }
    }
}
