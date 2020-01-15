using System;
using System.Collections.Generic;
using Model.Domain.Interface;
using Model.Dto;
using SchoolManagement.Features.GroupClass.Add;

namespace SchoolManagement.Features.GroupClass
{
    public interface IUpdateGroupClass : IUtcOffset
    {
        int GroupClassId { get; set; }
        string Name { get; set; }
        int? GroupLevelId { get; set; }
        int? RoomId { get; set; }
        int ParticipantLimit { get; set; }
        bool IsSolo { get; set; }
        DateTime Start { get; set; }
        List<string> Anchors { get; set; }
        List<ParticipantDto> Participants { get; set; }
        List<ClassDayOfWeekDto> DayOfWeeks { get; set; }
        int DurationTimeInMinutes { get; set; }
        int NumberOfClasses { get; set; }

        int PassPrice { get; set; }
    }
}
