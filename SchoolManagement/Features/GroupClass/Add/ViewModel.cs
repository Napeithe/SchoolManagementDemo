using System.ComponentModel;
using System.Linq;
using Model.Dto;
using Model.Extensions;

namespace SchoolManagement.Features.GroupClass.Add
{
    public class UpdateViewModel : Command
    {
        public UpdateViewModel()
        {
            
        }

        public bool IsEdit { get; set; }

        public UpdateViewModel(bool isEdit)
        {
            IsEdit = isEdit;
        }

        public UpdateViewModel AsEdit()
        {
            IsEdit = true;
            return this;
        }

        public static UpdateViewModel FromCommand(IUpdateGroupClass cmd, bool isEdit)
        {
            UpdateViewModel updateViewModel = new UpdateViewModel(isEdit)
            {
                GroupClassId =  cmd.GroupClassId,
                Name = cmd.Name,
                Start = cmd.Start,
                Anchors = cmd.Anchors,
                RoomId = cmd.RoomId,
                Participants = cmd.Participants,
                IsSolo = cmd.IsSolo,
                GroupLevelId = cmd.GroupLevelId,
                ParticipantLimit = cmd.ParticipantLimit,
                DayOfWeeks = cmd.DayOfWeeks,
                DurationTimeInMinutes = cmd.DurationTimeInMinutes,
                NumberOfClasses = cmd.NumberOfClasses
            };
            return updateViewModel;
        }

        public static UpdateViewModel FromGroupClass(Model.Domain.GroupClass groupClass, int utcOffset)
        {
            return new UpdateViewModel
            {
                Name = groupClass.Name,
                Anchors = groupClass.Anchors.Select(x => x.UserId).ToList(),
                RoomId = groupClass.Room?.Id,
                Participants = groupClass.Participants.Select(x => new ParticipantDto
                {
                    Id = x.UserId,
                    Name = $"{x.User.FirstName} {x.User.LastName}",
                    Role = x.Role
                }).OrderBy(x=>x.Name).ToList(),
                IsSolo = groupClass.IsSolo,
                GroupLevelId = groupClass.GroupLevel?.Id,
                Start = groupClass.StartClasses.AddMinutes(utcOffset),
                DurationTimeInMinutes = groupClass.DurationTimeInMinutes, 
                NumberOfClasses = groupClass.NumberOfClasses,
                GroupClassId = groupClass.Id,
                ParticipantLimit = groupClass.ParticipantLimits,
                DayOfWeeks = groupClass.ClassDaysOfWeek.Select(x=>new ClassDayOfWeekDto()
                {
                    BeginTime = x.Hour.UTCTimeSpanToLocal(utcOffset),
                    DayOfWeek = x.DayOfWeek
                    
                }).ToList()
            };
        }
    }
}
