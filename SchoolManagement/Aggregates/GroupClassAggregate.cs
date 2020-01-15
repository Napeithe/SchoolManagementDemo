using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Domain;
using Model.Dto;
using Model.Extensions;
using SchoolManagement.Features.GroupClass;

namespace SchoolManagement.Aggregates
{
    public class GroupClassAggregate : Aggregate<GroupClassAggregate, GroupClass>
    {
        public GroupClassAggregate UpdateAll(IUpdateGroupClass update)
        {
            State.DurationTimeInMinutes = update.DurationTimeInMinutes;
            State.NumberOfClasses = update.NumberOfClasses;
            State.StartClasses = update.Start.ToUniversalTime();
            State.ParticipantLimits = update.ParticipantLimit;
            State.IsSolo = update.IsSolo;
            State.Name = update.Name;
            State.PassPrice = update.PassPrice;
            return this;
        }

        public GroupClassAggregate UpdateDaysOfWeek(IUpdateGroupClass update, SchoolManagementContext context)
        {
            List<ClassDayOfWeek> classDayOfWeeks = update.DayOfWeeks.Select(x => new ClassDayOfWeek()
            {
                DayOfWeek = x.DayOfWeek,
                GroupClass = State,
                Hour = x.BeginTime.LocalTimeSpanToUTC(update.UtcOffsetInMinutes)
            }).ToList();

            if (State.ClassDaysOfWeek.Count == 0)
            {
                State.ClassDaysOfWeek.AddRange(classDayOfWeeks);
            }
            else
            {

                State.ClassDaysOfWeek.Clear();
                State.ClassDaysOfWeek.AddRange(classDayOfWeeks);
            }

            return this;
        }

        public GroupClassAggregate CreateSchedule(IUpdateGroupClass request)
        {
            var startDate = request.Start.ToUniversalTime();
            var duration = request.DurationTimeInMinutes;
            var numberOfClasses = request.NumberOfClasses;
            if (request.DayOfWeeks.Count <= 0)
            {

                State.ClassDaysOfWeek.Clear();


                State.Schedule.Clear();
                return this;
            }

            var classTimes = CreateSchedule(request, numberOfClasses, startDate, duration);
            List<ClassTime> times = State.Schedule.ToList();
            //Find new
            List<ClassTime> newDays = GetNewDays(classTimes, times);
            List<ClassTime> removedDays = GetRemovedDays(classTimes, times);

            foreach (var removedDay in removedDays)
            {
                State.Schedule.Remove(removedDay);
            }

            State.Schedule.AddRange(newDays);

            return this;
        }

        public GroupClassAggregate RemoveAnchor(string userId)
        {
            AnchorGroupClass anchorRelation = State.Anchors.First(x => x.UserId == userId);
            State.Anchors.Remove(anchorRelation);
            return this;
        }

        public GroupClassAggregate RemoveParticipant(string userId)
        {
            var participantRelation = State.Participants.First(x => x.UserId == userId);
            State.Participants.Remove(participantRelation);
            State.Schedule.ForEach(x => { ClassTimeAggregate.FromState(x).RemoveParticipant(userId); });
            return this;
        }

        public GroupClassAggregate ChangeParticipantRole(IUpdateGroupClass command)
        {
            State.Participants.ForEach(updatedParticipant =>
            {
                ParticipantDto participantDto = command.Participants.First(p => p.Id == updatedParticipant.UserId);
                ParticipantGroupClass participantGroupClass = State.Participants.First(x => x.UserId == updatedParticipant.UserId);
                participantGroupClass.WithRole(participantDto.Role);
            });
            return this;
        }

        public GroupClassAggregate AddAnchor(List<User> anchors)
        {
            anchors.ForEach(x => AddAnchor(x));
            return this;
        }

        public GroupClassAggregate AddAnchor(User anchor)
        {
            AnchorGroupClass anchorGroupClass = new AnchorGroupClass()
                .WithUser(anchor)
                .WithGroupClass(State) as AnchorGroupClass;

            State.Anchors.Add(anchorGroupClass);

            return this;
        }

        public GroupClassAggregate WithRoom(Room room)
        {
            State.Room = room;
            return this;
        }

        public GroupClassAggregate WithGroupLevel(GroupLevel groupLevel)
        {
            State.GroupLevel = groupLevel;
            return this;
        }

        public GroupClassAggregate AddParticipant(User participant, ParticipantRole role, PassAggregate passAggregate)
        {
            ParticipantGroupClass participantGroupClass = new ParticipantGroupClass()
                .WithUser(participant)
                .WithGroupClass(State) as ParticipantGroupClass;

            participantGroupClass.WithRole(role);
            participantGroupClass.Passes.Add(passAggregate.State);

            State.Participants.Add(participantGroupClass);

            AddParticipantToPresenceList(participant, PresenceType.Member);
            return this;
        }

        public bool IsParticipantExists(string userId)
        {
            return State.Participants.Any(x => x.UserId == userId);
        }

        public GroupClassAggregate AddParticipantToPresenceList(User participant, PresenceType presenceType)
        {
            State.Schedule.ForEach(x =>
                {
                    ClassTimeAggregate.FromState(x).AddParticipant(participant, presenceType);
                });

            return this;
        }

        private List<ClassTime> GetNewDays(List<ClassTime> classTimes, List<ClassTime> currentTimes)
        {
            return classTimes.Where(x => !currentTimes.Any(s => s.StartDate == x.StartDate && s.EndDate == x.EndDate && s.Room == x.Room)).ToList();
        }

        private List<ClassTime> GetRemovedDays(List<ClassTime> classTimes, List<ClassTime> currentTimes)
        {
            var ids = FindDuplicates(currentTimes);

            return currentTimes.Where(x => (!classTimes.Any(s => s.StartDate == x.StartDate && s.EndDate == x.EndDate && s.Room == x.Room))
            || ids.Contains(x.Id)
            ).ToList();
        }

        private static List<int> FindDuplicates(List<ClassTime> currentTimes)
        {
            List<List<int>> duplicates = currentTimes.GroupBy(x => (x.EndDate, x.StartDate, x.Room)).Select(x => new
            {
                Ids = x.Select(s => s.Id).ToList(),
            }).Select(x => x.Ids).ToList();

            List<IEnumerable<int>> list = duplicates.Select(x => x.Skip(1)).ToList();
            var ids = new List<int>();
            list.ForEach(x => { ids.AddRange(x); });
            return ids;
        }

        private List<ClassTime> CreateSchedule(IUpdateGroupClass request, int numberOfClasses, DateTime startDate, int duration)
        {
            var numberOfWeeks = numberOfClasses / request.DayOfWeeks.Count;
            List<ClassTime> classTimes = new List<ClassTime>();
            int numberOfClass = 1;
            for (int i = 0; i < numberOfWeeks; i++)
            {
                foreach (ClassDayOfWeekDto requestDayOfWeek in request.DayOfWeeks)
                {
                    DateTime nextDayOfWeeks = FindNearestOfDayOfWeek(startDate, requestDayOfWeek);
                    nextDayOfWeeks = nextDayOfWeeks.Add(requestDayOfWeek.BeginTime).AddMinutes(request.UtcOffsetInMinutes);

                    ClassTime classTime = ClassTimeAggregate.New()
                        .FromGroupClass(State)
                        .WithNumberOfClass(numberOfClass++)
                        .WithNumberOfClasses(numberOfClasses)
                        .AddParticipant(State.Participants.Select(x=>x.User).ToList(), PresenceType.None)
                        .WithDate(nextDayOfWeeks, duration).State;

                    classTimes.Add(classTime);
                }

                startDate = startDate.AddDays(7);
            }

            return classTimes;
        }

        private DateTime FindNearestOfDayOfWeek(in DateTime startDate, in ClassDayOfWeekDto classDayOfWeekDto)
        {
            System.DayOfWeek startDateDayOfWeek = startDate.DayOfWeek;
            int difference = (int)classDayOfWeekDto.DayOfWeek - (int)startDateDayOfWeek;
            if (difference < 0)
            {
                difference += 6;
            }
            var startedDay = startDate.AddDays(difference);
            return startedDay;
        }

        public List<AnchorGroupClass> GetCurrentAnchors()
        {
            return State.Anchors.ToList();
        }

        public List<ParticipantGroupClass> GetCurrentParticipants()
        {
            return State.Participants.ToList();
        }
    }
}
