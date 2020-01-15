using System;

namespace Model.Domain
{
    public enum PresenceType
    {
        None = 0,
        Member = 1,
        Help = 2,
        MakeUp = 3
    }
    public class ParticipantClassTime : Entity
    {

        public ClassTime ClassTime { get; set; }

        public ParticipantClassTime MakeUpParticipant { get; set; }
        public int? MakeUpParticipantId { get; set; }

        public int ClassTimeId { get; set; }
        public User Participant { get; set; }
        public string ParticipantId { get; set; }
        public bool WasPresence { get; set; }
        public PresenceType PresenceType { get; set; }
        public Pass Pass { get; set; }
        public int? PassId { get; set; }
    }
}