using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolManagement.Features.Shared.Components.SelectComponent
{
    public class SelectComponentViewComponent : ViewComponent
    {
        private readonly IMediator _mediator;

        public SelectComponentViewComponent(IMediator mediator)
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
