using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using Model.Domain.Interface;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Presences.GetPresenceInGroupOnDay
{
    public class PresenceAtDay
    {
        public int ClassTimeId { get; set; }
        public string GroupName { get; set; }
        public string Level { get; set; }
        public string Time { get; set; }
        public string RoomName { get; set; }

        public List<Participant> Participants{ get; set; } = new List<Participant>();
        public List<Participant> AdditionalParticipant { get; set; } = new List<Participant>();

        public class Participant
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsPresence { get; set; }
            public string ParticipantId { get; set; }
            public PresenceType PresenceType { get; set; }
        }
    }

    public class Query : IRequest<DataResult<PresenceAtDay>>, IUtcOffset
    {
        public int ClassTimeId { get; set; }
        public int UtcOffsetInMinutes { get; set; }
    }

    public class Handler : IRequestHandler<Query, DataResult<PresenceAtDay>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult<PresenceAtDay>> Handle(Query request, CancellationToken cancellationToken)
        {
            PresenceAtDay presenceAtDay = await _context.ClassTimes.Where(x=>x.Id == request.ClassTimeId)
                .Include(x=>x.GroupClass)
                .ThenInclude(x=>x.GroupLevel)
                .Include(x=>x.Room)
                .Include(x=>x.PresenceParticipants)
                .ThenInclude(x=>x.Participant)
                .Select(x=>new PresenceAtDay()
                {
                    ClassTimeId = x.Id,
                    GroupName = x.GroupClass.Name,
                    Level = x.GroupClass.GroupLevel.Name,
                    RoomName = x.Room.Name,
                    Time = $"{x.StartDate.AddMinutes(request.UtcOffsetInMinutes):dd.MM.yyyy HH:mm}-{x.EndDate.AddMinutes(request.UtcOffsetInMinutes):HH:mm}",
                    Participants = x.PresenceParticipants
                        .Where(p=>p.PresenceType == PresenceType.Member || p.PresenceType == PresenceType.None)
                        .Select(p=>new PresenceAtDay.Participant
                        {
                            Id = p.Id,
                            ParticipantId = p.ParticipantId,
                            IsPresence = p.WasPresence,
                            Name = $"{p.Participant.FirstName} {p.Participant.LastName}",
                            PresenceType = p.PresenceType
                        }).ToList(),
                    AdditionalParticipant = x.PresenceParticipants
                        .Where(p=>p.PresenceType == PresenceType.Help || p.PresenceType == PresenceType.MakeUp)
                        .Select(p=>new PresenceAtDay.Participant
                        {
                            Id = p.Id,
                            ParticipantId = p.ParticipantId,
                            IsPresence = p.WasPresence,
                            Name = $"{p.Participant.FirstName} {p.Participant.LastName}",
                            PresenceType = p.PresenceType
                        }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);

            return DataResult<PresenceAtDay>.Success(presenceAtDay);
        }
    }
}
