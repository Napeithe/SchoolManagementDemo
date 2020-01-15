using System.Collections.Generic;
using Model.Domain;

namespace Model.Dto.Presences
{
    public class PresenceInGroupDto
    {
        public class PresenceValue
        {
            public int Id { get; set; }
            public string Date { get; set; }
            public bool Value { get; set; }
            public PresenceType PresenceType { get; set; }
        }
        public class Participant
        {
            public string ParticipantName { get; set; }
            public List<PresenceValue> PresenceValues { get; set; } = new List<PresenceValue>();
            public string ParticipantId { get; set; }
        }

        public List<Participant> ParticipantsPresence { get; set; } = new List<Participant>();
        public List<string> Columns { get; set; }
    }
}