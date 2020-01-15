using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.Utils
{
    public class UrlBuilder 
    {
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string _action;
        private object _parameters;
        private string _controller;

        public UrlBuilder(IUrlHelper urlHelper, IHttpContextAccessor contextAccessor)
        {
            _urlHelper = urlHelper;
            _contextAccessor = contextAccessor;
        }

        public UrlBuilder WithAction(string action, string controller)
        {
            _action = action;
            _controller = controller;

            return this;
        }

        public UrlBuilder WithParameters(object parameters)
        {
            _parameters = parameters;
            return this;
        }

        public string Build()
        {
            var url = _parameters != null ? _urlHelper.Action(_action, _controller, _parameters) : _urlHelper.Action(_action, _controller);
            return MakeLink(url);
        }

        private string MakeLink(string callback)
        {
            string pageAddress =
                $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}";

            var link = $"{pageAddress}{callback}";
            return link;
        }
    }
}
