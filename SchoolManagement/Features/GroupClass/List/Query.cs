using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Dto;
using Model.Dto.GroupClasses;

namespace SchoolManagement.Features.GroupClass.List
{
    public class Query : IRequest<List<GroupClassItemDto>>
    {
    }

    public class Handler : IRequestHandler<Query, List<GroupClassItemDto>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }
        public async Task<List<GroupClassItemDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<GroupClassItemDto> groupClassItemDtos = await _context.GroupClass.Include(x=>x.Participants).Select(x=>new GroupClassItemDto()
            {
                Name = x.Name,
                Id = x.Id,
                LimitParticipants = x.ParticipantLimits,
                CurrentParticipants = x.Participants.Count
            }).ToListAsync(cancellationToken);

            return groupClassItemDtos;
        }
    }
}
