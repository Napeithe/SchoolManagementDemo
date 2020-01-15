using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Auth;
using SchoolManagement.Features.Shared;
using SchoolManagement.Features.Users.Add;
using SchoolManagement.Features.Users.UserList;
using SchoolManagement.Infrastructure;
using SchoolManagement.Infrastructure.Identity;

namespace SchoolManagement.Features.Users
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public UsersController(IMediator mediator, IUrlHelper urlHelper)
        {
            _mediator = mediator;
            _urlHelper = urlHelper;
        }

        [Authorize(Policy = Permissions.Users.General)]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            Query query = new Query
            {
                IsIncludeAdministrators = User.HasPermissionClaim(Permissions.Users.ShowAdministrator)
            };

            UsersListVierModel usersListVierModel = await _mediator.Send(query, cancellationToken);
            return View("UserList/Index", usersListVierModel);
        }

        public IActionResult Add(string roleName)
        {
            return GetViewForAddUser(new Command
            {
                RoleName = roleName
            });
        }

        [Authorize(Policy =  Permissions.Users.RemoveFromSystem)]
        public async Task<JsonResult> Remove([FromBody]Remove.Command cmd, CancellationToken token)
        {
            DataResult result = await _mediator.Send(cmd, token);
            if (result.Status == DataResult.ResultStatus.Success)
            {
                TempData[Messages.SuccessMessage] = PolishReadableMessage.User.Removed;
            }
            return Json(result);
        }

        private IActionResult GetViewForAddUser(Command cmd)
        {
            TempData.Keep("ReturnUrl");
            ViewModel viewModel = new ViewModel(User);
            ViewBag.CanChooseRole = viewModel.CanChooseRole;
            return View("Add/Index", cmd);
        }

        public async Task<IActionResult> AddUser(Command command, CancellationToken token)
        {
            string link = _urlHelper.Link("", null);

            command.UrlAddress = link;
            command.User = User;

            try
            {
                await _mediator.Send(command, token);
                TempData[Messages.SuccessMessage] = "Dodano nowego użytkownika";
                var returnUrl = TempData["ReturnUrl"];
                if (returnUrl != null)
                {
                    return Redirect(returnUrl.ToString());
                }

                if (User.HasPermissionClaim(Permissions.Users.General))
                {
                    return RedirectToAction("Index");
                }

                return Redirect("/");

            }
            catch (UserAddException e)
            {
                TempData[Messages.ErrorMessage] = e.Message;
                return GetViewForAddUser(command);
            }
        }

        public async Task<IActionResult> Edit(string id, CancellationToken token)
        {
            return await GetUserForEdit(id, token);
        }

        private async Task<IActionResult> GetUserForEdit(string id, CancellationToken token)
        {
            Edit.Query query = new Edit.Query()
            {
                Id = id
            };

            UserDto userDto = await _mediator.Send(query, token);
            return View("Edit/Index", Users.Edit.Command.FromUserDto(userDto));
        }

        public async Task<IActionResult> EditUser(Edit.Command command, CancellationToken token)
        {
            var result = await _mediator.Send(command, token);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Error = result.Errors
                .FirstOrDefault(x => x.Code != nameof(PolishIdentityErrorDescriber.DuplicateUserName))?.Description;

            return View("Edit/Index", command);
        }

        public async Task<IActionResult> ChangePasswordForUser(SetPassword.Command command, CancellationToken token)
        {
            try
            {
                await _mediator.Send(command, token);
                TempData[Messages.SuccessMessage] = "Hasło zostało zmienione";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData[Messages.ErrorMessage] = e.Message;
                return await GetUserForEdit(command.Id, token);
            }
        }
    }
}
