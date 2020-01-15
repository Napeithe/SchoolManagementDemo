using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Domain
{
    public class ClassDayOfWeek : Entity
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan Hour { get; set; }

        public GroupClass GroupClass { get; set; }
        public int GroupClassId { get; set; }
    }
}
