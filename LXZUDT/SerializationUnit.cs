/**************************************************************************
模    块:             

项    目:       LXZUDT

机    器:       EDZ-20171016QLJ

登 录 名：      Edianzu

作    者：      李行周

创建时间:       2019-06-27 11:00:59

Q Q     ：      910428123

微信    ：      lzq910428123 

clrversion :    4.0.30319.42000

描    述：

***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LXZUDT
{
    public class SerializationUnit
    {
        /// <summary>  
        /// 把对象序列化为字节数组  
        /// </summary>  
        public static byte[] SerializeObject(object obj)
        {
            if (obj == null)
                return null;
            //内存实例
            MemoryStream ms = new MemoryStream();
            //创建序列化的实例
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);//序列化对象，写入ms流中  
            ms.Position = 0;
            //byte[] bytes = new byte[ms.Length];//这个有错误
            byte[] bytes = ms.GetBuffer();
            ms.Read(bytes, 0, bytes.Length);
            ms.Close();
            return bytes;
        }

        /// <summary>  
        /// 把字节数组反序列化成对象  
        /// </summary>  
        public static object DeserializeObject(byte[] bytes)
        {
            object obj = null;
            if (bytes == null)
                return obj;
            //利用传来的byte[]创建一个内存流
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            obj = formatter.Deserialize(ms);//把内存流反序列成对象  
            ms.Close();
            return obj;
        }
        /// <summary>
        /// 把字典序列化
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static byte[] SerializeDic(Dictionary<string, object> dic)
        {
            if (dic.Count == 0)
                return null;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, dic);//把字典序列化成流

            byte[] bytes = new byte[ms.Length];//从流中读出byte[]
            ms.Read(bytes, 0, bytes.Length);

            return bytes;
        }
        /// <summary>
        /// 反序列化返回字典
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Dictionary<string, object> DeserializeDic(byte[] bytes)
        {
            Dictionary<string, object> dic = null;
            if (bytes == null)
                return dic;
            //利用传来的byte[]创建一个内存流
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            //把流中转换为Dictionary
            dic = (Dictionary<string, object>)formatter.Deserialize(ms);
            return dic;
        }
    }
}
