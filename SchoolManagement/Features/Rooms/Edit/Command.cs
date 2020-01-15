using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR; 
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using SchoolManagement.Features.Rooms.List;

namespace SchoolManagement.Features.Rooms.Edit
{
    public class Command : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public static Command FromRoomDto(RoomDto roomDto)
        {
            return new Command()
            {
                Id = roomDto.Id,
                Name = roomDto.Name,
                Color = roomDto.Color
            };
        }
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
            Room room = await _context.Rooms.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            Room newRoom = new Room().WithName(request.Name);
            if (newRoom.NormalizeName != room.NormalizeName)
            {
                bool isNameExist = await _context.Rooms.AnyAsync(x=>x.NormalizeName == newRoom.NormalizeName, cancellationToken);
                if (isNameExist)
                {
                    throw new EditRoomException(PolishReadableMessage.Room.NameDuplicate);
                }

                room.WithName(newRoom.Name);
            }

            room.WithColor(request.Color);
            _context.Update(room);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    public class EditRoomException : Exception
    {
        public EditRoomException(string message) : base(message)
        {
            
        }
    }

    public class AddRoomException : Exception
    {
        public AddRoomException(string message) : base(message)
        {
            
        }
    }
}
