using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;
using Model.Domain;
using Model.Dto;
using SchoolManagement.Features.Participants.Detail;
using SchoolManagement.Features.Shared;
using SchoolManagement.Features.Shared.Components.SearchComponent;
using SchoolManagement.Infrastructure;
using Query = SchoolManagement.Features.Participants.List.Query;

namespace SchoolManagement.Features.Participants
{
    public class ParticipantsController : Controller
    {
        private readonly IMediator _mediator;

        public ParticipantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Policy = Permissions.Participants.List)]
        public async Task<IActionResult> Index(CancellationToken token)
        {
            Query query = new List.Query();
            List<ParticipantItemDto> items = await _mediator.Send(query, token);
            return View("List/Index", items);
        }

        [Authorize(Policy = Permissions.Participants.Detail)]
        public async Task<IActionResult> Detail(Detail.Query query, CancellationToken token)
        {
            DataResult<ParticipantDetail> detail = await _mediator.Send(query, token);
            if (detail.Status == DataResult.ResultStatus.Success)
            {
                return View("Detail/Index", detail.Data);
            }

            TempData[Messages.ErrorMessage] = detail.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Participants.GetToSelect)]
        public async Task<Select2Results> ParticipantSearch(ParticipantsSelectList.Query request, CancellationToken token)
        { 
            var result = await _mediator.Send(request, token);
            return result;
        }


        [Authorize(Policy = Permissions.Participants.Add)]
        public IActionResult AddNewUser()
        {
            TempData["ReturnUrl"] = Url.Action("Index");

            return RedirectToAction("Add", "Users", new { roleName = Roles.Participant });
        }
    }
}
