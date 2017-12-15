using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewComponentAbout.Entities;

namespace ViewComponentAbout.Services
{
    public class TagService : ITagService
    {
        private static Func<List<Tag>> _tags = () =>
        {
            var tags = new List<Tag>();
            for (int i = 0; i < 100; ++i)
            {
                tags.Add(new Tag { Id = $"No.{i}", Name = $"Tag{i}", Description = "Tag entity", CreatedOn = DateTime.Now });
            }
            return tags;
        };

        public IEnumerable<Tag> LoadTopTags(int count)
        {
            return _tags().Take(count);
        }

        public async Task<IEnumerable<Tag>> LoadTopTagsAsync(int count)
        {
            return await Task.Run(() => _tags().Take(count));
        }
    }
}
