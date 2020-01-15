using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Features.Pass.Edit;
using SchoolManagement.Features.Shared;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Pass
{
    [Authorize]
    public class PassController : Controller
    {
        private readonly IMediator _mediator;

        public PassController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<JsonResult> Remove(Remove.Command command, CancellationToken token)
        {
            DataResult result = await _mediator.Send(command, token);
            return Json(result);
        }

        public async Task<IActionResult> Index(Query query, CancellationToken token)
        {
            DataResult<ViewModel> result = await _mediator.Send(query, token);

            if (result.Status == DataResult.ResultStatus.Success)
            {
                return View("Edit/Index", result.Data);
            }

            TempData[Messages.ErrorMessage] = result.Message;

            return RedirectToAction("Detail", "GroupMember", new {memberId = query.RedirectMemberId});
        }

        public async Task<IActionResult> Update(Command command, CancellationToken token)
        {
            DataResult result = await _mediator.Send(command, token);
            if (result.Status == DataResult.ResultStatus.Success)
            {
                TempData[Messages.SuccessMessage] = "Karnet został zaktualizowany";
                return RedirectToAction("Detail", "GroupMember", new {command.MemberId});
            }

            TempData[Messages.ErrorMessage] = result.Message;
            return View("Edit/Index", command);
        }

        public async Task<IActionResult> AddAction(Add.Command command, CancellationToken token)
        {
            DataResult result = await _mediator.Send(command, token);
            return RedirectToAction("Detail", "GroupMember", new {command.MemberId});
        }

        public IActionResult Add(int redirectMemberId)
        {
            return View("Add/Index", new ViewModel()
            {
                MemberId = redirectMemberId
            });
        }
    }
}
