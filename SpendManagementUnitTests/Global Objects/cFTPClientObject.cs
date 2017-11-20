using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests
{
    public class cFTPClientObject
    {
        public static cFTPClient getFTPClientObject()
        {
            string ftpServerName = "ftp:2100"; //ConfigurationManager.AppSettings["unitTestFTPServer"];
            cFTPClient client = new cFTPClient(ftpServerName, "anonymous", "123");

            return client;
        }

        public static cFTPClient getInvalidFTPClientObject()
        {
            string ftpServerName = "invalidFTPClient123";
            cFTPClient client = new cFTPClient(ftpServerName, "anonymous", "123");

            return client;
        }
    }
}
