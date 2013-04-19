using System;
using System.Collections.Generic;
using System.Text;
using GM.Utilities;

namespace GM.Utilities
{
    class Log : LogCatalogs
    {
        public static string Message = "消息发送组件";
        public static string Register = "注册消息发送组件";
        public static string AddMessage = "添加消息";
        public static string SetTargetAddress = "设置消息目标地址";
        public static string SetTargetStatus = "设置消息目标状态";
        public static string SendUdpMsg = "发送UDP消息";
        public static string Init = "初始化消息组件";
        public static string SetUdpPort = "设置UDP发送端口";
        static Log()
        {
            RegisteCatalogs(typeof(Log));
        }
    }
}
