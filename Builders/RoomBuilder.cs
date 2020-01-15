using System;
using System.Collections.Generic;
using System.Text;
using Model;
using Model.Domain;

namespace Builders
{
    public class RoomBuilder : Builder<RoomBuilder, Room>
    {
        public RoomBuilder(SchoolManagementContext context):base(context)
        {
            
        }
        public RoomBuilder WithName(string name)
        {
            State.Name = name;
            State.NormalizeName = name.ToUpperInvariant();

            return this;
        }
    }
}
