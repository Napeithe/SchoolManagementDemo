using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using SchoolManagement.Infrastructure;
using SchoolManagement.Infrastructure.Extensions;

namespace SchoolManagement.Features.Presences.GetUserDetailForAddingPresence
{
    public class NewParticipantDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public bool WasAbsent { get; set; }
    }
    public class Query : IRequest<DataResult<NewParticipantDto>>
    {
        public string UserId { get; set; }
    }

    public class  Handler : IRequestHandler<Query, DataResult<NewParticipantDto>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult<NewParticipantDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            bool isAnyAbsent = await _context.ParticipantPresences
                .Include(x=>x.ClassTime)
                .Where(x=>x.ParticipantId == request.UserId)
                .Where(x=>x.PresenceType == PresenceType.None)
                .Where(x=>!x.WasPresence)
                .Where(x=>x.ClassTime.StartDate < DateTime.Now)
                .AnyAsync(cancellationToken);

            NewParticipantDto newParticipant = await _context.Users.Where(x=>x.Id == request.UserId)
                .Select(x=> new NewParticipantDto()
                {
                    Name = $"{x.FirstName} {x.LastName} ({x.Email.HashEmail()})",
                    Id = x.Id
                }).FirstOrDefaultAsync(cancellationToken);

            newParticipant.WasAbsent = isAnyAbsent;

            return DataResult<NewParticipantDto>.Success(newParticipant);
        }
    }
}
