using System;
using System.Collections.Generic;
using System.Text;
using Model;
using Model.Domain;

namespace Builders
{
    public class GroupLevelBuilder : Builder<GroupLevelBuilder, GroupLevel>
    {
        public GroupLevelBuilder(SchoolManagementContext context) :base(context)
        {
            
        }
    }
}
