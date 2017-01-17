using System.Messaging;
using System.ServiceModel;

namespace MessagingFramework.Factory
{
    [ServiceContract]
    public interface IMSMQFacoty
    {
        [OperationContract]
        bool EnqueMessage(string messageType, string messageBody);

        [OperationContract]
        bool EnqueMessage(string messageType, Message message);

        [OperationContract]
        string DequeMessage(string messageType);

        [OperationContract]
        Message DequeBinaryMessage(string messageType);

        [OperationContract]
        string[] DequeAllMessage(string messageType);

        [OperationContract]
        bool ReceiveAcknowledgment(string messageId);

        [OperationContract]
        int ProcessMessage(string requestType, string responseType);

        [OperationContract]
        string[] ShowProcessedMessage(string requestType, string responseType);
    }
}