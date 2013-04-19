using System;
using System.Collections.Generic;
using System.Text;

namespace GM.Utilities
{
    public class MessageFactory
    {
        public static IMessageProcessor GetInstance()
        {
            return new MessageProcessor();
        }
    }
}
