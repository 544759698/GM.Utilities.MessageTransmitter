using System;
using System.Collections.Generic;
using System.Text;
using GM.Utilities;

namespace ConsoleReceiveMsg
{
    class Program
    {
        static void Main(string[] args)
        {
            IReceiveProcessor receiver = ReceiveProcessor.GetInstance();
            receiver.Register(GetIESMsg, EnumMsgType.IES);
            receiver.Start("10.130.36.225", 8013);
            receiver.Register(GetLIVEMsg, EnumMsgType.LIVE);
            receiver.Start("10.130.36.225", 9555);
            Console.ReadKey();
        }

        public static void GetIESMsg(NotifyContext context)
        {
            IESMessage msg = (IESMessage)context.Msg;
            Console.WriteLine(string.Format("消息类型：{0}，版本：{1}，ID：{2}，DeviceID：{3}", msg.MsgTag.MsgType, msg.MsgTag.Version, msg.ID, msg.DeviceID));
            Console.WriteLine(string.Format("消息发送者Ip：{0}，Port：{1}，时间：{2}", context.MsgSender.Ip, context.MsgSender.Port, context.MsgSender.SendTime));
        }


        public static void GetLIVEMsg(NotifyContext context)
        {
            LIVEMessage msg = (LIVEMessage)context.Msg;
            Console.WriteLine(string.Format("消息类型：{0}，版本：{1}，消息Body：{2}", msg.MsgTag.MsgType, msg.MsgTag.Version, msg.MsgBody));
            Console.WriteLine(string.Format("消息发送者Ip：{0}，Port：{1}，时间：{2}", context.MsgSender.Ip, context.MsgSender.Port, context.MsgSender.SendTime));
        }
    }
}
