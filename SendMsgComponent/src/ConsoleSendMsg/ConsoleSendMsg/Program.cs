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
            processor.Init(10000, 5000);
            processor.Register(new MessageContext() { Key = "IES", Address = new string[] { "10.130.36.225:9015" }, QueueSize = 16 });
            processor.AddMessage("IES", new IESMessage() { ID = "IES001", DeviceID = "Device001" });
            Thread.Sleep(5000);
            processor.Register(new MessageContext() { Key = "LIVE", Address = new string[] { "10.130.36.225:9017" }, QueueSize = 16 });
            processor.AddMessage("LIVE", new LIVEMessage() { MsgBody = "LIVE001&Device002" });
            Thread.Sleep(5000);
            processor.AddMessage("IES", new IESMessage() { ID = "IES002", DeviceID = "Device001" });
            processor.AddMessage("LIVE", new LIVEMessage() { MsgBody = "LIVE002&Device002"});
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
}
