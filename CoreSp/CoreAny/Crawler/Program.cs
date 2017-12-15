using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Crawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CheLink("ftp://ygdy8:ygdy8@y153.dydytt.net:9186/[阳光电影www.ygdy8.com].28岁未成年.HD.720p.国语中字.mkv");
            return;

            bool debug = false;
            if (debug)
            {
                List<Movie> mds = new List<Movie>
                {
                    new Movie {  Name="A", Url="http://ss.com", MovieType = MovieType.Film,
                        DownloadInfos = new List<DownloadInfo> { new DownloadInfo { Title="BD", Url="http://dwasdf.com" } }
                    }
                };

                MovieSerialier mda = new MovieSerialier(mds, "movies.xml");
                mda.ToXml();

                Console.Read();
                return;
            }

            FDCrawler fc = new Crawler.FDCrawler();
            var movies = fc.GetRes("http://www.dytt8.net/index.html");
            fc.CheckMovie(movies);
            //DD(movies[0]);            

            MovieSerialier ms = new MovieSerialier(movies, "movies.xml");
            ms.ToXml();


            Console.Read();
        }

        static void CheLink(string link)
        {
            BaseCode bc = new Crawler.BaseCode();
            string res = bc.GetRealLink(link);
        }

        static string DD(Movie movie)
        {
            try
            {
                FDCrawler fc = new Crawler.FDCrawler();
                var movieHTML = fc.LoadData(movie.Url);
                if (string.IsNullOrEmpty(movieHTML))
                    return null;
                var movieDoc = new HtmlParser().Parse(movieHTML);
                var zoom = movieDoc.GetElementById("Zoom");
                var lstDownLoadURL = movieDoc.QuerySelectorAll("[bgcolor='#fdfddf']");
                var updatetime = movieDoc.QuerySelector("span.updatetime"); var pubDate = DateTime.Now;
                if (updatetime != null && !string.IsNullOrEmpty(updatetime.InnerHtml))
                {
                    DateTime.TryParse(updatetime.InnerHtml.Replace("发布时间：", ""), out pubDate);
                }
                var lstOnlineURL = lstDownLoadURL.Select(a => a.QuerySelector("a")).Where(item => item != null).Select(item => item.InnerHtml).ToList();

                var movieName = movieDoc.QuerySelector("div.title_all");

                //var movieInfo = new MovieInfo()
                //{
                //    MovieName = movieName != null && movieName.QuerySelector("h1") != null ?
                //    movieName.QuerySelector("h1").InnerHtml : "找不到影片信息...",
                //    Dy2018OnlineUrl = onlineURL,
                //    MovieIntro = zoom != null && isContainIntro ? WebUtility.HtmlEncode(zoom.InnerHtml) : "暂无介绍...",
                //    XunLeiDownLoadURLList = lstOnlineURL,
                //    PubDate = pubDate,
                //};
                return null;// movieInfo;
            }
            catch (Exception ex)
            {
                //LogHelper.Error("GetMovieInfoFromOnlineURL Exception", ex, new { OnloneURL = onlineURL });
                return null;
            }
        }
    }

    /// <summary>
    /// 爬
    /// </summary>
    public class FDCrawler
    {
        private static readonly HtmlParser Parser = new HtmlParser();
        private static readonly BaseCode BaseCode = new BaseCode();

        /// <summary>
        /// 按条件获取div中movie信息
        /// </summary>
        private Func<IHtmlDocument, string, int, Func<int, MovieType>, List<Movie>> _queryMovie = (dom, htmlFilter, takes, getMovieTypeByIndex) =>
         {
             // 从DOM中提取所有class='co_content8'的div
             var lstDivInfo = dom.QuerySelectorAll(htmlFilter); // "div.co_content8");
             List<Movie> movies = new List<Movie>();
             if (lstDivInfo != null)
             {
                 Movie m;
                 int index = 0;
                 foreach (var divInfo in lstDivInfo.Take(takes)) // 最后一个为游戏
                 {
                     // 获取div所有a标签且a标签中含有"/html/"的
                     var asss = divInfo.QuerySelectorAll("a").Select(a => new { Name = a.TextContent, Url = a.GetAttribute("href") })
                        .Where(a => a.Url.IndexOf('/') == 0 && !a.Url.EndsWith("index.html")).ToList();

                     divInfo.QuerySelectorAll("a").Where(a => a.GetAttribute("href").IndexOf('/') == 0 && !a.GetAttribute("href").EndsWith("index.html")).ToList().ForEach(a =>
                      {
                          m = new Movie { Name = a.TextContent, Url = $"http://www.dytt8.net{a.GetAttribute("href")}#", MovieType = getMovieTypeByIndex(index++) };
                          movies.Add(m);
                          //Console.WriteLine(a.GetAttribute("href"));
                          Console.WriteLine(m);
                      });
                 }
             }
             return movies;
         };

        /// <summary>
        /// 获取实际下载地址
        /// </summary>
        private Action<IHtmlDocument, string, Movie> _checkMovie = (dom, htmlFilter, movie) =>
        {
            var divInfo = dom.QuerySelector(htmlFilter);
            if (divInfo != null)
            {
                divInfo.QuerySelectorAll("a").Where(a => a.GetAttribute("thunderrestitle") != null).ToList().ForEach(a =>
                {
                    movie.DownloadInfos.Add(new DownloadInfo
                    {
                        Title = a.GetAttribute("thunderrestitle"),
                        Url = a.GetAttribute("href")
                    });
                });
                divInfo.QuerySelectorAll("a").Where(a => a.GetAttribute("href").IndexOf("ftp://") == 0).ToList().ForEach(a =>
                {
                    movie.DownloadInfos.Add(new DownloadInfo
                    {
                        Title = a.TextContent,
                        Url = a.GetAttribute("href"),
                        ThunderUrl = BaseCode.GetRealLink(a.GetAttribute("href"))
                    });
                });
            }
        };

        /// <summary>
        /// 下载网页内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string LoadData(string url, string encoding = "gb2312")
        {
            HttpClient hc = new HttpClient();
            byte[] res = hc.GetByteArrayAsync(url).Result;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(encoding).GetString(res);
            //return hc.GetStringAsync(url).Result;
        }

        /// <summary>
        /// 获取视频列表
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<Movie> GetRes(string url)
        {
            List<Movie> movies = new List<Movie>();
            try
            {
                var htmlDoc = LoadData(url);
                // HTML解析成IDocument
                var dom = Parser.Parse(htmlDoc);
                movies = movies.Concat(_queryMovie(dom, "div.co_content8", 3, e => { return MovieType.Film; })).ToList();
                movies = movies.Concat(_queryMovie(dom, "div.co_content3", 4, e => { return e != 3 ? MovieType.Teleplay : MovieType.Variety; })).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Download exception:\r\n{ex.Message}");
                return null;
            }
            return movies;
        }

        /// <summary>
        /// 检测实际下载地址
        /// </summary>
        /// <param name="movie"></param>
        private void CheckMovie(Movie movie)
        {
            var htmlDoc = LoadData(movie.Url);
            // HTML解析成IDocument
            var dom = Parser.Parse(htmlDoc);
            _checkMovie(dom, "div.co_content8", movie);
        }

        /// <summary>
        /// 检测实际下载地址
        /// </summary>
        /// <param name="movies"></param>
        public void CheckMovie(List<Movie> movies)
        {
            if (movies == null || movies.Count == 0)
                return;
            foreach (var movie in movies)
            {
                CheckMovie(movie);
            }
        }

        #region
        //private string Link(string arrLink)
        //{
        //    var thunderSufix = ".asf;.avi;.iso;.mp3;.mpeg;.mpg;.mpga;.ra;.rar;.rm;.rmvb;.tar;.wma;.wmp;.wmv;.zip;.swf;.rmvb;.mp4;.3gp;.pdf;.mov;.wav;.scm;.mkv;.exe;.7z;.sub;.idx;.srt;.nfo;.bin;.aac;"
        //    var arrSufix = thunderSufix.Split(';');
        //    var temp = arrLink;
        //    var post = temp.LastIndexOf(".");
        //    var p = temp.Substring(post, temp.Length).ToLower();
        //    var k = arrSufix.Length;
        //    var flag = false;
        //    var thunder_url = arrLink;//[i].href;
        //    //dz锟斤拷锟斤拷
        //    var pathname = arrLink.Substring(arrLink.IndexOf('/', 6));
        //    var path = pathname.Substring(pathname.LastIndexOf("/"), pathname.Length);
        //    string thunderPath = "";

        //    if (path == thunderPath && thunderPath != "")
        //    {
        //        flag = true;
        //    }
        //    //
        //    for (var h = 0; h < arrSufix.Length; h++)
        //    {
        //        if (p == arrSufix[h])
        //        {
        //            flag = true;
        //            break;
        //        }
        //    }
        //    if (flag)
        //    {
        //    if (isIE)
        //    {
        //        try
        //        {
        //            var s = document.createElement("anchor");
        //            s.innerHTML += "<a target='_self' href='#' " + thunderHrefAttr + "='" + ThunderEncode(thunder_url) + "' thunderPid='" + thunderPid + "' thunderType='' thunderResTitle='" + arrLink[i].innerHTML + "' onClick='return OnDownloadClick_Simple(this,2)' oncontextmenu='ThunderNetwork_SetHref(this)'>" + arrLink[i].innerHTML + "</a>";
        //            arrLink[i].replaceNode(s);
        //        }
        //        catch (e)
        //        {
        //            arrLink[i].setAttribute('target', '_self');
        //            arrLink[i].setAttribute('href', '#');
        //            arrLink[i].setAttribute('thunderPid', thunderPid);
        //            arrLink[i].setAttribute('thunderType', '');
        //            arrLink[i].setAttribute('thunderResTitle', arrLink[i].innerHTML);
        //            arrLink[i].setAttribute('onclick', 'return OnDownloadClick_Simple(this,2);');
        //            arrLink[i].setAttribute('oncontextmenu', 'ThunderNetwork_SetHref(this)');
        //            arrLink[i].setAttribute(thunderHrefAttr, ThunderEncode(thunder_url));
        //        }
        //    }
        //    else
        //    {
        //        arrLink[i].setAttribute('target', '_self');
        //        arrLink[i].setAttribute('href', '#');
        //        arrLink[i].setAttribute('thunderPid', thunderPid);
        //        arrLink[i].setAttribute('thunderType', '');
        //        arrLink[i].setAttribute('thunderResTitle', arrLink[i].innerHTML);
        //        arrLink[i].setAttribute('onclick', 'return OnDownloadClick_Simple(this,2);');
        //        arrLink[i].setAttribute('oncontextmenu', 'ThunderNetwork_SetHref(this)');
        //        arrLink[i].setAttribute(thunderHrefAttr, ThunderEncode(thunder_url));
        //    }
        //}
        //}
        //}
        #endregion
    }

    /// <summary>
    /// 视频
    /// </summary>
    public class Movie
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public List<DownloadInfo> DownloadInfos { get; set; }
        public MovieType MovieType { get; set; }

        public Movie()
        {
            DownloadInfos = new List<DownloadInfo>();
        }

        public override string ToString()
        {
            return $"Name:{Name}({Url})";
        }
    }

    public class DownloadInfo
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string ThunderUrl { get; set; }
    }

    public static class ListExtension
    {
        public static bool IsEmpty<T>(this List<T> list)
        {
            return list == null || list.Count == 0;
        }
    }

    /// <summary>
    /// 序列化
    /// </summary>
    public class MovieSerialier
    {
        public string FileName { get; private set; }
        public List<Movie> Movies { get; private set; }
        public MovieSerialier(List<Movie> movies, string fileName)
        {
            FileName = fileName;
            Movies = movies;
        }

        public void ToXml()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Movie>));
                using (FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write))
                {
                    xs.Serialize(fs, Movies);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ToXml exception:{ex.Message}");
            }
        }
    }
    public enum MovieType
    {
        /// <summary>
        /// 电影
        /// </summary>
        Film,
        /// <summary>
        /// 电视剧
        /// </summary>
        Teleplay,
        /// <summary>
        /// 综艺节目
        /// </summary>
        Variety
    }
}
