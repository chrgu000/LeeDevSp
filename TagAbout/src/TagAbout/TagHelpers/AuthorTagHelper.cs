using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TagAbout.TagHelpers
{
    [HtmlTargetElement("a", Attributes = ForAttributeName)]
    [HtmlTargetElement("p", Attributes = ForAttributeName)]
    [HtmlTargetElement("ul", Attributes = ForAttributeName)]
    [HtmlTargetElement("li", Attributes = ForAttributeName)]
    [HtmlTargetElement("button", Attributes = ForAttributeName)]
    [HtmlTargetElement("span", Attributes = ForAttributeName)]
    [HtmlTargetElement("div", Attributes = ForAttributeName)]
    public class AuthorTagHelper : TagHelper
    {
        private const string ForAttributeName = "author-for";
        private const string TextAttributeName = "content";

        [HtmlAttributeName(ForAttributeName)]
        public string AuthorFor { get; set; }

        private ContentManager _contentManager;

        public AuthorTagHelper(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.Add("title", AuthorFor);
            output.PostContent.AppendHtml($"<span style='font-weight:bold;color:orange;'>[{_contentManager.GetContent()}]</span>");
            // 可用于验证
            if (false)
            {
                var builder = output.Content;
                output.SuppressOutput(); // nothing to output
                builder.AppendHtml(string.Empty);
            }
        }
    }
}
