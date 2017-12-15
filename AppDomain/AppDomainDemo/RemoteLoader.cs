using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppDomainDemo
{
    public class RemoteLoader : MarshalByRefObject
    {
        private Assembly assembly;

        /// <summary>
        /// 通过namespace读取
        /// </summary>
        /// <param name="fullName"></param>
        public void LoadAssembly(string fullName)
        {
            assembly = Assembly.Load(fullName);
        }

        /// <summary>
        /// 通过路径读取
        /// </summary>
        /// <param name="fullName"></param>
        public void LoadFromAssembly(string fullName)
        {
            assembly = Assembly.LoadFrom(fullName);
        }

        public object CompileExec(string sourceCode, string typeName, string functionName)
        {
            object objClass = assembly.CreateInstance("AutoCompile.Compiled"); // 获取编译方法
            object strResult = objClass.GetType().InvokeMember("GetCompiledByString", BindingFlags.InvokeMethod, null, objClass, new object[] { sourceCode, typeName, functionName });
            return strResult;
        }
    }
}
