using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BankBranchCrawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
        }
    }

    public class FDRequest
    {
        public string Post(string url)
        {
            WebRequest req = WebRequest.Create(url);
            req.Headers[""] = "";
            return string.Empty;
        }
    }
}
