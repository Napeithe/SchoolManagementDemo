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
    [HtmlTargetElement("script", Attributes = "asp-cut-key")]
    public class TagHelperScriptCut : TagHelper
    {
        [HtmlAttributeName("asp-cut-key")]
        public string CutKey { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            List<TagHelperCutPaste> deferedScripts = new List<TagHelperCutPaste>();

            if (this.ViewContext.HttpContext.Items.ContainsKey(TagHelperCutPaste.ItemsStorageKey))
            {
                deferedScripts = this.ViewContext.HttpContext.Items[TagHelperCutPaste.ItemsStorageKey] as List<TagHelperCutPaste>;

                if (deferedScripts == null)
                {
                    //conversion failed
                    throw new ApplicationException("Duplicate Items key : " + TagHelperCutPaste.ItemsStorageKey);
                }
            }
            else
            {
                this.ViewContext.HttpContext.Items.Add(TagHelperCutPaste.ItemsStorageKey, deferedScripts);
            }

            //solve content
            TagHelperContent result = await output.GetChildContentAsync();

            //add content to the dictionary
            deferedScripts.Add(new TagHelperCutPaste
            {
                CutPasteKey = this.CutKey,

                TagHelperContent = result,
                //pass the attributes
                Attributes = context.AllAttributes.Where(x => x.Name != "asp-cut-key").ToList()
            });

            //do not render content in this section
            output.Content.Clear();
        }
    }
}
