using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 注册上下文
    /// </summary>
    public class MessageContext
    {
        /// <summary>
        /// 发送地址
        /// </summary>
        public string[] Address { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 队列大小
        /// </summary>
        public int QueueSize { get; set; }

        public MessageContext()
        {
            this.QueueSize = 8;//默认为8
        }
    }
}
