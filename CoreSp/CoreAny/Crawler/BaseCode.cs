using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crawler
{
    public class BaseCode
    {
        string base64EncodeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        int[] base64DecodeChars = new int[] {
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
            52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
            -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
            15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
            -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
            41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1 };

        private string base64encode(string str)
        {
            //var outstr="", i=1, len=1;
            int c1, c2, c3;

            var len = str.Length;
            var i = 0;
            var outstr = "";

            while (i < len)
            {
                c1 = str[i++] & 0xff; //str.charCodeAt(i++) & 0xff;
                if (i == len)
                {
                    outstr += base64EncodeChars[(c1 >> 2)];
                    outstr += base64EncodeChars[((c1 & 0x3) << 4)];
                    outstr += "==";
                    break;
                }
                c2 = str[i++];//.charCodeAt(i++);
                if (i == len)
                {
                    outstr += base64EncodeChars[c1 >> 2];//.charAt(c1 >> 2);
                    outstr += base64EncodeChars[((c1 & 0x3) << 4) | ((c2 & 0xF0) >> 4)];//.charAt(((c1 & 0x3) << 4) | ((c2 & 0xF0) >> 4));
                    outstr += base64EncodeChars[(c2 & 0xF) << 2];//.charAt((c2 & 0xF) << 2);
                    outstr += "=";
                    break;
                }
                c3 = str[i++];//.charCodeAt(i++);
                outstr += base64EncodeChars[c1 >> 2];//.charAt(c1 >> 2);
                outstr += base64EncodeChars[((c1 & 0x3) << 4) | ((c2 & 0xF0) >> 4)];//.charAt(((c1 & 0x3) << 4) | ((c2 & 0xF0) >> 4));
                outstr += base64EncodeChars[((c2 & 0xF) << 2) | ((c3 & 0xC0) >> 6)];//.charAt(((c2 & 0xF) << 2) | ((c3 & 0xC0) >> 6));
                outstr += base64EncodeChars[c3 & 0x3F];//.charAt(c3 & 0x3F);
            }
            return outstr;
        }

        private string base64decode(string str)
        {
            int c1, c2, c3, c4;
            int i, len;
            string outstr;

            len = str.Length;
            i = 0;
            outstr = "";
            while (i < len)
            {
                /* c1 */
                do
                {
                    c1 = base64DecodeChars[str[i++] & 0xff];
                } while (i < len && c1 == -1);
                if (c1 == -1)
                    break;

                /* c2 */
                do
                {
                    c2 = base64DecodeChars[str[i++] & 0xff];
                } while (i < len && c2 == -1);
                if (c2 == -1)
                    break;

                outstr += (char)(((c1 << 2) | ((c2 & 0x30) >> 4)));

                /* c3 */
                do
                {
                    c3 = str[i++] & 0xff;
                    if (c3 == 61)
                        return outstr;
                    c3 = base64DecodeChars[c3];
                } while (i < len && c3 == -1);
                if (c3 == -1)
                    break;

                outstr += (char)(((c2 & 0XF) << 4) | ((c3 & 0x3C) >> 2)); //String.fromCharCode(((c2 & 0XF) << 4) | ((c3 & 0x3C) >> 2));

                /* c4 */
                do
                {
                    c4 = str[i++] & 0xff;
                    if (c4 == 61)
                        return outstr;
                    c4 = base64DecodeChars[c4];
                } while (i < len && c4 == -1);
                if (c4 == -1)
                    break;
                outstr += (char)(((c3 & 0x03) << 6) | c4); //String.fromCharCode(((c3 & 0x03) << 6) | c4);
            }
            return outstr;
        }

        private string utf16to8(string str)
        {
            string outstr; int i, len, c;

            outstr = "";
            len = str.Length;
            for (i = 0; i < len; i++)
            {
                c = str[i];
                if ((c >= 0x0001) && (c <= 0x007F))
                {
                    outstr += str[i];
                }
                else if (c > 0x07FF)
                {
                    outstr += (char)(0xE0 | ((c >> 12) & 0x0F));//String.fromCharCode(0xE0 | ((c >> 12) & 0x0F));
                    outstr += (char)(0x80 | ((c >> 6) & 0x3F));// String.fromCharCode(0x80 | ((c >> 6) & 0x3F));
                    outstr += (char)(0x80 | ((c >> 0) & 0x3F));//String.fromCharCode(0x80 | ((c >> 0) & 0x3F));
                }
                else
                {
                    outstr += (char)(0xC0 | ((c >> 6) & 0x1F));// String.fromCharCode(0xC0 | ((c >> 6) & 0x1F));
                    outstr += (char)(0x80 | ((c >> 0) & 0x3F)); //String.fromCharCode(0x80 | ((c >> 0) & 0x3F));
                }
            }
            return outstr;
        }

        private string utf8to16(string str)
        {
            string outstr; int i, len, c;
            char char2, char3;

            outstr = "";
            len = str.Length;
            i = 0;
            while (i < len)
            {
                c = str[i++];
                switch (c >> 4)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        // 0xxxxxxx
                        outstr += str[i - 1];
                        break;
                    case 12:
                    case 13:
                        // 110x xxxx   10xx xxxx
                        char2 = str[i++];
                        outstr += (char)(((c & 0x1F) << 6) | (char2 & 0x3F)); //String.fromCharCode(((c & 0x1F) << 6) | (char2 & 0x3F));
                        break;
                    case 14:
                        // 1110 xxxx  10xx xxxx  10xx xxxx
                        char2 = str[i++];
                        char3 = str[i++];
                        outstr += (char)(((c & 0x0F) << 12) | //String.fromCharCode(((c & 0x0F) << 12) |
                                       ((char2 & 0x3F) << 6) |
                                       ((char3 & 0x3F) << 0));
                        break;
                }
            }

            return outstr;
        }


        private string ThunderEncode(string t_url)
        {
            var thunderPrefix = "AA";
            var thunderPosix = "ZZ";
            var thunderTitle = "thunder://";

            var thunderUrl = thunderTitle + base64encode(utf16to8(thunderPrefix + t_url + thunderPosix));

            return thunderUrl;
        }

        public string GetRealLink(string arrLink)
        {
            var thunderSufix = ".asf;.avi;.iso;.mp3;.mpeg;.mpg;.mpga;.ra;.rar;.rm;.rmvb;.tar;.wma;.wmp;.wmv;.zip;.swf;.rmvb;.mp4;.3gp;.pdf;.mov;.wav;.scm;.mkv;.exe;.7z;.sub;.idx;.srt;.nfo;.bin;.aac;";
            var arrSufix = thunderSufix.Split(';');
            var temp = arrLink;
            var post = temp.LastIndexOf(".");
            var p = temp.Substring(post, temp.Length - post).ToLower();
            var k = arrSufix.Length;
            var flag = false;
            var thunder_url = arrLink;//[i].href;
            //dz锟斤拷锟斤拷
            var pathname = arrLink.Substring(arrLink.IndexOf('/', 6));
            var path = pathname.Substring(pathname.LastIndexOf("/"), pathname.Length);
            string thunderPath = "";

            if (path == thunderPath && thunderPath != "")
            {
                flag = true;
            }
            //
            for (var h = 0; h < arrSufix.Length; h++)
            {
                if (p == arrSufix[h])
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                return ThunderEncode(arrLink);
            }
            return string.Empty;
        }
    }
}