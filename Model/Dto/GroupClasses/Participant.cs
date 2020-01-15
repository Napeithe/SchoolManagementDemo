using Model.Domain;

namespace Model.Dto.GroupClasses
{
    public class Participant
    {
        public string UserName { get; set; }
        public ParticipantRole Role { get; set; }
        public string Id { get; set; }
        public int MemberId { get; set; }
        public string RoleDescription { get; set; }
        public bool PassWasPaid { get; set; }
    }
}
