using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain; 
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Participants.Detail
{
    public class ParticipantDetail
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public static ParticipantDetail FromUser(User user )
        {
            return new ParticipantDetail
            {
                Email = user.Email,
                Id = user.Id,
                LastName = user.LastName,
                FirstName = user.FirstName,
                PhoneNumber = user.PhoneNumber
            };
        }
    }

    public class Query:IRequest<DataResult<ParticipantDetail>>
    {
        public string Id { get; set; }
    }

    public class Handler :IRequestHandler<Query, DataResult<ParticipantDetail>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult<ParticipantDetail>> Handle(Query request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id))
            {
                return DataResult<ParticipantDetail>.Error(PolishReadableMessage.Anchors.NotFound);
            }

            ParticipantDetail participantDetail = await _context.Users.Where(x=>x.Id == request.Id)
                .Select(x=>ParticipantDetail.FromUser(x))
                .FirstOrDefaultAsync(cancellationToken);

            if (participantDetail is null)
            {
                return DataResult<ParticipantDetail>.Error(PolishReadableMessage.Anchors.NotFound);
            }

            return DataResult<ParticipantDetail>.Success(participantDetail);
        }
    }
}
