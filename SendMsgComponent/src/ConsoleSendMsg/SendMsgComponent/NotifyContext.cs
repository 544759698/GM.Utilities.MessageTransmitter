using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 回调数据
    /// </summary>
    public class NotifyContext
    {
        public IMessage Msg { get; set; }
        public SenderInfo MsgSender { get; set; }
    }
}
