using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 消息接口
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        byte[] Serialize();
    }
}
