using MessagingFramework.Factory;
using MessagingFramework.Logging;
using MessagingFramework.Logging.DTOS;
using System;
using System.Configuration;
using System.Text;

namespace MessagingFramework.ClientB
{
    internal class Program
    {
        private static Logger logger = new Logger();

        private static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Choose operation 1. Subscribe Message 2. Start Listener 3. Send Response");
                    Console.WriteLine("4. Read different strongly typed object messages");
                    var input = Convert.ToInt32(Console.ReadLine());
                    switch (input)
                    {
                        case (1):
                            Subscriber.Process();
                            break;

                        case (2):
                            StartListener();
                            break;

                        case (3):
                            Correlation.ProcessMessage();
                            break;

                        case (4):
                            ReadStronglyTypedMessages();
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    logger.LogError(ex.Message);
                }
            }
        }

        private static void ReadStronglyTypedMessages()
        {
            var queueName = ConfigurationManager.AppSettings["StronglyTypedQueue"];
            IMSMQFacoty queueOperation = new MSMQFacoty();
            var obj = queueOperation.DequeBinaryMessage("StronglyTypedQueue");
            if (obj != null && obj.Label == "Contact")
            {
                var data = (Contact)obj.Body;
                StringBuilder sb = new StringBuilder();
                sb.Append(data.FirstName + ";");
                sb.Append(data.LastName + ";");
                sb.Append(data.Id.ToString() + ";");
                sb.Append(data.Guid.ToString() + ";");

                Console.WriteLine(sb);
            }
            else if (obj != null && obj.Label == "StringMessage")
            {
                var data = Convert.ToString(obj.Body);
                Console.WriteLine(data);
            }
            else
            {
                Console.WriteLine("No Messages to read");
            }
        }

        private static void StartListener()
        {
            var queueName = ConfigurationManager.AppSettings["QueuePath"];
            MSMQListener _MSMQListener = new MSMQListener(queueName);
            _MSMQListener.Start();
            Console.ReadLine();
        }
    }
}