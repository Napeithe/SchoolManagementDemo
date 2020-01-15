using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.GroupClass.Remove
{
    public class Command : IRequest<DataResult>
    {
        public int Id { get; set; }
    }

    public class Handler: IRequestHandler<Command, DataResult>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult> Handle(Command request, CancellationToken cancellationToken)
        {
            Model.Domain.GroupClass groupClass = await _context.GroupClass.FirstOrDefaultAsync(x=>x.Id == request.Id, cancellationToken);
            if (groupClass is null)
            {
                return DataResult.Error(PolishReadableMessage.GroupClass.NotFound);
            }

            _context.Remove(groupClass);
            await _context.SaveChangesAsync(cancellationToken);
            return DataResult.Success();
        }
    }
}
