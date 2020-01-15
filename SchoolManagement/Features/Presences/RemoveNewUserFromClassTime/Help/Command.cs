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

namespace SchoolManagement.Features.Presences.RemoveNewUserFromClassTime.Help
{
    public class Command : IRequest<DataResult>
    {
        public string ParticipantId { get; set; }
        public int ClassTimeId { get; set; }
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
            ClassTime classTime = await _context.ClassTimes
                .Include(x => x.PresenceParticipants)
                .Where(x => x.Id == request.ClassTimeId)
                .Include(x => x.PresenceParticipants)
                .FirstOrDefaultAsync(cancellationToken);

            if (classTime is null)
            {
                return DataResult.Error(PolishReadableMessage.Presence.ClassNotFound);
            }

            ParticipantClassTime participantClassTime = classTime.PresenceParticipants.FirstOrDefault(x => x.ParticipantId == request.ParticipantId);
            if (participantClassTime is null)
            {
                return DataResult.Error(PolishReadableMessage.Presence.ParticipantNotFound);
            }
            if (participantClassTime.PresenceType != PresenceType.Help)
            {
                return DataResult.Error(PolishReadableMessage.Presence.RemoveWrongType);
            }
            ClassTimeAggregate classTimeAggregate = ClassTimeAggregate.FromState(classTime);

            classTimeAggregate.RemoveParticipant(participantClassTime.ParticipantId);
            _context.Remove(participantClassTime);
            _context.Update(classTimeAggregate.State);
            await _context.SaveChangesAsync(cancellationToken);

            return DataResult.Success();
        }
    }
}
