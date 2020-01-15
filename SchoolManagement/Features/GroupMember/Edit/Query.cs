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
    public class ViewModel
    {
        public ViewModel()
        {
            Passes = new List<PassDto>();
        }
        public ParticipantRole Role { get; set; }
        public string Name { get; set; }
        public int MemberId { get; set; }
        public List<PassDto> Passes { get; set; }
        public int GroupId { get; set; }

        public static ViewModel FromMember(ParticipantGroupClass member)
        {
            return new ViewModel()
            {
                Name = $"{member.User.FirstName} {member.User.LastName}",
                Role = member.Role,
                MemberId = member.Id,
                GroupId = member.GroupClassId,
                Passes = member.Passes.OrderBy(x=>x.PassNumber).Select(PassDto.FromPass).ToList()
            };
        }

        public static ViewModel FromCommand(Command cmd)
        {
            return new ViewModel()
            {
                MemberId = cmd.MemberId,
                Passes = cmd.Passes,
                Role = cmd.Role,
                GroupId = cmd.GroupId,
                Name = cmd.Name
            };
        }
    }

    public class Query : IRequest<DataResult<ViewModel>>
    {
        public int MemberId { get; set; }
    }

    public class Handler : IRequestHandler<Query, DataResult<ViewModel>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult<ViewModel>> Handle(Query request, CancellationToken cancellationToken)
        {
            ParticipantGroupClass member = await _context.GroupClassMembers
                .Where(x=>x.Id == request.MemberId)
                .Include(x=>x.Passes)
                .Include(x=>x.User)
                .FirstOrDefaultAsync(cancellationToken);

            if (member is null)
            {
                return DataResult<ViewModel>.Error(PolishReadableMessage.Member.NotFound);
            }

            ViewModel viewModel = ViewModel.FromMember(member);

            return DataResult<ViewModel>.Success(viewModel);
        }
    }
    
}
