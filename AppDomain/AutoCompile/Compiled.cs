using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoCompile
{
    public class Compiled
    {
        public object GetCompiledByString(string strSourceCode, string typeName, string functionName)
        {
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.GenerateInMemory = true;

            CSharpCodeProvider objCSharpCodeProvider = new CSharpCodeProvider();
            CompilerResults crs = objCSharpCodeProvider.CompileAssemblyFromSource(objCompilerParameters, strSourceCode);
            Assembly objAssembly = crs.CompiledAssembly;

            object objClass = objAssembly.CreateInstance(typeName); // 获取方法
            if (objClass == null) return "获取类失败";

            // 调用
            object objResult = objClass.GetType().InvokeMember(functionName, BindingFlags.InvokeMethod, null, objClass, new object[] { "Frames" });
            return objResult;
        }
    }
}
