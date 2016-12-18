using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 接收消息时，消息发送者的信息123
    /// </summary>
    public class SenderInfo
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public DateTime SendTime { get; set; }
    }
}
