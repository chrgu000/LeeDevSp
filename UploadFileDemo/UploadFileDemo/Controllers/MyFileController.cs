using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace UploadFileDemo.Controllers
{
    public class MyFileController : ApiController
    {
        [HttpPost]
        public string Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));

            //获取学员信息            
            //获取学员通过科目名称
            string passSubject = HttpContext.Current.Request.Form["passSubject"];
            //获取学员未通过科目名称
            string noPassSubject = HttpContext.Current.Request.Form["noPassSubject"];

            Trace.WriteLine("begin 添加学员信息");
            //添加学员信息

            Trace.WriteLine("end 添加学员信息");

            string path = HttpContext.Current.Server.MapPath("~/Files/");
            Trace.WriteLine("获取图片......");
            Request.Content.ReadAsMultipartAsync().ContinueWith(p =>
            {
                var content = p.Result.Contents;
                Trace.WriteLine("begin 图片");
                foreach (var item in content)
                {

                    if (string.IsNullOrEmpty(item.Headers.ContentDisposition.FileName))
                    {
                        continue;
                    }
                    item.ReadAsStreamAsync().ContinueWith(a =>
                    {
                        Stream stream = a.Result;
                        string fileName = item.Headers.ContentDisposition.FileName;
                        fileName = fileName.Substring(1, fileName.Length - 2);

                        //Image img = Image.FromStream(stream);
                        //img.Save(Path.Combine(path, fileName));                        
                        SaveFile(Path.Combine(path, fileName), stream);
                        Trace.WriteLine("图片名称：" + fileName);

                        //stream 转为 image
                        //saveImg(path, stream, fileName);
                    });
                }
                Trace.WriteLine("end 图片");
            });
            return "ok";
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        private void SaveFile(string fileName, Stream stream)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                BinaryWriter bw = new BinaryWriter(fs);
                byte[] buffer = new byte[1024];
                int readLen;
                while ((readLen = stream.Read(buffer, 0, 1024)) > 0)
                {
                    bw.Write(buffer, 0, readLen);
                }
            }
        }
    }

    public class PostMessage
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
