using System;
using System.Collections.Generic;
using System.Text;
using Model;
using Model.Domain;

namespace Builders
{
    public class ClassDayOfWeekBuilder : Builder<ClassDayOfWeekBuilder, ClassDayOfWeek>
    {
        public ClassDayOfWeekBuilder(SchoolManagementContext context):base(context)
        {
            
        }
        public ClassDayOfWeekBuilder WithDate(DayOfWeek dayOfWeek, TimeSpan hour)
        {
            State.DayOfWeek = dayOfWeek;
            State.Hour = hour;
            return this;
        }
        public ClassDayOfWeekBuilder WithGroupClass(GroupClass groupClass)
        {
            State.GroupClass= groupClass;
            return this;
        }
    }
}
