using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.Features.RoleAssign.Assign;
using SchoolManagement.Features.RoleAssign.Unassign;
using SchoolManagement.Features.Shared;
using Command = SchoolManagement.Features.RoleAssign.Assign.Command;

namespace SchoolManagement.Features.RoleAssign
{
    public class RoleAssignController : Controller
    {
        private readonly IMediator _mediator;

        public RoleAssignController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> AssignRole(Query query, CancellationToken token)
        {
            return await GetUsers(query, token);
        }

        public async Task<IActionResult> UnAssignFromRole(Unassign.Command cmd, string returnUrl, string failBackUrl, CancellationToken token)
        {
            try
            {
                cmd.ClaimsPrincipal = User;
                await _mediator.Send(cmd, token);
                TempData[Messages.SuccessMessage] = "Usunięto użytkownika z roli";
                returnUrl = returnUrl ?? "/";
                return Redirect(returnUrl);
            }
            catch (UnassignRoleException e)
            {
                TempData[Messages.ErrorMessage] = e.Message;
                return Redirect(failBackUrl);
            }
        }

        private async Task<IActionResult> GetUsers(Query query, CancellationToken token)
        {
            List<SelectListItem> notAssignedUsers = await _mediator.Send(query, token);

            return View("Assign/Index", new RoleAssignViewModel()
            {
                RoleName = query.RoleName,
                Users = notAssignedUsers,
                ReturnUrl = query.ReturnUrl
            });
        }


        public async Task<IActionResult> AssignRoleAction(Command cmd, CancellationToken token)
        {
            try
            {
                await _mediator.Send(cmd, token);
                TempData["SuccessCallback"] = "Przypisano użytkownika do roli";
                return Redirect(cmd.ReturnUrl);

            }
            catch (AssignToRoleException e)
            {
                Query query = new Query()
                {
                    RoleName = cmd.RoleName
                };

                ViewBag.Error = e.Message;
                return await GetUsers(query, token);
            }
        }
    }
}
