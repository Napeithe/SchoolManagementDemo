using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.Features.Users.UserRoles;

namespace SchoolManagement.Features.Shared.Components.UserRoles
{
    public class UserRolesViewComponent : ViewComponent
    {
        private readonly IMediator _mediator;

        public UserRolesViewComponent(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IViewComponentResult> InvokeAsync(List<string> selectedId)
        {
            Query query = new Query(){
                SelectedId = selectedId
            };
            List<SelectListItem> comboBoxItems = await _mediator.Send(query);
            return View(comboBoxItems);
        }
    }
}
