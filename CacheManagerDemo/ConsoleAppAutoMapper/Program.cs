using System;
using System.Text;

namespace ConsoleAppAutoMapper
{
    class Program
    {
        static void Main01(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            MapperTest();

            Console.Read();
        }

        static void MapperTest()
        {
            Article article = new Article
            {
                Title = "漫谈实体、对象、DTO及AutoMapper的使用",
                Content = "实体（Entity）、对象（Object）、DTO（Data Transfer Object）数据传输对象，老生常谈话题，简单的概念，换个角度你会发现更多的东西。个人拙见，勿喜请喷。",
                Author = "xishuai",
                PostTime = DateTime.Now,
                Remark = "文章备注"
            };
            // 配置AutoMapper
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Article, ArticleDTO>() // 创建映射
                .ForMember(dest => dest.ArticleID, opt => opt.MapFrom(src => src.Id)) // 指定映射规则
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author)) // 指定映射规则
                .ForMember(dest => dest.PostYear, opt => opt.MapFrom(src => src.PostTime.Year)) // 指定映射规则
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark)) // 指定映射规则
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Content.Substring(0, 12))) // 指定映射规则
                .ForMember(dest => dest.Remark, opt => opt.Ignore()); // 指定映射规则，有没的属性
            });

            // 调用映射
            ArticleDTO from = AutoMapper.Mapper.Map<Article, ArticleDTO>(article);
            Console.WriteLine($"{from.ArticleID},{from.Author},{from.PostYear},{from.Summary}");
        }
    }

    public interface IEntity
    {
        Guid Id { get; set; }
    }

    public class Article : IEntity
    {
        public Article()
        {
            Id = Guid.NewGuid();
        }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime PostTime { get; set; }
        public string Remark { get; set; }
        #region IEntity Members
        /// <summary>       
        /// 读取或设置文章的编号    
        /// </summary>       
        public Guid Id { get; set; }
        #endregion
    }

    public class ArticleDTO
    {
        /// <summary>
        /// 文章唯一编码
        /// </summary>
        public string ArticleID { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 文章摘要
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 文章作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 文章发表日期
        /// </summary>
        public DateTime PostTime { get; set; }
        /// <summary>
        /// 文章发表年份
        /// </summary>
        public int PostYear { get; set; }
        /// <summary>
        /// 文章备注
        /// </summary>
        public string Remark { get; set; }
    }
}