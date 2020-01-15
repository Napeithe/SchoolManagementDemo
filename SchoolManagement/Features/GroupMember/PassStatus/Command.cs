using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.GroupMember.PassStatus
{
    public class Command : IRequest<DataResult>
    {
        public int MemberId { get; set; }
        public bool Status { get; set; } = true;
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
            ParticipantGroupClass member = await _context.GroupClassMembers
                .Where(x => x.Id == request.MemberId)
                .Include(x => x.Passes)
                .FirstOrDefaultAsync(cancellationToken);

            Model.Domain.Pass pass = member.Passes.FirstOrDefault();
            if (pass != null)
            {
                pass.Paid = request.Status;
                _context.Update(pass);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return DataResult.Success();
        }
    }
}
