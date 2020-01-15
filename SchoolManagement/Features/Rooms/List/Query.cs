using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Model;

namespace SchoolManagement.Features.Rooms.List
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }

    public class Query : IRequest<List<RoomDto>>
    {

    }

    public class Handler: IRequestHandler<Query, List<RoomDto>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<List<RoomDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<RoomDto> rooms = await _context.Rooms.Select(x => new RoomDto()
            {
                Id = x.Id,
                Name = x.Name,
                Color = x.HexColor
            }).OrderBy(x => x.Name).ToListAsync(cancellationToken);

            return rooms;
        }
    }
}
