using System;
using Model;
using Model.Domain;

namespace Builders
{
    public class PassBuilder : Builder<PassBuilder, Pass>
    {
        public PassBuilder(SchoolManagementContext context):base(context)
        {
            State.Price = 200;
            State.NumberOfEntry = 20;
            State.Start = DateTime.Now;
        }

        public PassBuilder AsActive()
        {
            State.Status = Pass.PassStatus.Active;
            return this;
        }

        public PassBuilder WithStartDate(DateTime startDate)
        {
            State.Start = startDate;
            return this;
        }

        public PassBuilder WithNumberOfEntry(int numberOfEntry)
        {
            State.NumberOfEntry = numberOfEntry;
            return this;
        }

        public PassBuilder WithParticipant(User user)
        {
            State.Participant = user;
            return this;
        }

        public PassBuilder WithParticipantGroupClass(ParticipantGroupClass participantGroupClass)
        {
            State.ParticipantGroupClass = participantGroupClass;
            return this;
        }

        public override Pass Build()
        {
            if (State.ParticipantGroupClass is null)
            {
                throw new BuilderException("ParticipantGroupClass is not set");
            }

            return base.Build();
        }

        public override Pass BuildAndSave()
        {
            if (State.ParticipantGroupClass is null)
            {
                throw new BuilderException("ParticipantGroupClass is not set");
            }

            return base.BuildAndSave();
        }
    }
}
