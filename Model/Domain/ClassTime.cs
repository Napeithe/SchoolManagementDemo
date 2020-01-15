using System;
using System.Collections.Generic;

namespace Model.Domain
{
    public class ClassTime : Entity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Room Room { get; set; }
        public GroupClass GroupClass { get; set; }
        public int NumberOfClasses { get; set; }
        public int NumberOfClass { get; set; }

        public List<ParticipantClassTime> PresenceParticipants { get; set; } = new List<ParticipantClassTime>();


        public int RoomId { get; set; }
        public int GroupClassId { get; set; }
    }
}
