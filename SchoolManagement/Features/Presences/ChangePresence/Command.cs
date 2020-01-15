using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Presences.ChangePresence
{
    public class Command :IRequest<DataResult<int>>
    {
        public string ParticipantId { get; set; }
        public int ClassTimeId { get; set; }
        public bool IsPresence { get; set; }
        public PresenceType PresenceType { get; set; }
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
            ParticipantClassTime participantClassTime = await _context.ParticipantPresences
                .Where(x=>x.ParticipantId == request.ParticipantId && x.ClassTimeId == request.ClassTimeId)
                .FirstOrDefaultAsync(cancellationToken);

            if (participantClassTime is null)
            {
                return DataResult<int>.Error(PolishReadableMessage.Presence.ParticipantNotFound);
            }

            participantClassTime.WasPresence = request.IsPresence;
            _context.Update(participantClassTime);
            await _context.SaveChangesAsync(cancellationToken);
            return DataResult<int>.Success(participantClassTime.Id);
        }
    }
}
