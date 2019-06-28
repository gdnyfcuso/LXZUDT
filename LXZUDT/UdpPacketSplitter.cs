/**************************************************************************
模    块:             

项    目:       LXZUDT

机    器:       EDZ-20171016QLJ

登 录 名：      Edianzu

作    者：      李行周

创建时间:       2019-06-27 10:59:52

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
    /// <summary>
    /// UDP数据包分割器
    /// </summary>
    public static class UdpPacketSplitter
    {


        public static ICollection<UdpPacket> Split(Msg message)
        {
            byte[] datagram = null;
            try
            {
                datagram = SerializationUnit.SerializeObject(message);
            }
            catch (Exception e)
            {
                //AddTalkMessage("数据转型异常");
            }
            //产生一个序列号，用来标识包数据属于哪一组
            Random Rd = new Random();
            long SequenceNumber = Rd.Next(88888, 999999);
            ICollection<UdpPacket> udpPackets = UdpPacketSplitter.Split(SequenceNumber, datagram, 10240, message.destinationIP, message.port);

            return udpPackets;
        }
        /// <summary>
        /// 分割UDP数据包
        /// </summary>
        /// <param name="sequence">UDP数据包所持有的序号</param>
        /// <param name="datagram">被分割的UDP数据包</param>
        /// <param name="chunkLength">分割块的长度</param>
        /// <returns>
        /// 分割后的UDP数据包列表
        /// </returns>
        public static ICollection<UdpPacket> Split(long sequence, byte[] datagram, int chunkLength, string desip, int port)
        {
            if (datagram == null)
                throw new ArgumentNullException("datagram");

            List<UdpPacket> packets = new List<UdpPacket>();

            int chunks = datagram.Length / chunkLength;
            int remainder = datagram.Length % chunkLength;
            int total = chunks;
            if (remainder > 0) total++;

            for (int i = 1; i <= chunks; i++)
            {
                byte[] chunk = new byte[chunkLength];
                Buffer.BlockCopy(datagram, (i - 1) * chunkLength, chunk, 0, chunkLength);
                packets.Add(new UdpPacket(sequence, total, i, chunk, chunkLength, remainder, desip, port));
            }
            if (remainder > 0)
            {
                int length = datagram.Length - (chunkLength * chunks);
                byte[] chunk = new byte[length];
                Buffer.BlockCopy(datagram, chunkLength * chunks, chunk, 0, length);
                packets.Add(new UdpPacket(sequence, total, total, chunk, chunkLength, remainder, desip, port));
            }

            return packets;
        }
    }
}
