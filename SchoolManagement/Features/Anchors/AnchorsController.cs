using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using Model.Auth;
using Model.Domain;
using SchoolManagement.Features.Anchors.Detail;
using SchoolManagement.Features.Anchors.List;
using Query = SchoolManagement.Features.Anchors.List.Query;

namespace SchoolManagement.Features.Anchors
{
    [Authorize(Policy = Permissions.Anchors.General)]
    public class AnchorsController : Controller
    {
        private readonly IMediator _mediator;

        public AnchorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Policy = Permissions.Anchors.List)]
        public async Task<IActionResult> Index(CancellationToken token)
        {
            Query query = new Query();

            List<AnchorDto> anchors = await _mediator.Send(query, token);
            return View("List/Index", anchors);
        }

        [Authorize(Policy = Permissions.Anchors.Detail)]
        public async Task<IActionResult> Detail(string id)
        {
            AnchorDetail anchorDetail = await _mediator.Send(new Detail.Query
            {
                Id = id
            });

            return View("Detail/Index", anchorDetail);
        }

        [Authorize(Policy = Permissions.Anchors.Add)]
        public IActionResult AddNewUser()
        {
            TempData["ReturnUrl"] = Url.Action("Index");

            return RedirectToAction("Add", "Users", new {roleName = Roles.Anchor});
        }
    }
}
