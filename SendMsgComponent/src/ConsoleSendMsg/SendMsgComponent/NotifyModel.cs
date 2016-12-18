using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 对回调函数和回调数据的封装
    /// </summary>
    class NotifyModel
    {
        /// <summary>
        /// 回调数据
        /// </summary>
        public NotifyContext ModelContext { get; set; }
        /// <summary>
        /// 回调函数
        /// </summary>
        public ReceiveProcessor.NotifyHandler ModelHandler { get; set; }

    }
}
