using FDInput.Manage;
using System;
using System.Linq;
using System.Windows.Forms;

namespace FDInput
{
    public partial class FormMain : Form
    {
        private string _keys = string.Empty;
        private int _pageIndex = 1;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Program.keyboardHook.KeyUpEvent += KeyboardHook_KeyUpEvent;
            Program.keyboardHook.OnSpaced += KeyboardHook_OnSpaced;
            Program.keyboardHook.OnBacked += KeyboardHook_OnBacked;
            Program.keyboardHook.OnPaged += KeyboardHook_OnPaged;
        }

        private void KeyboardHook_OnPaged(int pageIncr)
        {
            _pageIndex += pageIncr;
            if (_pageIndex < 1)
                _pageIndex = 1;
            ShowCharacter();
        }

        private void KeyboardHook_OnBacked()
        {
            if (!string.IsNullOrEmpty(_keys))
            {
                _keys = _keys.Substring(0, _keys.Length - 1);
            }
            ShowCharacter();
        }

        private void KeyboardHook_OnSpaced(int choose)
        {
            try
            {
                if (FDHelper.Instance.ContainsKey(_keys))
                {
                    if (choose > 0)
                    {
                        choose--;
                    }
                    Program.keyboardHook.Send(FDHelper.Instance.Get(_keys)[choose]);
                    lblTip.Text = "";
                    lvItems.Clear();
                }
            }
            catch
            {

            }
            _keys = string.Empty;
        }

        private void KeyboardHook_KeyUpEvent(object sender, KeyEventArgs e)
        {
            _keys += e.KeyCode.ToString().ToLower();
            ShowCharacter();
            //File.AppendAllText($"{Application.StartupPath}/log.log", $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] keys:{_keys},keyCode:{e.KeyCode}");
        }

        private void ShowCharacter()
        {
            lvItems.BeginInvoke(new Action(() =>
            {
                lblTip.Text = _keys;
                try
                {
                    var arr = FDHelper.Instance.Get(_keys).ToList().Skip((_pageIndex - 1) * 9).Take(9).ToArray();
                    if (arr != null && arr.Any())
                    {
                        lvItems.Items.Clear();
                        for (int i = 0; i < arr.Count(); ++i)
                        {
                            lvItems.Items.Add($"{(i + 1)}、{arr[i]}");
                        }
                    }
                    else
                    {
                        _pageIndex--;
                    }
                }
                catch
                {
                    lblTip.Text = _keys = "";
                    lvItems.Clear();
                }
            }));
        }
    }
}
