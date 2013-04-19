using System;
using System.Collections.Generic;
using System.Text;
using GM.Utilities;

namespace GM.Utilities
{
    class Log : LogCatalogs
    {
        public static string Message = "消息发送组件";
        public static string SenderRegister = "注册消息发送组件";
        public static string AddMessage = "添加消息";
        public static string SetTargetAddress = "设置消息目标地址";
        public static string SetTargetStatus = "设置消息目标状态";
        public static string SendUdpMsg = "发送UDP消息";
        public static string Init = "初始化消息组件";
        public static string SetUdpPort = "设置UDP发送端口";

        public static string ReceiverRegister = "注册消息接收组件";
        public static string Start = "监听Ip和Port";
        public static string ReceiveUdpMsg = "接收Udp消息";
        public static string CallbackWorker = "执行回调函数";
        public static string Stop = "停止消息接收组件";
        public static string Serialize = "消息序列化";
        public static string Deserialize = "消息反序列化";
        public static string MessageFactoryStatic = "消息工厂静态构造函数";
        static Log()
        {
            RegisteCatalogs(typeof(Log));
        }
    }
}
