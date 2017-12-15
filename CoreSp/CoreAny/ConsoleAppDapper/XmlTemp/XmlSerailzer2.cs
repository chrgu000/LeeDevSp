using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FD.Generic.Xml
{
    // CreatedBy: Jackie Lee（天宇遊龍）
    // CreatedOn: 2017-04-13
    /// <summary>
    /// Xml序列及反序列化操作
    /// </summary>
    public class XmlSerializer<T>
    {
        private string _xmlHead;
        private string _rootTag;
        private ElementType _elemType;
        private Type _elementType;

        /// <summary>
        /// Xml序列及反序列化操作
        /// </summary>
        /// <param name="xmlHead">XML文件头<?xml ... ?></param>
        /// <param name="rootTag">根标签名称</param>
        public XmlSerializer(string xmlHead)
        {
            _xmlHead = xmlHead;
            if (typeof(T).GetTypeInfo().IsGenericType)
            {
                _elemType = ElementType.Generic;
                _elementType = typeof(T).GenericTypeArguments.FirstOrDefault();
                _rootTag = _elementType.Name;
            }
            else if (typeof(T).GetTypeInfo().IsArray)
            {
                _elemType = ElementType.Array;
                _elementType = typeof(T).GetTypeInfo().GetElementType();
                _rootTag = _elementType.Name;
            }
            else
            {
                _rootTag = typeof(T).Name;
            }
        }

        #region 对象转xml
        /// <summary>
        /// 序列化报文为xml
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public string ToXml(T packet)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(_xmlHead))
            {
                sb.AppendFormat("{0}\r\n", _xmlHead);
            }
            try
            {
                Visit(sb, packet);
            }
            catch (Exception ex)
            {
                throw new XmlSerializerException($"序列化对象异常:{ex.Message}", ex);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 访问对象入口
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="obj"></param>
        private void Visit(StringBuilder sb, object obj)
        {
            if (obj is IEnumerable)
            {
                sb.AppendFormat("<{0}s>", _rootTag);
                VisitCollection(sb, (IEnumerable)obj);
                sb.AppendFormat("</{0}s>", _rootTag);
            }
            else
            {
                VisitObject(sb, obj);
            }
        }

        /// <summary>
        /// 访问集合
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="obj"></param>
        private void VisitCollection(StringBuilder sb, IEnumerable obj)
        {
            foreach (var item in obj)
            {
                if (item is Enumerable)
                {
                    VisitCollection(sb, (IEnumerable)item);
                }
                else
                {
                    VisitObject(sb, item);
                }
            }
        }

        /// <summary>
        /// 访问对象
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="packet"></param>
        private void VisitObject(StringBuilder sb, object packet)
        {
            var pFields = packet.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (pFields.Count() > 0)
            {
                sb.AppendFormat("<{0}>", _rootTag);
                VisitFields(sb, packet, pFields);
                sb.AppendFormat("</{0}>", _rootTag);
            }
        }

        /// <summary>
        /// 访问属性
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="obj"></param>
        /// <param name="fields"></param>
        /// <param name="exceptFields"></param>
        private void VisitFields(StringBuilder sb, object obj, PropertyInfo[] fields, params string[] exceptFields)
        {
            foreach (var field in fields)
            {
                if (exceptFields != null && exceptFields.Contains(field.Name))
                    continue;
                sb.AppendFormat("<{0}>", field.Name.FirstToLower());

                if (!field.PropertyType.FullName.StartsWith("System."))
                {
                    object subObj = field.GetValue(obj);
                    var subFields = field.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    if (subFields.Count() > 0)
                    {
                        VisitFields(sb, subObj, subFields);
                    }
                    else
                    {
                        sb.Append(field.GetValue(obj));
                    }
                }
                else
                {
                    sb.Append(field.GetValue(obj));
                }
                sb.AppendFormat("</{0}>", field.Name.FirstToLower());
            }
        }
        #endregion

        #region xml转对象
        /// <summary>
        /// 序列化为报文内容
        /// </summary>
        /// <param name="xml">以<packet>标签开始的xml内容</param>
        /// <returns></returns>
        public T FromXml(string xml)
        {
            int index;
            if (xml.Trim().StartsWith("<?xml") && (index = xml.IndexOf("?>")) != -1)
            {
                xml = xml.Substring(index + 2).Trim('\r', '\n', ' ');
            }
            try
            {
                switch (_elemType)
                {
                    case ElementType.Generic:
                        return VisitXmlGeneric(xml);
                    case ElementType.Array:
                        return VisitXmlArray(xml);
                    default:
                        return VisitXmlObject(xml);
                }
            }
            catch (Exception ex)
            {
                throw new XmlSerializerException($"反序列化对象信息异常:{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 访问xml中对象集合
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private T VisitXmlGeneric(string xml)
        {
            T collection = Activator.CreateInstance<T>();
            List<string> xmlArr = XmlTagHelper.GetTagContents(xml, _rootTag, "");
            foreach (var itemXml in xmlArr)
            {
                AddElement(collection, itemXml, obj =>
                {
                    Add(collection, obj);
                });
            }
            return collection;
        }

        /// <summary>
        /// 访问xml中对象集合
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private T VisitXmlArray(string xml)
        {
            List<string> xmlArr = XmlTagHelper.GetTagContents(xml, _rootTag, "");
            Array array = Array.CreateInstance(_elementType, xmlArr.Count);
            T collection = (T)Convert.ChangeType(array, typeof(T));
            int index = 0;
            foreach (var itemXml in xmlArr)
            {
                AddElement(collection, itemXml, obj =>
                 {
                     SetValue(collection, obj, index++);
                 });
            }
            return collection;
        }

        /// <summary>
        /// 添加元素到集合
        /// </summary>
        /// <param name="collection">集合</param>
        /// <param name="itemXml">元素xml</param>
        /// <param name="addItem">集合项添加操作</param>
        private void AddElement(T collection, string itemXml, Action<object> addItem)
        {
            var obj = Activator.CreateInstance(_elementType);
            VisitXml($"<{_rootTag}>{itemXml}</{_rootTag}>", obj, _elementType.GetProperties(BindingFlags.Instance | BindingFlags.Public));
            addItem(obj);
        }

        /// <summary>
        /// 访问xml对象
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private T VisitXmlObject(string xml)
        {
            if (string.IsNullOrEmpty(xml) || !xml.StartsWith($"<{_rootTag}>"))
            {
                throw new XmlSerializerException($"反序列化对象信息异常:指定xml内容与指定对象类型{typeof(T)}不匹配");
            }
            T packet = Activator.CreateInstance<T>();
            VisitXml(xml, packet, typeof(T).GetProperties());
            return packet;
        }

        /// <summary>
        /// 添加元素到集合中
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="obj"></param>
        private void Add(T collection, object obj)
        {
            var methodInfo = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m => m.Name.Equals("Add"));
            if (methodInfo == null)
            {
                throw new XmlSerializerException($"反序列化集合xml内容失败，目标{typeof(T).FullName}非集合类型");
            }
            var instance = Expression.Constant(collection);
            var param = Expression.Constant(obj);
            var addExpression = Expression.Call(instance, methodInfo, param);
            var add = Expression.Lambda<Action>(addExpression).Compile();
            add.Invoke();
        }

        /// <summary>
        /// 添加元素到集合中
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="obj"></param>
        private void SetValue(T collection, object obj, int index)
        {
            var methodInfo = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m => m.Name.Equals("SetValue"));
            if (methodInfo == null)
            {
                throw new XmlSerializerException($"反序列化集合xml内容失败，目标{typeof(T).FullName}非集合类型");
            }
            var instance = Expression.Constant(collection);
            var param1 = Expression.Constant(obj);
            var param2 = Expression.Constant(index);
            var addExpression = Expression.Call(instance, methodInfo, param1, param2);
            var setValue = Expression.Lambda<Action>(addExpression).Compile();
            setValue.Invoke();
        }

        /// <summary>
        /// 对象序列化为xml
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="obj"></param>
        /// <param name="fields"></param>
        private void VisitXml(string xml, object obj, PropertyInfo[] fields)
        {
            foreach (var field in fields)
            {
                Type subType = field.PropertyType;
                if (!subType.FullName.StartsWith("System.") && !IsEnumType(subType))
                {
                    object subObj = Activator.CreateInstance(subType);// field.GetValue(obj);
                    var subFields = subType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    field.SetValue(obj, subObj);
                    if (subFields.Count() > 0)
                    {
                        VisitXml(xml, subObj, subFields);
                    }
                    else
                    {
                        field.SetValue(subObj, XmlTagHelper.GetTagContent(xml, field.Name.FirstToLower(), ""));
                    }
                }
                else
                {
                    var value = XmlTagHelper.GetTagContent(xml, field.Name.FirstToLower(), "");
                    if (subType != typeof(string))
                    {
                        if (IsEnumType(subType))
                        {
                            field.SetValue(obj, Enum.Parse(subType, value));
                        }
                        else
                        {
                            field.SetValue(obj, Convert.ChangeType(value, subType));
                        }
                    }
                    else
                    {
                        field.SetValue(obj, value);
                    }
                }
            }
        }
        #endregion

        #region 帮助项
        /// <summary>
        /// 是否为枚举类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsEnumType(Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        /// <summary>
        /// 元素类型
        /// </summary>
        private enum ElementType
        {
            Object, Array, Generic
        }
        #endregion
    }

    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string FirstToLower(this string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string FirstToUpper(this string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;
            return word.Substring(0, 1).ToUpper() + word.Substring(1);
        }
    }
    
    /// <summary>
    /// Xml标签帮助
    /// </summary>
    public class XmlTagHelper
    {
        /// <summary>  
        /// 获取字符中指定标签的值  
        /// </summary>  
        /// <param name="content">字符串</param>  
        /// <param name="tagName">标签</param>  
        /// <param name="attrib">属性名</param>  
        /// <returns>属性</returns>  
        public static string GetTagContent(string content, string tagName, string attrib)
        {
            string tmpStr = string.IsNullOrEmpty(attrib) ? $"<{tagName}>([\\s\\S]*?)</{tagName}>" :
                $"<{tagName}\\s*{attrib}\\s*=\\s*.*?>([\\s\\S]*?)</{tagName}>";
            Match match = Regex.Match(content, tmpStr, RegexOptions.IgnoreCase);

            string result = match.Groups[1].Value;
            return result;
        }

        /// <summary>  
        /// 获取字符中指定标签的值  
        /// </summary>  
        /// <param name="content">字符串</param>  
        /// <param name="tagName">标签</param>  
        /// <param name="attrib">属性名</param>  
        /// <returns>属性</returns>  
        public static List<string> GetTagContents(string content, string tagName, string attrib)
        {
            string tmpStr = string.IsNullOrEmpty(attrib) ? $"<{tagName}>([\\s\\S]*?)</{tagName}>" :
                $"<{tagName}\\s*{attrib}\\s*=\\s*.*?>([\\s\\S]*?)</{tagName}>";
            MatchCollection matchs = Regex.Matches(content, tmpStr, RegexOptions.IgnoreCase);

            var result = new List<string>();
            foreach (Match match in matchs)
            {
                result.Add(match.Groups[1].Value);
            }
            return result;
        }
    }

    /// <summary>
    /// XML序列化异常
    /// </summary>
    public class XmlSerializerException : Exception
    {
        public XmlSerializerException() { }
        public XmlSerializerException(string message) : base(message)
        {
        }
        public XmlSerializerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

namespace FD.Generic.XmlTest
{
    using FD.Generic.Xml;

    // --------------------------------- 测试类 ------------------------------------    
    /// <summary>
    /// 人员
    /// </summary>
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }
        public Gender Gender { get; set; }

        public override string ToString()
        {
            return $"{Id}:{Name},{Gender},{Phone},{Address}";
        }
    }

    /// <summary>
    /// 性别
    /// </summary>
    public enum Gender
    {
        Male, Female
    }

    /// <summary>
    /// 地址
    /// </summary>
    public class Address
    {
        public string Province { get; set; }
        public string City { get; set; }
        public string Detail { get; set; }

        public override string ToString()
        {
            return $"from:{Province}，{City}，{Detail}";
        }
    }

    public class PersonTest
    {
        public void Test()
        {
            Person p1 = new Person
            {
                Id = 1,
                Name = "Jackie",
                Gender = Gender.Male,
                Phone = "18412345678",
                Address = new Address { Province = "广东", City = "深圳", Detail = "xx区xx街道xxxx号" }
            };
            Person p2 = new Person
            {
                Id = 2,
                Name = "Hony",
                Gender = Gender.Female,
                Phone = "13512345678",
                Address = new Address { Province = "广东", City = "深圳", Detail = "yy区yy街道yyyy号" }
            };

            XmlSerializer<Person> xs = new XmlSerializer<Person>("<?xml version=\"1.0\" encoding=\"utf - 8\"?>");
            var xml1 = xs.ToXml(p1);
            OutPrint("对象序列化", xml1);
            var xml2 = xs.ToXml(p2);
            OutPrint("xml反序列化", xml2);

            Console.WriteLine("\r\n============= 数组对象 ===============");

            var pArr = new Person[] { p1, p2 };
            XmlSerializer<Person[]> xsArr = new XmlSerializer<Person[]>("");
            var xml4 = xsArr.ToXml(pArr);
            OutPrint("数组对象序列化", xml4);

            var pArr2 = xsArr.FromXml(xml4);
            Console.WriteLine("============= 数组对象反序列化 ===============");
            pArr2.ToList().ForEach(p =>
            {
                OutPrint("数组对象", p.ToString());
            });

            Console.WriteLine("\r\n============= 泛型集合对象 ===============");
            var ps = new List<Person> { p1, p2 };
            XmlSerializer<List<Person>> xsList = new XmlSerializer<List<Person>>("");
            var xml3 = xsList.ToXml(ps);
            OutPrint("泛型集合对象序列化", xml3);

            var ps2 = xsList.FromXml(xml3);
            Console.WriteLine("============= 泛型集合对象反序列化 ===============");
            ps2.ForEach(p =>
            {
                OutPrint("泛型集合对象", p.ToString());
            });
        }

        private void OutPrint(string tip, string msg)
        {
            Console.WriteLine("======>{0}：", tip);
            Console.WriteLine(msg);
        }
    }
}