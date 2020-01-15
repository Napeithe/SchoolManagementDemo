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

namespace SchoolManagement.Features.Presences.AddNewUserToClassTime.MakeUp
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
            User participant = await _context.Users.Where(x => x.Id == request.ParticipantId).FirstOrDefaultAsync(cancellationToken);

            if (participant is null)
            {
                return DataResult<int>.Error(PolishReadableMessage.Presence.ParticipantNotFound);
            }

            ClassTime classTime = await _context.ClassTimes
                .Where(x => x.Id == request.ClassTimeId)
                .Include(x => x.PresenceParticipants)
                .FirstOrDefaultAsync(cancellationToken);

            if (classTime is null)
            {
                return DataResult<int>.Error(PolishReadableMessage.Presence.ClassNotFound);
            }


            var makeUpClassTime = await _context.ParticipantPresences
                .Include(x => x.ClassTime)
                .Where(x => x.ParticipantId == request.ParticipantId)
                .Where(x => !x.WasPresence)
                .Where(x => x.PresenceType == PresenceType.None)
                .Where(x=>x.ClassTime.StartDate < classTime.StartDate)
                .OrderBy(x => x.ClassTime.StartDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (makeUpClassTime is null)
            {
                return DataResult<int>.Error(PolishReadableMessage.Presence.MakeUpClassNotFound);
            }

          
            ClassTimeAggregate classTimeAggregate = ClassTimeAggregate.FromState(classTime);

            ParticipantClassTime participantClassTime = classTimeAggregate.AddParticipant(participant, makeUpClassTime);
            participantClassTime.WasPresence = true;
            await _context.AddAsync(participantClassTime, cancellationToken);
            _context.Update(makeUpClassTime);
            _context.Update(classTimeAggregate.State);
            await _context.SaveChangesAsync(cancellationToken);

            return DataResult<int>.Success(makeUpClassTime.Id);
        }
    }
}
