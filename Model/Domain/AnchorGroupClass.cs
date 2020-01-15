using System;

namespace Model.Domain
{
    public class AnchorGroupClass : RelationUserGroupClass, IEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}