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

namespace SchoolManagement.Features.Pass.ReturnPass
{
    public class Command : IRequest<DataResult>
    {
        public int ClassTimeId { get; set; }
        public string ParticipantId { get; set; }
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
            ParticipantClassTime participantPresence = await _context.ParticipantPresences
                .Include(x=>x.Pass)
                .Where(x => x.ParticipantId == request.ParticipantId)
                .Where(x => x.ClassTimeId == request.ClassTimeId)
                .FirstOrDefaultAsync(cancellationToken);

            Model.Domain.Pass pass = participantPresence.Pass;
            if (pass is null )
            {
                if (participantPresence.PresenceType != PresenceType.Help)
                {
                    return DataResult.Error("Karnet nie został zarejestrowany ");
                }

                return DataResult.Success();
            }
            PassAggregate passAggregate = PassAggregate.FromState(pass);
            passAggregate.ReturnPass(participantPresence);

            if (pass.WasGenerateAutomatically && pass.Used == 0)
            {
                _context.Remove(pass);
            }
            else
            {
                _context.Update(pass);
            }

            _context.Update(participantPresence);
            await _context.SaveChangesAsync(cancellationToken);

            return DataResult.Success();
        }
    }
}
