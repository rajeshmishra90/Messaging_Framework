using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingFramework.Logging;
using MessagingFramework.Factory;

namespace MessagingFramework.ClientA
{
    internal class Correlation
    {
        static Logger logger = new Logger();

        internal static void SendMessage()
        {
            try
            {
                int count=10;
                IMSMQFacoty queueOperation = new MSMQFacoty();
                Console.Write("Enter message for processing : ");
                for (int index = 0; index < count; index++)
                {
                    string message = Console.ReadLine();
                    queueOperation.EnqueMessage("CorrelationRequest",message);                    
                }
                Console.WriteLine("{0} messages sent for processing", count);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }

        internal static void ReceiveProcessedMessage()
        {
            try
            {
                IMSMQFacoty queueOperation = new MSMQFacoty();
                string[] allMessage = queueOperation.ShowProcessedMessage("CorrelationRequest", "CorrelationResponse");
                for (int index = 0; index < allMessage.Length; index++)
                {
                    if (!string.IsNullOrEmpty(allMessage[index]))
                    {
                        Console.WriteLine(allMessage[index]);
                        Console.WriteLine();
                    }
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
