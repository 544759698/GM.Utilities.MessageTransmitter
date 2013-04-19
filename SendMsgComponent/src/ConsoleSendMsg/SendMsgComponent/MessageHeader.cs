using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 消息头（包括消息类型和版本）
    /// </summary>
    public class MessageHeader
    {
        public EnumMsgType MsgType { get; set; }
        public string Version { get; set; }
    }
}
