using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;

namespace SchoolManagement.Features.GroupClass.Add
{
    public class Query : IRequest<List<SelectListItem>>
    {

    }

    public class QueryHandler : IRequestHandler<Query, List<SelectListItem>>
    {
        private readonly SchoolManagementContext _context;

        public QueryHandler(SchoolManagementContext context)
        {
            _context = context;
        }
        public async Task<List<SelectListItem>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<SelectListItem> anchors = await _context.UserRoles.Join(_context.Roles, userRole => userRole.RoleId, role => role.Id, (userRole, role) =>
                    new
                    {
                        role.Name,
                        userRole.UserId
                    }).Where(x => x.Name == Roles.Anchor)
                .Join(_context.Users, userRole => userRole.UserId, user => user.Id, (_, user) => new SelectListItem()
                {
                    Text = $"{user.FirstName} {user.LastName}",
                    Value = user.Id
                }).OrderBy(x=>x.Text).ToListAsync(cancellationToken);

            return anchors;
        }
    }
}
