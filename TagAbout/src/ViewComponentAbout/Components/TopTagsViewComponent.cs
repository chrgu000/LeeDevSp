using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewComponentAbout.Services;
using ViewComponentAbout.ViewModels;

namespace ViewComponentAbout.Components
{
    public class TopTagsViewComponent : ViewComponent
    {
        private readonly ITagService _tagService;

        public TopTagsViewComponent(ITagService tagService)
        {
            _tagService = tagService;
        }

        public IViewComponentResult Inovke(int count)
        {
            var tags = _tagService.LoadTopTags(count);
            var models = tags.Select((tag) =>
                new TagViewModel
                {
                    Id = tag.Id,
                    Name = tag.Name
                });
            return View(models);
        }

        public async Task<IViewComponentResult> InvokeAsync(int count)
        {
            var tags = await _tagService.LoadTopTagsAsync(count);
            var models = tags.Select((tag) =>
                new TagViewModel
                {
                    Id = tag.Id,
                    Name = tag.Name
                });
            return View(models);
        }
    }
}
