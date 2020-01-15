using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.Features.Shared.Components.SearchComponent
{
    public class SearchComponentViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(SearchViewModel viewModel)
        {
            return View(viewModel);
        }
    }
}
