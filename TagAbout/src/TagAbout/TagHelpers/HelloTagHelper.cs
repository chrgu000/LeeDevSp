using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TagAbout.TagHelpers
{
    [HtmlTargetElement("hello")]
    public class HelloTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "p";
            output.Attributes.Add("id", context.UniqueId);
            output.Attributes.Add("style", "color:red;font-weight:bold;");
            output.PreContent.SetContent("Hello ");
            output.PostContent.SetContent($", time is now: {DateTime.Now.ToString("HH:mm:")}");
        }
    }
}
