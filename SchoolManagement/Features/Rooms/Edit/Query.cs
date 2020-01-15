using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using SchoolManagement.Features.Rooms.List;

namespace SchoolManagement.Features.Rooms.Edit
{
    public class Query : IRequest<RoomDto>
    {
        public int Id { get; set; }
    }

    public class QueryHandler: IRequestHandler<Query, RoomDto>
    {
        private readonly SchoolManagementContext _context;

        public QueryHandler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<RoomDto> Handle(Query request, CancellationToken cancellationToken)
        {
            RoomDto roomDto = await _context.Rooms.Where(x => x.Id == request.Id).Select(x => new RoomDto()
            {
                Id = x.Id,
                Name = x.Name,
                Color = x.HexColor
            }).FirstOrDefaultAsync(cancellationToken);

            if (roomDto is null)
            {
                throw new EditRoomException(PolishReadableMessage.Room.RoomNotExist);
            }

            return roomDto;
        }
    }
}
