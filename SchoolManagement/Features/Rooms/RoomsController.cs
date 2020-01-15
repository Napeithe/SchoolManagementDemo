using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;
using SchoolManagement.Features.Rooms.Edit;
using SchoolManagement.Features.Rooms.List;
using SchoolManagement.Features.Rooms.Remove;
using SchoolManagement.Features.Shared;
using SchoolManagement.Infrastructure;
using SchoolManagement.Models;
using AddRoomException = SchoolManagement.Features.Rooms.Add.AddRoomException;
using Command = SchoolManagement.Features.Rooms.Add.Command;
using Query = SchoolManagement.Features.Rooms.List.Query;

namespace SchoolManagement.Features.Rooms
{
    [Authorize(Policy = Permissions.Rooms.General)]
    public class RoomsController : Controller
    {
        private readonly IMediator _mediator;

        public RoomsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Policy = Permissions.Rooms.ShowRoomList)]
        public async Task<IActionResult> Index(CancellationToken token)
        {
            return await GetRooms(token);
        }

        [Authorize(Policy = Permissions.Rooms.AddRoom)]
        public IActionResult Add(CancellationToken token)
        {
            return View("Add/Index", new Command());
        }

        [Authorize(Policy = Permissions.Rooms.AddRoom)]
        public async Task<IActionResult> AddRoom(Command command, CancellationToken token)
        {
            try
            {
                await _mediator.Send(command, token);
                return RedirectToAction("Index");
            }
            catch (AddRoomException exception)
            {
                TempData[Messages.ErrorMessage] = exception.Message;
                return View("Add/Index", command);
            }
        }

        [Authorize(Policy = Permissions.Rooms.EditRoom)]
        public async Task<IActionResult> Edit(int id, CancellationToken token)
        {
            return await GetRoomForEdit(id, token);
        }

        [Authorize(Policy = Permissions.Rooms.EditRoom)]
        public async Task<IActionResult> EditRoom(Edit.Command command, CancellationToken token)
        {
            try
            {
                await _mediator.Send(command, token);
                return RedirectToAction("Index");
            }
            catch (EditRoomException e)
            {
                TempData[Messages.ErrorMessage] = e.Message;
                return await GetRoomForEdit(command.Id, token);
            }
        }

        [Authorize(Policy = Permissions.Rooms.RemoveRoom)]
        public async Task<JsonResult> Remove([FromBody]Remove.Command cmd, CancellationToken token)
        {
            try
            {
                await _mediator.Send(cmd, token);
                TempData[Messages.SuccessMessage] = "Pokój został usunięty";
                return Json(DataResult.Success());
            }
            catch (RemoveRoomException e)
            {
                return Json(DataResult.Error(e.Message));
            }
        }

        private async Task<IActionResult> GetRoomForEdit(int id, CancellationToken token)
        {
            try
            {
                Edit.Query query = new Edit.Query()
                {
                    Id = id
                };

                RoomDto roomDto = await _mediator.Send(query, token);
                return View("Edit/Index", Rooms.Edit.Command.FromRoomDto(roomDto));
            }
            catch (EditRoomException e)
            {
                return View("Error", new ErrorViewModel()
                {
                    Message = e.Message
                });
            }

        }

        private async Task<IActionResult> GetRooms(CancellationToken token)
        {
            List<RoomDto> rooms = await _mediator.Send(new Query(), token);
            return View("List/Index", rooms);
        }
    }
}
