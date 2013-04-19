using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace GM.Utilities
{
    /// <summary>
    /// 消息工厂
    /// </summary>
    public class MessageFactory
    {
        /// <summary>
        /// 程序及名称
        /// </summary>
        private static readonly string AssemblyName = "GM.Utilities.MessageTransmitter";
        /// <summary>
        /// 消息序列化类字典
        /// </summary>
        private static Dictionary<EnumMsgType, IMessageSerialize> _serializeDic = new Dictionary<EnumMsgType, IMessageSerialize>();

        static MessageFactory()
        {
            try
            {
                var interfaceType = typeof(IMessageSerialize);
                var assembly = Assembly.Load(AssemblyName);
                var asmtypes = assembly.GetTypes();
                foreach (var asmtype in asmtypes)
                {
                    //是否继承接口IMessageSerialize
                    if (interfaceType.IsAssignableFrom(asmtype))
                    {
                        //获取MessageHeaderAttribute
                        var attributes = asmtype.GetCustomAttributes(typeof(MessageHeaderAttribute), false);
                        if (attributes.Length > 0)
                        {
                            MessageHeaderAttribute myAttribute = attributes[0] as MessageHeaderAttribute;
                            if (myAttribute != null)
                            {
                                _serializeDic[myAttribute.MsgType] = (IMessageSerialize)Activator.CreateInstance(asmtype);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.MessageFactoryStatic, "执行消息工厂静态构造函数时出现异常", ex);
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static byte[] Serialize(IMessage message)
        {
            string package = string.Empty;
            try
            {
                if (_serializeDic.ContainsKey(message.MsgTag.MsgType))
                {
                    package = _serializeDic[message.MsgTag.MsgType].Serialize(message);
                    Logger.WriteInfoFmt(Log.Serialize, "消息内容：{0}", new object[] { package });
                }
                else
                {
                    throw new ApplicationException(string.Format("未找到消息类型{0}的序列化方法", message.MsgTag.MsgType));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.Serialize, "消息序列化时出现异常", ex);
            }
            return Encoding.Default.GetBytes(package);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="iep"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static NotifyContext Deserialize(IPEndPoint iep, string content)
        {
            var notify = new NotifyContext
            {
                MsgSender = new SenderInfo { Ip = iep.Address.ToString(), Port = iep.Port, SendTime = DateTime.Now }
            };

            try
            {
                string[] contentArr = content.Split(new[] { ',' });
                if (contentArr.Length < 3)
                {
                    throw new ApplicationException("消息解析长度小于3，消息内容：" + content);
                }
                var msgType = (EnumMsgType)Convert.ToInt32(contentArr[0].Substring(contentArr[0].IndexOf('=') + 1));
                string msgBody = contentArr[contentArr.Length - 1].Substring(contentArr[0].IndexOf('=') + 1);
                if (_serializeDic.ContainsKey(msgType))
                {
                    notify.Msg = _serializeDic[msgType].Deserialize(msgType, contentArr[1].Substring(contentArr[1].IndexOf('=') + 1), msgBody);
                    Logger.WriteInfoFmt(Log.Deserialize, "消息类型：{0}，消息内容：{1}", new object[] { msgType.ToString(), content });
                }
                else
                {
                    throw new ApplicationException(string.Format("未找到消息类型{0}的反序列化方法", msgType));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(Log.Deserialize, "消息反序列化时出现异常", ex);
            }
            return notify;
        }

        /// <summary>
        /// 获取消息发送器实例
        /// </summary>
        /// <returns></returns>
        public static IMessageProcessor GetInstance()
        {
            return new MessageProcessor();
        }
    }
}
