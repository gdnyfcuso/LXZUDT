using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections;

namespace LXZUDT
{
    public class UDPThread
    {
        #region 私有变量

        public UdpClient client;//UDP客户端

        List<UdpPacket> sendlist;// 用于轮询是否发送成功的记录

        Dictionary<long, RecDataList> RecListDic = new Dictionary<long, RecDataList>();//数据接收列表,每一个sequence对应一个

        IPEndPoint remotIpEnd = null;//用来在接收数据的时候对远程主机的信息存放

        #endregion

        #region 属性
        public int CheckQueueTimeInterval { get; set; }//检查发送队列间隔

        public int MaxResendTimes { get; set; }//没有收到确认包时，最大重新发送的数目，超过此数目会丢弃并触发PackageSendFailture事件
        #endregion

        #region 事件

        /// <summary>
        /// 当数据包收到时触发
        /// </summary>
        public event EventHandler<PackageEventArgs> PackageReceived;

        public event Action<Msg, IPEndPoint> ReceivedOk;
        /// <summary>
        /// 当数据包收到事件触发时，被调用
        /// </summary>
        /// <param name="e">包含事件的参数</param>
        protected virtual void OnPackageReceived(PackageEventArgs e)
        {
            if (PackageReceived != null)
                PackageReceived(this, e);
        }

        /// <summary>
        /// 数据包发送失败
        /// </summary>
        public event EventHandler<PackageEventArgs> PackageSendFailure;
        /// <summary>
        /// 当数据发送失败时调用
        /// </summary>
        /// <param name="e">包含事件的参数</param>
        protected virtual void OnPackageSendFailure(PackageEventArgs e)
        {
            if (PackageSendFailure != null)
                PackageSendFailure(this, e);
        }

        /// <summary>
        /// 数据包未接收到确认，重新发送
        /// </summary>
        public event EventHandler<PackageEventArgs> PackageResend;
        /// <summary>
        /// 触发重新发送事件
        /// </summary>
        /// <param name="e">包含事件的参数</param>
        protected virtual void OnPackageResend(PackageEventArgs e)
        {
            if (PackageResend != null)
                PackageResend(this, e);
        }
        #endregion


        /// <summary>
        /// 发函数用这个
        /// </summary>
        public UDPThread()
        {
            client = new UdpClient();
            sendlist = new List<UdpPacket>();
        }
        /// <summary>
        /// 构造函数,接收函数用这个
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="port"></param>
        public UDPThread(string ipaddress, int port)
        {
            IPAddress ipA = IPAddress.Parse(ipaddress);//构造远程连接的参数
            IPEndPoint ipEnd = new IPEndPoint(ipA, port);
            client = new UdpClient(ipEnd);//这样的话就没有创建远程连接
            //client.Connect(ipEnd);//使用指定的远程主机信息建立默认远程主机连接
            sendlist = new List<UdpPacket>();

            CheckQueueTimeInterval = 2000;//轮询间隔时间
            MaxResendTimes = 5;//最大发送次数

            new Thread(new ThreadStart(CheckUnConfirmedQueue)) { IsBackground = true }.Start();//启动轮询线程
            //开始监听数据
            AsyncReceiveData();
        }
        /// <summary>
        /// 同步数据接收方法
        /// </summary>
        public void ReceiveData()
        {
            while (true)
            {
                IPEndPoint retip = null;
                UdpPacket udpp = null;
                try
                {
                    byte[] data = client.Receive(ref retip);//接收数据,当Client端连接主机的时候，retip就变成Cilent端的IP了
                    udpp = (UdpPacket)SerializationUnit.DeserializeObject(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //异常处理操作
                }
                if (udpp != null)
                {
                    if (!RecListDic.ContainsKey(udpp.sequence))
                    {
                        RecListDic.Add(udpp.sequence, new RecDataList(udpp));
                        RecDataList rdl = RecListDic[udpp.sequence];
                        rdl.addPacket(udpp);
                    }
                    else
                    {
                        RecDataList rdl = RecListDic[udpp.sequence];
                        rdl.addPacket(udpp);

                    }
                    foreach (KeyValuePair<long, RecDataList> ss in RecListDic)//循环看看是否有哪一个满了
                    {
                        Msg m = ss.Value.show();
                        if (m != null)
                        {
                            if (ReceivedOk != null)
                                ReceivedOk(m, retip);
                            RecListDic.Remove(ss.Value.sequence);
                            break;
                        }
                    }

                    PackageEventArgs arg = new PackageEventArgs(udpp, retip);
                    OnPackageReceived(arg);//数据包收到触发事件
                }
            }
        }

        //异步接受数据
        public void AsyncReceiveData()
        {
            try
            {
                client.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            }
            catch (SocketException ex)
            {
                throw ex;
            }
        }
        //接收数据的回调函数
        public void ReceiveCallback(IAsyncResult param)
        {
            if (param.IsCompleted)
            {
                UdpPacket udpp = null;
                try
                {
                    byte[] data = client.EndReceive(param, ref remotIpEnd);//接收数据,当Client端连接主机的时候，test就变成Cilent端的IP了
                    udpp = (UdpPacket)SerializationUnit.DeserializeObject(data);
                }
                catch (Exception ex)
                {
                    //异常处理操作
                }
                finally
                {
                    AsyncReceiveData();
                }
                if (udpp != null)//触发数据包收到事件
                {

                    if (!RecListDic.ContainsKey(udpp.sequence))
                    {
                        RecListDic.Add(udpp.sequence, new RecDataList(udpp));
                        RecDataList rdl = RecListDic[udpp.sequence];
                        rdl.addPacket(udpp);
                    }
                    else
                    {
                        RecDataList rdl = RecListDic[udpp.sequence];
                        rdl.addPacket(udpp);

                    }
                    foreach (KeyValuePair<long, RecDataList> ss in RecListDic)//循环看看是否有哪一个满了
                    {
                        Msg m = ss.Value.show();
                        if (m != null)
                        {
                            if (ReceivedOk != null)
                                ReceivedOk(m, remotIpEnd);
                            RecListDic.Remove(ss.Value.sequence);
                            break;
                        }
                    }

                    PackageEventArgs arg = new PackageEventArgs(udpp, remotIpEnd);
                    OnPackageReceived(arg);
                }
            }
        }


        /// <summary>
        /// 同步发送分包数据
        /// </summary>
        /// <param name="message"></param>
        public void SendData(Msg message)
        {

            ICollection<UdpPacket> udpPackets = UdpPacketSplitter.Split(message);
            foreach (UdpPacket udpPacket in udpPackets)
            {
                byte[] udpPacketDatagram = SerializationUnit.SerializeObject(udpPacket);
                this.client.Connect(udpPacket.remoteip);
                //使用同步发送
                client.Send(udpPacketDatagram, udpPacketDatagram.Length, udpPacket.remoteip);
                if (udpPacket.IsRequireReceiveCheck)
                    PushSendItemToList(udpPacket);//将该消息压入列表

            }
        }

        /// <summary>
        /// 异步分包发送数组的方法
        /// </summary>
        /// <param name="message"></param>
        public void AsyncSendData(Msg message)
        {

            ICollection<UdpPacket> udpPackets = UdpPacketSplitter.Split(message);
            this.client.Connect(udpPackets.FirstOrDefault().remoteip);

            foreach (UdpPacket udpPacket in udpPackets)
            {
                byte[] udpPacketDatagram = SerializationUnit.SerializeObject(udpPacket);

                //使用同步发送
                //client.Send(udpPacketDatagram, udpPacketDatagram.Length);

                //使用异步的方法发送数据
                this.client.BeginSend(udpPacketDatagram, udpPacketDatagram.Length, new AsyncCallback(SendCallback), null);
                //Thread.Sleep(10);
            }
        }
        //发送完成后的回调方法
        public void SendCallback(IAsyncResult param)
        {
            if (param.IsCompleted)
            {
                try
                {
                    client.EndSend(param);//这句话必须得写，BeginSend（）和EndSend（）是成对出现的 
                }
                catch (Exception e)
                {
                    throw new Exception(param.IsCompleted.ToString());
                    //其他处理异常的操作
                }
            }

        }
        static object lockObj = new object();
        /// <summary>
        /// 自由线程，检测未发送的数据并发出,存在其中的就是没有收到确认包的数据包
        /// </summary>
        void CheckUnConfirmedQueue()
        {
            do
            {
                if (sendlist.Count > 0)
                {
                    UdpPacket[] array = null;

                    lock (sendlist)
                    {
                        array = sendlist.ToArray();
                    }
                    //挨个重新发送并计数
                    Array.ForEach(array, s =>
                    {
                        s.sendtimes++;
                        if (s.sendtimes >= MaxResendTimes)
                        {
                            //sOnPackageSendFailure//出发发送失败事件
                            sendlist.Remove(s);//移除该包
                        }
                        else
                        {
                            //重新发送
                            byte[] udpPacketDatagram = SerializationUnit.SerializeObject(s);
                            client.Send(udpPacketDatagram, udpPacketDatagram.Length, s.remoteip);
                        }
                    });
                }

                Thread.Sleep(CheckQueueTimeInterval);//间隔一定时间重发数据
            } while (true);
        }
        /// <summary>
        /// 将数据信息压入列表
        /// </summary>
        /// <param name="item"></param>
        void PushSendItemToList(UdpPacket item)
        {
            sendlist.Add(item);
        }
        /// <summary>
        /// 将数据包从列表中移除
        /// </summary>
        /// <param name="packageNo">数据包编号</param>
        /// <param name="packageIndex">数据包分包索引</param>
        public void PopSendItemFromList(long packageNo, int packageIndex)
        {
            lock (lockObj)
            {
                Array.ForEach(sendlist.Where(s => s.sequence == packageNo && s.index == packageIndex).ToArray(), s => sendlist.Remove(s));
            }
        }
        /// <summary>
        /// 关闭客户端并释放资源
        /// </summary>
        public void Dispose()
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }
    }
}
