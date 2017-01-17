using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagingFramework.MSMQ;
using System.Configuration;
using MessagingFramework.Logging;

namespace MessagingFramework.Factory
{
    public class MSMQFacoty : IMSMQFacoty
    {
        QueueOperation queueOperation = null;
        Logger logger = new Logger();

        public MSMQFacoty()
        {
            queueOperation = new QueueOperation();
        }

        /// <summary>
        /// Enque message in application specific queue
        /// </summary>
        /// <param name="messageLabel">System.String : messageLabel</param>
        /// <param name="messageBody">System.String : messageBody</param>
        /// <returns></returns>
        public bool EnqueMessage(string messageType, string messageBody)
        {
            bool status = false;
            try
            {
                string queueName = GetQueueName(messageType);
                string ackQueueName = GetAckQueueName();
                Message myMessage = new Message();
                myMessage.Formatter = new BinaryMessageFormatter();
                myMessage.Body = messageBody;
                myMessage.Label = "MSMQ";
                myMessage.UseJournalQueue = true;
                myMessage.UseDeadLetterQueue = true;
                myMessage.Recoverable = true;
                myMessage.AdministrationQueue = new MessageQueue(ackQueueName);
                myMessage.AcknowledgeType = AcknowledgeTypes.PositiveArrival | AcknowledgeTypes.FullReceive;
                queueOperation.EnqueMessage(queueName, myMessage);
                using (MessageQueue myQueue = new MessageQueue(queueName))
                {
                    bool acknowledged = ReceiveAcknowledgment(myMessage.Id);
                    if (!acknowledged)
                    {
                        throw new InvalidOperationException("Acknowledgement was not received");
                    }
                    status = true;
                }
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return status;
        }

        /// <summary>
        /// Returns message body of message if success, string.Empty if error
        /// </summary>
        /// <returns>messageBody if success, string.empty if error</returns>
        public string DequeMessage(string messageType)
        {
            string messageBody = string.Empty;
            try
            {
                string queueName = GetQueueName(messageType);
                messageBody = queueOperation.DequeMessage(queueName);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            return messageBody;
        }

        /// <summary>
        /// Returns messageBody of all messages if success, null if error
        /// </summary>
        /// <returns></returns>
        public string[] DequeAllMessage(string messageType)
        {
            string[] allmessageBody = null;
            try
            {
                string queueName = GetQueueName(messageType);
                allmessageBody = queueOperation.DequeAllMessage(queueName);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            return allmessageBody;
        }

        /// <summary>
        /// Process all messages from request queue and sends them in response queue
        /// </summary>
        /// <param name="requestType">System.String requestType</param>
        /// <param name="responseType">System.String responseType</param>
        /// <returns></returns>
        public int ProcessMessage(string requestType, string responseType)
        {
            int count = 0;
            try
            {
                string ackQueueName = GetAckQueueName();
                string requestQueue = GetQueueName(requestType);
                string responseQueue = GetQueueName(responseType);
                Message[] allMessage = queueOperation.DequeMessages(requestQueue);
                Console.WriteLine("Enter response for :");
                for (int index = 0; index < allMessage.Length; index++)
                {
                    Message myMessage = allMessage[index];
                    Console.WriteLine(myMessage.Body);
                    string messageBody = Console.ReadLine();
                    myMessage.Body = string.Format("{0} : Processed at {1}", messageBody, DateTime.UtcNow.ToString());
                    myMessage.CorrelationId = myMessage.Id;
                    myMessage.Formatter = new BinaryMessageFormatter();
                    myMessage.Body = messageBody;
                    myMessage.Label = "MSMQ";
                    myMessage.UseJournalQueue = true;
                    myMessage.UseDeadLetterQueue = true;
                    myMessage.Recoverable = true;
                    myMessage.AdministrationQueue = new MessageQueue(ackQueueName);
                    myMessage.AcknowledgeType = AcknowledgeTypes.FullReachQueue | AcknowledgeTypes.FullReceive;
                    queueOperation.EnqueMessage(responseQueue, myMessage);
                    using (MessageQueue myQueue = new MessageQueue(responseQueue))
                    {
                        bool acknowledged = ReceiveAcknowledgment(myMessage.Id);
                        if (!acknowledged)
                        {
                            throw new InvalidOperationException("Acknowledgement was not received");
                        }
                    }
                    count++;
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            finally
            {
            }
            return count;
        }

        /// <summary>
        /// Returns all processed messages from response queue
        /// </summary>
        /// <param name="requestType">System.String requestType</param>
        /// <param name="responseType">System.String responseType</param>
        /// <returns></returns>
        public string[] ShowProcessedMessage(string requestType, string responseType)
        {
            string[] allMessage = null;
            try
            {
                string journalQueueName = GetQueueName(requestType) + ";JOURNAL";
                string responseQueueName = GetQueueName(responseType);
                Message[] allResponseMessage = queueOperation.DequeMessages(responseQueueName);
                allMessage = new string[allResponseMessage.Length];
                MessageQueue journalQueue = new MessageQueue(journalQueueName);
                journalQueue.Formatter = new BinaryMessageFormatter();
                journalQueue.MessageReadPropertyFilter.SetAll();              
                int count = journalQueue.GetAllMessages().Count();
                for (int index = 0; index < allResponseMessage.Length; index++)
                {
                    Message foundMessage = null;
                    try
                    {                        
                        foundMessage = journalQueue.PeekById(allResponseMessage[index].CorrelationId);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                    }
                    if (foundMessage != null)
                    {
                        allMessage[index] = string.Format("Message Body : {0}",foundMessage.Body);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            finally
            {
            }
            return allMessage;
        }

        /// <summary>
        /// Reads acknowledgement message from acknowledgemenr queue
        /// </summary>
        /// <param name="messageId">System.String : messageId</param>
        /// <returns></returns>
        public bool ReceiveAcknowledgment(string messageId)
        {
            bool status = false;
            try
            {
                string queueName = GetAckQueueName();
                status = queueOperation.ReceiveAcknowledgment(messageId, queueName);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            return status;
        }

        /// <summary>
        /// Returns number of message's in message queue
        /// </summary>
        /// <param name="queue">MessageQueue : queue</param>
        /// <returns>integer</returns>
        private int GetMessageCount(MessageQueue queue)
        {
            int totalCount = 0;
            try
            {
                System.Messaging.MessageEnumerator me = queue.GetMessageEnumerator2();
                while (me.MoveNext(new TimeSpan(0, 0, 0)))
                {
                    totalCount++;
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            return totalCount;
        }

        /// <summary>
        /// Returns queue name
        /// </summary>
        /// <param name="messageType">Systemstring : messageType</param>
        /// <returns></returns>
        private string GetQueueName(string messageType)
        {
            string queueName = string.Empty;
            try
            {
                if (messageType == "CorrelationRequest")
                {
                    queueName = Command.CorrelationRequest;
                }                
                else if (messageType == "CorrelationResponse")
                {
                    queueName = Command.CorrelationResponse;
                }
                else if (messageType == "Queue")
                {
                    queueName = Command.Queue;
                }
                else if (messageType == "StronglyTypedQueue")
                {
                    queueName = Command.StronglyTypedQueue;
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            return queueName;
        }

        /// <summary>
        /// Returns acknowledgement queue name
        /// </summary>
        /// <returns></returns>
        private string GetAckQueueName()
        {
            string ackQueueName = string.Empty;
            try
            {
                ackQueueName = ConfigurationManager.AppSettings["AckQueueName"].ToString();
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            return ackQueueName;
        }

        public bool EnqueMessage(string messageType, Message message)
        {
            try
            {
                string queueName = GetQueueName(messageType);
                string ackQueueName = GetAckQueueName();
                queueOperation.EnqueMessage(queueName, message);
                return true;
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Message DequeBinaryMessage(string messageType)
        {
            try
            {
                string queueName = GetQueueName(messageType);
                return (Message)queueOperation.DequeBinaryMessage(queueName);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }
    }
}