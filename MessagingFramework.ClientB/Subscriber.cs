using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MessagingFramework.ClientB
{
    class Subscriber
    {
        internal static void Process()
        {
            int messagesReceived = 0;
            var messages = new Queue<string>(5000);
            var filePath = typeof(Subscriber).FullName + ".txt";
            var path = @".\private$\hello-queue";

            using (var helloQueue = new MessageQueue(path))
            {
                helloQueue.MulticastAddress = "234.1.1.1:8003";
                while (true)
                {
                    var message = helloQueue.Receive();
                    if (message == null)
                        return;

                    var reader = new StreamReader(message.BodyStream);
                    var body = reader.ReadToEnd();

                    messagesReceived += 1;

                    messages.Enqueue(body);
                    Console.WriteLine(" [MSMQ] {0} Received {1}", messagesReceived, body);

                    if (string.CompareOrdinal("reset", body) == 0)
                    {
                        messagesReceived = 0;
                        File.WriteAllText(filePath, body);
                        messages.Clear();
                    }
                }
            }
        }
    }
}
