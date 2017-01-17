using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingFramework.Factory;

namespace MessagingFramework.ClientA
{
   internal class MSMQ
    {
       internal static void SendMessage()
       {
           IMSMQFacoty queueOperation = new MSMQFacoty();
           queueOperation.EnqueMessage("Queue", string.Format("Test message coming from ClientA at {0}", DateTime.UtcNow));
           Console.ReadLine();
       }
    }
}
