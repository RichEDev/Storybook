namespace SpendManagementLibrary.Hotels
{
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using SpendManagementLibrary.Helpers;
using SpendManagementLibrary.Interfaces;
using Utilities.DistributedCaching;

    /// <summary>
    ///     The hotel.
    /// </summary>
    [Serializable]
    public class Hotel
    {
        private readonly int nHotelid;
        private readonly string sHotelname;
        private readonly string sAddress1;
        private readonly string sAddress2;
        private readonly string sCity;
        private readonly string sCounty;
        private readonly string sPostcode;
        private readonly string sCountry;
        private readonly byte bRating;
        private readonly string sTelno;
        private readonly string sEmail;
        private readonly DateTime dtCreatedon;
        private readonly int nCreatedby;
        private readonly ICurrentUserBase currentUser;
        private string Sql;
        public const string cacheArea = "hotel";

        /// <summary>
        ///  This class is concerend with maintaining hotel data
        /// </summary>
        public Hotel()
        {
        }

        /// <summary>
        ///  This Hotel
        /// </summary>
        public Hotel(int hotelid, string hotelname, string address1, string address2, string city, string county,
            string postcode, string country, byte rating, string telno, string email, DateTime createdon, int createdby,
            ICurrentUserBase currentUser = null)
        {
            nHotelid = hotelid;
            sHotelname = hotelname;
            sAddress1 = address1;
            sAddress2 = address2;
            sCity = city;
            sCounty = county;
            sPostcode = postcode;
            sCountry = country;
            bRating = rating;
            sTelno = telno;
            sEmail = email;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            this.currentUser = currentUser;
        }

        #region properties

        public int hotelid
        {
            get { return nHotelid; }
        }

        public string hotelname
        {
            get { return sHotelname; }
        }

        public string address1
        {
            get { return sAddress1; }
        }

        public string address2
        {
            get { return sAddress2; }
        }

        public string city
        {
            get { return sCity; }
        }

        public string county
        {
            get { return sCounty; }
        }

        public string postcode
        {
            get { return sPostcode; }
        }

        public string country
        {
            get { return sCountry; }
        }

        public byte rating
        {
            get { return bRating; }
        }

        public string telno
        {
            get { return sTelno; }
        }

        public string email
        {
            get { return sEmail; }
        }

        public DateTime createdon
        {
            get { return dtCreatedon; }
        }

        public int createdby
        {
            get { return nCreatedby; }
        }

        #endregion

        #region Methods and Operators

        /// <summary>
        ///     Saves new hotel details, providing the hotel name does not already exist in the DB.
        /// </summary>
        /// <param name="hotelName">The name of the hotel</param>
        /// <param name="address1">The first line of the hotel's address</param>
        /// <param name="address2">The second line of the hotel's address</param>
        /// <param name="city">The city the hotel is situated</param>
        /// <param name="county">The county the hotel is situated</param>
        /// <param name="postcode">The postal code of the hotel's</param>
        /// <param name="country">The country the hotel is situated</param>
        /// <param name="rating">The rating of the hotel</param>
        /// <param name="telNo">The telephone number of the hotel</param>
        /// <param name="email">The email address of the hotel</param>
        /// <param name="employeeID">The employeeID of the user creating the record</param>
        /// <param name="connection">The database connection to use</param>
        /// <returns>
        ///     Returns the Indenity of the new hotel record (hotelID) if the record successfully added, else returns -1 if
        ///     hotel name already exists in the DB
        /// </returns>
        public int Add(string hotelName, string address1, string address2, string city, string county, string postcode,
            string country, byte rating, string telNo, string email, int employeeID, IDBConnection connection = null)
        {
            int hotelID;
            int userID = 0;
            var hotels = new Hotels();

            if (hotels.AlreadyExists(hotelName, connection))
            {
                return -1;
            }

            try
            {
                userID = currentUser.EmployeeID;
            }
            catch
            {
                userID = employeeID;
            }

            IDBConnection expData = connection ??
                                    new DatabaseConnection(
                                        ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            Sql =
                "insert into hotels (hotelname, address1, address2, city, county, postcode, country, rating, telno, email, createdon, createdby) " +
                "values (@hotelname, @address1, @address2, @city, @county, @postcode, @country, @rating, @telno, @email, @createdon, @createdby);set @identity = @@identity;";
            expData.sqlexecute.Parameters.AddWithValue("@hotelname", hotelName);
            expData.sqlexecute.Parameters.AddWithValue("@address1", address1);
            expData.sqlexecute.Parameters.AddWithValue("@address2", address2);
            expData.sqlexecute.Parameters.AddWithValue("@city", city);
            expData.sqlexecute.Parameters.AddWithValue("@county", county);
            expData.sqlexecute.Parameters.AddWithValue("@postcode", postcode);
            expData.sqlexecute.Parameters.AddWithValue("@country", country);
            expData.sqlexecute.Parameters.AddWithValue("@rating", rating);
            expData.sqlexecute.Parameters.AddWithValue("@telno", telNo);
            expData.sqlexecute.Parameters.AddWithValue("@email", email);
            expData.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expData.sqlexecute.Parameters.AddWithValue("@createdby", userID);
            expData.sqlexecute.Parameters.AddWithValue("@identity", SqlDbType.Int);
            expData.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.Output;
            expData.ExecuteSQL(Sql);
            hotelID = (int) expData.sqlexecute.Parameters["@identity"].Value;
            expData.sqlexecute.Parameters.Clear();

            return hotelID;
        }

        /// <summary>
        ///     Updates an existing hotel's details, providing  the hotel name does exist  in the DB.
        /// </summary>
        /// <param name="hotelName">The name of the hotel</param>
        /// <param name="address1">The first line of the hotel's address</param>
        /// <param name="address2">The second line of the hotel's address</param>
        /// <param name="city">The city the hotel is situated</param>
        /// <param name="county">The county the hotel is situated</param>
        /// <param name="postcode">The postal code of the hotel's</param>
        /// <param name="country">The country the hotel is situated</param>
        /// <param name="rating">The rating of the hotel</param>
        /// <param name="telNo">The telephone number of the hotel</param>
        /// <param name="email">The email address of the hotel</param>
        /// <param name="connection">The database connection to use</param>
        /// <returns>Returns 0 if signigying record has been added</returns>
        public int Update(int hotelID, string hotelName, string address1, string address2, string city, string county,
            string postcode, string country, byte rating, string telNo, string email, IDBConnection connection = null)
        {
            var hotels = new Hotels();

            IDBConnection expData = connection ??
                                    new DatabaseConnection(
                                        ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            Sql =
                "update hotels set hotelname = @hotelname, address1 = @address1, address2 = @address2, city = @city, county = @county, postcode = @postcode, country = @country, rating = @rating, telno = @telno, email = @email " +
                "where hotelid = @hotelid";
            expData.sqlexecute.Parameters.AddWithValue("@hotelid", hotelID);
            expData.sqlexecute.Parameters.AddWithValue("@hotelname", hotelName);
            expData.sqlexecute.Parameters.AddWithValue("@address1", address1);
            expData.sqlexecute.Parameters.AddWithValue("@address2", address2);
            expData.sqlexecute.Parameters.AddWithValue("@city", city);
            expData.sqlexecute.Parameters.AddWithValue("@county", county);
            expData.sqlexecute.Parameters.AddWithValue("@postcode", postcode);
            expData.sqlexecute.Parameters.AddWithValue("@country", country);
            expData.sqlexecute.Parameters.AddWithValue("@rating", rating);
            expData.sqlexecute.Parameters.AddWithValue("@telno", telNo);
            expData.sqlexecute.Parameters.AddWithValue("@email", email);
            expData.ExecuteSQL(Sql);
            expData.sqlexecute.Parameters.Clear();

            CacheRemove(hotelID);
            return 0;
        }

        /// <summary>
        ///     Deletes a hotel from the DB based on its ID.
        /// </summary>
        /// <param name="hotelID">The ID of the hotel</param>
        /// <param name="connection">The database connection to use</param>
        public void DeleteHotel(int hotelID, IDBConnection connection = null)
        {
            string Sql;
            IDBConnection expData = connection ??
                                    new DatabaseConnection(
                                        ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);

            Sql = "delete from hotels where hotelid = @hotelid";
            expData.sqlexecute.Parameters.AddWithValue("@hotelid", hotelID);
            expData.ExecuteSQL(Sql);
            expData.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        ///     Gets a hotel based on its ID.
        /// </summary>
        /// <param name="hotelID">The ID of the hotel</param>
        /// <param name="connection">The database connection to use</param>
        /// <returns>The <see cref="Hotel" />.</returns>
        public static Hotel Get(int hotelID, IDBConnection connection = null)
        {
            Hotel hotel;

            IDBConnection expData = connection ??
                                    new DatabaseConnection(
                                        ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            hotel = CacheGet(hotelID) ?? GetHotelFromDB(hotelID, connection);
            return hotel;
        }

        /// <summary>
        ///     Gets a hotel based on its name
        /// </summary>
        /// <param name="hotelName">The name of the hotel</param>
        /// <param name="connection">The database connection to use</param>
        /// <returns>The <see cref="Hotel" />.</returns>
        public static Hotel Get(string hotelName, IDBConnection connection = null)
        {
            Hotel hotel;
            int hotelID = 0;
            const string SqlNameCity = "select hotelid from hotels where (hotelname + ', ' + city) = @name";
            const string SqlName = "select hotelid from hotels where hotelname = @name";

            IDBConnection expData = connection ??
                                    new DatabaseConnection(
                                        ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            expData.sqlexecute.Parameters.AddWithValue("@name", hotelName);
            expData.sqlexecute.CommandText = SqlNameCity;

            using (IDataReader reader = expData.GetReader(SqlNameCity))
            {
                while (reader.Read())
                {
                    hotelID = reader.GetInt32(0);
                }
                reader.Close();
            }

            if (hotelID == 0)
            {
                using (IDataReader reader = expData.GetReader(SqlName))
                {
                    while (reader.Read())
                    {
                        hotelID = reader.GetInt32(0);
                    }
                    reader.Close();
                }
            }
            expData.sqlexecute.Parameters.Clear();
            hotel = Get(hotelID, connection);

            return hotel;
        }

        /// <summary>
        ///     Gets a hotel from the database.
        /// </summary>
        /// <param name="hotelID">The ID of the hotel</param>
        /// <param name="connection">The database connection to use</param>
        /// <returns>The <see cref="Hotel" />.</returns>
        private static Hotel GetHotelFromDB(int hotelID, IDBConnection connection = null,
            ICurrentUserBase currentUser = null)
        {
            Hotel hotel = null;
            DateTime createdon;
            int createdby;
            string hotelName, address1, address2, city, county, postcode, country, telNo, email;
            byte rating;
            const string Sql =
                "select hotelid, hotelname, address1, address2, city, county, postcode, country, rating, telno, email, CreatedOn, CreatedBy from dbo.hotels where hotelid = @hotelid";

            IDBConnection expData = connection ??
                                    new DatabaseConnection(
                                        ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
            expData.sqlexecute.Parameters.AddWithValue("@hotelid", hotelID);
            expData.sqlexecute.CommandText = Sql;

            using (IDataReader reader = expData.GetReader(Sql))
            {
                expData.sqlexecute.Parameters.Clear();

                if (reader.Read())
                {
                    hotelID = reader.GetInt32(reader.GetOrdinal("hotelid"));
                    hotelName = reader.GetString(reader.GetOrdinal("hotelname"));
                    if (reader.IsDBNull(reader.GetOrdinal("address1")))
                    {
                        address1 = "";
                    }
                    else
                    {
                        address1 = reader.GetString(reader.GetOrdinal("address1"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("address2")))
                    {
                        address2 = "";
                    }
                    else
                    {
                        address2 = reader.GetString(reader.GetOrdinal("address2"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("city")))
                    {
                        city = "";
                    }
                    else
                    {
                        city = reader.GetString(reader.GetOrdinal("city"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("county")))
                    {
                        county = "";
                    }
                    else
                    {
                        county = reader.GetString(reader.GetOrdinal("county"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("postcode")))
                    {
                        postcode = "";
                    }
                    else
                    {
                        postcode = reader.GetString(reader.GetOrdinal("postcode"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("country")))
                    {
                        country = "";
                    }
                    else
                    {
                        country = reader.GetString(reader.GetOrdinal("country"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("telno")))
                    {
                        telNo = "";
                    }
                    else
                    {
                        telNo = reader.GetString(reader.GetOrdinal("telno"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("email")))
                    {
                        email = "";
                    }
                    else
                    {
                        email = reader.GetString(reader.GetOrdinal("email"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("createdon")))
                    {
                        createdon = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("createdby")))
                    {
                        createdby = 0;
                    }
                    else
                    {
                        createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                    }
                    rating = reader.GetByte(reader.GetOrdinal("rating"));
                    hotel = new Hotel(hotelID, hotelName, address1, address2, city, county, postcode, country, rating,
                        telNo, email, createdon, createdby, currentUser);
                    CacheAdd(hotel);
                }
                reader.Close();
            }

            return hotel;
        }

        /// <summary>
        ///     Adds an instance of this hotel to the cache object.
        /// </summary>
        /// <param name="hotel">The hotel</param>
        public static void CacheAdd(Hotel hotel)
        {
            var caching = new Cache();
            caching.Add(0, cacheArea, hotel.hotelid.ToString(CultureInfo.InvariantCulture), hotel);
        }

        /// <summary>
        ///     Adds an instance of this hotel to the cache object.
        /// </summary>
        /// <param name="hotelID">The hotelID this hotel belongs to.</param>
        private static Hotel CacheGet(int hotelID)
        {
            var cache = new Cache();
            Hotel hotel;
            return hotel = (Hotel) cache.Get(0, cacheArea, hotelID.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Deletes this hotel from the cache object.
        /// </summary>
        /// <param name="hotelID">The hotelID this hotel belongs to.</param>
        public static void CacheRemove(int hotelID)
        {
            var caching = new Cache();
            caching.Delete(0, cacheArea, hotelID.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
