using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Auto_Tests.Tools;
using System.Data.SqlClient;

namespace Auto_Tests
{
    /// <summary>
    /// Class for the sql statement that returns the correct sorting order
    ///</summary>
    class MobileDevicesSQLStatements
    {
        public static string sortingOrderForMobileDevices = "SELECT deviceName, deviceTypeID, pairingKey FROM mobileDevices WHERE employeeID = @employeeID ORDER BY {0} {1}";
    }

    /// <summary>
    /// Class for opening the database connection
    ///</summary>
    class MobileDevicesDAO
    {
        private cDatabaseConnection db;
        public MobileDevicesDAO(cDatabaseConnection db)
        {
            this.db = db;
        }

        /// <summary>
        /// Used to return the correct sorting order from the database 
        ///</summary>
        public List<MobileDevicesMethods.MobileDevice> GetCorrectSortingOrderFromDB(SortMobileDevicesByColumn sortby, EnumHelper.TableSortOrder sortingOrder, int employeeID, ProductType executingProduct)
        {
            List<MobileDevicesMethods.MobileDevice> mobileDevices = new List<MobileDevicesMethods.MobileDevice>();
            MobileDevicesMethods cMobileDeviceMethods = new MobileDevicesMethods();
            db.sqlexecute.Parameters.AddWithValue("@employeeId", employeeID);
            SqlDataReader reader = db.GetReader(string.Format(MobileDevicesSQLStatements.sortingOrderForMobileDevices, EnumHelper.GetEnumDescription(sortby), EnumHelper.GetEnumDescription(sortingOrder)));
            while (reader.Read())
            {
                MobileDevicesMethods.MobileDevice device = new MobileDevicesMethods.MobileDevice();
                
                device.DeviceName = reader.GetString(0);
                device.DeviceType = cMobileDeviceMethods.GetDeviceTypeByDeviceTypeID(reader.GetInt32(1), executingProduct); 
                device.PairingKey = reader.GetString(2);

                mobileDevices.Add(device);
            }
            reader.Close();
            return mobileDevices;
        }
    }
}
