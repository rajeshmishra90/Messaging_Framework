using MessagingFramework.Logging;
using System;
using System.Configuration;
using System.Messaging;
using System.Transactions;

namespace MessagingFramework.Service
{
    public class QueueMaintenance
    {
        Logger logger = null;

        public QueueMaintenance()
        {
            logger = new Logger();
        }

        public void Start()
        {
            try
            {
                int totalCount = DeleteOldJournalMessage(Convert.ToString(ConfigurationManager.AppSettings["QueueName"]));
                logger.LogInformation(String.Format("Total journal messages deleted {0}", totalCount));
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Returns number of journal message's deleted from queue
        /// </summary>
        /// <param name="queueName">System.String : queueName</param>
        /// <returns></returns>
        private int DeleteOldJournalMessage(string queueName)
        {
            int totalCount = 0;
            try
            {
                queueName += ";JOURNAL";
                using (TransactionScope transaction = new TransactionScope())
                {
                    using (MessageQueue queue = new MessageQueue(queueName))
                    {
                        queue.Formatter = new BinaryMessageFormatter();
                        queue.MessageReadPropertyFilter.ArrivedTime = true;
                        using (MessageEnumerator messageReader = queue.GetMessageEnumerator2())
                        {
                            int JournalLogDays = Convert.ToInt32(ConfigurationManager.AppSettings["JournalLogDays"]);
                            while (messageReader.MoveNext())
                            {
                                Message message = messageReader.Current;
                                if (message.ArrivedTime.AddMinutes(JournalLogDays) < DateTime.Now)
                                {
                                    queue.ReceiveById(message.Id);
                                    totalCount++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return totalCount;
        }
    }
}