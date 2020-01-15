using Model.Domain;

namespace Model.Dto
{
    public class ParticipantDto
    {
        public string Id { get; set; }
        public ParticipantRole Role { get; set; }
        public string Name { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ParticipantDto participantDto)
            {
                return this.Id == participantDto.Id && this.Name.Equals(participantDto.Name) && this.Role == participantDto.Role;
            }
            else
            {
                return false;
            }
        }
    }
}