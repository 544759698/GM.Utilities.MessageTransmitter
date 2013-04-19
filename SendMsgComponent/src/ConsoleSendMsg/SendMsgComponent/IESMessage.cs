using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// IES消息
    /// </summary>
    public class IESMessage : IMessage
    {
        public MessageHeader MsgTag { get; set; }
        public string ID { get; set; }
        public string DeviceID { get; set; }
        public IESMessage()
        {
            MsgTag = new MessageHeader();
            this.MsgTag.MsgType = EnumMsgType.IES;
            this.MsgTag.Version = "1.0";
        }
    }
}
