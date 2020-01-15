using System;
using System.Collections.Generic;
using System.Linq;
using Model.Domain;

namespace SchoolManagement.Aggregates
{
    public class ClassTimeAggregate : Aggregate<ClassTimeAggregate, ClassTime>
    {
        public ClassTimeAggregate WithRoom(Room room)
        {
            State.Room = room;
            return this;
        }

        public ClassTimeAggregate WithGroupClass(GroupClass groupClass)
        {
            State.GroupClass = groupClass;
            return this;
        }

        public ClassTimeAggregate WithDate(DateTime startTime, int durationInMinutes)
        {
            State.StartDate = startTime.ToUniversalTime();
            State.EndDate = startTime.ToUniversalTime().AddMinutes(durationInMinutes);
            return this;
        }

        public void WithDate(in DateTime requestStart, in DateTime endTime)
        {
            State.StartDate = requestStart.ToUniversalTime();
            State.EndDate = endTime.ToUniversalTime();
        }

        public ClassTimeAggregate FromGroupClass(GroupClass groupClass)
        {
            WithRoom(groupClass.Room);
            WithGroupClass(groupClass);
            return this;
        }

        public ClassTimeAggregate AddParticipant(List<User> participant, PresenceType presenceType)
        {
            participant.ForEach(x => { AddParticipant(x, presenceType); });
            return this;
        }

        public ParticipantClassTime AddParticipant(User participant, PresenceType presenceType)
        {
            ParticipantClassTime participantClassTime = new ParticipantClassTime()
            {
                ClassTime = State,
                Participant = participant,
                ParticipantId = participant.Id,
                ClassTimeId = State.Id,
                PresenceType = presenceType
            };

            var isAlreadyExist = IsAlreadyExist(participantClassTime);
            if (!isAlreadyExist)
            {
                State.PresenceParticipants.Add(participantClassTime);
            }

            return participantClassTime;
        }

        public ParticipantClassTime AddParticipant(User participant, ParticipantClassTime makeUpClassTime)
        {
            ParticipantClassTime participantClassTime = AddParticipant(participant, PresenceType.MakeUp);
            participantClassTime.MakeUpParticipant = makeUpClassTime;
            makeUpClassTime.PresenceType = PresenceType.MakeUp;
            return participantClassTime;
        }

        private bool IsAlreadyExist(ParticipantClassTime participantClassTime)
        {
            if (participantClassTime.ClassTimeId == default)
            {
                return false;
            }

            return State.PresenceParticipants.Any(x=>x.ClassTimeId == participantClassTime.ClassTimeId && x.ParticipantId == participantClassTime.ParticipantId);
        }

        public ClassTimeAggregate RemoveParticipant(string participantId)
        {
            ParticipantClassTime participant = State.PresenceParticipants.FirstOrDefault(x => x.ParticipantId == participantId);
            State.PresenceParticipants.Remove(participant);
            return this;
        }

        public ClassTimeAggregate WithNumberOfClass(int numberOfClass)
        {
            State.NumberOfClass = numberOfClass;
            return this;
        }

        public ClassTimeAggregate WithNumberOfClasses(int numberOfClasses)
        {
            State.NumberOfClasses = numberOfClasses;
            return this;
        }

     
    }
}
