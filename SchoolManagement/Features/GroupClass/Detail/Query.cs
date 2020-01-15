using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain.Interface;
using Model.Dto.GroupClasses;
using SchoolManagement.Features.GroupClass.Add;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.GroupClass.Detail
{
    public class Query : IRequest<DataResult<GroupClassDetail>>, IUtcOffset
    {
        public int Id { get; set; }
        public int UtcOffsetInMinutes { get; set; }
    }

    public class Handler :IRequestHandler<Query, DataResult<GroupClassDetail>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult<GroupClassDetail>> Handle(Query request, CancellationToken cancellationToken)
        {
            Model.Domain.GroupClass groupClass = await _context.GroupClass
                .Include(x=>x.GroupLevel)
                .Include(x=>x.Room)
                .Include(x=>x.Anchors)
                .ThenInclude(x=>x.User)
                .Include(x => x.ClassDaysOfWeek)
                .FirstOrDefaultAsync(x=>x.Id ==request.Id, cancellationToken);

            if (groupClass is null)
            {
                return DataResult<GroupClassDetail>.Error(PolishReadableMessage.GroupClass.NotFound);
            }

            GroupClassDetail groupClassDetail = GroupClassDetail.From(groupClass, request.UtcOffsetInMinutes);

            return DataResult<GroupClassDetail>.Success(groupClassDetail);
        }
    }
}
