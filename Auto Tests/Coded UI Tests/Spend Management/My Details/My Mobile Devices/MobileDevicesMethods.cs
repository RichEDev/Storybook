using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.ComponentModel;
using Microsoft.SqlServer.Server;
using System.Data;

namespace Auto_Tests
{
    internal class MobileDevicesMethods
    {
        /// <summary>
        /// Class used to define a mobile device
        ///</summary>
        internal class MobileDevice
        {
            internal int MobileDeviceID { get; set; }
            internal int EmployeeID { get; set; }
            internal string DeviceType { get; set; }
            internal string DeviceName { get; set; }
            internal string PairingKey { get; set; }
            internal string SerialKey { get; set; }
            internal int CreatedBy { get; set; }
            internal OSVendor VendorDetails { get; set; }

            internal MobileDevice() { }
        }

        internal class OSVendor
        {
            internal string Model { get; set; }
            internal int MobileDeviceOSType { get; set; }
            internal string MobileInstallFrom { get; set; }
            internal string MobileImage { get; set; }
        }

        /// <summary>
        /// Class used to store the data read from lithium for mobile devices 
        ///</summary>
        internal class CachePopulator
        {
            protected cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            internal List<MobileDevice> PopulateCache(ProductType executingProduct)
            {
                int employeeID = AutoTools.GetEmployeeIDByUsername(executingProduct);
                List<MobileDevice> mobileDevices = new List<MobileDevice>();
                using (System.Data.SqlClient.SqlDataReader reader = db.GetReader("SELECT mobileDeviceID, employeeID, deviceTypeID, deviceName, pairingKey, deviceSerialKey, createdBy FROM mobileDevices"))
                {
                    while (reader.Read())
                    {
                        #region Set variables

                        MobileDevice mobileDevice = new MobileDevice();

                        mobileDevice.MobileDeviceID = 0;
                        mobileDevice.EmployeeID = employeeID;
                        mobileDevice.DeviceType = reader.GetString(2);
                        mobileDevice.DeviceName = reader.GetString(3);
                        if (!reader.IsDBNull(4))
                        {
                            mobileDevice.PairingKey = reader.GetString(4);
                        }
                        else
                        {
                            mobileDevice.PairingKey = null;
                        }
                        if (!reader.IsDBNull(5))
                        {
                            mobileDevice.SerialKey = reader.GetString(5);
                        }
                        else
                        {
                            mobileDevice.SerialKey = null;
                        }
                        if (!reader.IsDBNull(6))
                        {
                            mobileDevice.CreatedBy = employeeID;
                        }
                        else
                        {
                            mobileDevice.SerialKey = null;
                        }
                        mobileDevice.VendorDetails = GetOSVendorDetailsByDeviceType(executingProduct, mobileDevice.DeviceType);
                        mobileDevices.Add(mobileDevice);

                        #endregion
                    }
                    reader.Close();
                }
                return mobileDevices;
            }
        }

        /// <summary>
        /// Calls the stored procedure to create a mobile device 
        ///</summary>
        internal int CreateMobileDevice(MobileDevice mobileDeviceToCreate, ProductType executingProduct)
        {
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            db.sqlexecute.Parameters.AddWithValue("@mobileDeviceID", mobileDeviceToCreate.MobileDeviceID);
            db.sqlexecute.Parameters.AddWithValue("@employeeID", mobileDeviceToCreate.EmployeeID);

            db.sqlexecute.Parameters.AddWithValue("@deviceTypeID", GetDeviceTypeIdByDeviceType(mobileDeviceToCreate.DeviceType, executingProduct));


            db.sqlexecute.Parameters.AddWithValue("@deviceName", mobileDeviceToCreate.DeviceName);

            if (string.IsNullOrWhiteSpace(mobileDeviceToCreate.PairingKey))
            {
                mobileDeviceToCreate.PairingKey = string.Empty;
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@pairingKey", mobileDeviceToCreate.PairingKey);
            }
            if (string.IsNullOrWhiteSpace(mobileDeviceToCreate.SerialKey))
            {
                mobileDeviceToCreate.SerialKey = string.Empty;
            }
            
            
            db.sqlexecute.Parameters.AddWithValue("@deviceSerialKey", mobileDeviceToCreate.SerialKey);

            db.sqlexecute.Parameters.AddWithValue("@requestorEmployeeId", mobileDeviceToCreate.CreatedBy);

            db.sqlexecute.Parameters.Add("@newMobileDeviceID", SqlDbType.Int);
            db.sqlexecute.Parameters["@newMobileDeviceID"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("saveMobileDevice");
            int result  = (int)db.sqlexecute.Parameters["@newMobileDeviceID"].Value;
            db.sqlexecute.Parameters.Clear();

            return result;
        }

        /// <summary>
        /// Calls stored procedure to delete mobile device
        ///</summary>
        internal static void DeleteMobileDevice(int mobileDeviceID, ProductType executingProduct, int? employeeID, int? delegateID)
        {
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            db.sqlexecute.Parameters.AddWithValue("@mobileDeviceID", mobileDeviceID);
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeID);
            if (delegateID.HasValue)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateID.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            db.sqlexecute.Parameters.Add("@affectedRows", SqlDbType.Int);
            db.sqlexecute.Parameters["@affectedRows"].Direction = ParameterDirection.ReturnValue;
            db.ExecuteProc("deleteMobileDevice");
            int result = (int)db.sqlexecute.Parameters["@affectedRows"].Value;
            db.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Returns device type id using device type
        ///</summary>
        internal int GetDeviceTypeIdByDeviceType(string deviceType, ProductType executingProduct)
        {
            int deviceTypeID = -1;
            Dictionary<string, int> dictionary = GetDeviceTypes(executingProduct);

            if(dictionary.ContainsKey(deviceType))
            {
                deviceTypeID = dictionary[deviceType];
            }
            return deviceTypeID;
        }

        /// <summary>
        /// Returns a dictionary of device types from the metabase
        ///</summary>
        internal static Dictionary<string, int> GetDeviceTypes(ProductType executingProduct) 
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            cDatabaseConnection db = new cDatabaseConnection(cGlobalVariables.MetabaseConnectionString(executingProduct));
            string strSQL = "SELECT model, mobileDeviceTypeID FROM mobileDeviceTypes";
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
            {
                while(reader.Read())
                {
                    dictionary.Add(reader.GetString(0), reader.GetInt32(1));
                }
                reader.Close();
            }

            return dictionary;
        }

        /// <summary>
        /// Gets the mobile device type from the database based on the mobile device id
        ///</summary>
        internal string GetDeviceTypeByDeviceTypeID(int deviceTypeID, ProductType executingProduct)
        {
            string deviceType = string.Empty;
            Dictionary<string, int> dictionary = GetDeviceTypes(executingProduct);

            foreach (KeyValuePair<string, int> pair in dictionary)
            { 
                if(pair.Value == deviceTypeID)
                {
                    deviceType = pair.Key;
                    break;
                }
            }

            return deviceType;
        }

        /// <summary>
        /// Gets the mobile device ID from the database based on the mobile Device name
        ///</summary>
        internal static int getDeviceIDByDeviceName(string deviceName, int employeeID, ProductType executingProduct)
        {
            int deviceID =0;
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            string strSQL = "SELECT mobileDeviceID FROM mobileDevices WHERE deviceName = @deviceName AND employeeID = @employeeID";
            db.sqlexecute.Parameters.AddWithValue("@deviceName", deviceName);
            db.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    deviceID = reader.GetInt32(0);
                    reader.Close();
                }
            }
            db.sqlexecute.Parameters.Clear();
            return deviceID;
        }

        /// <summary>
        /// Enables mobile devices to employees access role 
        ///</summary>
        internal static void SetMobileDevices(string mobileDevicesStringValue, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));

            string mobileDevicesSQL = "UPDATE accountProperties SET stringValue = @mobileDevicesStringValue WHERE stringKey = 'useMobileDevices'";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@mobileDevicesStringValue", mobileDevicesStringValue);
            dbex_CodedUI.ExecuteSQL(mobileDevicesSQL);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Passing employee id returns users access role
        ///</summary>
        internal int GetUsersAccessRoleIDByEmployeeID(int employeeID, ProductType executingProduct)
        {
            int accessRoleID;
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            string strSQL = "SELECT accessRoleID FROM employeeAccessRoles WHERE employeeID = @employeeID";
            db.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
            {
                reader.Read();
                accessRoleID = reader.GetInt32(0);
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();
            return accessRoleID;
        }
 
        /// <summary>
        /// Using Employees Username get the activation key of the mobile device
        ///</summary>
        public string GetMobileActivationcodeByEmployeeUserName(int employeeId, string deviceName, ProductType executingProduct)
        {
            string paringKey = string.Empty;
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            string strSQL = "SELECT pairingKey FROM mobiledevices WHERE employeeID = @employeeID and deviceName = @deviceName";
            db.sqlexecute.Parameters.AddWithValue("@employeeID", employeeId);
            db.sqlexecute.Parameters.AddWithValue("@deviceName", deviceName);
            using (SqlDataReader reader = db.GetReader(strSQL))
            {
                reader.Read();
                paringKey = reader.GetString(0);
                reader.Close();
            }
            db.sqlexecute.Parameters.Clear();
            return paringKey;
        }

        /// <summary>
        /// Archives and then deletes an employee based on the employees id
        ///</summary>
        internal static void DeleteEmployeeByEmployeeID(int employeeid, ProductType executingProduct)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            
            //archived employee
            string archiveEmployeeSql = "UPDATE employees SET archived = 1 WHERE employeeid = @employeeid";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            dbex_CodedUI.ExecuteSQL(archiveEmployeeSql);
            dbex_CodedUI.sqlexecute.Parameters.Clear();

            //Delete employee
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@CUemployeeID", DBNull.Value);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            dbex_CodedUI.ExecuteProc("deleteEmployee");
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Gets details of the os vendor based on the type of the device
        ///</summary>
        internal static OSVendor GetOSVendorDetailsByDeviceType(ProductType executingProduct, string deviceType)
        {
            OSVendor vendorDetails = new OSVendor();
            DBConnection db = new DBConnection(cGlobalVariables.MetabaseConnectionString(executingProduct));
            string strSQL = "SELECT mobileDeviceTypes.model, mobileDeviceTypes.mobileDeviceOSType, mobileDeviceOSTypes.mobileinstallfrom, mobileDeviceOSTypes.mobileImage "
                            + "FROM mobileDeviceTypes "
                            + "INNER JOIN mobileDeviceOSTypes "
                            + "ON mobileDeviceTypes.mobileDeviceOSType = mobileDeviceOSTypes.mobileDeviceOSTypeId "
                            + "WHERE mobileDeviceTypes.model = @deviceType";
            db.sqlexecute.Parameters.AddWithValue("@deviceType", deviceType);
            using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(strSQL))
            {
                reader.Read();
                vendorDetails.Model = reader.GetString(0);
                vendorDetails.MobileDeviceOSType = reader.GetInt32(1);
                vendorDetails.MobileInstallFrom = reader.GetString(2);
                vendorDetails.MobileImage = reader.GetString(3);
                reader.Close();
            }
            
            db.sqlexecute.Parameters.Clear();
            return vendorDetails;
        }
    }
}
