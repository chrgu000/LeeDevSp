using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TagAbout
{
    public class ContentManager
    {
        public static ContentManager ContentMgr { get; private set; } = new ContentManager();
        public string GetContent()
        {
            return $"Well, this is the injected data by the tag helper.";
        }
    }
}
