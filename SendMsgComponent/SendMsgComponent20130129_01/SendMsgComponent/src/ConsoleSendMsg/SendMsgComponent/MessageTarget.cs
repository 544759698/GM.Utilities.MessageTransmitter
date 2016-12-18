using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 消息目标
    /// </summary>
    internal class MessageTarget
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
        /// 系统状态
        /// </summary>
        public bool Status { get; set; }
    }
}
