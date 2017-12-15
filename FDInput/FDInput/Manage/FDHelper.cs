using FD.General.Lib.InputLibrary.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;


namespace FDInput.Manage
{
    /// <summary>
    /// 帮助
    /// </summary>
    public class FDHelper
    {
        private static ICache _charCache;
        private FDHelper() { }
        private static FDHelper _instance;
        private static object _syncObj = new object();

        public static FDHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new FDHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        static FDHelper()
        {
            _charCache = new CharMemCache();
            _charCache.Init();
        }

        public ICache CharCache
        {
            get { return _charCache; }
        }
    }

    public static class ObjExtensions
    {
        public static bool ContainsKey(this FDHelper help, string key)
        {
            return help.CharCache.ContainsKey(key);
        }

        public static string[] Get(this FDHelper help, string key)
        {
            return help.CharCache.Get(key);
        }

        public static void Clear(this FDHelper help)
        {
            help.CharCache.Clear();
        }
    }
}
