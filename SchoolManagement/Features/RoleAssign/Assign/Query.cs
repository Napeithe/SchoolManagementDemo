using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;

namespace SchoolManagement.Features.RoleAssign.Assign
{
    public class Query : IRequest<List<SelectListItem>>
    {
        public string RoleName { get; set; }
        public string ReturnUrl { get; set; }

        public void Valid()
        {
            if (string.IsNullOrEmpty(RoleName))
            {
                throw new AssignToRoleException(PolishReadableMessage.Assign.RoleNameIsNotSet);
            }
        }
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
            request.Valid();
            List<string> superAdminAndAnchorId = await _context.Roles
                .Where(x=>x.Name == request.RoleName || x.Name == Roles.SuperAdmin)
                .Select(x=>x.Id)
                .ToListAsync(cancellationToken);

            List<string> usersIdInAnchorRoleOrSuperAdmin = await _context.UserRoles
                .Where(x => superAdminAndAnchorId.Contains(x.RoleId))
                .Select(x=>x.UserId)
                .ToListAsync(cancellationToken);

            List<SelectListItem> usersNotAssigned = await _context.Users
                .Where(x=> !usersIdInAnchorRoleOrSuperAdmin.Contains(x.Id))
                .OrderBy(x=>x.LastName)
                .ThenBy(x=>x.FirstName)
                .Select(x=> new SelectListItem
            {
                Value = x.Id,
                Text = $"{x.FirstName} {x.LastName} ({x.Email})"
            }).ToListAsync(cancellationToken);

            return usersNotAssigned;
        }
    }
}
