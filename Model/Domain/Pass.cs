using System;
using System.Collections.Generic;

namespace Model.Domain
{
    public class Pass : Entity
    {
        public enum PassStatus
        {
            NotActive,
            Active,
            Removed
        }

        public Pass()
        {
            ParticipantClassTimes = new List<ParticipantClassTime>();
        }

        public ParticipantGroupClass ParticipantGroupClass { get; set; }

        public int ParticipantGroupClassId { get; set; }

        public int NumberOfEntry { get; set; }

        public User Participant { get; set; }

        public string ParticipantId { get; set; }

        public int Price { get; set; }

        public bool Paid { get; set; }

        public DateTime Start { get; set; }

        public int Used { get; set; }

        public bool WasGenerateAutomatically { get; set; }

        public List<ParticipantClassTime> ParticipantClassTimes { get; set; }

        public bool IsStudent { get; set; }

        public PassStatus Status { get; set; }
        public int PassNumber { get; set; }

        public Pass Copy()
        {
            return new Pass()
            {
                Status = PassStatus.NotActive,
                ParticipantGroupClass = ParticipantGroupClass,
                NumberOfEntry = NumberOfEntry,
                Participant =  Participant,
                ParticipantGroupClassId = ParticipantGroupClassId,
                ParticipantId = ParticipantId,
                Price = Price,
            };
        }
    }
}
