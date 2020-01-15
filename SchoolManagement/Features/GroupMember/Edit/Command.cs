using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using Model.Dto;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.GroupMember.Edit
{
    public class Command: IRequest<DataResult<int>>
    {
        public int MemberId { get; set; }
        public ParticipantRole Role { get; set; }
        public string Name { get; set; }
        public List<PassDto> Passes { get; set; }
        public int GroupId { get; set; }
    }

    public class CommandHandler: IRequestHandler<Command, DataResult<int>>
    {
        private readonly SchoolManagementContext _context;

        public CommandHandler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            ParticipantGroupClass member = await _context.GroupClassMembers.Where(x => x.Id == request.MemberId).FirstOrDefaultAsync(cancellationToken);

            member.Role = request.Role;

            _context.Update(member);
            await _context.SaveChangesAsync(cancellationToken);

            return DataResult<int>.Success(request.GroupId);
        }
    }
}
