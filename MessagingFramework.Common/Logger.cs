using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace MessagingFramework.Logging
{
    public class Logger
    {
        private ILog log;

        public Logger()
        {
            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void LogInformation(string message)
        {
            log.Info(message);
        }

        public void LogError(string message)
        {
            log.Error(message);
        }

        public void LogError(string message, Exception exception)
        {
            log.Error(message, exception);
        }

    }
}
