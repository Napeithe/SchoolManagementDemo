using System;
using System.Collections.Generic;

namespace Model.Domain
{
    public enum ParticipantRole
    {
        None,
        Leader,
        Follower
    }

    public class ParticipantGroupClass : RelationUserGroupClass, IEntity
    {
        public ParticipantGroupClass()
        {
            Passes = new List<Pass>();
        }
        public int Id { get; set; }
        public ParticipantRole Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public List<Pass> Passes { get; set; }

        public ParticipantGroupClass WithRole(ParticipantRole role)
        {
            Role = role;
            return this;
        }
    }
}