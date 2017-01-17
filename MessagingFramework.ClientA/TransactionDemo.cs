using System;
using System.Configuration;
using System.Messaging;

namespace MessagingFramework.ClientA
{
    internal class TransactionDemo
    {
        public static void SendMessageToMultipleQueues()
        {
            MessageQueueTransaction mqTran = new MessageQueueTransaction();

            MessageQueue queueA = new MessageQueue();
            queueA.Path = ConfigurationManager.AppSettings["TransactionalQ1"]; ;
            MessageQueue queueB = new MessageQueue();
            queueB.Path = ConfigurationManager.AppSettings["TransactionalQ2"]; ;

            mqTran.Begin();
            try
            {
                queueA.Send("Message A", "Label A", mqTran);
                queueB.Send("Message B", "Label B", mqTran);
                mqTran.Commit();
            }
            catch (Exception ex)
            {
                mqTran.Abort();
                throw;
            }
            finally
            {
                queueA.Close();
                queueB.Close();
            }
        }
    }
}