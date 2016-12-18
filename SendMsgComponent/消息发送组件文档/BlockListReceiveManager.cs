using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using GM.Utilities;
namespace GM.Services
{
    class BlockListReceiveManager
    {
        private static BlockListReceiveManager _instance;

        private Dictionary<string,List<string>> _blockList = new Dictionary<string,List<string> >();
        

        public static BlockListReceiveManager Instance
        {
            get 
            {
                if (_instance == null)
                    _instance = new BlockListReceiveManager();

                return _instance;
            }
            
        }

        private Socket _udpSocket;
        private IPEndPoint _point = new IPEndPoint(IPAddress.Any, 0);

        private Thread _udpMonitor;

        public void Start(string ip,int port)
        {
            _point.Port = port;

            if (string.IsNullOrEmpty(ip) == false)
                _point.Address = IPAddress.Parse(ip);

            if (_udpSocket != null)
                _udpSocket.Close();

            //初始化UDP发送器
            uint SIO_UDP_CONNRESET = 0x80000000 | 0x18000000 | 12;
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _udpSocket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            _udpSocket.Bind(_point);

            _udpMonitor = new Thread(ReceiveBlockList);
            _udpMonitor.Start();
        }

        public void Start(int port)
        {
            Start("", port);
        }

        public void Stop()
        {
            try 
            {
                _udpMonitor.Abort();
            }
            catch
            {}
            _udpSocket.Close();

        }

        public bool BlockFilter(string deviceId, string id)
        {
            if (_blockList.ContainsKey(deviceId))
            {
                string r = _blockList[deviceId].Find(a => (a == id));
                return !string.IsNullOrEmpty(r);
            }
            
            return false;
        }

        private void ReceiveBlockList()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 0);
            EndPoint ep = (EndPoint)iep;
            byte[] buffer = new byte[1024];

            while (true)
            {
                int recv = _udpSocket.ReceiveFrom(buffer, ref ep);

                try
                {
                    string blockStr = Encoding.UTF8.GetString(buffer, 0, recv);

                    if (blockStr.StartsWith("redcdn,tag="))
                    {
                        UpdateBlockList(blockStr.Substring(11));

                        Logger.WriteDebugingFmt(Log.BlockManager, "接收到{0}发送的黑名单数据=>{1}", ep, blockStr);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteErrorFmt(Log.BlockManager, ex, "处理{0}发送的黑名单数据异常", ep);
                }
            }

        }

        private void UpdateBlockList(string blockStr)
        {
            try
            {
                if (string.IsNullOrEmpty(blockStr))
                    return;
                string[] serverInfo = blockStr.Split('|');
                if (serverInfo.Length >= 2)
                {
                    if(string.Equals(serverInfo[1], "null", StringComparison.CurrentCultureIgnoreCase))
                        _blockList.Remove(serverInfo[0]);
                    else
                       _blockList[serverInfo[0]] =serverInfo[1].Split(',').ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorFmt(Log.BlockManager, ex, "更新黑名单数据异常，数据={0}", blockStr);
            }
        }
    }
}
