using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FD.General.Lib.InputLibrary.Cache
{
    /// <summary>
    /// 缓存
    /// </summary>
    public interface ICache
    {
        void Init();
        string[] Get(string key);
        bool ContainsKey(string key);
        void Clear();
    }
}
