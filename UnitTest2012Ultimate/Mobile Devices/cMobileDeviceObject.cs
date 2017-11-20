namespace UnitTest2012Ultimate
{
    #region Using Directives

    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Mobile;

    using Spend_Management;

    using ExpenseItem = Spend_Management.ExpenseItem;

    #endregion

    /// <summary>
    /// The c mobile device object.
    /// </summary>
    internal class MobileDeviceObject
    {
        #region Public Methods and Operators

        /// <summary>
        /// The new.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        /// <returns>
        /// The <see cref="MobileDevice"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Mobile device setup error.
        /// </exception>
        public static MobileDevice New(MobileDevice device = null, ICurrentUser currentUser = null)
        {
            currentUser = currentUser ?? Moqs.CurrentUser();
            device = device ?? Template();

            cMobileDevices clsMobileDevices = null;
            int deviceId = -1;

            try
            {
                clsMobileDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                deviceId = clsMobileDevices.SaveMobileDevice(device, currentUser.EmployeeID);
                Assert.IsTrue(
                    deviceId > 0, 
                    string.Format("cMobileDeviceObject.New failure : cMobileDevices.SaveMobileDevice method return code = {0}", deviceId));

                var clsMobileDevices2 = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                device = clsMobileDevices2.GetMobileDeviceById(deviceId);
            }
            catch (Exception e)
            {
                try
                {
                    if (deviceId > 0 && clsMobileDevices != null)
                    {
                        clsMobileDevices.DeleteMobileDevice(
                            deviceId, 
                            currentUser.EmployeeID, 
                            currentUser.isDelegate ? currentUser.Delegate.EmployeeID : 0);
                    }
                }
                finally
                {
                    throw new Exception(
                        "Error during setup of unit test dummy object of type <" + typeof(MobileDeviceObject) + ">", e);
                }
            }

            return device;
        }

        /// <summary>
        /// The tear down.
        /// </summary>
        /// <param name="deviceId">
        /// The device id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool TearDown(int deviceId)
        {
            if (deviceId > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    var clsDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
                    int? delegateid = null;
                    if (currentUser.isDelegate)
                    {
                        delegateid = currentUser.Delegate.EmployeeID;
                    }

                    return clsDevices.DeleteMobileDevice(deviceId, currentUser.EmployeeID, delegateid);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// The template.
        /// </summary>
        /// <param name="deviceId">
        /// The device Id.
        /// </param>
        /// <param name="deviceName">
        /// The device name.
        /// </param>
        /// <param name="deviceType">
        /// The device type.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="pairingKey">
        /// The pairing key.
        /// </param>
        /// <param name="serialKey">
        /// The serial key.
        /// </param>
        /// <returns>
        /// The <see cref="MobileDevice"/>.
        /// </returns>
        public static MobileDevice Template(
            int deviceId = 0, 
            string deviceName = "UT CE <DateTime.UtcNow.Ticks> <CallingMethodName>", 
            MobileDeviceType deviceType = null, 
            int employeeId = -1, 
            string pairingKey = null, 
            string serialKey = null)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            string dt = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);

            deviceName = (deviceName == "UT CE <DateTime.UtcNow.Ticks> <CallingMethodName>")
                             ? "UT CE " + dt + " " + new StackFrame(1).GetMethod().Name
                             : deviceName;
            employeeId = (employeeId == -1) ? currentUser.EmployeeID : employeeId;
            serialKey = serialKey ?? "be21d28605cf201291c4a2ad0ae93a5f0daaabbb";
            var epoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            pairingKey = string.IsNullOrEmpty(pairingKey)
                             ? currentUser.AccountID.ToString("00000") + "-" + epoch.ToString(CultureInfo.InvariantCulture).Substring(5) + "-"
                               + currentUser.EmployeeID.ToString("000000")
                             : pairingKey;
            deviceType = deviceType ?? MobileDeviceTypeObject.Template();

            return new MobileDevice(
                deviceId, 
                deviceName: deviceName, 
                mobileDeviceType: deviceType, 
                employeeId: employeeId, 
                pairingKey: pairingKey, 
                serialKey: serialKey);
        }

        #endregion
    }

    /// <summary>
    /// The c mobile device type object.
    /// </summary>
    internal class MobileDeviceTypeObject
    {
        #region Public Methods and Operators

        /// <summary>
        /// The template.
        /// </summary>
        /// <param name="deviceTypeId">
        /// The device type id.
        /// </param>
        /// <param name="typeDescription">
        /// The type description.
        /// </param>
        /// <param name="deviceOsTypeId"></param>
        /// <returns>
        /// The <see cref="MobileDeviceType"/>.
        /// </returns>
        public static MobileDeviceType Template(int deviceTypeId = 1, string typeDescription = "iPhone", int deviceOsTypeId = 1)
        {
            return new MobileDeviceType(deviceTypeId, typeDescription, deviceOsTypeId);
        }

        #endregion
    }

    /// <summary>
    /// The c mobile expense item object.
    /// </summary>
    internal class MobileExpenseItemObject
    {
        #region Public Methods and Operators

        /// <summary>
        /// The new.
        /// </summary>
        /// <param name="mobileItem">
        /// The mobile item. 
        /// </param>
        /// <param name="generateReceipt">
        /// The generate receipt. 
        /// </param>
        /// <returns>
        /// The <see cref="ExpenseItem"/> . 
        /// </returns>
        public static SpendManagementLibrary.Mobile.ExpenseItem New(ExpenseItem mobileItem, bool generateReceipt = false)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            var devices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);
            int mobileDevTypeId = mobileItem.MobileDeviceTypeId.HasValue ? mobileItem.MobileDeviceTypeId.Value : 1;

            int mobileItemId = devices.saveMobileItem(
                currentUser.EmployeeID, 
                mobileItem.OtherDetails, 
                mobileItem.ReasonID, 
                mobileItem.Total, 
                mobileItem.SubcatID, 
                mobileItem.dtDate, 
                mobileItem.CurrencyID, 
                mobileItem.Miles, 
                mobileItem.Quantity, 
                mobileItem.FromLocation, 
                mobileItem.ToLocation, 
                mobileItem.dtAllowanceStartDate, 
                mobileItem.dtAllowanceEndDate, 
                mobileItem.AllowanceTypeID, 
                mobileItem.AllowanceDeductAmount, 
                mobileItem.ItemNotes, 
                mobileDevTypeId,
                1,
                1);

            SpendManagementLibrary.Mobile.ExpenseItem retItem = devices.getMobileItemByID(mobileItemId);

            if (generateReceipt)
            {
                using (
                    var filesource = new FileStream(
                        GlobalTestVariables.ImagesPath + "/lunch receipt.jpg", FileMode.Open, FileAccess.Read))
                {
                    var bytes = new byte[filesource.Length];
                    var numBytesToRead = (int)filesource.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        int n = filesource.Read(bytes, numBytesRead, numBytesToRead);

                        // Break when the end of the file is reached.
                        if (n == 0)
                        {
                            break;
                        }

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }

                    devices.saveMobileItemReceipt(retItem.MobileID, bytes);
                }
            }

            return retItem;
        }

        /// <summary>
        /// The tear down.
        /// </summary>
        /// <param name="mobileExpenseItemId">
        /// The mobile expense item id. 
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> . 
        /// </returns>
        public static bool TearDown(int mobileExpenseItemId)
        {
            if (mobileExpenseItemId > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    var clsDevices = new cMobileDevices(currentUser, GlobalVariables.MetabaseConnectionString);

                    return clsDevices.DeleteMobileItemByID(mobileExpenseItemId);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// The template.
        /// </summary>
        /// <param name="mobileItemId">
        /// The mobile item id.
        /// </param>
        /// <param name="otherdetails">
        /// The otherdetails.
        /// </param>
        /// <param name="reasonid">
        /// The reasonid.
        /// </param>
        /// <param name="total">
        /// The total.
        /// </param>
        /// <param name="subcatid">
        /// The subcatid.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="currencyid">
        /// The currencyid.
        /// </param>
        /// <param name="miles">
        /// The miles.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="fromlocation">
        /// The fromlocation.
        /// </param>
        /// <param name="tolocation">
        /// The tolocation.
        /// </param>
        /// <param name="allowancestartdate">
        /// The allowancestartdate.
        /// </param>
        /// <param name="allowanceenddate">
        /// The allowanceenddate.
        /// </param>
        /// <param name="itemnotes">
        /// The itemnotes.
        /// </param>
        /// <param name="allowancedeductamount">
        /// The allowancedeductamount.
        /// </param>
        /// <param name="allowancetypeid">
        /// The allowancetypeid.
        /// </param>
        /// <param name="hasreceipt">
        /// The hasreceipt.
        /// </param>
        /// <param name="mobiledevicetypeid">
        /// The mobiledevicetypeid.
        /// </param>
        /// <returns>
        /// The <see cref="ExpenseItem"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Invalid subcat id exception.
        /// </exception>
        public static ExpenseItem Template(
            int mobileItemId = 0, 
            string otherdetails = default(string), 
            int? reasonid = null, 
            decimal total = 0, 
            int subcatid = 0, 
            DateTime date = default(DateTime), 
            int? currencyid = null, 
            int miles = 0, 
            double quantity = 0, 
            string fromlocation = default(string), 
            string tolocation = default(string), 
            DateTime allowancestartdate = default(DateTime), 
            DateTime allowanceenddate = default(DateTime), 
            string itemnotes = default(string), 
            decimal allowancedeductamount = 0, 
            int? allowancetypeid = null, 
            bool hasreceipt = false, 
            int? mobiledevicetypeid = null)
        {
            date = date == default(DateTime) ? DateTime.UtcNow : date;
            string strDate = date.Year + date.Month.ToString("00") + date.Day.ToString("00");
            if (subcatid == 0)
            {
                throw new Exception("You must provide a valid subCatID that belongs to the user's item role");
            }

            int devTypeId = mobiledevicetypeid.HasValue ? mobiledevicetypeid.Value : 1;

            // reasonid = reasonid ?? -1;
            string strAllowanceStartDate = allowancestartdate == default(DateTime)
                                               ? string.Empty
                                               : allowancestartdate.Year
                                                 + allowancestartdate.Month.ToString("00")
                                                 + allowancestartdate.Day.ToString("00");
            string strAllowanceEndDate = allowanceenddate == default(DateTime)
                                             ? string.Empty
                                             : allowanceenddate.Year + allowanceenddate.Month.ToString("00")
                                               + allowanceenddate.Day.ToString("00");

            return new ExpenseItem
                {
                    MobileID = mobileItemId, 
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
                    MobileDeviceTypeId = devTypeId
                };
        }

        #endregion
    }
}