using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class Program3
    {
        static void Main02(string[] args)
        {
            Uri uri = new Uri("http://localhost:800/Admin/AutoConfig/List?key=ZKCloud.App.Core.Finance.Domain.CallBacks.PaySceneConfig&id=1&name=lee");

            // https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxbabc0c70725d4c46&redirect_uri=http://wxtest.5ug.com/User/Weixin/ReturnUrl&response_type=code&scope=snsapi_base&state=L1VzZXIvZnJvbS8/aWQ9MQ==#wechat_redirect

            string s = Encoding.UTF8.GetString(Convert.FromBase64String("L1VzZXIvZnJvbS8/aWQ9MQ=="));
            Console.WriteLine(s);

            Console.Read();
        }

        static void Main01(string[] args)
        {
            int[] a = { 1, 2, 5, 8, 10, 28, 30 };
            int[] b = { 1, 5, 8, 10, 18, 29, 32 };

            List<int> result = new List<int>(); // 求交集，拉链法
            int j = 0;
            int i = 0;
            while (i != a.Length && j != b.Length)
            {
                if (a[i] == b[j])
                {
                    result.Add(a[i]);
                    i++;
                }
                else if (a[i] < b[j])
                {
                    i++;
                }
                else
                {
                    j++;
                }
            }

            Console.WriteLine($"element in array {nameof(a)}");
            Output(a);
            Console.WriteLine($"element in array {nameof(b)}");
            Output(b);
            Console.WriteLine($"element in array {nameof(result)}");
            Output(result.ToArray());


            Console.Read();
        }

        static void Output(int[] arr)
        {
            if (arr == null || arr.Length == 0)
                return;
            for (int i = 0; i < arr.Length; ++i)
            {
                Console.Write($"{arr[i]} ");
            }
            Console.WriteLine();
        }
    }
}
