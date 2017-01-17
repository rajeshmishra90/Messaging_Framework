using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Messaging;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ApplicationName("MSMQ_Transaction_Sample")]
[assembly: ApplicationActivation(ActivationOption.Server)]
[assembly: AssemblyKeyFileAttribute("KeyFile2.snk")]
[assembly: ApplicationAccessControl(false)]

namespace MSMQ_Transaction_Sample
{
    [Transaction(TransactionOption.Required)]
    [ComVisible(true)]
    public class ExternalTransaction : ServicedComponent
    {
        public ExternalTransaction()
        {

        }

        public void ExternalTransactionViaADO()
        {
            MessageQueue queueA = new MessageQueue();
            string ConnectionString = @"data source=AJGUSHC2DB08PD.ajgcodev.int\dev2;initial catalog=Learn;integrated security=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            try
            {
                queueA.Path = @"formatName:DIRECT=OS:corpvmdev67.ajgcodev.int\private$\TransactionalQ1";
                queueA.Send("OrderA for Message", "Order A", MessageQueueTransactionType.Automatic);
                Random r = new Random();
                int inserted = 0;
                string QryString = "insert into TestOrder values(" + r.Next(1, 10) + ",'" + DateTime.Now.ToString() + "','Order_" + r.Next(11, 100) + "')";
                SqlCommand CmdObj = new SqlCommand(QryString, conn);
                conn.Open();
                inserted = CmdObj.ExecuteNonQuery();
                ContextUtil.SetComplete();
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "ExternalTransaction";
                    eventLog.WriteEntry("Error Getting " + ex.Message, EventLogEntryType.Error, 101, 1);
                }
                ContextUtil.SetAbort();
            }
            finally
            {
                conn.Close();
                queueA.Close();
            }
        }

    }
}