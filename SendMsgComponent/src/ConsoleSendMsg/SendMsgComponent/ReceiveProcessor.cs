using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GM.Utilities
{
    /// <summary>
    /// 消息接收器
    /// </summary>
    public class ReceiveProcessor : IReceiveProcessor
    {
        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="context"></param>
        public delegate void NotifyHandler(NotifyContext context);
        /// <summary>
        /// 各类型消息回调函数字典
        /// </summary>
        private Dictionary<EnumMsgType, List<NotifyHandler>> _callbackDic = new Dictionary<EnumMsgType, List<NotifyHandler>>();
        /// <summary>
        /// 回调队列
        /// </summary>
        private static Queue _callbackQueue = new Queue();
        /// <summary>
        /// 执行回调函数的线程
        /// </summary>
        private static Thread _callbackThread = new Thread(CallbackWorker);
        /// <summary>
        /// 执行回调时间间隔
        /// </summary>
        private static int _callbackInterval = 500;

        private static ReceiveProcessor _instance;
        private static readonly object _sync = new object();

        private ReceiveProcessor() { }
        /// <summary>
        /// 获取消息接收器实例
        /// </summary>
        /// <returns></returns>
        public static ReceiveProcessor GetInstance()
        {
            if (_instance == null)
            {
                lock (_sync)
                {
                    if (_instance == null)
                    {
                        _instance = new ReceiveProcessor();
                        _callbackThread.Start();
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        /// 注册相应类型的回调
        /// </summary>
        /// <param name="handler">回调函数</param>
        /// <param name="msgType">消息类型</param>
        public void Register(NotifyHandler handler, EnumMsgType msgType)
        {
            try
            {
                //判断消息类型是否存在,为消息类型添加回调函数   
                if (_callbackDic.ContainsKey(msgType))
                {
                    _callbackDic[msgType].Add(handler);
                }
                else
                {
                    _callbackDic[msgType] = new List<NotifyHandler> { handler };
                }
                Logger.WriteInfoFmt(Log.ReceiverRegister, "消息类型：{0}", new object[] { msgType.ToString() });
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.ReceiverRegister, "注册消息接收组件时出现异常", ex);
            }
        }

        /// <summary>
        /// 监听Ip和Port
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Start(string ip, int port)
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
                Logger.WriteInfoFmt(Log.Start, "开始监听Ip：{0}，Port：{1}", new object[] { ip, port });

            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.Start, "监听Ip和Port时出现异常", ex);
            }
        }

        private void ReceiveUdpMsg(object data)
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
                    //反序列化得到NotifyContext
                    NotifyContext context = MessageFactory.Deserialize((IPEndPoint)ep, blockStr);
                    //根据MsgType将消息添加到相应的回调队列
                    List<NotifyHandler> handlers = _callbackDic[context.Msg.MsgTag.MsgType];
                    foreach (var notifyHandler in handlers)
                    {
                        _callbackQueue.Enqueue(new NotifyModel { ModelContext = context, ModelHandler = notifyHandler });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.ReceiveUdpMsg, "接收Udp消息时出现异常", ex);
            }

        }

        /// <summary>
        /// 执行回调函数的线程
        /// </summary>
        private static void CallbackWorker()
        {
            try
            {
                while (true)
                {
                    while (_callbackQueue.Count > 0)
                    {
                        NotifyModel model = (NotifyModel)_callbackQueue.Dequeue();
                        model.ModelHandler(model.ModelContext);
                    }
                    Thread.Sleep(_callbackInterval);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.CallbackWorker, "执行回调函数时出现异常", ex);
            }
        }


        public void Stop()
        {
            try
            {
                _callbackThread.Abort();
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.Stop, "停止消息接收组件时出现异常", ex);
            }
        }
    }
}
