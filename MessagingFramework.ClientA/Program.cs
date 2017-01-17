using MessagingFramework.Factory;
using MessagingFramework.Logging;
using MessagingFramework.Logging.DTOS;
using System;
using System.Configuration;
using System.Messaging;
using System.Text;

namespace MessagingFramework.ClientA
{
    public class Program
    {
        private static Logger logger = new Logger();

        private static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("1. Publish Message");
                    Console.WriteLine("2. Send Message for Receive Function demo");
                    Console.WriteLine("3. Send Request Message");
                    Console.WriteLine("4. Receive correlation message");
                    Console.WriteLine("5. Send Contact Object as Message 6. Send String as Message");
                    Console.WriteLine("7. Send Message to Multiple Queues (Internal Transaction)");
                    var input = Convert.ToInt32(Console.ReadLine());
                    switch (input)
                    {
                        case (1):
                            Publisher.Process();
                            break;

                        case (2):
                            SendMessageNotifications();
                            break;

                        case (3):
                            Correlation.SendMessage();
                            break;

                        case (4):
                            Correlation.ReceiveProcessedMessage();
                            break;

                        case (5):
                            SendContactObjectAsMessage(true);
                            break;

                        case (6):
                            SendContactObjectAsMessage(false);
                            break;

                        case (7):
                            TransactionDemo.SendMessageToMultipleQueues();
                            break;

                        default:
                            break;
                    }
                    //MSMQ.SendMessage();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    logger.LogError(ex.Message);
                }
            }
        }

        private static void SendContactObjectAsMessage(bool sendObject)
        {
            var queueName = ConfigurationManager.AppSettings["StronglyTypedQueue"];
            Contact contact = new Contact() { FirstName = "Rajesh", Id = 100, LastName = "Mishra", Guid = new Guid() };

            Message myMessage = new Message();
            if (sendObject)
            {
                //myMessage.Formatter = new XmlMessageFormatter(new Type[] { typeof(Contact) });
                myMessage.Formatter = new BinaryMessageFormatter();
                myMessage.Body = contact;
                myMessage.Label = "Contact";
            }
            else
            {
                myMessage.Formatter = new BinaryMessageFormatter();
                myMessage.Body = "This is string Message " + DateTime.Now.ToString();
                myMessage.Label = "StringMessage";
            }
            myMessage.UseJournalQueue = true;
            myMessage.UseDeadLetterQueue = true;
            myMessage.Recoverable = true;
            myMessage.AdministrationQueue = new MessageQueue(ConfigurationManager.AppSettings["AckQueueName"].ToString());
            myMessage.AcknowledgeType = AcknowledgeTypes.PositiveArrival | AcknowledgeTypes.FullReceive;
            IMSMQFacoty queueOperation = new MSMQFacoty();
            queueOperation.EnqueMessage("StronglyTypedQueue", myMessage);
            Console.WriteLine("Message Sent");
            Console.ReadLine();
        }

        private static void SendMessageNotifications()
        {
            var queueName = ConfigurationManager.AppSettings["QueuePath"];
            for (int i = 0; i < 1; i++)
            {
                Contact contact = new Contact() { FirstName = "Rajesh_" + i, Id = i, LastName = "Mishra_" + i, Guid = new Guid() };
                using (MessageQueue myQueue = new MessageQueue(queueName))
                {
                    Message myMessage = new Message();
                    myMessage.Formatter = new BinaryMessageFormatter();
                    myMessage.Body = contact;
                    myMessage.Label = "NotificationMessages";
                    myMessage.UseJournalQueue = true;
                    myMessage.UseDeadLetterQueue = true;
                    myMessage.Recoverable = true;
                    myQueue.Send(myMessage);
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(contact.FirstName + ";");
                sb.Append(contact.LastName + ";");
                sb.Append(contact.Id.ToString() + ";");
                sb.Append(contact.Guid.ToString() + ";");

                Console.WriteLine(sb);
            }
            Console.WriteLine("Messages Sent");
            Console.ReadLine();
        }
    }
}