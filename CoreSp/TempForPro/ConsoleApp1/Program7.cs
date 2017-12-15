using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ConsoleApp1
{
    class Program7
    {
        static void Main01(string[] args)
        {
            var res = new Tamp().FromXml(@"<xml>
           <ToUserName><![CDATA[gh_7f083739789a]]></ToUserName>
           <FromUserName><![CDATA[oia2TjuEGTNoeX76QEjQNrcURxG8]]></FromUserName>
           <CreateTime>1395658984</CreateTime>
           <MsgType><![CDATA[event]]></MsgType>
           <Event><![CDATA[TEMPLATESENDJOBFINISH]]></Event>
           <MsgID>200163840</MsgID>
           <Status><![CDATA[failed:user block]]></Status>
           </xml>");

            Console.Read();
        }
    }

    class Tamp
    {
        public SortedDictionary<string, object> FromXml(string xml)
        {
            SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();
            if (string.IsNullOrEmpty(xml))
            {
                throw new Exception("将空的xml串转换为WxPayData不合法!");
            }
            XElement doc = XElement.Parse(xml);
            foreach (XElement xe in doc.Elements())
            {
                m_values[xe.Name.LocalName] = xe.Value;//获取xml的键值对到WxPayData内部的数据中
            }
            try
            {
                //2015-06-29 错误是没有签名
                if (m_values["return_code"].ToString() != "SUCCESS")
                {
                    return m_values;
                }
                //CheckSign(config);//验证签名,不通过会抛异常
            }
            catch (Exception ex)
            {
                //throw new WxPayException(ex.Message);
            }

            return m_values;
        }
    }
}
