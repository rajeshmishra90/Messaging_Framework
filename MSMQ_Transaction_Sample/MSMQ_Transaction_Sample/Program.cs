using System;
using System.Diagnostics;
using System.Messaging;

namespace MSMQ_Transaction_Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    MSMQ_Transaction_Sample.ExternalTransaction exampleNine = new MSMQ_Transaction_Sample.ExternalTransaction();
                    exampleNine.ExternalTransactionViaADO();
                }
                catch (Exception ex)
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "ExternalTransaction";
                        eventLog.WriteEntry("Error Getting " + ex.Message, EventLogEntryType.Error, 101, 1);
                    }
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("End...");
                Console.ReadLine(); 
            }
        }
    }
}