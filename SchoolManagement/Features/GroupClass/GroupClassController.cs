using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Auth;
using Model.Dto.GroupClasses;
using SchoolManagement.Features.GroupClass.Add;
using SchoolManagement.Features.Shared;
using SchoolManagement.Infrastructure;
using SchoolManagement.Infrastructure.Identity;
using Command = SchoolManagement.Features.GroupClass.ParticipantImport.Command;

namespace SchoolManagement.Features.GroupClass
{
    [Authorize(Policy = Permissions.GroupClass.General)]
    public class GroupClassController : Controller
    {
        private readonly IMediator _mediator;

        public GroupClassController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Policy = Permissions.GroupClass.List)]
        public async Task<IActionResult> Index(CancellationToken token)
        {
            List<GroupClassItemDto> result = await _mediator.Send(new List.Query(), token);
            return View("List/Index", result);
        }

        [Authorize(Policy = Permissions.GroupClass.Add)]
        public IActionResult Add()
        {
            return View("Add/Index", new UpdateViewModel(false));
        }

        [Authorize(Policy = Permissions.GroupClass.GetParticipant)]
        public async Task<JsonResult> GetParticipant([FromQuery]GetParticipant.Query query, CancellationToken token)
        {
            Participant participant = await _mediator.Send(query, token);
            return Json(participant);
        }

        [Authorize(Policy = Permissions.GroupClass.GetGroupMembers)]
        public async Task<JsonResult> GetGroupMembers([FromQuery]GetGroupMembers.Query query, CancellationToken token)
        {
            List<Participant> participant = await _mediator.Send(query, token);
            return Json(participant);
        }

        [Authorize(Policy = Permissions.GroupClass.Remove)]
        public async Task<JsonResult> Remove([FromBody]Remove.Command cmd, CancellationToken token)
        {
            DataResult result = await _mediator.Send(cmd, token);
            if (result.Status == DataResult.ResultStatus.Success)
            {
                TempData[Messages.SuccessMessage] = PolishReadableMessage.GroupClass.RemovedSuccess;
            }
            return Json(result);
        }

        [Authorize(Policy = Permissions.GroupClass.Add)]
        public async Task<IActionResult> AddGroup(Add.Command cmd, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return View("Add/Index", UpdateViewModel.FromCommand(cmd, false));
            }

            cmd.UtcOffsetInMinutes = User.GetUtc();

            DataResult dataResult = await _mediator.Send(cmd, token);

            if (dataResult.Status != DataResult.ResultStatus.Success)
            {
                TempData[Messages.ErrorMessage] = dataResult.Message;
                return View("Add/Index", UpdateViewModel.FromCommand(cmd, false));
            }

            TempData[Messages.SuccessMessage] = PolishReadableMessage.GroupClass.AddSuccess;

            return RedirectToAction("Index");

        }

        [Authorize(Policy = Permissions.GroupClass.Detail)]
        public async Task<IActionResult> Detail(Detail.Query query, CancellationToken token)
        {
            query.UtcOffsetInMinutes = User.GetUtc();
            var dataResult = await _mediator.Send(query, token);

            if (dataResult.Status == DataResult.ResultStatus.Success)
            {
                return View("Detail/Index", dataResult.Data);
            }

            TempData[Messages.ErrorMessage] = dataResult.Message;
            return RedirectToAction("Index");
        }

        [Authorize(Policy = Permissions.GroupClass.Edit)]
        public async Task<IActionResult> Edit(Edit.Query query, CancellationToken token)
        {
            query.UtcOffsetInMinutes = User.GetUtc();
            DataResult<UpdateViewModel> dataResult = await _mediator.Send(query, token);
            if (dataResult.Status == DataResult.ResultStatus.Success)
            {
                return View("Add/Index", dataResult.Data);
            }

            TempData[Messages.ErrorMessage] = dataResult.Message;

            return RedirectToAction("Index");
        }

        [Authorize(Policy = Permissions.GroupClass.Edit)]
        public async Task<IActionResult> EditGroup(Edit.Command command, CancellationToken token)
        {
            DataResult dataResult = await _mediator.Send(command, token);
            if (dataResult.Status == DataResult.ResultStatus.Success)
            {
                TempData[Messages.SuccessMessage] = PolishReadableMessage.GroupClass.EditSuccess;
                return RedirectToAction("Detail", new { id = command.GroupClassId });
            }

            TempData[Messages.ErrorMessage] = dataResult.Message;

            return View("Add/Index", UpdateViewModel.FromCommand(command, true));
        }

        public IActionResult Import(int? groupId)
        {
            if (groupId is null)
            {
                throw new Exception("Nie ustawiono grupy");
            }
            return View("ParticipantImport/Import", groupId.Value);
        }

        public async Task<JsonResult> PostImport(CancellationToken token)
        {
            IFormFile file = Request.Form.Files.FirstOrDefault();
            string groupdIdString = Request.Form["id"].FirstOrDefault();
            bool groupParsed = int.TryParse(groupdIdString, out int groupId);
       
            if (!groupParsed)
            {
                throw new Exception("Niepoprawny indetyfikator grupy.");
            }

            Command command = new Command
            {
                File = file,
                GroupId =groupId
            };
            DataResult result = await _mediator.Send(command, token);

            return Json(result);
        }
    }
}
