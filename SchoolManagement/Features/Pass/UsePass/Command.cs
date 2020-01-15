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

namespace SchoolManagement.Features.Pass.UsePass
{
    public class PassMessage
    {
        public enum PassMessageType
        {
            Success = 0,
            Warning, 
            Error
        }
            
        public PassMessageType Type { get; set; }
        public string Message { get; set; }

        public static PassMessage Success(string message)
        {
            return new PassMessage
            {
                Type = PassMessageType.Success,
                Message = message
            };
        }

        public static PassMessage Warning(string message)
        {

            return new PassMessage
            {
                Type = PassMessageType.Warning,
                Message = message
            };
        }

        public static PassMessage Error(string message)
        {

            return new PassMessage
            {
                Type = PassMessageType.Error,
                Message = message
            };
        }
    }

    public class Command : IRequest<DataResult<PassMessage>>
    {
        public int ParticipantClassTimeId { get; set; }
    }

    public class Handler : IRequestHandler<Command, DataResult<PassMessage>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult<PassMessage>> Handle(Command request, CancellationToken cancellationToken)
        {
            ParticipantClassTime participantPresence = await _context.ParticipantPresences
                .Where(x => x.Id == request.ParticipantClassTimeId)
                .FirstOrDefaultAsync(cancellationToken);

            var classTimes = await _context.ClassTimes.Where(x=>x.Id == participantPresence.ClassTimeId).Select(x=>new {x.GroupClassId, x.StartDate}).FirstOrDefaultAsync(cancellationToken);


            List<Model.Domain.Pass> passes = await _context.Passes
                .Include(x=>x.ParticipantGroupClass)
                .Where(x=>x.ParticipantId == participantPresence.ParticipantId)
                .Where(x=>x.ParticipantGroupClass.GroupClassId == classTimes.GroupClassId)
                .ToListAsync(cancellationToken);

            Model.Domain.Pass activePass = passes
                .Where(x=> x.Status == Model.Domain.Pass.PassStatus.Active)
                .OrderBy(x=>x.PassNumber)
                .FirstOrDefault();

            if (activePass is null)
            {
                await GenerateNextPass(cancellationToken, passes, classTimes.StartDate, participantPresence);
                return DataResult<PassMessage>.Success(PassMessage.Error("Brak ważnego karnetu"));
            }

            PassAggregate passAggregate = PassAggregate.FromState(activePass);
            passAggregate.UsePass(participantPresence);
            _context.Update(passAggregate.State);
            _context.Update(participantPresence);
            await _context.SaveChangesAsync(cancellationToken);

            return DataResult<PassMessage>.Success(PassMessage.Success("Udanej nauki!"));
        }

        private async Task GenerateNextPass(CancellationToken cancellationToken, List<Model.Domain.Pass> passes, DateTime startDate,
            ParticipantClassTime participantPresence)
        {
            Model.Domain.Pass lastPass = passes.Where(x => x.Status == Model.Domain.Pass.PassStatus.NotActive)
                .OrderBy(x => x.PassNumber).First();
            Model.Domain.Pass newPass = lastPass.Copy();
            PassAggregate newPassAggregate = PassAggregate.FromState(newPass);
            newPassAggregate.AsActive()
                .WithStartDate(startDate)
                .WithPassNumber(passes.Count)
                .AsGenerateAutomatically();
            newPassAggregate.UsePass(participantPresence);

            await _context.AddAsync(newPassAggregate.State, cancellationToken);
            _context.Update(participantPresence);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
