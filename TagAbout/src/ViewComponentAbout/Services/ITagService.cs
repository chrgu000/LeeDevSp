using System.Collections.Generic;
using System.Threading.Tasks;
using ViewComponentAbout.Entities;

namespace ViewComponentAbout.Services
{
    public interface ITagService
    {
        IEnumerable<Tag> LoadTopTags(int count);
        Task<IEnumerable<Tag>> LoadTopTagsAsync(int count);
    }
}