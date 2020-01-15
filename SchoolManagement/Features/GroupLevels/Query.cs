using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Model;

namespace SchoolManagement.Features.GroupLevels
{
    public class Query : IRequest<List<SelectListItem>>
    {

    }

    public class Handler : IRequestHandler<Query, List<SelectListItem>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<List<SelectListItem>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<SelectListItem> groupLevels = await _context.GroupLevel
                .OrderBy(x => x.Level)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToListAsync(cancellationToken);

            return groupLevels;
        }
    }
}
