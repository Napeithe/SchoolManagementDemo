using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using SchoolManagement.Aggregates;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Pass.Remove
{
    public class Command : IRequest<DataResult>
    {
        public int Id { get; set; }
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
            Model.Domain.Pass pass = await _context.Passes
                .Where(x=>x.Id == request.Id)
                .Include(x=>x.ParticipantClassTimes)
                .FirstOrDefaultAsync(cancellationToken);

            if (pass is null)
            {
                return DataResult.Error(PolishReadableMessage.Pass.NotFound);
            }
            
            if (pass.Used == 0 && !pass.ParticipantClassTimes.Any())
            {
                _context.Remove(pass);
                await _context.SaveChangesAsync(cancellationToken);
                return DataResult.Success();
            }

            PassAggregate.FromState(pass)
                .AsRemoved();
            _context.Update(pass);
            await _context.SaveChangesAsync(cancellationToken);

            return DataResult.Success();
        }
    }
}
