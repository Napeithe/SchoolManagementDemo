using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using SchoolManagement.Aggregates;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Calendar.ChangeClassDate
{
    public class Command : IRequest<DataResult>
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
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
            ClassTime classTime = await _context.ClassTimes.Where(x => x.Id == request.Id)
                .Include(x=>x.GroupClass)
                .FirstOrDefaultAsync(cancellationToken);
            ClassTimeAggregate.FromState(classTime)
                .WithDate(request.Start, request.End);

            //Update order
            List<ClassTime> allClasses = await _context.ClassTimes.Where(x=>x.Id != request.Id)
                .Where(x=>x.GroupClassId == classTime.GroupClassId)
                .OrderBy(x=>x.StartDate)
                .ToListAsync(cancellationToken);
            allClasses.Add(classTime);
            allClasses = allClasses.OrderBy(x => x.StartDate).ToList();
            int nextNumber = 1;
            allClasses.ForEach(x=>
            {
                x.NumberOfClass = nextNumber++;
                x.NumberOfClasses = classTime.GroupClass.NumberOfClasses;
            });

         

            _context.UpdateRange(allClasses);
            await _context.SaveChangesAsync(cancellationToken);

            return DataResult.Success();
        }
    }
}
