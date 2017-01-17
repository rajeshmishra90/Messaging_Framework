using System;
using System.Messaging;
using System.ServiceModel;

namespace MessagingFramework.MSMQ
{
    public class QueueOperation
    {
        /// <summary>
        /// Enque message in application specific queue
        /// </summary>
        /// <param name="queueName">System.String : queueName</param>
        /// <param name="Message">Message : myMessage</param>
        /// <returns></returns>
        public bool EnqueMessage<T>(string queueName, T myMessage)
        {
            bool status = false;
            try
            {
                using (MessageQueue myQueue = new MessageQueue(queueName))
                {
                    //myQueue.MessageReadPropertyFilter.SetAll();
                    myQueue.Send(myMessage);
                    status = true;
                }
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (MessageQueueException ex)
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
        /// <param name="queueName">System.String : queueName</param>
        /// <returns>messageBody if success, string.empty if error</returns>
        public string DequeMessage(string queueName)
        {
            string messageBody = string.Empty;
            try
            {
                using (MessageQueue myQueue = new MessageQueue(queueName))
                {
                    if (GetMessageCount(myQueue) > 0)
                    {
                        Message myMessage = myQueue.Receive();
                        myMessage.UseJournalQueue = true;
                        myMessage.Formatter = new BinaryMessageFormatter();
                        messageBody = Convert.ToString(myMessage.Body);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            finally
            {
            }
            return messageBody;
        }

        public Message DequeBinaryMessage(string queueName)
        {
            Message messageBody = null;
            try
            {
                using (MessageQueue myQueue = new MessageQueue(queueName))
                {
                    if (GetMessageCount(myQueue) > 0)
                    {
                        Message myMessage = myQueue.Receive();
                        myMessage.UseJournalQueue = true;
                        myMessage.Formatter = new BinaryMessageFormatter();
                        //myMessage.Formatter = new XmlMessageFormatter(new Type[] { typeof(string), typeof(Contact) });
                        messageBody = myMessage;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            finally
            {
            }
            return messageBody;
        }

        /// <summary>
        /// Returns messageBody of all messages if success, null if error
        /// </summary>
        /// <param name="queueName">System.String : queueName</param>
        /// <returns></returns>
        public string[] DequeAllMessage(string queueName)
        {
            string[] allmessageBody = null;
            try
            {
                using (MessageQueue myQueue = new MessageQueue(queueName))
                {
                    int totalCount = GetMessageCount(myQueue);
                    allmessageBody = new string[totalCount];
                    for (int index = 0; index < totalCount; index++)
                    {
                        Message myMessage = myQueue.Receive();
                        myMessage.UseJournalQueue = true;
                        myMessage.Formatter = new BinaryMessageFormatter();
                        allmessageBody[index] = Convert.ToString(myMessage.Body);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            finally
            {
            }
            return allmessageBody;
        }

        /// <summary>
        /// Returns all messages from queue if success, null if error
        /// </summary>
        /// <param name="queueName">System.String : queueName</param>
        /// <returns></returns>
        public Message[] DequeMessages(string queueName)
        {
            Message[] allmessage = null;
            try
            {
                using (MessageQueue myQueue = new MessageQueue(queueName))
                {
                    myQueue.MessageReadPropertyFilter.SetAll();
                    int totalCount = GetMessageCount(myQueue);
                    allmessage = new Message[totalCount];
                    for (int index = 0; index < totalCount; index++)
                    {
                        Message myMessage = myQueue.Receive();
                        myMessage.UseJournalQueue = true;
                        myMessage.Formatter = new BinaryMessageFormatter();
                        allmessage[index] = myMessage;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            return allmessage;
        }

        /// <summary>
        /// Returns copy of all messages from queue if success, null if error
        /// </summary>
        /// <param name="queueName">System.String : queueName</param>
        /// <returns></returns>
        public Message[] GetAllMessageCopy(string queueName)
        {
            Message[] allmessage = null;
            try
            {
                using (MessageQueue myQueue = new MessageQueue(queueName))
                {
                    allmessage = myQueue.GetAllMessages();
                }
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (MessageQueueException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
            return allmessage;
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
                throw new Exception(ex.Message);
            }
            return totalCount;
        }

        /// <summary>
        /// Reads acknowledgement message from acknowledgemenr queue
        /// </summary>
        /// <param name="messageId">System.String : messageId</param>
        /// <param name="queueName">System.String : queueName</param>
        /// <returns></returns>
        public bool ReceiveAcknowledgment(string messageId, string queueName)
        {
            bool status = false;
            try
            {
                MessageQueue queue = new MessageQueue(queueName);
                queue.MessageReadPropertyFilter.CorrelationId = true;
                queue.MessageReadPropertyFilter.Acknowledgment = true;
                Message message = queue.ReceiveByCorrelationId(messageId);
                status = true;
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (ArgumentException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (MessageQueueException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return status;
        }
    }
}