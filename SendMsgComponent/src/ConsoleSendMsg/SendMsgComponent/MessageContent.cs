using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 消息内容
    /// </summary>
    internal class MessageContent
    {
        /// <summary>
        /// 内容
        /// </summary>
        public byte[] Content { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }
    }
}
