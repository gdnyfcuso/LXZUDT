/**************************************************************************
模    块:             

项    目:       LXZUDT

机    器:       EDZ-20171016QLJ

登 录 名：      Edianzu

作    者：      李行周

创建时间:       2019-06-27 10:59:07

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
    //封装消息类
    [Serializable]
    public class Msg
    {
        //所属用户的用户名
        public string name { get; set; }

        //所属用户的ip
        public string host { get; set; }

        //命令的名称
        public string command { get; set; }

        //收信人的姓名
        public string desname { get; set; }

        //你所发送的消息的目的地ip,应该是对应在服务器的列表里的主键值
        public string destinationIP { get; set; }

        //端口号
        public int port { get; set; }

        //文本消息
        public string msg { get; set; }

        //二进制消息
        public byte[] byte_msg { get; set; }

        //附加数据
        public string extend_msg { get; set; }

        //时间戳
        public DateTime time { get; set; }

        //构造函数
        public Msg(string command, string desip, string msg, string host)
        {
            this.command = command;
            this.destinationIP = desip;
            this.msg = msg;
            this.time = DateTime.Now;
            this.host = host;
        }
        override
        public string ToString()
        {
            return name + "说:\r\n" + msg;
        }
    }
}
