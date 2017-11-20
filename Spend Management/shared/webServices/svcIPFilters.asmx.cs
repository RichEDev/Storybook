using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Data;
using SpendManagementLibrary;
using System.Net;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcCars
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    
    [ScriptService]
    public class svcIPFilters : WebService
    {
        /// <summary>
        /// Saves the desired IP Filter record
        /// </summary>
        /// <param name="active"></param>
        /// <returns>The ID of the saved IP Filter</returns>
        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public int saveIPFilter(int ipfilterid, string ipaddress, string description, bool active)
        {
            int retVal = 0;

            CurrentUser currentUser = cMisc.GetCurrentUser();
            cIPFilters clsipfilters = new cIPFilters(currentUser.AccountID);

            if (currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.IPAdressFiltering, true) || currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.IPAdressFiltering, true))
            {
                cIPFilter ipfilter;
                if (ipfilterid > 0)
                {
                    cIPFilter oldipfilter = clsipfilters.GetIPFilterById(ipfilterid);

                    ipfilter = new cIPFilter(ipfilterid, ipaddress, description, active, oldipfilter.CreatedOn, oldipfilter.CreatedBy, DateTime.Now, currentUser.EmployeeID);
                }
                else
                {
                    ipfilter = new cIPFilter(ipfilterid, ipaddress, description, active, DateTime.Now, currentUser.EmployeeID, DateTime.Now, currentUser.EmployeeID);
                }

                int nEmployeeID = currentUser.EmployeeID;

                int? nDelegateID = currentUser.isDelegate ? (int?)currentUser.Delegate.EmployeeID : null;

                retVal = clsipfilters.saveIPFilter(ipfilter, nEmployeeID, nDelegateID);
            }

            return retVal;
        }

        /// <summary>
        /// Deletes the desired IP Filter record
        /// </summary>
        /// <returns>The number of rows that have been removed after deletion</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int deleteIPFilter(int ipFilterID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int retVal = 0;

            if (currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.IPAdressFiltering, true))
            {
                cIPFilters clsIPFilters = new cIPFilters(currentUser.AccountID);
                int nEmployeeID = currentUser.EmployeeID;
                int? nDelegateID = currentUser.isDelegate ? (int?)currentUser.Delegate.EmployeeID : null; 

                retVal = clsIPFilters.deleteIPFilter(ipFilterID, nEmployeeID, nDelegateID);
            }

            return retVal;
        }

        /// <summary>
        /// Returns the desired cIPFilter
        /// </summary>                
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cIPFilter getIPFilterByID(int ipFilterID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();                        
            cIPFilters clsIPFilters = new cIPFilters(currentUser.AccountID);

            cIPFilter ipFilter = clsIPFilters.GetIPFilterById(ipFilterID);

            return ipFilter;
        }   
    }
}
