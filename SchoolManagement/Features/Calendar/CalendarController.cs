using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;
using Model.Dto;
using SchoolManagement.Features.Calendar.ChangeClassDate;
using SchoolManagement.Features.Calendar.LoadClasses;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Calendar
{
    public class CalendarController : Controller
    {
        private readonly IMediator _mediator;

        public CalendarController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Permissions.Calendar.Show)]
        public IActionResult Index()
        {
            return View("Show/Index");
        }

        [Authorize(Permissions.Calendar.Show)]
        public async Task<JsonResult> LoadClasses(Query query, CancellationToken token)
        {
            DataResult<List<CalendarEvent>> result = await _mediator.Send(query, token);
            return Json(result.Data);
        }

        [Authorize(Permissions.Calendar.ChangeDate)]
        [HttpPost]
        public async Task<JsonResult> ChangeClassDate([FromBody]Command command, CancellationToken token)
        {
            DataResult result = await _mediator.Send(command, token);
            return Json(result);
        }
    }
}
