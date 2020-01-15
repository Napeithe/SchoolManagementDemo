using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;

namespace SchoolManagement.Features.Anchors.Detail
{
    public class AnchorDetail
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public static AnchorDetail FromUser(User user )
        {
            return new AnchorDetail
            {
                Email = user.Email,
                Id = user.Id,
                LastName = user.LastName,
                FirstName = user.FirstName,
                PhoneNumber = user.PhoneNumber
            };
        }
    }

    public class Query:IRequest<AnchorDetail>
    {
        public string Id { get; set; }
    }

    public class Handler :IRequestHandler<Query, AnchorDetail>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<AnchorDetail> Handle(Query request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id))
            {
                throw new AnchorDetailException(PolishReadableMessage.Anchors.NotFound);
            }

            AnchorDetail anchorDetail = await _context.Users.Where(x=>x.Id == request.Id)
                .Select(x=>AnchorDetail.FromUser(x))
                .FirstOrDefaultAsync(cancellationToken);

            if (anchorDetail is null)
            {
                throw new AnchorDetailException(PolishReadableMessage.Anchors.NotFound);
            }

            return anchorDetail;
        }
    }
}
