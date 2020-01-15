using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Dto.Presences;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Presences.GetAllPresenceOnGroupClass
{
    public class Query : IRequest<DataResult<PresenceInGroupDto>>
    {
        public int GroupClassId { get; set; }
    }

    public class Handler : IRequestHandler<Query, DataResult<PresenceInGroupDto>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult<PresenceInGroupDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var presenceForGroup = await _context.ParticipantPresences.Join(_context.ClassTimes, time => time.ClassTimeId, time => time.Id,
                    (relation, classTime) => new {classTime.GroupClassId, classTime.StartDate, relation})
                .Where(x => x.GroupClassId == request.GroupClassId)
                .Select(x => new {x.relation, x.StartDate})
                .Join(_context.Users, anonymous => anonymous.relation.ParticipantId, user => user.Id,
                    (anonymous, user) => new
                    {
                        ParticipantName = $"{user.FirstName} {user.LastName}",
                        Type = anonymous.relation.PresenceType,
                        WasPresence = anonymous.relation.WasPresence,
                        Date = anonymous.StartDate,
                        ParticipantId = anonymous.relation.ParticipantId,
                        ClassTimeId = anonymous.relation.ClassTimeId
                    }).ToListAsync(cancellationToken);

            List<string> columns = await _context.ClassTimes.Where(x=>x.GroupClassId == request.GroupClassId)
                .OrderBy(x=>x.StartDate).Select(x=>x.StartDate.ToString("dd.MM")).ToListAsync(cancellationToken);

            List<PresenceInGroupDto.Participant> presenceDtos = presenceForGroup.GroupBy(x=>x.ParticipantId).Select(x=>new PresenceInGroupDto.Participant()
            {
                ParticipantName = x.FirstOrDefault()?.ParticipantName,
                ParticipantId = x.Key,
                PresenceValues = x.OrderBy(s=>s.Date).Select(s=>new PresenceInGroupDto.PresenceValue
                {
                    Id = s.ClassTimeId,
                    Value = s.WasPresence,
                    PresenceType = s.Type,
                    Date = s.Date.ToString("dd.MM")
                }).ToList(),
            }).ToList();



            return DataResult<PresenceInGroupDto>.Success(new PresenceInGroupDto
            {
                ParticipantsPresence = presenceDtos,
                Columns = columns
            });
        }
    }
}
