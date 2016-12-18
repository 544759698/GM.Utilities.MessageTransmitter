using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using GM.Utilities;

namespace GM.Utilities
{
    class MessageProcessor : IMessageProcessor
    {
        private static Socket _sendSocket;
        /// <summary>
        /// 发送消息的线程
        /// </summary>
        private static Thread _sendThread = new Thread(SendMsgWorker);
        /// <summary>
        /// 消息目标字典
        /// </summary>
        private static Dictionary<string, MessageTarget> _targetInfoDic = new Dictionary<string, MessageTarget>();
        /// <summary>
        /// 消息目标队列字典
        /// </summary>
        private static Dictionary<string, Queue> _targetQueueDic = new Dictionary<string, Queue>();
        /// <summary>
        /// 同步事件
        /// </summary>
        private static ManualResetEvent _manualEvent = new ManualResetEvent(false);
        /// <summary>
        /// 发送消息线程运行间隔
        /// </summary>
        private static int _sendInterval = 3000;
        /// <summary>
        /// 发送线程是否为等待状态
        /// </summary>
        private static bool _isWaiting = false;
        /// <summary>
        /// 发送消息的socket端口
        /// </summary>
        private static int _port;

        public void Register(MessageContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(context.Key))
                {
                    throw new ApplicationException("Key为空");
                }
                if (context.Address.Length == 0)
                {
                    throw new ApplicationException("发送地址列表为空");
                }
                //添加一个消息目标
                _targetInfoDic[context.Key] = new MessageTarget { Address = context.Address, Key = context.Key, Status = true };
                //添加消息目标相应的队列
                _targetQueueDic[context.Key] = new Queue(context.QueueSize);
                //设置注册状态并判断是否唤醒线程
                SetTargetStatus(context.Key, true);
                Logger.WriteInfoFmt(Log.Register, "Key:{0},地址个数:{1},队列大小:{2}", new object[] { context.Key, context.Address.Length, context.QueueSize });
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.Message, "注册到消息发送组件时出现异常", ex);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddMessage(string key, IMessage imessage)
        {
            try
            {
                //获取消息
                byte[] msgByte = imessage.Serialize();
                //插入消息
                if (_targetQueueDic.ContainsKey(key))
                {
                    lock (_targetQueueDic[key].SyncRoot)
                    {
                        _targetQueueDic[key].Enqueue(new MessageContent { Content = msgByte, Key = key });
                    }
                }
                Logger.WriteInfoFmt(Log.AddMessage, "Key:{0},消息内容:{1}", new object[] { key, Encoding.Default.GetString(msgByte) });
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.Message, "添加消息时出现异常", ex);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetTargetAddress(string key, string[] address)
        {
            try
            {
                //更新Address
                if (_targetInfoDic.ContainsKey(key))
                {
                    _targetInfoDic[key].Address = address;
                }
                Logger.WriteInfoFmt(Log.SetTargetAddress, "Key:{0},地址个数:{1}", new object[] { key, address.Length });
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.Message, "设置消息目标发送地址时出现异常", ex);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetTargetStatus(string key, bool status)
        {
            try
            {
                //设置状态
                if (_targetInfoDic.ContainsKey(key))
                {
                    _targetInfoDic[key].Status = status;
                    //若新设置状态为true且之前所有目标状态均为false
                    if (status && _isWaiting)
                    {
                        //唤醒发送消息线程
                        _manualEvent.Set();
                        _isWaiting = false;
                        Logger.WriteDebuging(Log.Message, "Set--唤醒发送消息线程");
                    }
                }
                Logger.WriteInfoFmt(Log.SetTargetStatus, "Key:{0},状态:{1}", new object[] { key, status });
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.Message, "设置消息目标状态时出现异常", ex);
            }
        }

        private static void SendMsgWorker()
        {
            try
            {
                while (true)
                {
                    //判断是否阻塞线程
                    int unabledTarget = 0;
                    foreach (string key in new ArrayList(_targetQueueDic.Keys))
                    {
                        if (!_targetInfoDic[key].Status)
                            unabledTarget++;
                        else
                            break;
                    }
                    //若所有目标状态均为false，则阻塞
                    if (unabledTarget == _targetInfoDic.Count)
                    {
                        _isWaiting = true;
                        Logger.WriteDebuging(Log.Message, "WaitOne--阻塞发送消息线程");
                        _manualEvent.WaitOne();
                        Logger.WriteDebuging(Log.Message, "Reset--重置Event");
                        _manualEvent.Reset();
                    }
                    else
                    {
                        //遍历队列，发送消息
                        foreach (string key in new ArrayList(_targetQueueDic.Keys))
                        {
                            if (_targetInfoDic[key].Status)
                            {
                                lock (_targetQueueDic[key].SyncRoot)
                                {
                                    while (_targetQueueDic[key].Count > 0)
                                    {
                                        MessageContent msg = (MessageContent)_targetQueueDic[key].Dequeue();
                                        foreach (var addr in _targetInfoDic[key].Address)
                                        {
                                            SendUdpMsg(addr, msg.Content);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Thread.Sleep(_sendInterval);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.Message, "发送消息时出现异常", ex);
            }
        }

        private static void SendUdpMsg(string addr, byte[] content)
        {
            try
            {
                CreateSocket();
                string[] addrArr = addr.Split(new char[] { ':' });
                if (addrArr.Length == 2)
                {
                    var endPoint = new IPEndPoint(IPAddress.Parse(addrArr[0]), Convert.ToInt32(addrArr[1]));
                    uint SIO_UDP_CONNRESET = 0x80000000 | 0x18000000 | 12;
                    _sendSocket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                    _sendSocket.SendTo(content, endPoint);
                }
                Logger.WriteInfoFmt(Log.SendUdpMsg, "发送地址:{0},内容:{1}", new object[] { addr, Encoding.UTF8.GetString(content) });
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.Message, "发送UDP消息时出现异常", ex);
                _sendSocket.Close();
                _sendSocket = null;
            }
        }

        private static void CreateSocket()
        {
            if (_sendSocket == null && _port > 0)
            {
                _sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
                //绑定端口
                _sendSocket.Bind(new IPEndPoint(IPAddress.Any, _port));  //Bind Sender UDPPort
            }
        }

        public void Init(int port, int interval)
        {
            try
            {
                if (port > 0)
                    _port = port;
                else
                    throw new ApplicationException("端口号不正确，Port:" + port);
                if (interval > 0)
                    _sendInterval = interval;
                _sendThread.Start();

                Logger.WriteInfoFmt(Log.Init, "端口:{0},间隔:{1}", new object[] { port, interval });
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.Init, "初始化时出现异常", ex);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetUdpPort(int port)
        {
            try
            {
                if (port > 0)
                    _port = port;
                else
                    throw new ApplicationException("端口号不正确，Port:" + port);
                if (_sendSocket != null)
                {
                    _sendSocket.Close();
                    _sendSocket = null;
                }
                Logger.WriteInfoFmt(Log.SetUdpPort, "设置端口:{0}", new object[] { port });
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.SetUdpPort, "设置UDP发送端口时出现异常", ex);
            }
        }
    }
}
