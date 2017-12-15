using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace UploadFileDemo.Controllers
{
    public class FileController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> PostFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));

            string path = HttpContext.Current.Server.MapPath("~/Files/");

            PostMessage pm = new PostMessage { Success = false, Message = "failed", Code = 301 };
            await Request.Content.ReadAsMultipartAsync().ContinueWith(p =>
            {
                var content = p.Result.Contents;
                foreach (var file in content)
                {
                    if (string.IsNullOrEmpty(file.Headers.ContentDisposition.FileName))
                        continue;
                    file.ReadAsStreamAsync().ContinueWith(f =>
                    {
                        Stream stream = f.Result;
                        string fileName = file.Headers.ContentDisposition.FileName.Trim('"');
                        Trace.WriteLine($"Received File:{fileName}");
                        SaveFile(Path.Combine(path, fileName), stream);
                        Trace.WriteLine($"File {fileName} Saved.");
                    });
                }
                pm = new PostMessage { Code = 200, Message = "文件上传成功", Success = true };
            });
            return Json(pm);
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
}
