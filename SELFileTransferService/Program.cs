using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Net.Mail;
using System.Configuration;

namespace SELFileTransferService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(new ThreadExceptionHandler().ApplicationErrorHandler);

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new SELFileTransferService() 
			};
            ServiceBase.Run(ServicesToRun);
        }

        /// <summary>
        /// Deal with all errors that occur in the Service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>

        public class ThreadExceptionHandler
        {
            public void ApplicationErrorHandler(object sender, UnhandledExceptionEventArgs args)
            {
                Exception e = (Exception)args.ExceptionObject;

                //Send email on stop of the service

                string emailServer = ConfigurationManager.AppSettings["EmailServer"];
                string errorsAddress = ConfigurationManager.AppSettings["ErrorEmailAddress"];

                string subject = "Error occured in SEL File Transfer Service on devserv";
                string message = "An error has occurred on the SEL File Transfer Service on devserv";

                message += "\n\n\n";
                message += "**Error Information**\n";
                message += "\n\n";
                message += "**Message**\n";
                message += e.Message;
                message += "\n\n";
                message += "**Stack trace**\n";
                message += e.StackTrace;

                EmailSender emailSender = new EmailSender(emailServer);
                emailSender.SendEmail(new MailMessage("admin@sel-expenses.com", errorsAddress, subject, message));
            }
        }
    }
}
