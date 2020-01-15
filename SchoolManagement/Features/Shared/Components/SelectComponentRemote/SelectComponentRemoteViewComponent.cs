using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.Features.Shared.Components.SelectComponent;

namespace SchoolManagement.Features.Shared.Components.SelectComponentRemote
{
    public class SelectComponentRemoteViewComponent : ViewComponent
    {
        private readonly IMediator _mediator;

        public SelectComponentRemoteViewComponent(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IViewComponentResult> InvokeAsync(SelectViewModel selectViewModel, IRequest<List<SelectListItem>> query)
        { 
            List<SelectListItem> comboBoxItems = await _mediator.Send(query);
            selectViewModel.WithItems(comboBoxItems);
            return View(selectViewModel);
        }
    }
}
