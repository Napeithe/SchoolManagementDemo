using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain.Interface;
using SchoolManagement.Features.GroupClass.Add;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.GroupClass.Edit
{
    public class Query : IRequest<DataResult<UpdateViewModel>>, IUtcOffset
    {
        public int GroupId { get; set; }
        public int UtcOffsetInMinutes { get; set; }
    }

    public class QueryHandler: IRequestHandler<Query, DataResult<UpdateViewModel>>
    {
        private readonly SchoolManagementContext _context;

        public QueryHandler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult<UpdateViewModel>> Handle(Query request, CancellationToken cancellationToken)
        {
            Model.Domain.GroupClass groupClass = await _context.GroupClass.Where(x=>x.Id == request.GroupId)
                .Include(x=>x.Anchors)
                .Include(x=>x.Room)
                .Include(x=>x.Participants)
                .ThenInclude(x=>x.User)
                .Include(x=>x.ClassDaysOfWeek)
                .FirstOrDefaultAsync(cancellationToken);

            if (groupClass is null)
            {
                return DataResult<UpdateViewModel>.Error(PolishReadableMessage.GroupClass.NotFound);
            }

            UpdateViewModel updateViewModel = UpdateViewModel.FromGroupClass(groupClass, request.UtcOffsetInMinutes).AsEdit();

            return DataResult<UpdateViewModel>.Success(updateViewModel);
        }
    }
}
