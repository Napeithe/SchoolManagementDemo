using System;
using Model.Domain;
using Model.Dto;
using SchoolManagement.Features.GroupClass;

namespace SchoolManagement.Aggregates
{
    public class PassAggregate : Aggregate<PassAggregate, Pass>
    {

        public PassAggregate WithPassNumber(int number)
        {
            State.PassNumber = number;
            return this;
        }

        public PassAggregate WithPrice(int price)
        {
            State.Price = price;
            return this;
        }

        public PassAggregate WithParticipant(User participant)
        {
            State.Participant = participant;
            return this;
        }

        public PassAggregate WithNumberOfEntry(int numberOfEntry)
        {
            State.NumberOfEntry = numberOfEntry;
            if (State.Used < numberOfEntry)
            {
                AsActive();
            }
            else
            {
                AsNotActive();
            }
            return this;
        }

        public PassAggregate WithStartDate(in DateTime start)
        {
            State.Start = start;
            return this;
        }

        public PassAggregate UpdateByCommand(IUpdateGroupClass request)
        {
            WithNumberOfEntry(request.NumberOfClasses);
            WithPrice(request.PassPrice);
            WithStartDate(request.Start);
            return this;
        }

        public PassAggregate UpdateByCommand(IUpdatePass updateCommand)
        {
            WithNumberOfEntry(updateCommand.NumberOfEntry);
            WithPrice(updateCommand.Price);
            WithIsStudent(updateCommand.IsStudent);
            WithIsPaid(updateCommand.WasPaid);
            WithStartDate(updateCommand.Start);
            return this;
        }

        private void WithIsPaid(bool isPaid)
        {
            State.Paid = isPaid;
        }

        private void WithIsStudent(bool isStudent)
        {
            State.IsStudent = isStudent;
        }


        public void UsePass(ParticipantClassTime classTime)
        {
            State.Used++;
            if (State.Used == State.NumberOfEntry)
            {
                AsNotActive();
            }
            State.ParticipantClassTimes.Add(classTime);
            classTime.Pass = State;
        }



        public void ReturnPass(ParticipantClassTime classTime)
        {
            State.Used--;
            if (State.Used < State.NumberOfEntry)
            {
                AsActive();
            }
            State.ParticipantClassTimes.Remove(classTime);
            classTime.Pass = null;
        }

        public PassAggregate AsActive()
        {
            State.Status = Pass.PassStatus.Active;
            return this;
        }

        public PassAggregate AsNotActive()
        {
            State.Status = Pass.PassStatus.NotActive;
            return this;
        }

        public PassAggregate AsGenerateAutomatically()
        {
            State.WasGenerateAutomatically = true;
            return this;
        }

        public void AsRemoved()
        {
            State.Status = Pass.PassStatus.Removed;
        }
    }
}
