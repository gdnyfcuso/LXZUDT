/**************************************************************************
模    块:             

项    目:       LXZUDT

机    器:       EDZ-20171016QLJ

登 录 名：      Edianzu

作    者：      李行周

创建时间:       2019-06-27 11:00:40

Q Q     ：      910428123

微信    ：      lzq910428123 

clrversion :    4.0.30319.42000

描    述：

***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXZUDT
{
    public class EncodingTool
    {
        //编码
        public static byte[] EncodingASCII(string buf)
        {
            byte[] data = Encoding.Unicode.GetBytes(buf);
            return data;
        }
        //解码
        public static string DecodingASCII(byte[] bt)
        {
            string st = Encoding.Unicode.GetString(bt);
            return st;
        }



        //编码
        public static byte[] EncodingUTF_8(string buf)
        {
            byte[] data = Encoding.UTF8.GetBytes(buf);
            return data;
        }
        //编码
        public static string DecodingUTF_8(byte[] bt)
        {
            string st = Encoding.UTF8.GetString(bt);
            return st;
        }

    }
}
