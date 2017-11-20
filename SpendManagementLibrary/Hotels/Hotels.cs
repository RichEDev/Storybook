using System.Collections.Generic;
using System.Data.SqlClient;

namespace SpendManagementLibrary.Hotels
{
using System;
using System.Configuration;
using System.Data;
using Helpers;
using Interfaces;

    /// <summary>
    ///     The class is concerend with the searching and returning of hotels, and returning county and city data
    /// </summary>
        [Serializable]
    public class Hotels
    {
        private string strSQL;
        private ICurrentUserBase currentUser;

        public Hotels()
        {
        }

        public Hotels(ICurrentUserBase currentUser)
        {
            this.currentUser = currentUser;
        }

        #region Methods and Operators

        /// <summary>
        ///     Checks if the hotel already exists in the database.
        /// </summary>
        /// <param name="hotelName">The name of the hotel</param>
        /// <param name="connection">The database connection to use</param>
        /// <returns>True if this hotel already exists in the database, otherwise return false.</returns>
        public bool AlreadyExists(string hotelName, IDBConnection connection = null)
        {
            int count;
         
            strSQL = "select count(*) from hotels where hotelname = @hotelname";           
            IDBConnection expData = connection ?? new DatabaseConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            expData.sqlexecute.Parameters.AddWithValue("@hotelname", hotelName);  
            count = expData.ExecuteScalar<int>(strSQL);
 
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Gets distinct counties associated with hotels from the database.
        /// </summary>
        /// <param name="connection">The database connection to use</param>
        /// <returns>A dataset of counties</returns>
        public DataSet GetCountyList(IDBConnection connection = null)
        {
            var ds = new DataSet();
            string strSQL = "select distinct county from hotels order by county";

            IDBConnection expData = connection ?? new DatabaseConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            ds = expData.GetDataSet(strSQL);

            return ds;
        }

        /// <summary>
        ///     Gets distinct cities associated with hotels from the database.
        /// </summary>
        /// <param name="county">The county filtering by</param>
        /// <param name="connection">The database connection to use</param>
        /// <returns>A dataset of cities</returns>
        public DataSet GetCityList(string county, IDBConnection connection = null)
        {
            var ds = new DataSet();
            string strSQL = "select distinct city from hotels where county = @county order by city";

            IDBConnection expData = connection ?? new DatabaseConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            expData.sqlexecute.Parameters.AddWithValue("@county", county);
            ds = expData.GetDataSet(strSQL);
            expData.sqlexecute.Parameters.Clear();

            return ds;
        }

        /// <summary>
        ///     Gets hotel data associated with a hotel name from the database.
        /// </summary>
        /// <param name="hotelName">The name of the hotel</param>
        /// <param name="connection">The database connection to use</param>
        /// <returns>A dataset of hotel data</returns>
        public DataSet SearchForHotels(string hotelName, IDBConnection connection = null)
        {
            var ds = new DataSet();

            IDBConnection expData = connection ?? new DatabaseConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            strSQL = "select * from hotels where hotelname like @hotelname order by hotelname";
            expData.sqlexecute.Parameters.AddWithValue("@hotelname", hotelName + "%");
            ds = expData.GetDataSet(strSQL);

            return ds;
        }

        /// <summary>
        ///     Gets hotel data assoicated with a a county/city from the database.
        /// </summary>
        /// <param name="county">County searching on</param>
        /// <param name="city">City searching on</param>
        /// <param name="connection">The database connection to use</param>
        /// <returns>A dataset of hotel data</returns>
        public DataSet SearchForHotels(string county, string city, IDBConnection connection = null)
        {
            var ds = new DataSet();

            IDBConnection expData = connection ?? new DatabaseConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            if (city == "")
            {
                strSQL = "select * from hotels where county =  @county order by hotelname";
            }
            else
            {
                strSQL = "select * from hotels where county =  @county and city = @city order by hotelname";
            }
            expData.sqlexecute.Parameters.AddWithValue("@city", city);
            expData.sqlexecute.Parameters.AddWithValue("@county", county);
            ds = expData.GetDataSet(strSQL);

            return ds;
        }

        /// <summary>
        /// Queries the database for hotels that match the inputted hotel name
        /// </summary>
        /// <param name="hotelName">The hotel name</param>
        /// <returns>A list of <see cref="Hotel">Hotels</see></returns>
        public IEnumerable<Hotel> GetHotelsByName(string hotelName)
        {
            List<Hotel> hotels = new List<Hotel>();

            using (var expdata = new DatabaseConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString))
            {      
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.AddWithValue("@hotelname", hotelName);

                using (var reader = expdata.GetReader("GetHotelsByName", CommandType.StoredProcedure))
                {

                    int idOrd = reader.GetOrdinal("hotelId");
                    int nameOrd = reader.GetOrdinal("hotelName");
                    int address1Ord = reader.GetOrdinal("address1");
                    int address2Ord = reader.GetOrdinal("address2");
                    int cityOrd  = reader.GetOrdinal("city");
                    int countyOrd = reader.GetOrdinal("county");
                    int postCodeOrd = reader.GetOrdinal("postcode");
                    int countryOrd = reader.GetOrdinal("country");
                    int ratingOrd = reader.GetOrdinal("rating");
                    int telNoOrd = reader.GetOrdinal("telNo");
                    int emailOrd = reader.GetOrdinal("email");
                    int createdOnOrd = reader.GetOrdinal("createdOn");
                    int createdByOrd = reader.GetOrdinal("createdBy");

                    while (reader.Read())
                    {
                        int id, createdBy;            
                        string name, address1, address2, city, county, postCode, country, email, telNo;
                        DateTime createdOn;
                        byte rating;

                        id = reader.GetInt32(idOrd);
                        name = reader.GetString(nameOrd);
                        address1 = reader.GetString(address1Ord) ?? string.Empty;
                        address2 = reader.GetString(address2Ord) ?? string.Empty;
                        city = reader.GetString(cityOrd) ?? string.Empty;
                        county = reader.GetString(countyOrd) ?? string.Empty;
                        postCode = reader.GetString(postCodeOrd) ?? string.Empty;
                        country = reader.GetString(countryOrd) ?? string.Empty;
                        rating = reader.GetByte(ratingOrd);
                        telNo = reader.GetString(telNoOrd) ?? string.Empty;
                        email = reader.GetString(emailOrd) ?? string.Empty;
                        createdOn = reader.IsDBNull(createdOnOrd) ? DateTime.MinValue : reader.GetDateTime(createdOnOrd);
                        createdBy = reader.IsDBNull(createdByOrd) ? 0 : reader.GetInt32(createdByOrd);

                        hotels.Add(new Hotel(id,name,address1,address2,city,county,postCode,country,rating,telNo,email,createdOn, createdBy));
                    }
                    reader.Close();
                }
            }

            return hotels;
        }

        #endregion
    }
}

