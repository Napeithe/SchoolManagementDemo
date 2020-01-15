using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Dto.GroupClasses;

namespace SchoolManagement.Features.GroupClass.GetGroupMembers
{
    public class Query : IRequest<List<Participant>>
    {
        public int GroupClassId { get; set; }
    }

    public class Handler :IRequestHandler<Query, List<Participant>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Participant>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<Participant> participants = await _context.GroupClass
                .Include(x=>x.Participants)
                .ThenInclude(x=>x.User)
                .Include(x=>x.Participants)
                .ThenInclude(x=>x.Passes)
                .Where(x=>x.Id == request.GroupClassId)
                .SelectMany(x=>x.Participants)
                .Select(x=> new Participant()
                {
                    Id = x.UserId,
                    MemberId = x.Id,
                    UserName = $"{x.User.FirstName} {x.User.LastName}",
                    Role = x.Role,
                    RoleDescription = x.Role.ToString(),
                    PassWasPaid = x.Passes.Where(p=>p.Status == Model.Domain.Pass.PassStatus.Active).Any(p => p.Paid)
                }).OrderBy(x=>x.UserName).ToListAsync(cancellationToken);

            return participants;
        }
    }
}
