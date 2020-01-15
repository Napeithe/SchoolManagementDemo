namespace Model.Domain
{
    public abstract class RelationUserGroupClass
    {
        public User User { get; set; }
        public string UserId { get; set; }
        public GroupClass GroupClass { get; set; }
        public int GroupClassId { get; set; }

        public RelationUserGroupClass WithUser(User user)
        {
            User = user;
            UserId = user.Id;
            return this;
        }

        public RelationUserGroupClass WithGroupClass(GroupClass groupClass)
        {
            GroupClass = groupClass;
            GroupClassId = groupClass.Id;
            return this;
        }
    }
}