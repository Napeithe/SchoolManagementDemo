using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using Model.Dto;

namespace SchoolManagement.Features.Participants.List
{
    public class Query : IRequest<List<ParticipantItemDto>>
    {

    }

    public class Handler :IRequestHandler<Query, List<ParticipantItemDto>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<List<ParticipantItemDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<ParticipantItemDto> participantDto = await _context.UserRoles
                .Join(_context.Roles, userRoles => userRoles.RoleId, role => role.Id, (userRoles, role) => new
                {
                    RoleName = role.Name,
                    userRoles.UserId
                }).Where(x => x.RoleName == Roles.Participant)
                .Join(_context.Users, anonymous => anonymous.UserId, user => user.Id, (role, user) => new ParticipantItemDto()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = $"{user.FirstName} {user.LastName}"
                }).ToListAsync(cancellationToken);

            return participantDto;
        }
    }
}
