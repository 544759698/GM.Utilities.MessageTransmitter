using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleReadMsg
{
    class Program
    {
        static void Main(string[] args)
        {
            Start("10.130.36.225", 8013);
            Console.ReadKey();
        }

        /// <summary>
        /// 监听Ip和Port
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public static void Start(string ip, int port)
        {
            try
            {
                IPEndPoint point = new IPEndPoint(IPAddress.Any, 0);
                point.Port = port;

                if (string.IsNullOrEmpty(ip) == false)
                    point.Address = IPAddress.Parse(ip);
                //Ip和Port绑定到Socket
                uint SIO_UDP_CONNRESET = 0x80000000 | 0x18000000 | 12;
                Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                udpSocket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                udpSocket.Bind(point);

                Thread udpMonitor = new Thread(ReceiveUdpMsg);
                udpMonitor.Start(udpSocket);
                Console.WriteLine(string.Format("开始监听Ip：{0}，Port：{1}", ip, port));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void ReceiveUdpMsg(object data)
        {
            try
            {
                //传入Socket
                Socket udpSocket = (Socket)data;
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, 0);
                EndPoint ep = (EndPoint)iep;
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int recv = udpSocket.ReceiveFrom(buffer, ref ep);
                    string blockStr = Encoding.UTF8.GetString(buffer, 0, recv);
                    Console.WriteLine(string.Format("{0}  接收到{1}：{2}发来的消息：{3}", DateTime.Now.ToString(), ((IPEndPoint)ep).Address, ((IPEndPoint)ep).Port, blockStr));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
