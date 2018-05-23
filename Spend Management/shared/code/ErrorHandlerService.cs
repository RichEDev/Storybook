﻿namespace Spend_Management
{
    using System;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Generates notification e-mails when errors occur in the service
    /// </summary>
    public class ErrorHandlerService : ErrorHandler
    {
        /// <summary>
        /// Send an error when a report generation fails, although this currently uses 
        /// a report request the functionality could be expanded to accept something else service related
        /// </summary>
        /// <param name="reportRequest">the report request that couldn't be successfully processed</param>
        /// <param name="ex">the exception</param>
        /// <returns>true if the email was sent, false if it wasn't</returns>
        public bool sendError(cReportRequest reportRequest, Exception ex) 
        {
            var module =
                FunkyInjector.Container.GetInstance<IDataFactory<IProductModule, Modules>>()[GlobalVariables.DefaultModule];

            cAccounts accounts = new cAccounts();
            cAccount account = accounts.GetAccountByID(reportRequest.accountid);

            cAccountSubAccounts subAccounts = new cAccountSubAccounts(reportRequest.accountid);
            cAccountSubAccount subAccount = subAccounts.getSubAccountById(reportRequest.SubAccountId);

            cEmployees employees = new cEmployees(reportRequest.accountid);
            Employee employee = employees.GetEmployeeById(reportRequest.employeeid);

            return GenerateEmail(account, subAccount, module, employee, null, ex, false, reportRequest);

        }

    }
}