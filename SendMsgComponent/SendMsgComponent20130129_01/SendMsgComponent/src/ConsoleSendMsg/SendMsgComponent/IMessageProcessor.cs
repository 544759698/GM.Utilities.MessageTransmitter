using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 消息处理接口
    /// </summary>
    public interface IMessageProcessor
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="interval">发送间隔(单位:毫秒)</param>
        void Init(int port, int interval);
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="context"></param>
        void Register(MessageContext context);
        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="imessage"></param>
        void AddMessage(string key, IMessage imessage);
        /// <summary>
        /// 设置目标地址
        /// </summary>
        /// <param name="key"></param>
        /// <param name="address"></param>
        void SetTargetAddress(string key, string[] address);
        /// <summary>
        /// 设置目标状态
        /// </summary>
        /// <param name="key"></param>
        /// <param name="status"></param>
        void SetTargetStatus(string key, bool status);
        /// <summary>
        /// 设置UDP发送端口
        /// </summary>
        /// <param name="port"></param>
        void SetUdpPort(int port);
    }
}
