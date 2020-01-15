using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Domain;
using Model.Extensions;

namespace Model.Dto.GroupClasses
{
    public class GroupClassDetail
    {
        public string GroupName { get; set; }
        public bool IsSolo { get; set; }
        public int  ParticipantLimits { get; set; }
        public string GroupLevel { get; set; }
        public string RoomName { get; set; }
        public List<ClassDayOfWeekDto> ClassDayOfWeek { get; set; } = new List<ClassDayOfWeekDto>();
        public int DurationTime { get; set; }
        public int NumberOfClasses { get; set; }
        public string StartDate { get; set; }
        public List<string> Anchors { get; set; }
        public int Id { get; set; }
        public int PassPrice { get; set; }


        public static GroupClassDetail From(GroupClass groupClass, int utcOffset)
        {
            return new GroupClassDetail
            {
                Id = groupClass.Id,
                IsSolo = groupClass.IsSolo,
                GroupName = groupClass.Name,
                ParticipantLimits = groupClass.ParticipantLimits,
                RoomName = groupClass.Room?.Name,
                GroupLevel = groupClass.GroupLevel?.Name,
                Anchors = groupClass.Anchors.Select(x=>x.User).Select(x=>$"{x.FirstName} {x.LastName}").ToList(),
                StartDate = groupClass.StartClasses.ToString("dd.MM.yyyy"),
                PassPrice = groupClass.PassPrice,
                ClassDayOfWeek = groupClass.ClassDaysOfWeek.Select(x=>new ClassDayOfWeekDto()
                {
                    DayOfWeek = x.DayOfWeek,
                    BeginTime = x.Hour.UTCTimeSpanToLocal(utcOffset)
                }).ToList(),
                NumberOfClasses = groupClass.NumberOfClasses,
                DurationTime = groupClass.DurationTimeInMinutes
            };
        }
    }
}
