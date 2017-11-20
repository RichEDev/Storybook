using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SpendManagementLibrary;
using SpendManagementLibrary.Mobile;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for mobileDevices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcMobileDevices : System.Web.Services.WebService
    {
        [WebMethod(EnableSession=true)]
        public MobileDevice GetMobileDeviceByID(int mobileDeviceID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cEmployees clsEmployees = new cEmployees(currentUser.AccountID);
            MobileDevice reqMobileDevice = null;

            cMobileDevices devices = new cMobileDevices(currentUser.AccountID);
            reqMobileDevice = devices.GetMobileDeviceById(mobileDeviceID);

            return reqMobileDevice;
        }

        [WebMethod(EnableSession = true)]
        public string GetNewPairingKey(int employeeID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            string newKey = string.Empty;

            cMobileDevices clsMobileDevices = new cMobileDevices(currentUser.AccountID);

            newKey = clsMobileDevices.GeneratePairingKey(employeeID);

            return newKey;
        }


        [WebMethod(EnableSession = true)]
        public int SaveMobileDevice(MobileDevice mobileDevice)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cMobileDevices clsMobileDevices = new cMobileDevices(currentUser.AccountID);
            
            if (mobileDevice.MobileDeviceID == 0)
            {
                // New phone so add the required parts
                mobileDevice.SerialKey = string.Empty;
            }
            else
            {
                MobileDevice currentDevice = clsMobileDevices.GetMobileDeviceById(mobileDevice.MobileDeviceID);
                mobileDevice.PairingKey = !string.IsNullOrEmpty(currentDevice.PairingKey) ? currentDevice.PairingKey : mobileDevice.PairingKey;
                mobileDevice.SerialKey = !string.IsNullOrEmpty(currentDevice.SerialKey) ? currentDevice.SerialKey : mobileDevice.SerialKey;
            }

            return clsMobileDevices.SaveMobileDevice(mobileDevice, currentUser.EmployeeID);
        }


        [WebMethod(EnableSession = true)]
        public bool DeleteMobileDevice(int mobileDeviceID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cMobileDevices clsMobileDevices = new cMobileDevices(currentUser.AccountID);
            int? delegateID = null;
            if (currentUser.isDelegate)
                delegateID = currentUser.Delegate.EmployeeID;

            return clsMobileDevices.DeleteMobileDevice(mobileDeviceID, currentUser.EmployeeID, delegateID);
        }

        /// <summary>
        /// Get the install location and image for the selected device type.
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string[] GetMobileActivateHelpText(int deviceType)
        {
            var mobileDevice = new cMobileDevices(cMisc.GetCurrentUser().AccountID);
            
            MobileDeviceType selectedDevice;
            if (mobileDevice.MobileDeviceTypes.TryGetValue(deviceType, out selectedDevice))
            {
                MobileDeviceOsType selectedOs;
                if (mobileDevice.MobileDeviceOsTypes.TryGetValue(selectedDevice.DeviceOsTypeId, out selectedOs))
                {
                    return new string[] { selectedOs.MobileDeviceInstallFrom, selectedOs.MobileDeviceImage };
                }
            }
            
            return new string[1];
        }
    }
}
