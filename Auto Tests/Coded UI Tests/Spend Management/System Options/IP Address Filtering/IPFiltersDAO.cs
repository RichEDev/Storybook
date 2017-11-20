using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Auto_Tests.Tools;
using System.Data.SqlClient;

namespace Auto_Tests
{
    class IPFiltersSQLStatements
    {
        public static string GET_ALL_IPFILTERS = "SELECT ipAddress, description, active FROM ipfilters ORDER BY {0} {1}";
    }

    class IPFiltersDAO 
    {
        private cDatabaseConnection db;
        public IPFiltersDAO(cDatabaseConnection db)
        {
            this.db = db;
        }

        public List<IPFiltersDTO> GetAll(SortIPFiltersByColumn sortby, EnumHelper.TableSortOrder order)
        {
            List<IPFiltersDTO> ipAddressFilters = new List<IPFiltersDTO>();
            SqlDataReader reader = db.GetReader(String.Format(IPFiltersSQLStatements.GET_ALL_IPFILTERS, EnumHelper.GetEnumDescription(sortby), EnumHelper.GetEnumDescription(order)));
            while (reader.Read())
            {
                ipAddressFilters.Add(new IPFiltersDTO(reader.GetString(0), reader.GetString(1),reader.GetBoolean(2)));
            }
            reader.Close();
            return ipAddressFilters;
        }

        public int DeleteIPFiltersFromDB(List<IPFiltersDTO> IPFiltersToDelete)
        {
            int result = -1;

            foreach (IPFiltersDTO IPFilter in IPFiltersToDelete)
            {
                result = DeleteIPFiltersFromDB(IPFilter);
            }
            return result;
        }

        public int DeleteIPFiltersFromDB(IPFiltersDTO IPFilterToDelete)
        {
            int result = -1;

            string strSQL0 = "DELETE ipfilters WHERE ipAddress = @IPAddress";
            db.sqlexecute.Parameters.AddWithValue("@IPAddress", IPFilterToDelete.ipAddress);
            result = db.ExecuteSQL2(strSQL0);
            db.sqlexecute.Parameters.Clear();

            return result;
        }
    }
}
