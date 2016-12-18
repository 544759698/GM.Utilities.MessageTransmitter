using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 消息接口
    /// </summary>
    public interface IMessage
    {
        MessageHeader MsgTag { get; set; }
    }
}
