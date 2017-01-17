using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingFramework.Logging;
using MessagingFramework.Factory;

namespace MessagingFramework.ClientB
{
    internal class Correlation
    {
        static Logger logger = new Logger();

        internal static void ProcessMessage()
        {
            try
            {
                IMSMQFacoty queueOperation = new MSMQFacoty();
                int count = queueOperation.ProcessMessage("CorrelationRequest", "CorrelationResponse");
                Console.WriteLine("{0} messages processed",count);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logger.LogError(ex.Message);
            }
        }
    }
}
