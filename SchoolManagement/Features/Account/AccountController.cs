using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Model.Auth;
using Model.Dto;
using SchoolManagement.Features.Account.Login;
using SchoolManagement.Infrastructure;
using SchoolManagement.Infrastructure.Identity;

namespace SchoolManagement.Features.Account
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;
        private readonly SignInManager<Model.Domain.User> _signInManager;

        public AccountController(IMediator mediator, SignInManager<Model.Domain.User> signInManager)
        {
            _mediator = mediator;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View("Login/Index", new LoginViewModel());
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied/Index");
        }

        [HttpPost]
        public async Task<IActionResult> Login(Command user, CancellationToken cancellationToken)
        {
            DataResult<ClaimsPrincipal> result = await _mediator.Send(user, cancellationToken);
            if (result.Status == DataResult.ResultStatus.Success)
            {
                if (result.Data.HasPermissionClaim(Permissions.Users.SeeAllUser))
                {
                    return RedirectToAction("Index", "Users");
                }
                return RedirectToAction("Index", "Calendar");
            }
            else
            {
                return View("Login/Index", new LoginViewModel()
                {
                    Email = user.Email,
                    RememberMe = user.RememberMe,
                    Error = result.Message
                });
            }
        }
    }
}
