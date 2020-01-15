using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain.Interface;
using Model.Dto;
using SchoolManagement.Infrastructure;
using SchoolManagement.Infrastructure.Extensions;

namespace SchoolManagement.Features.Calendar.LoadClasses
{
    public class Query : IRequest<DataResult<List<CalendarEvent>>>, IUtcOffset
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int UtcOffsetInMinutes { get; set; }
    }

    public class Handler : IRequestHandler<Query, DataResult<List<CalendarEvent>>>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }
        public async Task<DataResult<List<CalendarEvent>>> Handle(Query request, CancellationToken cancellationToken)
        {
            request.Start = request.Start.ToUniversalTime();
            request.End = request.End.ToUniversalTime();
            List<CalendarEvent> calendarEvents = await _context.ClassTimes
                .Where(x=>x.StartDate >= request.Start)
                .Where(x=>x.StartDate <= request.End)
                .Select(x=>new CalendarEvent()
                {
                    Id = x.Id.ToString(),
                    Start = x.StartDate.AddMinutes(request.UtcOffsetInMinutes).ToString("s"),
                    End = x.EndDate.AddMinutes(request.UtcOffsetInMinutes).ToString("s"),
                    Title = $"{x.GroupClass.Name} ({x.Room.Name}) {x.NumberOfClass} / {x.NumberOfClasses}",
                    BackgroundColor = $"#{x.Room.HexColor}",
                    Editable = true
                }).ToListAsync(cancellationToken);

            return DataResult<List<CalendarEvent>>.Success(calendarEvents);
        }
    }
}
