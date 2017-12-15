using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheManagerDemo
{
    class Program2
    {
        static void Main(string[] args)
        {
            var dt20170501 = DateTime.Parse("2017-05-01");
            var dtNow = DateTime.Now;
            Console.WriteLine((dtNow - dt20170501).Days);

            Console.Read();
            int? a = null;
            if (a.HasValue)
                Console.WriteLine("Yes");
            else
                Console.WriteLine("No");
            Console.Read();

            return;
            Random rand = new Random();
            for (var i = 0; i < 50; i++)
                Console.Write("{0} ", rand.Next(62));

            Console.Read();
        }

        static void Main123(string[] args)
        {
            var json = "[{\"ParentLevel\":1,\"UserId\":1,\"Location\":0},{\"ParentLevel\":2,\"UserId\":2,\"Location\":0},{\"ParentLevel\":3,\"UserId\":3,\"Location\":0},{\"ParentLevel\":4,\"UserId\":4,\"Location\":0},{\"ParentLevel\":5,\"UserId\":6,\"Location\":1},{\"ParentLevel\":6,\"UserId\":10,\"Location\":1},{\"ParentLevel\":7,\"UserId\":35,\"Location\":0},{\"ParentLevel\":8,\"UserId\":36,\"Location\":0},{\"ParentLevel\":9,\"UserId\":37,\"Location\":0},{\"ParentLevel\":10,\"UserId\":38,\"Location\":0},{\"ParentLevel\":11,\"UserId\":43,\"Location\":0},{\"ParentLevel\":12,\"UserId\":44,\"Location\":0},{\"ParentLevel\":13,\"UserId\":45,\"Location\":0},{\"ParentLevel\":14,\"UserId\":46,\"Location\":0},{\"ParentLevel\":15,\"UserId\":47,\"Location\":0},{\"ParentLevel\":16,\"UserId\":48,\"Location\":0},{\"ParentLevel\":17,\"UserId\":49,\"Location\":0},{\"ParentLevel\":18,\"UserId\":50,\"Location\":0},{\"ParentLevel\":19,\"UserId\":51,\"Location\":0},{\"ParentLevel\":20,\"UserId\":52,\"Location\":0},{\"ParentLevel\":21,\"UserId\":53,\"Location\":0},{\"ParentLevel\":22,\"UserId\":54,\"Location\":0},{\"ParentLevel\":23,\"UserId\":55,\"Location\":0},{\"ParentLevel\":24,\"UserId\":56,\"Location\":0},{\"ParentLevel\":25,\"UserId\":57,\"Location\":0},{\"ParentLevel\":26,\"UserId\":58,\"Location\":0},{\"ParentLevel\":27,\"UserId\":59,\"Location\":0},{\"ParentLevel\":28,\"UserId\":60,\"Location\":0},{\"ParentLevel\":29,\"UserId\":61,\"Location\":0},{\"ParentLevel\":30,\"UserId\":62,\"Location\":0},{\"ParentLevel\":31,\"UserId\":63,\"Location\":0},{\"ParentLevel\":32,\"UserId\":64,\"Location\":0},{\"ParentLevel\":33,\"UserId\":65,\"Location\":0},{\"ParentLevel\":34,\"UserId\":767,\"Location\":1},{\"ParentLevel\":35,\"UserId\":768,\"Location\":0},{\"ParentLevel\":36,\"UserId\":769,\"Location\":0},{\"ParentLevel\":37,\"UserId\":770,\"Location\":0},{\"ParentLevel\":38,\"UserId\":824,\"Location\":0},{\"ParentLevel\":39,\"UserId\":825,\"Location\":0},{\"ParentLevel\":40,\"UserId\":826,\"Location\":0},{\"ParentLevel\":41,\"UserId\":827,\"Location\":0},{\"ParentLevel\":42,\"UserId\":828,\"Location\":0},{\"ParentLevel\":43,\"UserId\":829,\"Location\":0},{\"ParentLevel\":44,\"UserId\":830,\"Location\":0},{\"ParentLevel\":45,\"UserId\":831,\"Location\":0},{\"ParentLevel\":46,\"UserId\":832,\"Location\":0},{\"ParentLevel\":47,\"UserId\":833,\"Location\":0},{\"ParentLevel\":48,\"UserId\":1238,\"Location\":1},{\"ParentLevel\":49,\"UserId\":1527,\"Location\":1},{\"ParentLevel\":50,\"UserId\":1763,\"Location\":0},{\"ParentLevel\":51,\"UserId\":1769,\"Location\":0},{\"ParentLevel\":52,\"UserId\":1771,\"Location\":0},{\"ParentLevel\":53,\"UserId\":2743,\"Location\":0},{\"ParentLevel\":54,\"UserId\":2746,\"Location\":0},{\"ParentLevel\":55,\"UserId\":2753,\"Location\":0},{\"ParentLevel\":56,\"UserId\":2755,\"Location\":0},{\"ParentLevel\":57,\"UserId\":2756,\"Location\":0},{\"ParentLevel\":58,\"UserId\":3033,\"Location\":0},{\"ParentLevel\":59,\"UserId\":3405,\"Location\":0},{\"ParentLevel\":60,\"UserId\":3448,\"Location\":0},{\"ParentLevel\":61,\"UserId\":3455,\"Location\":0},{\"ParentLevel\":62,\"UserId\":3456,\"Location\":0},{\"ParentLevel\":63,\"UserId\":3503,\"Location\":0},{\"ParentLevel\":64,\"UserId\":3508,\"Location\":0},{\"ParentLevel\":65,\"UserId\":3511,\"Location\":0},{\"ParentLevel\":66,\"UserId\":3745,\"Location\":0},{\"ParentLevel\":67,\"UserId\":4630,\"Location\":0},{\"ParentLevel\":68,\"UserId\":4856,\"Location\":0},{\"ParentLevel\":69,\"UserId\":5511,\"Location\":0},{\"ParentLevel\":70,\"UserId\":5513,\"Location\":0},{\"ParentLevel\":71,\"UserId\":5514,\"Location\":0},{\"ParentLevel\":72,\"UserId\":5515,\"Location\":0},{\"ParentLevel\":73,\"UserId\":6035,\"Location\":0},{\"ParentLevel\":74,\"UserId\":6129,\"Location\":0},{\"ParentLevel\":75,\"UserId\":6246,\"Location\":0},{\"ParentLevel\":76,\"UserId\":6248,\"Location\":0},{\"ParentLevel\":77,\"UserId\":6517,\"Location\":0},{\"ParentLevel\":78,\"UserId\":8553,\"Location\":0},{\"ParentLevel\":79,\"UserId\":14322,\"Location\":1},{\"ParentLevel\":80,\"UserId\":14951,\"Location\":0},{\"ParentLevel\":81,\"UserId\":14957,\"Location\":0},{\"ParentLevel\":82,\"UserId\":14968,\"Location\":0},{\"ParentLevel\":83,\"UserId\":14974,\"Location\":0},{\"ParentLevel\":84,\"UserId\":14980,\"Location\":1},{\"ParentLevel\":85,\"UserId\":15801,\"Location\":0},{\"ParentLevel\":86,\"UserId\":17110,\"Location\":0},{\"ParentLevel\":87,\"UserId\":18265,\"Location\":0},{\"ParentLevel\":88,\"UserId\":18304,\"Location\":0},{\"ParentLevel\":89,\"UserId\":18475,\"Location\":0},{\"ParentLevel\":90,\"UserId\":18482,\"Location\":0},{\"ParentLevel\":91,\"UserId\":19559,\"Location\":0},{\"ParentLevel\":92,\"UserId\":19569,\"Location\":0},{\"ParentLevel\":93,\"UserId\":19572,\"Location\":1},{\"ParentLevel\":94,\"UserId\":22208,\"Location\":0},{\"ParentLevel\":95,\"UserId\":22664,\"Location\":0},{\"ParentLevel\":96,\"UserId\":22668,\"Location\":0},{\"ParentLevel\":97,\"UserId\":22670,\"Location\":0},{\"ParentLevel\":98,\"UserId\":22681,\"Location\":0},{\"ParentLevel\":99,\"UserId\":27535,\"Location\":0},{\"ParentLevel\":100,\"UserId\":27544,\"Location\":0},{\"ParentLevel\":101,\"UserId\":27546,\"Location\":0},{\"ParentLevel\":102,\"UserId\":27685,\"Location\":0},{\"ParentLevel\":103,\"UserId\":27968,\"Location\":1},{\"ParentLevel\":104,\"UserId\":30650,\"Location\":0},{\"ParentLevel\":105,\"UserId\":30657,\"Location\":0},{\"ParentLevel\":106,\"UserId\":33121,\"Location\":0},{\"ParentLevel\":107,\"UserId\":233656,\"Location\":1},{\"ParentLevel\":108,\"UserId\":234187,\"Location\":1},{\"ParentLevel\":109,\"UserId\":234543,\"Location\":0},{\"ParentLevel\":110,\"UserId\":234824,\"Location\":0},{\"ParentLevel\":111,\"UserId\":239070,\"Location\":0},{\"ParentLevel\":112,\"UserId\":240868,\"Location\":1}]";
            var data = JsonConvert.DeserializeObject<List<Data>>(json);
            StringBuilder sb = new StringBuilder();
            sb.Append("	SELECT u.UserName,pr.Level FROM dbo.User_PlacementRelation pr INNER JOIN dbo.User_User u ON u.Id=pr.UserId");
            sb.Append(" WHERE u.Id IN(");
            foreach (var d in data)
            {
                sb.AppendFormat("{0},", d.UserId);
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            Console.WriteLine(sb.ToString());

            Console.Read();
        }
    }
    class Data
    {
        public long ParentLevel { get; set; }
        public long UserId { get; set; }
    }
}
