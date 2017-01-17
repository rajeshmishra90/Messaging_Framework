using MessagingFramework.Logging.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MessagingFramework.ClientB
{
    public delegate void MessageReceivedEventHandler(object sender, MessageEventArgs args);

    public class MSMQListener
    {
        private bool _listen;
        private Type[] _types;
        private MessageQueue _queue;

        public event MessageReceivedEventHandler MessageReceived;

        public Type[] FormatterTypes
        {
            get { return _types; }
            set { _types = value; }
        }

        public MSMQListener(string queuePath)
        {
            _queue = new MessageQueue(queuePath);
        }

        public void Start()
        {
            _listen = true;

            if (_types != null && _types.Length > 0)
            {
                // Using only the XmlMessageFormatter. You can use other formatters as well
                _queue.Formatter = new XmlMessageFormatter(_types);
            }

            _queue.PeekCompleted += new PeekCompletedEventHandler(OnPeekCompleted);
            _queue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnReceiveCompleted);

            StartListening();
        }

        public void Stop()
        {
            _listen = false;
            _queue.PeekCompleted -= new PeekCompletedEventHandler(OnPeekCompleted);
            _queue.ReceiveCompleted -= new ReceiveCompletedEventHandler(OnReceiveCompleted);

        }

        private void StartListening()
        {
            Console.WriteLine("Listener Started...");
            if (!_listen)
            {
                return;
            }

            // The MSMQ class does not have a BeginRecieve method that can take in a 
            // MSMQ transaction object. This is a workaround - we do a BeginPeek and then 
            // recieve the message synchronously in a transaction.
            // Check documentation for more details
            //if (_queue.Transactional)
            //{
            //    _queue.BeginPeek();
            //}
            //else
            {
                _queue.BeginReceive();
            }
        }

        private void OnPeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            _queue.EndPeek(e.AsyncResult);
            MessageQueueTransaction trans = new MessageQueueTransaction();
            System.Messaging.Message msg = null;
            try
            {
                trans.Begin();
                msg = _queue.Receive(trans);
                trans.Commit();

                StartListening();

                FireRecieveEvent(msg.Body);
            }
            catch
            {
                trans.Abort();
            }
        }

        private void FireRecieveEvent(object body)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, new MessageEventArgs(body));
            }
        }

        private void OnReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            System.Messaging.Message msg = _queue.EndReceive(e.AsyncResult);

            StartListening();

            Contact myContact = new Contact();
            Object o = new Object();

            System.Type[] arrTypes = new System.Type[2];
            arrTypes[0] = myContact.GetType();
            arrTypes[1] = o.GetType();
            msg.Formatter = new XmlMessageFormatter(arrTypes);
            myContact = ((Contact)msg.Body);

            StringBuilder sb = new StringBuilder();
            sb.Append(myContact.FirstName + ";");
            sb.Append(myContact.LastName + ";");
            sb.Append(myContact.Id.ToString() + ";");
            sb.Append(myContact.Guid.ToString() + ";");

            Console.WriteLine(sb);

            FireRecieveEvent(msg.Body);
        }
    }

    public class MessageEventArgs : EventArgs
    {
        private object _messageBody;

        public object MessageBody
        {
            get { return _messageBody; }
        }

        public MessageEventArgs(object body)
        {
            _messageBody = body;

        }
    }
}
