using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;

namespace SchoolManagement.Features.Users.UserRoles
{
    public class Query : IRequest<List<SelectListItem>>
    {
        public List<string> SelectedId { get; set; }
    }

    public class Handler : IRequestHandler<Query, List<SelectListItem>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public Task<List<SelectListItem>> Handle(Query request, CancellationToken cancellationToken)
        {
            return _context.Roles
                .Where(x=>x.Name != Roles.SuperAdmin)
                .Select(x => new SelectListItem()
            {
                Value = x.Id,
                Selected = request.SelectedId.Contains(x.Id),
                Text = x.Description
            }).OrderBy(x=>x.Text).ToListAsync(cancellationToken);
        }
    }
}
