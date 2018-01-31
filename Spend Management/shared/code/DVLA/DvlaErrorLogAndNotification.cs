
namespace Spend_Management.shared.code.DVLA
{
    using Common.Logging;
    using Common.Logging.Log4Net;
    using SpendManagementLibrary.DVLA;
    using SpendManagementLibrary.Employees;
    using System;

    /// <summary>
    /// DVLA Service Error log and Email Notification class
    /// </summary>
    public class DvlaServiceEventLogAndEmailNotification
    {
        public static ILog Logger = new LogFactory<DvlaServiceEventLogAndEmailNotification>().GetLogger();

        /// <summary>
        /// Create the Dvla error response information from the error code and Email for certain error code.
        /// </summary>
        /// <param name="errorCode">Error code returned by Dvla service</param>
        /// <param name="errorDetails">Error details from api response</param>
        /// <param name="companyId">Company Id of the logged in user or the employee for which the error encountered while running the autolookup console app</param>
        /// <param name="source">Source(service) where the error occured, This can be consent submission error or DVLA look up error </param>
        /// <param name="notifications">The <see cref="NotificationTemplates"></see></param>
        /// <param name="employee">The <see cref="Employee"></see></param>
        public static void DvlaLogEntry(string errorCode,string errorDetails, string companyId, string source, NotificationTemplates notifications, Employee employee)
        {
            var dutyOfCareConstant = new DutyOfCareConstants();
            var responseCodes = dutyOfCareConstant.MapError(errorCode);
            var errorMessage =$"Dvla {source} Error : company id - {companyId} : Error Code - {errorCode} : Error Description : {responseCodes.ResponseCodeFriendlyMessages} ";
            
            if (errorCode != null && (errorCode == "504" || errorCode == "506" || errorCode == "501"))
            {
                var recipients = new int[1];
                recipients[0] = employee.EmployeeID;
                notifications.SendMessage(new Guid("9354DBDB-3F77-40CD-843E-02E23F9F23F5"), employee.EmployeeID, recipients);
                if (errorCode== "501")
                {
                    notifications.SendMessage(new Guid("877D893C-3FCB-40A3-A3DD-B2382D79459C"),employee.EmployeeID, recipients);
                }
            }
            else
            {
                //Details of the errors added along with the reponse messages
                if (errorCode != null && (errorCode == "300" || errorCode == "302" || errorCode == "500"))
                {
                    errorMessage = errorMessage + " : " + errorDetails + " : Employee Id - " + employee.EmployeeID;
                }
                else
                {
                    errorMessage = errorMessage + " : Employee Id - " + employee.EmployeeID;
                }
            }
           
            Logger.Error(errorMessage);
        }

    }
}