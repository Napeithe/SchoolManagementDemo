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

namespace SchoolManagement.Features.Presences.AddNewUserToClassTime.Help
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
            User participant = await _context.Users.Where(x => x.Id == request.ParticipantId).FirstOrDefaultAsync(cancellationToken);
            if (participant is null)
            {
                return DataResult.Error(PolishReadableMessage.Presence.ParticipantNotFound);
            }

            ClassTime classTime = await _context.ClassTimes
                .Where(x => x.Id == request.ClassTimeId)
                .Include(x => x.PresenceParticipants)
                .FirstOrDefaultAsync(cancellationToken);

            if (classTime is null)
            {
                return DataResult.Error(PolishReadableMessage.Presence.ClassNotFound);
            }

            ClassTimeAggregate classTimeAggregate = ClassTimeAggregate.FromState(classTime);
            ParticipantClassTime participantClassTime = classTimeAggregate.AddParticipant(participant, PresenceType.Help);
            participantClassTime.WasPresence = true;
            await _context.AddAsync(participantClassTime, cancellationToken);
            _context.Update(classTimeAggregate.State);
            await _context.SaveChangesAsync(cancellationToken);

            return DataResult.Success();
        }
    }
}
