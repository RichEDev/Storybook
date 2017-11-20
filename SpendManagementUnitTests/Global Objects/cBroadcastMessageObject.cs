using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cBroadcastMessageObject
    {
        /// <summary>
        /// Global static variable of a broadcast object
        /// </summary>
        /// <returns></returns>
        public static cBroadcastMessage CreateBroadcastObject()
        {
            cBroadcastMessages clsBroadcast = new cBroadcastMessages(cGlobalVariables.AccountID);
            byte ID = clsBroadcast.addBroadcastMessage("Unit Test Broadcast" + DateTime.Now.Ticks, "This is a test message", new DateTime(1999, 01, 01), new DateTime(2099, 01, 01), true, broadcastLocation.HomePage, true, DateTime.UtcNow, cGlobalVariables.EmployeeID);

            cBroadcastMessage broadcast = clsBroadcast.getBroadcastMessageById(ID);
            cGlobalVariables.BroadcastID = ID;
            return broadcast;
        }

        /// <summary>
        /// Delete the global static variable from the database
        /// </summary>
        public static void DeleteBroadcast()
        {
            cBroadcastMessages clsBroadcast = new cBroadcastMessages(cGlobalVariables.AccountID);
            clsBroadcast.deleteBroadcastMessage(cGlobalVariables.BroadcastID);
        }
    }
}
