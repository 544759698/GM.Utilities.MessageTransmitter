using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GM.Utilities;

namespace ConsoleSendMsg
{
    class Program
    {
        static void Main(string[] args)
        {
            IMessageProcessor processor = MessageFactory.GetInstance();
            processor.Init(10000, 3000);
            processor.Register(new MessageContext() { Key = "IES", Address = new string[] { "10.130.36.225:8013" }, QueueSize = 16 });
            processor.AddMessage("IES", new MyMessage());
            processor.AddMessage("IES", new MyMessage());
            processor.AddMessage("IES", new MyMessage());
            Thread.Sleep(3005);
            processor.SetUdpPort(11111);
            processor.AddMessage("IES", new MyMessage());
            processor.AddMessage("IES", new MyMessage());
            processor.AddMessage("IES", new MyMessage());
            Thread.Sleep(5000);
            processor.AddMessage("IES", new MyMessage());
            processor.AddMessage("IES", new MyMessage());
            processor.AddMessage("IES", new MyMessage());

            ////processor.Register(new MessageContext() { Key = "key1", Address = new string[] { "10.130.36.225:11000" }, QueueSize = 16 });
            ////processor.AddMessage("key1", new MyMessage());
            //processor.Register(new MessageContext() { Key = "key1", Address = new string[] { "10.130.36.225:10000", "10.130.36.224:10000" }, QueueSize = 16 });
            //processor.AddMessage("key1", new MyMessage());
            //Thread.Sleep(5000);
            //processor.Register(new MessageContext() { Key = "key2", Address = new string[] { "10.130.36.111:10000", "10.130.36.222:10000" }, QueueSize = 16 });
            //processor.AddMessage("key2", new MyMessage());
            //Thread.Sleep(5000);
            //Thread t = new Thread(new ParameterizedThreadStart(SetStatus1));
            //t.Start(processor);
            //Thread.Sleep(5000);
            //Console.WriteLine(DateTime.Now + " Main SetTargetStatus");
            //processor.SetTargetStatus("key1", true);
            //Console.WriteLine(DateTime.Now + " AddMessage");
            //processor.AddMessage("key2", new MyMessage());
            //processor.AddMessage("key1", new MyMessage());
            //Thread.Sleep(5000);
            //processor.SetTargetAddress("key2", new string[] { "10.130.36.100:1111" });
            //processor.SetTargetStatus("key2", true);
            //Thread.Sleep(5000);
            //Thread t2 = new Thread(new ParameterizedThreadStart(SetStatus2));
            //t2.Start(processor);
            //Thread.Sleep(5000);
            //Console.WriteLine(DateTime.Now + " Main SetTargetStatus");
            //processor.SetTargetStatus("key2", true);
            //processor.AddMessage("key2", new MyMessage());
            Console.ReadKey();
        }

        static void SetStatus1(object processor)
        {
            IMessageProcessor proc = (IMessageProcessor)processor;
            proc.SetTargetStatus("key1", false);
            proc.SetTargetStatus("key2", false);
        }

        static void SetStatus2(object processor)
        {
            IMessageProcessor proc = (IMessageProcessor)processor;
            proc.SetTargetStatus("key1", false);
            proc.SetTargetStatus("key2", false);
        }
    }

    internal class MyMessage : IMessage
    {
        public byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes("Message123456");
        }
    }
}
