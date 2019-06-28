/**************************************************************************
模    块:             

项    目:       LXZUDT

机    器:       EDZ-20171016QLJ

登 录 名：      Edianzu

作    者：      李行周

创建时间:       2019-06-27 11:01:44

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
    /// <summary>
    /// 数据包事件数据
    /// </summary>
    public class PackageEventArgs : EventArgs
    {
        /// <summary>
        /// 网络消息包
        /// </summary>
        public UdpPacket udpPackage { get; set; }

        /// <summary>
        /// 网络消息包组
        /// </summary>
        public UdpPacket[] udpPackages { get; set; }

        /// <summary>
        /// 远程IP
        /// </summary>
        public IPEndPoint RemoteIP { get; set; }

        /// <summary>
        /// 是否已经处理
        /// </summary>
        public bool IsHandled { get; set; }

        /// <summary>
        /// 创建一个新的 PackageEventArgs 对象.
        /// </summary>
        public PackageEventArgs(UdpPacket package, IPEndPoint RemoteIP)
        {
            this.udpPackage = package;
            this.RemoteIP = RemoteIP;
            this.IsHandled = false;
        }
    }
}
