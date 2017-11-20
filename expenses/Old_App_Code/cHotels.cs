using System;
using ExpensesLibrary;
using expenses.Old_App_Code;
using System.Web.Caching;
using System.Configuration;
using System.Collections.Generic;
using SpendManagementLibrary;

namespace expenses
{
	/// <summary>
	/// Summary description for cHotels.
	/// </summary>
	public class cHotels
	{
		DBConnection expdata = null;
		string strsql;
		int nAccountid = 0;

		System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;

        //public cHotels()
        //{

        //}
		public cHotels(int accountid)
		{
            nAccountid = accountid;
            expdata = new DBConnection(ConfigurationManager.ConnectionStrings["expenses_metabase"].ConnectionString);
		}

        public int accountid
        {
            get { return nAccountid; }
        }
		private bool alreadyExists(string hotelname, int hotelid, int action)
		{
			int count;
			if (action == 2)
			{
				strsql = "select count(*) from hotels where hotelname = @hotelname and hotelid <> @hotelid";
			}
			else
			{
				strsql = "select count(*) from hotels where hotelname = @hotelname";
			}
			expdata.sqlexecute.Parameters.AddWithValue("@hotelname",hotelname);
			expdata.sqlexecute.Parameters.AddWithValue("@hotelid",hotelid);
			count = expdata.getcount(strsql);
			expdata.sqlexecute.Parameters.Clear();

			if (count == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		public int addHotel (string hotelname, string address1, string address2, string city, string county, string postcode, string country, byte rating, string telno, string email, int employeeid)
		{
			if (alreadyExists(hotelname,0,0) == true)
			{
				return -1;
			}

            int userid = 0;

            try
            {
                CurrentUser user = new CurrentUser();
                user = cMisc.getCurrentUser(System.Web.HttpContext.Current.User.Identity.Name);
                userid = user.employeeid;
            }
            catch
            {
                userid = employeeid;
            }

			int hotelid;
			strsql = "insert into hotels (hotelname, address1, address2, city, county, postcode, country, rating, telno, email, createdon, createdby) " +
				"values (@hotelname, @address1, @address2, @city, @county, @postcode, @country, @rating, @telno, @email, @createdon, @createdby);set @identity = @@identity;";
			expdata.sqlexecute.Parameters.AddWithValue("@hotelname",hotelname);
			expdata.sqlexecute.Parameters.AddWithValue("@address1",address1);
			expdata.sqlexecute.Parameters.AddWithValue("@address2",address2);
			expdata.sqlexecute.Parameters.AddWithValue("@city",city);
			expdata.sqlexecute.Parameters.AddWithValue("@county",county);
			expdata.sqlexecute.Parameters.AddWithValue("@postcode",postcode);
			expdata.sqlexecute.Parameters.AddWithValue("@country",country);
			expdata.sqlexecute.Parameters.AddWithValue("@rating",rating);
			expdata.sqlexecute.Parameters.AddWithValue("@telno",telno);
			expdata.sqlexecute.Parameters.AddWithValue("@email",email);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now.ToUniversalTime());
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", userid);
			expdata.sqlexecute.Parameters.AddWithValue("@identity",System.Data.SqlDbType.Int);
			expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;
			expdata.ExecuteSQL(strsql);
			hotelid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
			expdata.sqlexecute.Parameters.Clear();

			return hotelid;
		}

		public int updateHotel (int hotelid, string hotelname, string address1, string address2, string city, string county, string postcode, string country, byte rating, string telno, string email)
		{
			if (alreadyExists(hotelname,hotelid,2) == true)
			{
				return -1;
			}
            CurrentUser user = new CurrentUser();
            user = cMisc.getCurrentUser(System.Web.HttpContext.Current.User.Identity.Name);
            int userid = user.employeeid;

			strsql = "update hotels set hotelname = @hotelname, address1 = @address1, address2 = @address2, city = @city, county = @county, postcode = @postcode, country = @country, rating = @rating, telno = @telno, email = @email " +
				"where hotelid = @hotelid";
			expdata.sqlexecute.Parameters.AddWithValue("@hotelid",hotelid);
			expdata.sqlexecute.Parameters.AddWithValue("@hotelname",hotelname);
			expdata.sqlexecute.Parameters.AddWithValue("@address1",address1);
			expdata.sqlexecute.Parameters.AddWithValue("@address2",address2);
			expdata.sqlexecute.Parameters.AddWithValue("@city",city);
			expdata.sqlexecute.Parameters.AddWithValue("@county",county);
			expdata.sqlexecute.Parameters.AddWithValue("@postcode",postcode);
			expdata.sqlexecute.Parameters.AddWithValue("@country",country);
			expdata.sqlexecute.Parameters.AddWithValue("@rating",rating);
			expdata.sqlexecute.Parameters.AddWithValue("@telno",telno);
			expdata.sqlexecute.Parameters.AddWithValue("@email",email);
            
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

			return 0;
		}

		public void deleteHotel (int hotelid)
		{
			strsql = "delete from hotels where hotelid = @hotelid";
			expdata.sqlexecute.Parameters.AddWithValue("@hotelid",hotelid);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		public cHotel getHotelById (int hotelid)
		{
			cHotel reqhotel = (cHotel)Cache["hotel" + hotelid];
			if (reqhotel == null)
			{
				reqhotel = getHotelFromDB(hotelid);
			}

			return reqhotel;
		}

        public cHotel getHotelByName(string name)
        {
            cHotel hotel;
            int hotelid = 0;
            System.Data.SqlClient.SqlDataReader reader;
            strsql = "select hotelid from hotels where (hotelname + ', ' + city) = @name";
            expdata.sqlexecute.Parameters.AddWithValue("@name", name);
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                hotelid = reader.GetInt32(0);
            }
            reader.Close();

            if (hotelid == 0) //try hotel name
            {
                strsql = "select hotelid from hotels where hotelname = @name";
                
                reader = expdata.GetReader(strsql);
                while (reader.Read())
                {
                    hotelid = reader.GetInt32(0);
                }
                reader.Close();
            }
            expdata.sqlexecute.Parameters.Clear();
            hotel = getHotelById(hotelid);

            return hotel;
        }
		private cHotel getHotelFromDB(int hotelid)
		{
			cHotel reqhotel = null;

            DateTime createdon;
            int createdby;
			string hotelname, address1, address2, city, county, postcode, country, telno, email;
			byte rating;
			System.Data.SqlClient.SqlDataReader reader;
			strsql = "select hotelid, hotelname, address1, address2, city, county, postcode, country, rating, telno, email, CreatedOn, CreatedBy from dbo.hotels where hotelid = @hotelid";
			expdata.sqlexecute.Parameters.AddWithValue("@hotelid",hotelid);
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
			reader = expdata.GetReader(strsql);
			expdata.sqlexecute.Parameters.Clear();

			while (reader.Read())
			{
				hotelid = reader.GetInt32(reader.GetOrdinal("hotelid"));
				hotelname = reader.GetString(reader.GetOrdinal("hotelname"));
				if (reader.IsDBNull(reader.GetOrdinal("address1")) == true)
				{
					address1 = "";
				}
				else
				{
					address1 = reader.GetString(reader.GetOrdinal("address1"));
				}
				if (reader.IsDBNull(reader.GetOrdinal("address2")) == true)
				{
					address2 = "";
				}
				else
				{
					address2 = reader.GetString(reader.GetOrdinal("address2"));
				}
				if (reader.IsDBNull(reader.GetOrdinal("city")) == true)
				{
					city = "";
				}
				else
				{
					city = reader.GetString(reader.GetOrdinal("city"));
				}
				if (reader.IsDBNull(reader.GetOrdinal("county")) == true)
				{
					county = "";
				}
				else
				{
					county = reader.GetString(reader.GetOrdinal("county"));
				}
				if (reader.IsDBNull(reader.GetOrdinal("postcode")) == true)
				{
					postcode = "";
				}
				else
				{
					postcode = reader.GetString(reader.GetOrdinal("postcode"));
				}
				if (reader.IsDBNull(reader.GetOrdinal("country")) == true)
				{
					country = "";
				}
				else
				{
					country = reader.GetString(reader.GetOrdinal("country"));
				}
				if (reader.IsDBNull(reader.GetOrdinal("telno")) == true)
				{
					telno = "";
				}
				else
				{
					telno = reader.GetString(reader.GetOrdinal("telno"));
				}
				if (reader.IsDBNull(reader.GetOrdinal("email")) == true)
				{
					email = "";
				}
				else
				{
					email = reader.GetString(reader.GetOrdinal("email"));
				}
                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                {
                    createdby = 0;
                }
                else
                {
                    createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                }
                
				rating = reader.GetByte(reader.GetOrdinal("rating"));
				reqhotel = new cHotel(hotelid, hotelname, address1, address2, city, county, postcode, country, rating,telno, email, createdon, createdby);
				Cache.Insert("hotel" + hotelid,reqhotel,dep,System.Web.Caching.Cache.NoAbsoluteExpiration,System.TimeSpan.FromMinutes(15), System.Web.Caching.CacheItemPriority.NotRemovable, null);
			}
			reader.Close();
			

			return reqhotel;
		}

		public object[] getNextHotel (string search)
		{
			object[] hotel = new object[2];
			System.Data.SqlClient.SqlDataReader reader;
			search += "%";

			strsql = "select top 1 hotelid, hotelname from hotels where hotelname like @hotel";
			expdata.sqlexecute.Parameters.AddWithValue("@hotel",search);
			reader = expdata.GetReader(strsql);
			expdata.sqlexecute.Parameters.Clear();
			while (reader.Read())
			{
				hotel[0] = reader.GetInt32(0);
				hotel[1] = reader.GetString(1);
			}
            reader.Close();
			return hotel;
		}
		public System.Data.DataSet getCountyList()
		{
			System.Data.DataSet ds = new System.Data.DataSet();
			strsql = "select distinct county from hotels order by county";
			ds = expdata.GetDataSet(strsql);
			return ds;
		}

		public System.Data.DataSet getCityList (string county)
		{
			System.Data.DataSet ds = new System.Data.DataSet();
			strsql = "select distinct city from hotels where county = @county order by city";
			expdata.sqlexecute.Parameters.AddWithValue("@county",county);
			ds = expdata.GetDataSet(strsql);
			expdata.sqlexecute.Parameters.Clear();
			return ds;
		}

		public System.Data.DataSet searchForHotels (string hotelname)
		{
			System.Data.DataSet ds;
			strsql = "select * from hotels where hotelname like @hotelname order by hotelname";
			expdata.sqlexecute.Parameters.AddWithValue("@hotelname",hotelname + "%");
			ds = expdata.GetDataSet(strsql);
			return ds;
		}

		public System.Data.DataSet searchForHotels (string county, string city)
		{
			System.Data.DataSet ds;
			if (city == "")
			{
				strsql = "select * from hotels where county =  @county order by hotelname";
			}
			else
			{
				strsql = "select * from hotels where county =  @county and city = @city order by hotelname";
				expdata.sqlexecute.Parameters.AddWithValue("@city",city);
			}
			expdata.sqlexecute.Parameters.AddWithValue("@county",county);
			ds = expdata.GetDataSet(strsql);
			return ds;
		}

		public void requestReview (int hotelid, int employeeid)
		{
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);


            if (clsproperties.sendreviewrequest)
            {
                cHotel reqhotel = getHotelById(hotelid);
                string message;
                cEmployees clsemployees = new cEmployees(accountid);
                cEmployee reqemp =  clsemployees.GetEmployeeById(employeeid);
                message = "Thank you for processing your hotel claim from your recent trip to " + reqhotel.hotelname;
                message += "\n\n";
                message += "expenses now includes an extensive hotel ratings service for locations in the UK.  This rating system will help all expense claimants find the most highly rated accommodation for business trips.";

                message += "\n\nTo rate the " + reqhotel.hotelname + ", simply follow this link:";
                message += "\n\n";
                message += "https://www.sel-expenses.com/information/addreview.aspx?hotelid=" + hotelid;
                //message += "http://localhost/expenses/information/addreview.aspx?close=1&hotelid=" + hotelid;
                message += "\n\n";
                message += "Rating your hotel will only take a moment, and the hotel ratings service will help you to find the best possible accommodation for future trips.";
                message += "\n\n";
                message += "Many thanks for taking part in the expenses(tm) hotel ratings service.";

                
                System.Web.Mail.MailMessage msg = new System.Web.Mail.MailMessage();
                msg.To = reqemp.email;

                msg.From = "expenses@software-europe.co.uk";
                msg.Subject = "Your recent trip to " + reqhotel.hotelname;
                msg.Body = message;

                

                System.Web.Mail.SmtpMail.SmtpServer = clsproperties.server;
                try
                {
                    System.Web.Mail.SmtpMail.Send(msg);
                }
                catch
                {
                    return;
                }

            }
		}

        public Dictionary<int, cHotel> getModifiedHotels(DateTime date)
        {
            Dictionary<int, cHotel> lst = new Dictionary<int, cHotel>();
            System.Data.SqlClient.SqlDataReader reader;
            int hotelid, createdby, modifiedby;
            byte rating;
            string hotelname, address1, address2, city, county, postcode, country, telNo, email;
            DateTime createdon, modifiedon;

            strsql = "SELECT * FROM hotels WHERE createdon > @date;";
            expdata.sqlexecute.Parameters.AddWithValue("@date", date);
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                hotelid = reader.GetInt32(reader.GetOrdinal("hotelid"));
                hotelname = reader.GetString(reader.GetOrdinal("hotelname"));
                if (reader.IsDBNull(reader.GetOrdinal("address1")) == false)
                {
                    address1 = reader.GetString(reader.GetOrdinal("address1"));
                }
                else
                {
                    address1 = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("address2")) == false)
                {
                    address2 = reader.GetString(reader.GetOrdinal("address2"));
                }
                else
                {
                    address2 = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("city")) == false)
                {
                    city = reader.GetString(reader.GetOrdinal("city"));
                }
                else
                {
                    city = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("county")) == false)
                {
                    county = reader.GetString(reader.GetOrdinal("county"));
                }
                else
                {
                    county = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("postcode")) == false)
                {
                    postcode = reader.GetString(reader.GetOrdinal("postcode"));
                }
                else
                {
                    postcode = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("country")) == false)
                {
                    country = reader.GetString(reader.GetOrdinal("country"));
                }
                else
                {
                    country = "";
                }

                rating = reader.GetByte(reader.GetOrdinal("rating"));

                if (reader.IsDBNull(reader.GetOrdinal("telno")) == false)
                {
                    telNo = reader.GetString(reader.GetOrdinal("telno"));
                }
                else
                {
                    telNo = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("email")) == false)
                {
                    email = reader.GetString(reader.GetOrdinal("email"));
                }
                else
                {
                    email = "";
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                {
                    createdby = 0;
                }
                else
                {
                    createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                }
                

                cHotel hotel = new cHotel(hotelid, hotelname, address1, address2, city, county, postcode, country, rating, telNo, email, createdon, createdby);
                lst.Add(hotel.hotelid, hotel);
            }
            reader.Close();
            return lst;
        }
	}

	
}
