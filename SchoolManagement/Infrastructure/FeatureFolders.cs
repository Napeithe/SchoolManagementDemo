using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Infr
{
    public static class FeatureFolders
    {
        public static MvcOptions WithFeatureConvention(this MvcOptions options)
        {
            options.Conventions.Add(new FeatureConvention());
            return options;
        }

        public static RazorViewEngineOptions WithFeatureFolderLocalizations(this RazorViewEngineOptions o)
        {
            // {0} - Action Name
            // {1} - Controller Name
            // {2} - Area Name
            // {3} - Feature Name
            // Replace normal view location entirely
            o.ViewLocationFormats.Clear();
            o.ViewLocationFormats.Add("/Features/{3}/{1}/{0}.cshtml");
            o.ViewLocationFormats.Add("/Features/{3}/{0}.cshtml");
            o.ViewLocationFormats.Add("/Features/Shared/{0}.cshtml");
            o.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
            return o;
        }
    }
}