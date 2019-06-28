/**************************************************************************
模    块:             

项    目:       LXZUDT

机    器:       EDZ-20171016QLJ

登 录 名：      Edianzu

作    者：      李行周

创建时间:       2019-06-27 11:00:09

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
    //一个sequence对应一组的数据包的数据结构
    public class RecDataList
    {
        public long sequence { get; set; }//序列号
        //对应的存储包的List
        List<UdpPacket> RecudpPackets = new List<UdpPacket>();
        public int total { get; set; }
        public int dataLength { get; set; }
        public int remainder { get; set; }
        public byte[] DataBuffer = null;
        public RecDataList(UdpPacket udp)
        {

            this.sequence = udp.sequence;
            this.total = udp.total;
            this.dataLength = udp.dataLength;
            this.remainder = udp.remainder;
            if (DataBuffer == null)
            {
                DataBuffer = new byte[dataLength * (total - 1) + remainder];
            }
        }
        public RecDataList(long sequence, int total, int chunkLength, int remainder)
        {

            this.sequence = sequence;
            this.total = total;
            this.dataLength = chunkLength;
            this.remainder = remainder;
            if (DataBuffer == null)
            {
                DataBuffer = new byte[this.dataLength * (this.total - 1) + this.remainder];
            }
        }
        public void addPacket(UdpPacket p)
        {
            RecudpPackets.Add(p);
        }
        public Msg show()
        {
            if (RecudpPackets.Count == total)//表示已经收集满了
            {
                //重组数据
                foreach (UdpPacket udpPacket in RecudpPackets)
                {
                    //偏移量
                    int offset = (udpPacket.index - 1) * udpPacket.dataLength;
                    Buffer.BlockCopy(udpPacket.data, 0, DataBuffer, offset, udpPacket.data.Length);
                }
                Msg rmsg = (Msg)SerializationUnit.DeserializeObject(DataBuffer);
                DataBuffer = null;
                RecudpPackets.Clear();
                return rmsg;
            }
            else
            {
                return null;
            }
        }
        public bool containskey(UdpPacket udp)
        {
            foreach (UdpPacket udpPacket in RecudpPackets)
            {
                if (udpPacket.index == udp.index)
                    return true;
            }
            return false;
        }
    }
}
