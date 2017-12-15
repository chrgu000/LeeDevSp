using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppDomainDemo
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            InitData();
        }

        private void InitData()
        {
            rtbCode.Text = @"
using System;
namespace Test
{
    public class HelloWorld
    {
        public string GetTime(string strName)
        {
            return ""你好： "" + strName + "", 现在是北京时间： "" + System.DateTime.Now.ToString();
        }
    }
}";
        }

        private void btnCompile_Click(object sender, EventArgs e)
        {
            string code = rtbCode.Text.Trim();
            object obj = CreateAppDomainExec(code, "Test.HelloWorld", "GetTime");
            MessageBox.Show(obj.ToString());
        }

        private object CreateAppDomainExec(string code, string typeName, string functionName)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "TestAppDomain";
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            AppDomain appDomain = AppDomain.CreateDomain("CreateDomain", null, setup);
            RemoteLoader remoteLoader = (RemoteLoader)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().FullName, typeof(RemoteLoader).FullName); // 创建访问对象
            remoteLoader.LoadAssembly("AutoCompile"); // 加载编译代码程序集

            object result = remoteLoader.CompileExec(code, typeName, functionName);

            // 卸载程序集
            AppDomain.Unload(appDomain);
            appDomain = null;

            return result;
        }
    }
}
