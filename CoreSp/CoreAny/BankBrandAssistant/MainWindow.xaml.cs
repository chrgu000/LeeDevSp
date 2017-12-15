using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BankBrandAssistant
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly List<CharItem> _chars = new List<CharItem>
        {
            new CharItem { Name = "回车", Tag="@ReturnR", Char = "\r" },
             new CharItem { Name = "换行", Tag="@Return",Char ="\n" },
             new CharItem { Name = "缩进", Tag="@Tab",Char ="\t" }//,
             //new CharItem { Name = "回车", Tag="@Rb",Char ="\\" },
             //new CharItem { Name = "回车", Tag="回车",Char ="\r" },
             //new CharItem { Name = "回车", Tag="回车",Char ="\r" },
             //new CharItem { Name = "回车", Tag="回车",Char ="\r" },
             //new CharItem { Name = "回车", Tag="回车",Char ="\r" },
             //new CharItem { Name = "回车", Tag="回车",Char ="\r" },
        };

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            string path = System.Windows.Application.ResourceAssembly.Location;
            var versionInfo = FileVersionInfo.GetVersionInfo(path);//.ExecutablePath)
            Title = $"{versionInfo.ProductName} ({versionInfo.ProductVersion})";

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            btnGenerate.Click += BtnGenerate_Click;

            txtSpliter.Text = "@Tab";
            txtSql.Text = "INSERT INTO dbo.Finance_BankBranch ( BankId , ProvinceId, CityId, BankName,  BankNo)";
            txtValue.Text = "6,440000,440300";

            StringBuilder sb = new StringBuilder();
            sb.Append("特殊字符输入方式：");
            _chars.ForEach(c =>
            {
                sb.AppendFormat("\"{0}\"输入为\"{1}\",", c.Name, c.Tag);
            });
            txtSpliter.ToolTip = sb.ToString().Trim(',');
            txtSql.ToolTip = "将所有固定列，已有的固定值的列全部写在前面，而将需要处理的列放在最后面，并与贴入的值列一一对应";
            txtValue.ToolTip = "固定列，值项";
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = GenerateData();
                if (string.IsNullOrEmpty(data))
                {
                    return;
                }

                if (rdoToClipboard.IsChecked ?? false)
                {
                    System.Windows.Clipboard.SetText(data);
                    System.Windows.MessageBox.Show($"结果已复制，请直接到所需要处进行粘贴即可!");
                }
                else
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Sql脚本|*.sql|文本文件|*.txt|所有文件|*.*";
                    if (System.Windows.Forms.DialogResult.OK == sfd.ShowDialog())
                    {
                        SaveToFile(data, sfd.FileName);
                        if (MessageBoxResult.OK == System.Windows.MessageBox.Show($"结果生成至文件，是否打开？", "温馨提示", MessageBoxButton.OKCancel))
                        {
                            Process.Start(sfd.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"生成异常：{ex.Message}");
            }
        }

        private void SaveToFile(string content, string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.Write(content);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"保存到文件失败：{ex.Message}");
            }
        }

        private string GenerateData()
        {
            string sql = txtSql.Text.Trim();
            string strValue = txtValue.Text.Trim();
            string spliter = txtSpliter.Text;
            if (!Check(sql, "固定SQL", txtSql)) return string.Empty;
            if (!Check(strValue, "SQL前n项值", txtValue)) return string.Empty;
            //if (!Check(spliter, "粘入值分隔符")) return string.Empty;

            TextRange tr = new TextRange(rtbContent.Document.ContentStart, rtbContent.Document.ContentEnd);
            string content = tr.Text.Trim();
            if (!Check(content, "内容值项不能为空", rtbContent)) return string.Empty;

            string[] lines = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            return DealBus(sql, strValue, spliter, lines);
        }

        private string DealBus(string sql, string strValue, string spliter, string[] lines)
        {
            StringBuilder sb = new StringBuilder();
            strValue = strValue.EndsWith(",") ? strValue.TrimEnd(',') : strValue;
            spliter = GetSpliter(spliter);
            if (lines != null && lines.Length > 0)
            {
                bool bEverySql = ckbGenEveryOne.IsChecked ?? false;
                for (int i = 0; i < lines.Length; ++i) // (string line in lines)
                {
                    if (i % 1000 == 0 || bEverySql)
                    {
                        sb.Append(sql).Append(" VALUES ");
                    }
                    string line = lines[i];
                    string value = line;
                    if (spliter.Length > 0)
                    {
                        string[] values = value.Split(new string[] { spliter }, StringSplitOptions.RemoveEmptyEntries);
                        //value = value.EndsWith(spliter) ? value.Remove(value.Length - spliter.Length, spliter.Length) : value;
                        //value = value.Replace(spliter, ",");
                        value = string.Empty;
                        foreach (string va in values)
                        {
                            value += $"'{va}',";
                        }
                    }
                    sb.AppendFormat("({0},{1}){2}", strValue, value.TrimEnd(','), bEverySql ? Environment.NewLine : ",");
                }
            }
            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 检测分隔符，一般只允许第一个为特殊符
        /// </summary>
        /// <param name="spliter"></param>
        /// <returns></returns>
        private string GetSpliter(string spliter)
        {
            _chars.ForEach(c =>
            {
                spliter = spliter.Replace(c.Tag, c.Char);
            });
            return spliter;
        }

        private bool Check(string data, string name, System.Windows.Controls.Control ctrlFocus = null, string tip = "不能为空")
        {
            if (string.IsNullOrEmpty(data))
            {
                System.Windows.MessageBox.Show($"{name}{tip}!");
                if (ctrlFocus != null)
                    ctrlFocus.Focus();
                return false;
            }
            return true;
        }
    }

    public class CharItem
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Char { get; set; }
    }
}
