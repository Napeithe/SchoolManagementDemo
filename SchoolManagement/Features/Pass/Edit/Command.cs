using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Dto;
using SchoolManagement.Aggregates;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Pass.Edit
{
    public class Command : IUpdatePass, IRequest<DataResult>
    {
        public int MemberId { get; set; }
        public DateTime Start { get; set; }
        public int Price { get; set; }
        public int NumberOfEntry { get; set; }
        public bool WasPaid { get; set; }
        public int Used { get; set; }
        public int Id { get; set; }
        public bool IsStudent { get; set; }
    }

    public class Handler : IRequestHandler<Command, DataResult>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult> Handle(Command request, CancellationToken cancellationToken)
        {
            Model.Domain.Pass pass = await _context.Passes.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (pass is null)
            {
                return DataResult.Error(PolishReadableMessage.Pass.NotFound);
            }

            PassAggregate.FromState(pass).UpdateByCommand(request);
            _context.Passes.Update(pass);
            await _context.SaveChangesAsync(cancellationToken);
            return DataResult.Success();
        }
    }
}
