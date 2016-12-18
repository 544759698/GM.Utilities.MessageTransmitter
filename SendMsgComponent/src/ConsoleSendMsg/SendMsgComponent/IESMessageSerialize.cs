using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// IES消息序列化类
    /// </summary>
    [MessageHeader(MsgType = EnumMsgType.IES, Version = "1.0")]
    class IESMessageSerialize : IMessageSerialize
    {
        public string Serialize(IMessage message)
        {
            var iesmsg = (IESMessage)message;
            string msgBody = string.Format("{0}|{1}", iesmsg.ID, iesmsg.DeviceID);
            //为UDP包增加报头, 以便接收方业务处理
            return string.Format("type={0},version={1},body={2}", (int)message.MsgTag.MsgType, message.MsgTag.Version, msgBody);
        }

        public IMessage Deserialize(EnumMsgType msgType, string version, string body)
        {
            string[] propArr = body.Split(new[] { '|' });
            return new IESMessage { MsgTag = { MsgType = msgType, Version = version }, ID = propArr[0], DeviceID = propArr[1] };
        }
    }
}
