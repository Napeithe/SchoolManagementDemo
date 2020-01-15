using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;

namespace SchoolManagement.Features.Rooms.Remove
{
    public class Command : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            Room room = await _context.Rooms.FirstOrDefaultAsync(x=>x.Id == request.Id, cancellationToken);
            if (room is null)
            {
                throw new RemoveRoomException(PolishReadableMessage.Room.RoomNotExist);
            }

            _context.Remove(room);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    public class RemoveRoomException  : Exception
    {
        public RemoveRoomException(string message):base(message)
        {
            
        }
    }

}
