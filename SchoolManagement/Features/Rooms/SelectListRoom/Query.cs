using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Model;

namespace SchoolManagement.Features.Rooms.SelectListRoom
{
    public class Query : IRequest<List<SelectListItem>>
    {
    }

    public class Handler : IRequestHandler<Query,List<SelectListItem>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<List<SelectListItem>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<SelectListItem> items = await _context
                .Rooms.OrderBy(x => x.Name).Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToListAsync(cancellationToken);

            return items;
        }
    }
}
