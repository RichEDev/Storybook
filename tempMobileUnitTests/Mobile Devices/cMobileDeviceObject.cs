using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace tempMobileUnitTests
{
    internal class cMobileDeviceObject
    {
        public static cMobileDevice New(cMobileDevice device = null, ICurrentUser currentUser = null)
        {
            currentUser = currentUser ?? Moqs.CurrentUser();
            device = device ?? Template();

            cMobileDevices clsMobileDevices = null;
            int deviceID = -1;

            try
            {
                clsMobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                
                deviceID = clsMobileDevices.SaveMobileDevice(device, currentUser.EmployeeID);
                
                cMobileDevices clsMobileDevices2 = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                device = clsMobileDevices2.GetMobileDeviceById(deviceID);
            }
            catch(Exception e)
            {
                try
                {
                    #region Cleanup

                    if(deviceID > 0 && clsMobileDevices != null)
                    {
                        clsMobileDevices.DeleteMobileDevice(deviceID, currentUser.EmployeeID, (currentUser.isDelegate ? currentUser.Delegate.employeeid : 0));
                    }

                    #endregion Cleanup
                }
                finally
                {
                    throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cMobileDeviceObject).ToString() + ">", e);
                }
            }

            return device;
        }

        public static bool TearDown(int deviceID)
        {
            if(deviceID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cMobileDevices clsDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                    int? delegateid = null;
                    if(currentUser.isDelegate)
                        delegateid = currentUser.Delegate.employeeid;

                    return clsDevices.DeleteMobileDevice(deviceID, currentUser.EmployeeID, delegateid);
                }
                catch(Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public static cMobileDevice Template(int deviceID = 0, string deviceName = "UT CE <DateTime.UtcNow.Ticks> <CallingMethodName>", cMobileDeviceType deviceType = null, int employeeId = -1, string pairingKey = null, string serialKey = null)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            string dt = DateTime.UtcNow.Ticks.ToString();

            deviceName = (deviceName == "UT CE <DateTime.UtcNow.Ticks> <CallingMethodName>")
                             ? "UT CE " + dt + " " + new StackFrame(1).GetMethod().Name
                             : deviceName;
            employeeId = (employeeId == -1) ? currentUser.EmployeeID : employeeId;
            serialKey = serialKey ?? "be21d28605cf201291c4a2ad0ae93a5f0daaabbb";
            pairingKey = string.IsNullOrEmpty(pairingKey) ?  currentUser.AccountID.ToString("00000") + "-88888-" + currentUser.EmployeeID.ToString("000000") : pairingKey;
            deviceType = (deviceType ?? cMobileDeviceTypeObject.Template());

            return new cMobileDevice(
                mobileDeviceID: deviceID,
                deviceName: deviceName,
                mobileDeviceType: deviceType,
                employeeID: employeeId,
                pairingKey: pairingKey,
                serialKey: serialKey
                );
        }
    }

    internal class cMobileDeviceTypeObject
    {
        public static cMobileDeviceType Template(int deviceTypeID = 1, string typeDescription = "iPhone")
        {
            string dt = DateTime.UtcNow.Ticks.ToString();

            return new cMobileDeviceType(
                mobileDeviceTypeId: deviceTypeID,
                mobileDeviceTypeDescription: typeDescription

                );
        }

    }

    internal class cMobileExpenseItemObject
    {
        public static ExpenseItem Template(int mobileItemID = 0, string otherdetails = default(string), int? reasonid = null, decimal total = 0, int subcatid = 0, DateTime date = default(DateTime), int? currencyid = null, int miles = 0, double quantity = 0, string fromlocation = default(string), string tolocation = default(string), DateTime allowancestartdate = default(DateTime), DateTime allowanceenddate = default(DateTime), string itemnotes = default(string), decimal allowancedeductamount = 0, int? allowancetypeid = null, bool hasreceipt = false, int? mobiledevicetypeid = null)
        {

            date = (date == default(DateTime) ? DateTime.UtcNow : date);
            string strDate = date.Year.ToString() + date.Month.ToString("00") + date.Day.ToString("00");
            DateTime? dtAllowanceStartDate = null;
            if(allowancestartdate != default(DateTime))
                dtAllowanceStartDate = allowancestartdate;
            DateTime? dtAllowanceEndDate = null;
            if(allowanceenddate != default(DateTime))
                dtAllowanceEndDate = allowanceenddate;
            int devTypeID = (mobiledevicetypeid.HasValue ? mobiledevicetypeid.Value : 1);

            string strAllowanceStartDate = (allowancestartdate == default(DateTime) ? string.Empty : allowancestartdate.Year.ToString() + allowancestartdate.Month.ToString("00") + allowancestartdate.Day.ToString("00"));
            string strAllowanceEndDate = (allowanceenddate == default(DateTime) ? string.Empty : allowanceenddate.Year.ToString() + allowanceenddate.Month.ToString("00") + allowanceenddate.Day.ToString("00"));

            return new ExpenseItem
                   {
                       MobileID = mobileItemID,
                       OtherDetails = otherdetails,
                       ReasonID = reasonid,
                       Total = total,
                       SubcatID = subcatid,
                       Date = strDate,
                       CurrencyID = currencyid,
                       Miles = miles,
                       Quantity = quantity,
                       FromLocation = fromlocation,
                       ToLocation = tolocation,
                       allowanceStartDate = strAllowanceStartDate,
                       allowanceEndDate = strAllowanceEndDate,
                       ItemNotes = itemnotes,
                       AllowanceDeductAmount = allowancedeductamount,
                       AllowanceTypeID = allowancetypeid,
                       HasReceipt = hasreceipt,
                       MobileDeviceTypeId = devTypeID
                   };
        }

        public static ExpenseItem New(ExpenseItem mobileItem, bool generateReceipt = false)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cMobileDevices devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
            int mobileDevTypeId = (mobileItem.MobileDeviceTypeId.HasValue ? mobileItem.MobileDeviceTypeId.Value : 1);

            int mobileItemID = devices.saveMobileItem(currentUser.EmployeeID, mobileItem.OtherDetails, mobileItem.ReasonID, mobileItem.Total, mobileItem.SubcatID, mobileItem.dtDate, mobileItem.CurrencyID, mobileItem.Miles, mobileItem.Quantity, mobileItem.FromLocation, mobileItem.ToLocation, mobileItem.dtAllowanceStartDate, mobileItem.dtAllowanceEndDate, mobileItem.AllowanceTypeID, mobileItem.AllowanceDeductAmount, mobileItem.ItemNotes, mobileDevTypeId);

            ExpenseItem retItem = devices.getMobileItemByID(mobileItemID);

            if(generateReceipt)
            {
                using(FileStream fsSource = new FileStream("images/lunch receipt.jpg", FileMode.Open, FileAccess.Read))
                {
                    byte[] aBytes = new byte[fsSource.Length];
                    int numBytesToRead = (int) fsSource.Length;
                    int numBytesRead = 0;
                    while(numBytesToRead > 0)
                    {
                        int n = fsSource.Read(aBytes, numBytesRead, numBytesToRead);

                        // Break when the end of the file is reached.
                        if(n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                    devices.saveMobileItemReceipt(retItem.MobileID, aBytes);
                }
            }
            return retItem;
        }

        public static bool TearDown(int mobileExpenseItemID)
        {
            if(mobileExpenseItemID > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cMobileDevices clsDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                    return clsDevices.DeleteMobileItemByID(mobileExpenseItemID);
                }
                catch(Exception e)
                {
                    return false;
                }
            }

            return false;
        }
    }
}