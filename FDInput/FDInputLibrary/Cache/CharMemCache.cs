using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FD.General.Lib.InputLibrary.Cache
{
    /// <summary>
    /// 字体缓存
    /// </summary>
    public class CharMemCache : ICache
    {
        private static MemoryCache _wubiCache = new MemoryCache("wubi");
        private static MemoryCache _pinyinCache = new MemoryCache("pinyin");

        public void Clear()
        {
            _wubiCache.Dispose();
            _pinyinCache.Dispose();
            GC.Collect(-1);
        }

        public bool ContainsKey(string key)
        {
            return _wubiCache.Contains(key) || _pinyinCache.Contains(key);
        }

        public string[] Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            string str = string.Empty;
            try
            {
                if (_wubiCache.Contains(key))
                    str = _wubiCache[key].ToString();
            }
            catch { }
            try
            {
                if (_pinyinCache.Contains(key))
                    str = _pinyinCache[key].ToString();
            }
            catch { }
            if (!string.IsNullOrEmpty(str))
            {
                var arr = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                int index;
                for (int i = 0; i < arr.Length; ++i)
                {
                    if ((index = arr[i].IndexOf('*')) != -1)
                    {
                        arr[i] = arr[i].Substring(0, index);
                    }
                }
                return arr;
            }
            return null;
        }

        public void Init()
        {
            var paths = new string[] { Path.Combine(Application.StartupPath,"Win32","word.dll"),
                Path.Combine(Application.StartupPath,"Win32","pinyin.dll") };

            Action<string, MemoryCache> initData = (path, cache) =>
            {
                var arr = File.ReadAllLines(path);
                foreach (string item in arr)
                {
                    var key = item.Substring(0, item.IndexOf(' '));
                    var value = item.Substring(item.IndexOf(' ') + 1);
                    cache.Add(key, value, DateTimeOffset.MaxValue);
                }
            };

            initData(paths[0], _wubiCache);
            initData(paths[1], _pinyinCache);
        }
    }
}
