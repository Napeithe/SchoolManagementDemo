using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;

namespace SchoolManagement.Features.Rooms.Add
{
    public class Command : IRequest<bool>
    {
        public string Name { get; set; }
        public string Color { get; set; }
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
            Room room = new Room()
                .WithColor(request.Color)
                .WithName(request.Name);

            bool isRoomExist = await _context.Rooms
                .AnyAsync(x=>x.NormalizeName == room.NormalizeName, cancellationToken);

            if (isRoomExist)
            {
                throw new AddRoomException(PolishReadableMessage.Room.NameDuplicate);
            }

            await _context.AddAsync(room, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    public class AddRoomException : Exception
    {
        public AddRoomException(string message) : base(message)
        {
            
        }
    }
}
