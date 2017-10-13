using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DownloadDynamicPictureAssist
{
    class Program
    {
        private static readonly string _url = "https://api.i-meto.com/bing";// ?type=S
        private static readonly string[] _arguments = { "color", "type", "new", "dcount", "saveto" };

        static void Main(string[] args)
        {
            Menu:
            ShowMenu();
            string uargs = Console.ReadLine();
            string parameters = string.Empty;
            int count = 1;
            string saveTo = string.Empty;
            bool finish = false;
            if (!string.IsNullOrWhiteSpace(uargs))
            {
                parameters = ResoveArgs(uargs.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries),
                    out count, out saveTo, out finish);
            }
            if (finish)
            {
                goto End;
            }
            string url = _url;
            if (!string.IsNullOrEmpty(parameters))
            {
                url = $"{url}?{parameters}";
            }
            if (count < 0 && count != -1)
            {
                Output("参数有误，请确认输入");
                goto Menu;
            }
            saveTo = string.IsNullOrEmpty(saveTo) ? "images" : saveTo;
            ImageDownloader id = new ImageDownloader(url, saveTo, count);
            id.Download();

            goto Menu;

            End:
            Output("*******************************************************************************");
            Output("Byte! ^_^");
            Console.Read();
        }

        static string ResoveArgs(string[] args, out int count, out string saveTo, out bool finish)
        {
            count = 1;
            saveTo = string.Empty;
            finish = false;
            if (args == null || args.Length < 1)
                return "";
            StringBuilder sb = new StringBuilder();
            var newStr = "";

            if (args.Length == 1 && args[0].Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                finish = true;
            }

            foreach (string arg in args)
            {
                var items = arg.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (items == null || (items.Length == 0 ||
                    (items.Length != 2 && items.Length != 1 && items[0].Equals("new", StringComparison.OrdinalIgnoreCase))))
                    continue;
                if (!_arguments.Contains(items[0]))
                    continue;
                if (items[0].Equals("new", StringComparison.OrdinalIgnoreCase))
                {
                    newStr = items[0];
                }
                else if (items[0].Equals("dcount", StringComparison.OrdinalIgnoreCase))
                {
                    if (!int.TryParse(items[1], out count))
                    {
                        count = 1;
                    }
                }
                else if (items[0].Equals("saveto", StringComparison.OrdinalIgnoreCase))
                {
                    saveTo = items[1];
                }
                else
                {
                    sb.AppendFormat("{0}={1}&", items[0], items[1]);
                }
            }
            if (!string.IsNullOrEmpty(newStr))
            {
                sb.Append(newStr);
            }
            else
            {
                if (sb.Length > 1)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            return sb.ToString();
        }

        static void ShowMenu()
        {
            Output("+++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Output(@"参数：
    color - 图片颜色
        Black,Blue,Brown,Green,Multi,Orange,Pink,Purple,Red,White,Yellow
    type - 图片分类
        A（动物）
        C（文化）
        N（自然）
        S（太空）
        T（旅行）
    new - 返回最新日期的图片
    dcount（下载数量，默认为1，-1为下载全部）
    saveto（下载保存路径，默认为程序所在路径下的images目录）
    quit（退出程序）
");
            Output("+++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Output("例：color:Blue type:S new dcount:2");
        }

        static void Output(string msg)
        {
            Console.WriteLine(msg);
        }
    }    
}
