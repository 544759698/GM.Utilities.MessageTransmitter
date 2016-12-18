using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 消息标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class MessageHeaderAttribute : Attribute
    {
        public EnumMsgType MsgType { get; set; }
        public String Version { get; set; }
    }
}
