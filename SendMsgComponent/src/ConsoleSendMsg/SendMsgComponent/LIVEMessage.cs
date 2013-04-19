using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// LIVE消息
    /// </summary>
    public class LIVEMessage : IMessage
    {
        public MessageHeader MsgTag { get; set; }
        public string MsgBody { get; set; }
        public LIVEMessage()
        {
            MsgTag = new MessageHeader();
            this.MsgTag.MsgType = EnumMsgType.LIVE;
            this.MsgTag.Version = "1.0";
        }
    }
}
