using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;
using SchoolManagement.Features.GroupMember.Edit;
using SchoolManagement.Features.Shared;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.GroupMember
{
    [Authorize]
    public class GroupMemberController : Controller
    {
        private readonly IMediator _mediator;

        public GroupMemberController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Detail([FromQuery]Query query, CancellationToken token)
        {
            DataResult<ViewModel> result = await _mediator.Send(query, token);

            return View("Edit/Index", result.Data);
        }

        [Authorize(Permissions.Members.Update)]
        public async Task<IActionResult> Update(Command cmd, CancellationToken token)
        {
            DataResult<int> result = await _mediator.Send(cmd, token);
            if (result.Status == DataResult.ResultStatus.Success)
            {
                return RedirectToAction("Detail", "GroupClass", new {id = cmd.GroupId});
            }

            TempData[Messages.ErrorMessage] = result.Message;
            return View("Edit/Index", ViewModel.FromCommand(cmd));
        }

        [Authorize(Permissions.Members.SetPassStatusAsPaid)]
        [HttpPost]
        public async Task<JsonResult> ChangePassStatus([FromBody]PassStatus.Command cmd, CancellationToken token)
        {
            DataResult result = await _mediator.Send(cmd, token);

            return Json(result);
        }
    }
}
