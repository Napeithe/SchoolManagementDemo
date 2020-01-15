using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;
using Model.Dto.Presences;
using SchoolManagement.Features.Pass.UsePass;
using SchoolManagement.Features.Presences.ChangePresence;
using SchoolManagement.Features.Presences.GetUserDetailForAddingPresence;
using SchoolManagement.Infrastructure;
using Query = SchoolManagement.Features.Presences.GetAllPresenceOnGroupClass.Query;

namespace SchoolManagement.Features.Presences
{
    [Authorize]
    public class PresencesController : Controller
    {
        private readonly IMediator _mediator;

        public PresencesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<JsonResult> GetPresenceInGroup(Query query, CancellationToken token)
        {
            DataResult<PresenceInGroupDto> dataResult = await _mediator.Send(query, token);
            return Json(dataResult.Data);
        }

        [Authorize(Policy = Permissions.Presence.ChangePresence)]
        [HttpPost]
        public async Task<JsonResult> ChangePresenceForParticipant([FromBody]ChangePresence.Command cmd, CancellationToken token)
        {
            if (cmd.IsPresence)
            {
                DataResult<PassMessage> result = await new PresenceFacade(_mediator).SetAsPresence(cmd, token);
                return Json(result);
            }
            else
            {
                DataResult result = await new PresenceFacade(_mediator).SetAsNotPresence(cmd, token);
                return Json(result);
            }
        }

        [Authorize(Policy = Permissions.Presence.ShowPresence)]
        public async Task<IActionResult> GetPresenceOnDay(GetPresenceInGroupOnDay.Query query, CancellationToken token)
        {
            DataResult<GetPresenceInGroupOnDay.PresenceAtDay> dataResult = await _mediator.Send(query, token);
            return View("GetPresenceInGroupOnDay/Index", dataResult.Data);
        }

        [Authorize(Policy = Permissions.Presence.ChangePresence)]
        public async Task<JsonResult> GetUserOutOfGroupToAddToClass(GetUserDetailForAddingPresence.Query query, CancellationToken token)
        {
            DataResult<NewParticipantDto> dataResult = await _mediator.Send(query, token);
            return Json(dataResult);
        }

        [Authorize(Policy = Permissions.Presence.ChangePresence)]
        [HttpPost]
        public async Task<JsonResult> AddNewUser([FromBody]GetUserDetailForAddingPresence.Query query, CancellationToken token)
        {
            DataResult<NewParticipantDto> dataResult = await _mediator.Send(query, token);
            return Json(dataResult);
        }
    }
}
