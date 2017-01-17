using System;
using System.Diagnostics;
using System.Messaging;
using System.Threading;

namespace MessagingFramework.ClientA
{
    internal class Publisher
    {
        internal static void Process()
        {
            using (var helloQueue = new MessageQueue("FormatName:MULTICAST=234.1.1.1:8003"))
            {
                while (true)
                {
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    for (var i = 0; i < 1000; i++)
                    {
                        SendMessage(helloQueue,
                            string.Format("{0}: msg:{1} hello world ", DateTime.UtcNow.Ticks, i));
                    }

                    stopWatch.Stop();
                    Console.ReadLine();

                    Console.WriteLine("====================================================");
                    Console.WriteLine("[MSMQ] done sending 1000 messages in " + stopWatch.ElapsedMilliseconds);
                    Console.WriteLine("[MSMQ] Sending reset counter to consumers.");

                    SendMessage(helloQueue, "reset");
                    Console.ReadLine();
                }
            }
        }

        private static void SendMessage(MessageQueue queue, string content)
        {
            queue.Send(content);
            Console.WriteLine(" [MSMQ] Sent {0}", content);
        }
    }
}