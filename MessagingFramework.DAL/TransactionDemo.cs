using System;
using System.Configuration;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Messaging;
using System.Reflection;

[assembly: ApplicationName("MessagingFramework.DAL")]
[assembly: ApplicationActivation(ActivationOption.Server)]

namespace MessagingFramework.DAL
{
    [Transaction(TransactionOption.Required)]
    public class TransactionDemo : ServicedComponent
    {
        [AutoComplete(true)]
        public static void ExternalTransactionViaADO()
        {
            MessageQueue queueA = new MessageQueue();

            try
            {
                queueA.Path = ConfigurationManager.AppSettings["TransactionalQ1"]; ;
                queueA.Send("OrderA for Message", "Order A", MessageQueueTransactionType.Automatic);
                string ConnectionString = ConfigurationManager.ConnectionStrings["TestConnection"].ToString();
                Random r = new Random();
                // testOrder = new TestOrder() { Id = r.Next(1, 10), CreatedDate = DateTime.Now.ToString(), Name = "Order_" + r.Next(11, 100) };
                int inserted = 0;
                string QryString = "insert into TestOrder values(" + r.Next(1, 10) + ",'" + DateTime.Now.ToString() + "','Order_" + r.Next(11, 100) + "')";
                SqlConnection conn = new SqlConnection(ConnectionString);
                SqlCommand CmdObj = new SqlCommand(QryString, conn);
                conn.Open();
                inserted = CmdObj.ExecuteNonQuery();
                if(ContextUtil.IsInTransaction)
                ContextUtil.SetComplete();
                conn.Close();
            }
            catch (Exception ex)
            {
                if (ContextUtil.IsInTransaction)
                ContextUtil.SetAbort();
                throw;
            }
            finally
            {
                queueA.Close();
            }
        }
    }
}