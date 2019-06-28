/**************************************************************************
模    块:             

项    目:       LXZUDT

机    器:       EDZ-20171016QLJ

登 录 名：      Edianzu

作    者：      李行周

创建时间:       2019-06-27 10:59:32

Q Q     ：      910428123

微信    ：      lzq910428123 

clrversion :    4.0.30319.42000

描    述：

***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace LXZUDT
{
    [Serializable]
    public class UdpPacket
    {
        public long sequence { get; set; }//所属组的唯一序列号 包编号
        public int total { get; set; }//分包总数
        public int index { get; set; }//消息包的索引
        public byte[] data { get; set; }//包的内容数组
        public int dataLength { get; set; }//分割的数组包大小
        public int remainder { get; set; }//最后剩余的数组的数据长度
        public int sendtimes { get; set; }//发送次数
        public IPEndPoint remoteip { get; set; }//接受该包的远程地址
        public bool IsRequireReceiveCheck { get; set; }//获得或设置包收到时是否需要返回确认包
        public static int HeaderSize = 30000;
        public UdpPacket(long sequence, int total, int index, byte[] data, int dataLength, int remainder, string desip, int port)
        {
            this.sequence = sequence;
            this.total = total;
            this.index = index;
            this.data = data;
            this.dataLength = dataLength;
            this.remainder = remainder;
            this.IsRequireReceiveCheck = true;//默认都需要确认包
            //构造远程地址
            IPAddress ipA = IPAddress.Parse(desip);
            this.remoteip = new IPEndPoint(ipA, port);
        }
        //把这个对象生成byte[]
        public byte[] ToArray()
        {
            return SerializationUnit.SerializeObject(this);
        }
    }
}
