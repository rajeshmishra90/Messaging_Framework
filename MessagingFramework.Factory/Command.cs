using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MessagingFramework.Factory
{
    internal static class Command
    {
        public static string CorrelationRequest = ConfigurationManager.AppSettings["CorrelationRequest"];
        public static string CorrelationResponse = ConfigurationManager.AppSettings["CorrelationResponse"];
        public static string Queue = ConfigurationManager.AppSettings["QueueName"].ToString();

        public static string StronglyTypedQueue = ConfigurationManager.AppSettings["StronglyTypedQueue"].ToString();
    }
}
