using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 消息序列化接口
    /// </summary>
    interface IMessageSerialize
    {
        string Serialize(IMessage message);
        IMessage Deserialize(EnumMsgType msgType, string version, string body);
    }
}
