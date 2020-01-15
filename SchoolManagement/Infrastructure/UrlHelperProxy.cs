using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SchoolManagement.Infrastructure
{
    public class UrlHelperProxy : IUrlHelper
    {
        private readonly IActionContextAccessor _accessor;
        private readonly IUrlHelperFactory _factory;

        public UrlHelperProxy(IActionContextAccessor accessor, IUrlHelperFactory factory)
        {
            this._accessor = accessor;
            this._factory = factory;
        }

        public ActionContext ActionContext => UrlHelper.ActionContext;
        public string Action(UrlActionContext context) => UrlHelper.Action(context);
        public string Content(string contentPath) => UrlHelper.Content(contentPath);
        public bool IsLocalUrl( string url) => UrlHelper.IsLocalUrl(url);
        public string Link(string name, object values) => UrlHelper.Link(name, values);
        public string RouteUrl(UrlRouteContext context) => UrlHelper.RouteUrl(context);
        private IUrlHelper UrlHelper => _factory.GetUrlHelper(_accessor.ActionContext);
    }
}
