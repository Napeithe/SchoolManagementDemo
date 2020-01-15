using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using Model.Dto.GroupClasses;

namespace SchoolManagement.Features.GroupClass.GetParticipant
{
    public class Query : IRequest<Participant>
    {
        public string Id { get; set; }
        public ParticipantRole Role { get; set; }
    }

    public class Handler : IRequestHandler<Query, Participant>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<Participant> Handle(Query request, CancellationToken cancellationToken)
        {
            Participant participant = await _context.Users.Where(x => x.Id == request.Id).Select(x => new Participant()
            {
                Id = x.Id,
                UserName = $"{x.FirstName} {x.LastName}",
                Role = request.Role,
                RoleDescription = request.Role.ToString()
            }).FirstOrDefaultAsync(cancellationToken);

            return participant;
        }
    }
}
