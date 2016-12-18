using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 消息接收器接口
    /// </summary>
    public interface IReceiveProcessor
    {
        void Register(ReceiveProcessor.NotifyHandler handler, EnumMsgType msgType);
        void Start(string ip, int port);
        void Stop();
    }
}
