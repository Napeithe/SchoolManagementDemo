using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Domain
{
    public class GroupClass : Entity
    {
        public GroupClass()
        {
            Anchors = new List<AnchorGroupClass>();
            Participants = new List<ParticipantGroupClass>();
            Schedule = new List<ClassTime>();
            ClassDaysOfWeek = new List<ClassDayOfWeek>();
        }
        public string Name { get; set; }

        public bool IsSolo { get; set; }

        public GroupLevel GroupLevel { get; set; }
        public Room Room { get; set; }


        public List<ClassTime> Schedule { get; set; }
        public List<AnchorGroupClass> Anchors { get; set; }
        public List<ClassDayOfWeek> ClassDaysOfWeek { get; set; }

        public List<ParticipantGroupClass> Participants { get; set; }
        public int ParticipantLimits { get; set; }
        public DateTime StartClasses { get; set; }
        public int DurationTimeInMinutes { get; set; }
        public int NumberOfClasses { get; set; }
        public int PassPrice { get; set; }
    }
}
