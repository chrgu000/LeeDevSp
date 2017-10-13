using MorseCoder.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MorseCoder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnTransfer.Click += BtnTransfer_Click;
            ckbFromEnglish.Checked += CkbFromEnglish_Checked;
            ckbFromEnglish.Unchecked += CkbFromEnglish_Checked;
            // FDHelper.Instance.GetTime();
        }

        private void CkbFromEnglish_Checked(object sender, RoutedEventArgs e)
        {
            ckbEnglishSentence.IsEnabled = !(ckbFromEnglish.IsChecked ?? false);
        }

        private void BtnTransfer_Click(object sender, RoutedEventArgs e)
        {
            string source;
            if (ckbFromEnglish.IsChecked ?? false)
            {
                source = txtEnglish.Text.Trim();
                if (source.Length == 0)
                {
                    FDHelper.MsgShow("转换内容不能为空，请输入要转换的英文内容");
                    return;
                }
                string res = FDHelper.Instance.ToMorseCode(source);
                txtMorseCode.Text = res;
                FDHelper.Instance.Play(res);
            }
            else
            {
                source = txtMorseCode.Text.Trim();
                if (source.Length == 0)
                {
                    FDHelper.MsgShow("转换内容不能为空，请输入要转换的摩斯密码");
                    return;
                }
                txtEnglish.Text = FDHelper.Instance.ToEnglish(source, ckbEnglishSentence.IsChecked ?? false);
                FDHelper.Instance.Play(source);
            }
        }
    }
}
