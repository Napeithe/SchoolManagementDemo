using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;

namespace SchoolManagement.Features.Anchors.List
{
    public class AnchorDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }

    }

    public class Query : IRequest<List<AnchorDto>>
    {

    }

    public class Handler :IRequestHandler<Query, List<AnchorDto>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<List<AnchorDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<AnchorDto> anchorDtos = await _context.UserRoles
                .Join(_context.Roles, userRoles => userRoles.RoleId, role => role.Id, (userRoles, role) => new
                {
                    RoleName = role.Name,
                    userRoles.UserId
                }).Where(x => x.RoleName == Roles.Anchor)
                .Join(_context.Users, anonymous => anonymous.UserId, user => user.Id, (role, user) => new AnchorDto()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = $"{user.FirstName} {user.LastName}"
                }).ToListAsync(cancellationToken);

            return anchorDtos;
        }
    }
}
