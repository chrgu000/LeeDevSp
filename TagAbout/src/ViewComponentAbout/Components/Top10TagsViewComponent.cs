using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewComponentAbout.Services;
using ViewComponentAbout.ViewModels;

namespace ViewComponentAbout.Components
{
    public class Top10TagsViewComponent : ViewComponent
    {
        private readonly ITagService _tagService;

        public Top10TagsViewComponent(ITagService tagService)
        {
            _tagService = tagService;
        }

        public IViewComponentResult Inovke()
        {
            var tags = _tagService.LoadTopTags(10);
            var models = tags.Select((tag) =>
                new TagViewModel
                {
                    Id = tag.Id,
                    Name = tag.Name
                });
            return View("TagComponentName", models);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tags = await _tagService.LoadTopTagsAsync(10);
            var models = tags.Select((tag) =>
                new TagViewModel
                {
                    Id = tag.Id,
                    Name = tag.Name
                });
            return View("TagComponentName", models);
        }
    }
}
