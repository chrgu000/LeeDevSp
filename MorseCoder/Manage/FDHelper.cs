using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MorseCoder.Manage
{
    public class FDHelper
    {
        private static readonly IDictionary<char, string> _morseDic = new Dictionary<char, string>
        {
            {'A',".-" },{'B',"-..." },{'C',"-.-." },{'D',"-.." },{'E',"." },{'F',"..-." },{'G',"--." },
            {'H',"...." },{'I',".." },{'J',".---" },{'K',"-.-" },{'L',".-.." },{'M',"--" },{'N',"-." },
            {'O',"---" },{'P',".--." },{'Q',"--.-" },{'R',".-." },{'S',"..." },{'T',"-" },{'U',"..-" },
            {'V',"...-" },{'W',".--" },{'X',"-..-" },{'Y',"-.--" },{'Z',"--.." },{'0',"-----" },{'1',".----" },
            {'2',"..---" },{'3',"...--" },{'4',"....-" },{'5',"....." },{'6',"-...." },{'7',"--..." },{'8',"---.." },
            {'9',"----." },{'.',".-.-.-" },{':',"---..." },{',',"--..--" },{';',"-.-.-." },{'?',"..--.." },{'=',"-...-" },
            {'\'',".----." },{'/',"-..-." },{'!',"-.-.--" },{'-',"-....-" },{'_',"..--.-" },{'"',".-..-." },{'(',"-.--." },
            {')',"-.--.-" },{'$',"...-..-" },{'&',".-..." },{'@',".--.-." },{'+',".-.-." }
        };

        private static readonly IDictionary<string, char> _englishDic;

        private static readonly char[] _englishSpliter = { '.', '!', '?', ';' };
        private static readonly SoundPlayer _soundDas, _soundDot;

        private static FDHelper _instance = new FDHelper();
        private FDHelper() { }

        static FDHelper()
        {
            _englishDic = new Dictionary<string, char>();
            foreach (var key in _morseDic.Keys)
            {
                _englishDic.Add(_morseDic[key], key);
            }
            _soundDas = new SoundPlayer(Properties.Resources.das); // 295ms
            _soundDot = new SoundPlayer(Properties.Resources.dot); // 221ms
            //WaveInfo waveInfo = new WaveInfo(Properties.Resources.dot);
        }
        
        public void GetTime()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _soundDas.PlaySync();
            stopwatch.Stop();
            MsgShow(stopwatch.ElapsedMilliseconds.ToString());
        }

        public static FDHelper Instance
        {
            get { return _instance; }
        }

        public bool TryGetMorseCode(char ch, out string code)
        {
            return _morseDic.TryGetValue(ch, out code);
        }

        public bool TryGetChar(string morseCode, out char ch)
        {
            return _englishDic.TryGetValue(morseCode, out ch);
        }

        public string ToMorseCode(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            var words = source.Trim().ToUpper().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < words.Length; ++i)
            {
                var word = words[i];
                if (string.IsNullOrEmpty(word))
                    continue;
                var chrs = word.ToArray();
                for (int j = 0; j < chrs.Length; ++j)
                {
                    if (TryGetMorseCode(chrs[j], out string code))
                    {
                        sb.AppendFormat("{0} ", code);
                    }
                }
                if (i != words.Length - 1)
                {
                    sb.Append("/");
                }
            }
            return sb.ToString();
        }

        public string ToEnglish(string source, bool isSentence)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            var words = source.Trim().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < words.Length; ++i)
            {
                var word = words[i];
                if (string.IsNullOrEmpty(word))
                    continue;
                var codes = word.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < codes.Length; ++j)
                {
                    if (TryGetChar(codes[j], out char ch))
                    {
                        sb.Append(ch);
                    }
                }
                if (i != words.Length - 1)
                {
                    sb.Append(" ");
                }
            }
            if (isSentence)
            {
                string str = sb.ToString();
                if (string.IsNullOrEmpty(str))
                    return str;
                int index;
                IDictionary<int, char> spliterInfo = new SortedDictionary<int, char>();
                for (int i = 0; i < _englishSpliter.Length; ++i)
                {
                    index = 0;
                    while ((index = str.IndexOf(_englishSpliter[i], index)) != -1)
                    {
                        spliterInfo.Add(index, _englishSpliter[i]);
                        index++;
                    }
                }
                var sentens = str.Split(_englishSpliter, StringSplitOptions.RemoveEmptyEntries);
                sb.Clear();
                for (int i = 0; i < sentens.Length; ++i)
                {
                    sb.Append(ToWord(sentens[i]));
                    index = sb.Length;
                    if (spliterInfo.ContainsKey(index))
                    {
                        sb.Append(spliterInfo[index]);
                    }
                }
            }
            //else
            //{
            //    string str = sb.ToString();
            //    sb.Clear();
            //    var res = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //    for (int i = 0; i < res.Length; ++i)
            //    {
            //        var w = res[i];
            //        sb.Append(ToWord(w));
            //        if (i != res.Length - 1)
            //        {
            //            sb.Append(" ");
            //        }
            //    }
            //}
            return sb.ToString();
        }

        public void Play(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return;
            var chrs = str.Trim().ToArray();
            char ch;
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < chrs.Length; ++i)
                {
                    ch = chrs[i];
                    switch (ch)
                    {
                        case ' ':
                            Thread.Sleep(100); break;
                        case '/':
                            Thread.Sleep(100); break;
                        case '.':
                            _soundDot.PlaySync();
                            break;
                        case '-':
                            _soundDas.PlaySync();
                            break;
                    }
                }
            });
        }

        private string ToWord(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Trim().Length < 2)
                return str;
            return $"{ str.Substring(0, 1).ToUpper()}{str.Substring(1).ToLower()}";
        }

        public static MessageBoxResult MsgShow(string msg)
        {
            return MessageBox.Show(msg, "温馨提示");
        }
    }
}