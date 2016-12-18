using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// LIVE消息序列化类
    /// </summary>
    [MessageHeader(MsgType = EnumMsgType.LIVE, Version = "1.0")]
    class LIVEMessageSerialize : IMessageSerialize
    {
        public string Serialize(IMessage message)
        {
            var livemsg = (LIVEMessage)message;
            string msgBody = string.Format("{0}", livemsg.MsgBody);
            //为UDP包增加报头, 以便接收方业务处理
            return string.Format("type={0},version={1},body={2}", (int)message.MsgTag.MsgType, message.MsgTag.Version, msgBody);

        }

        public IMessage Deserialize(EnumMsgType msgType, string version, string body)
        {
            string[] propArr = body.Split(new[] { '|' });
            return new LIVEMessage { MsgTag = { MsgType = msgType, Version = version }, MsgBody = propArr[0] };
        }
    }
}
