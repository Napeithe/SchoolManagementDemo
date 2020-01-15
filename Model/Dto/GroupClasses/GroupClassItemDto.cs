namespace Model.Dto.GroupClasses
{
    public class GroupClassItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LimitParticipants { get; set; }
        public int CurrentParticipants { get; set; }
    }
}
