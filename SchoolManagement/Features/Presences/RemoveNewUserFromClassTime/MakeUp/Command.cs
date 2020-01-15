using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using SchoolManagement.Aggregates;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Presences.RemoveNewUserFromClassTime.MakeUp
{
    public class Command : IRequest<DataResult<int>>
    {
        public string ParticipantId { get; set; }
        public int ClassTimeId { get; set; }
    }

    public class Handler : IRequestHandler<Command, DataResult<int>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            ClassTime classTime = await _context.ClassTimes
                .Include(x => x.PresenceParticipants)
                .ThenInclude(x=>x.MakeUpParticipant)
                .Where(x => x.Id == request.ClassTimeId)
                .Include(x => x.PresenceParticipants)
                .FirstOrDefaultAsync(cancellationToken);

            if (classTime is null)
            {
                return DataResult<int>.Error(PolishReadableMessage.Presence.ClassNotFound);
            }

            ParticipantClassTime participantClassTime = classTime.PresenceParticipants.FirstOrDefault(x => x.ParticipantId == request.ParticipantId);
            if (participantClassTime is null)
            {
                return DataResult<int>.Error(PolishReadableMessage.Presence.ParticipantNotFound);
            }

            if (participantClassTime.PresenceType != PresenceType.MakeUp)
            {
                return DataResult<int>.Error(PolishReadableMessage.Presence.RemoveWrongType);
            }

          
            ClassTimeAggregate classTimeAggregate = ClassTimeAggregate.FromState(classTime);
            participantClassTime.MakeUpParticipant.PresenceType = PresenceType.None;
            _context.Update(participantClassTime.MakeUpParticipant);
            classTimeAggregate.RemoveParticipant(participantClassTime.ParticipantId);
            _context.Update(classTimeAggregate.State);
            await _context.SaveChangesAsync(cancellationToken);

            return DataResult<int>.Success(participantClassTime.MakeUpParticipant.Id);
        }
    }
}
