using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using Model.Dto;
using SchoolManagement.Aggregates;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.GroupClass.Add
{
    public class Command : IRequest<DataResult>, IUpdateGroupClass
    {
        private int _utcOffset;
        public Command()
        {
            DayOfWeeks = new List<ClassDayOfWeekDto>
            {
                new ClassDayOfWeekDto()
                {
                    BeginTime =new TimeSpan(18,0,0)
                }
            };
            Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }
        public int GroupClassId { get; set; }
        public string Name { get; set; }
        public int? GroupLevelId { get; set; }
        public int? RoomId { get; set; }
        public int ParticipantLimit { get; set; }
        public bool IsSolo { get; set; }
        public List<string> Anchors { get; set; } = new List<string>();
        public List<ParticipantDto> Participants { get; set; } = new List<ParticipantDto>();

        public DateTime Start { get; set; }
        public int NumberOfClasses { get; set; } = 1;
        public int DurationTimeInMinutes { get; set; } = 60;
        public List<ClassDayOfWeekDto> DayOfWeeks { get; set; }
        public int PassPrice { get; set; }

        public int UtcOffsetInMinutes
        {
            get
            {
                if (!UtcWasSet)
                {
                    throw new Exception("Utc was not set");
                }

                return _utcOffset;
            }
            set
            {
                _utcOffset = value;
                UtcWasSet = true;
            }
        }

        public bool UtcWasSet { get; set; }
    }

    public class Handler : IRequestHandler<Command, DataResult>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult> Handle(Command request, CancellationToken cancellationToken)
        {
            GroupClassAggregate groupClassAggregate = GroupClassAggregate.New();
            groupClassAggregate.UpdateAll(request);

            await AddAnchors(request, groupClassAggregate, cancellationToken);

            await AddRoom(request, groupClassAggregate, cancellationToken);

            await AddGroupLevel(request, groupClassAggregate, cancellationToken);

            await AddParticipants(request, groupClassAggregate, cancellationToken);

            groupClassAggregate.UpdateDaysOfWeek(request,_context).CreateSchedule(request);

            await _context.GroupClass.AddAsync(groupClassAggregate.State, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return DataResult.Success();
        }

       

        private async Task AddParticipants(Command request, GroupClassAggregate groupClass, 
            CancellationToken cancellationToken)
        {
            if (request.Participants.Any())
            {
                List<string> membersIds = request.Participants.Select(x => x.Id).ToList();
                List<User> members = await _context.Users.Where(x => membersIds.Contains(x.Id)).ToListAsync(cancellationToken);
                members.ForEach(x =>
                {
                    PassAggregate passAggregate = PassAggregate.New()
                        .UpdateByCommand(request)
                        .WithParticipant(x);

                    ParticipantDto participantDto = request.Participants.First(p => p.Id == x.Id);
                    groupClass.AddParticipant(x, participantDto.Role, passAggregate);
                });
            }
        }

        private async Task AddGroupLevel(Command request, GroupClassAggregate groupClass, CancellationToken cancellationToken)
        {
            if (request.GroupLevelId != default)
            {
                GroupLevel groupLevel = await _context.GroupLevel.Where(x => x.Id == request.GroupLevelId.Value)
                    .FirstOrDefaultAsync(cancellationToken);
                groupClass.WithGroupLevel(groupLevel);
            }
        }

        private async Task AddRoom(Command request, GroupClassAggregate groupClassAggregate, CancellationToken cancellationToken)
        {
            if (request.RoomId != default)
            {
                Room room = await _context.Rooms.Where(x => x.Id == request.RoomId.Value)
                    .FirstOrDefaultAsync(cancellationToken);
                groupClassAggregate.WithRoom(room);
            }
        }

        private async Task AddAnchors(Command request, GroupClassAggregate groupClassAggregate, CancellationToken cancellationToken)
        {
            if (request.Anchors.Any())
            {
                List<User> anchors =
                    await _context.Users.Where(x => request.Anchors.Contains(x.Id)).ToListAsync(cancellationToken);
                groupClassAggregate.AddAnchor(anchors);
            }
        }
    }
}
