using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SchoolManagement.Infrastructure.TagHelpers.Common;

namespace SchoolManagement.Infrastructure.TagHelpers
{
    [HtmlTargetElement("script", Attributes = "asp-paste-key")]
    public class TagHelperScriptPaste : TagHelper
    {
        [HtmlAttributeName("asp-paste-key")]
        public string DeferDestinationId { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private IHtmlGenerator Generator { get; set; }

        public TagHelperScriptPaste(IHtmlGenerator generator)
        {
            this.Generator = generator;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            if (this.ViewContext.HttpContext.Items.ContainsKey(TagHelperCutPaste.ItemsStorageKey))
            {
                //get list of script contents
                object storage = this.ViewContext.HttpContext.Items[TagHelperCutPaste.ItemsStorageKey];

                if (!(storage is List<TagHelperCutPaste> cutKeys))
                {
                    //the key was found but conversion failed
                    throw new ApplicationException($"Conversion failed for item type {storage.GetType()} to type" + typeof(Dictionary<string, TagHelperCutPaste>));
                }

                if (cutKeys.Count == 0)
                {
                    return;
                }
                //get those items that match the key for this tag helper
                List<TagHelperCutPaste> componentsWithStorageKey = cutKeys.Where(x => x.CutPasteKey == DeferDestinationId).ToList();

                if (!componentsWithStorageKey.Any())
                {
                    return;
                }
                //render first item in place of this script
                TagHelperCutPaste firstScript = componentsWithStorageKey.First();

                output.Content.SetHtmlContent(firstScript.TagHelperContent.GetContent());

                foreach (TagHelperAttribute attr in firstScript.Attributes)
                {
                    output.Attributes.Add(attr);
                }

                //add the rest of script items after the current one
                if (componentsWithStorageKey.Count > 0)
                {
                    for (int i = 1; i < componentsWithStorageKey.Count; i++)
                    {
                        TagHelperCutPaste script = componentsWithStorageKey[i]; TagBuilder builder = new TagBuilder("script"); builder.MergeAttributes(script.Attributes.ToDictionary(x => x.Name, x => x.Value));

                        builder.InnerHtml.AppendHtml(script.TagHelperContent.GetContent());

                        output.PostElement.AppendHtml(builder);
                    }
                }
            }
        }
}
}
