using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Users.Remove
{
    public class Command : IRequest<DataResult>
    {
        public string Id { get; set; }
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
            User user = await _context.Users.FirstOrDefaultAsync(x=>x.Id == request.Id, cancellationToken);
            _context.Remove(user);

            await _context.SaveChangesAsync(cancellationToken);

            return DataResult.Success();
        }
    }
}
