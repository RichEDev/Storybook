using System;
using System.Configuration;

using SpendManagementLibrary;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for cReviews.
    /// </summary>
    public class cReviews
    {
        private DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);

        private string strsql;

        public cReviews()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string getCountyGrid()
        {
            int i;
            cGridRow reqrow;
            strsql = "select county, count(*) as hotelcount, count(reviewid) as reviewcount from hotels left join hotel_reviews on hotel_reviews.hotelid = hotels.hotelid group by county order by county";
            System.Data.DataSet ds = expdata.GetDataSet(strsql);
            cGrid clsgrid = new cGrid(ds, true, false);

            clsgrid.getColumn("county").description = "County";
            clsgrid.getColumn("hotelcount").description = "Hotels";
            clsgrid.getColumn("reviewcount").description = "Reviews";
            //clsgrid.width = "99%";
            clsgrid.getColumn("hotelcount").width = "50";
            clsgrid.getColumn("reviewcount").width = "50";
            clsgrid.getColumn("hotelcount").align = "center";
            clsgrid.getColumn("reviewcount").align = "center";
            clsgrid.tblclass = "datatbl";
            clsgrid.getData();
            for (i = 0; i < clsgrid.gridrows.Count; i++)
            {
                reqrow = (cGridRow)clsgrid.gridrows[i];
                reqrow.getCellByName("county").thevalue = "<a href=\"reviews.aspx?county=" + reqrow.getCellByName("county").thevalue.ToString().Replace("&", "%26") + "\">" + reqrow.getCellByName("county").thevalue + "</a>";
            }
            return clsgrid.CreateGrid();
        }

        public string getCityGrid(string county)
        {
            int i;
            cGridRow reqrow;
            strsql = "select distinct county, city, count(*) as hotelcount, count(reviewid) as reviewcount from hotels left join hotel_reviews on hotel_reviews.hotelid = hotels.hotelid where county = @county group by county, city order by city";
            expdata.sqlexecute.Parameters.AddWithValue("@county", county);
            System.Data.DataSet ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            cGrid clsgrid = new cGrid(ds, true, false);
            clsgrid.tblclass = "datatbl";
            clsgrid.getColumn("city").description = "City / Location";
            clsgrid.getColumn("hotelcount").description = "Hotels";
            clsgrid.getColumn("reviewcount").description = "Reviews";
            //clsgrid.width = "99%";
            clsgrid.getColumn("hotelcount").width = "50";
            clsgrid.getColumn("reviewcount").width = "50";
            clsgrid.getColumn("hotelcount").align = "center";
            clsgrid.getColumn("reviewcount").align = "center";
            clsgrid.getColumn("county").hidden = true;
            clsgrid.getData();
            for (i = 0; i < clsgrid.gridrows.Count; i++)
            {
                reqrow = (cGridRow)clsgrid.gridrows[i];
                reqrow.getCellByName("city").thevalue = "<a href=\"reviews.aspx?county=" + reqrow.getCellByName("county").thevalue + "&city=" + reqrow.getCellByName("city").thevalue + "\">" + reqrow.getCellByName("city").thevalue + "</a>";
            }
            return clsgrid.CreateGrid();
        }

        public string getHotelGrid(string county, string city)
        {
            int i, x;
            byte stars = 0;
            int avgstars;
            cGridRow reqrow;
            strsql = "select hotelid, hotelname, rating, (select count(*) from hotel_reviews where hotelid = hotels.hotelid) as reviewcount, (select avg(rating) from hotel_reviews where hotelid = hotels.hotelid) as reviewavg  from hotels where county = @county and city = @city order by hotelname";
            expdata.sqlexecute.Parameters.AddWithValue("@county", county);
            expdata.sqlexecute.Parameters.AddWithValue("@city", city);
            System.Data.DataSet ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            cGrid clsgrid = new cGrid(ds, true, false);

            //clsgrid.width = "99%";
            clsgrid.getColumn("hotelid").hidden = true;
            clsgrid.getColumn("hotelname").description = "Hotel";
            clsgrid.getColumn("rating").description = "Official Rating";
            clsgrid.getColumn("reviewcount").description = "Reviews";
            clsgrid.getColumn("reviewavg").description = "Average Rating";
            clsgrid.getColumn("reviewcount").width = "50";
            clsgrid.getColumn("rating").width = "120";
            clsgrid.getColumn("reviewavg").width = "120";
            clsgrid.tblclass = "datatbl";
            clsgrid.getData();
            for (i = 0; i < clsgrid.gridrows.Count; i++)
            {
                reqrow = (cGridRow)clsgrid.gridrows[i];

                reqrow.getCellByName("hotelname").thevalue = "<a href=\"reviewdetails.aspx?hotelid=" + reqrow.getCellByName("hotelid").thevalue + "\">" + reqrow.getCellByName("hotelname").thevalue + "</a>";
                //rating
                if (reqrow.getCellByName("rating").thevalue != DBNull.Value)
                {
                    stars = (byte)reqrow.getCellByName("rating").thevalue;
                }
                else
                {
                    stars = 0;
                }
                if (stars == 0)
                {
                    reqrow.getCellByName("rating").thevalue = "-";
                }
                else
                {
                    reqrow.getCellByName("rating").thevalue = "";
                    for (x = 1; x <= stars; x++)
                    {
                        reqrow.getCellByName("rating").thevalue += "<img border=0 src=\"../icons/star_yellow.gif\">";
                    }
                }

                //avgrating
                if (reqrow.getCellByName("reviewavg").thevalue != DBNull.Value)
                {
                    avgstars = (int)reqrow.getCellByName("reviewavg").thevalue;
                }
                else
                {
                    avgstars = 0;
                }
                if (avgstars == 0)
                {
                    reqrow.getCellByName("reviewavg").thevalue = "-";
                }
                else
                {
                    reqrow.getCellByName("reviewavg").thevalue = "";
                    for (x = 1; x <= avgstars; x++)
                    {
                        reqrow.getCellByName("reviewavg").thevalue += "<img border=0 src=\"../icons/star_yellow.gif\">";
                    }
                }
            }
            return clsgrid.CreateGrid();
        }


        public string getReviewsGrid(int accountid, int hotelid)
        {


            string name = "";
            int i, x;
            byte displaytype, rating;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            System.Data.DataSet ds;
            strsql = "select hotel_reviews.* from hotel_reviews " + "where hotelid = @hotelid order by reviewdate desc";
            expdata.sqlexecute.Parameters.AddWithValue("@hotelid", hotelid);
            ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();

            cEmployees clsemployees = new cEmployees(accountid);
            cAccounts clsaccounts = new cAccounts();
            cAccount reqaccount;
            Employee reqemp;
            output.Append("<table width=70% bgcolor=#eeeeee>");
            for (i = 0; i < ds.Tables[0].Rows.Count; i++)
            {


                displaytype = (byte)ds.Tables[0].Rows[i]["displaytype"];
                switch (displaytype)
                {
                    case 1: //just name
                        reqemp = clsemployees.GetEmployeeById((int)ds.Tables[0].Rows[i]["employeeid"]);
                        name = reqemp.Title + " " + reqemp.Forename + " " + reqemp.Surname;
                        break;
                    case 2:
                        reqemp = clsemployees.GetEmployeeById((int)ds.Tables[0].Rows[i]["employeeid"]);
                        reqaccount = clsaccounts.GetAccountByID(accountid);
                        name = reqemp.Title + " " + reqemp.Forename + " " + reqemp.Surname + ", " + reqaccount.companyname;
                        break;
                    case 3:
                        name = "Anonymous Review";
                        break;
                }
                output.Append("<tr>");
                
                output.Append("<td style=\"border-top: 1px dashed #666;padding-left: 10px;\"><strong>Name:</strong>");
                output.Append("&nbsp;" + name + "</td>");
                output.Append("<td style=\"border-top: 1px dashed #666;padding-left: 10px;\"><strong>Overall Rating:&nbsp;</strong>");
                rating = (byte)ds.Tables[0].Rows[i]["rating"];
                output.Append("");
                for (x = 1; x <= rating; x++)
                {
                    output.Append("<img src=\"../icons/star_yellow.gif\" border=0>");
                }
                output.Append("</td>");
                output.Append("</tr>");
                output.Append("<tr>");
                output.Append("<td style=\"padding-left: 10px; padding-top: 10px;\"><table align=left>");
                //standard
                output.Append("<tr>");
                output.Append("<td>Standard of Rooms</td>");
                output.Append("<td style=\"padding-left: 10px;\">");
                rating = (byte)ds.Tables[0].Rows[i]["standardrooms"];
                for (x = 1; x <= rating; x++)
                {
                    output.Append("<img src=\"../icons/star_yellow.gif\" border=0>");
                }
                output.Append("</td>");
                output.Append("</tr>");
                //facilities
                output.Append("<tr>");
                output.Append("<td>Hotel Facilities</td>");
                output.Append("<td style=\"padding-left: 10px;\">");
                rating = (byte)ds.Tables[0].Rows[i]["hotelfacilities"];
                for (x = 1; x <= rating; x++)
                {
                    output.Append("<img src=\"../icons/star_yellow.gif\" border=0>");
                }
                output.Append("</td>");
                output.Append("</tr>");
                //value
                output.Append("<tr>");
                output.Append("<td>Value For Money</td>");
                output.Append("<td style=\"padding-left: 10px;\">");
                rating = (byte)ds.Tables[0].Rows[i]["valuemoney"];
                for (x = 1; x <= rating; x++)
                {
                    output.Append("<img src=\"../icons/star_yellow.gif\" border=0>");
                }
                output.Append("</td>");
                output.Append("</tr>");
                //performance
                output.Append("<tr>");
                output.Append("<td>Performance of Employees</td>");
                output.Append("<td style=\"padding-left: 10px;\">");
                rating = (byte)ds.Tables[0].Rows[i]["performancestaff"];
                for (x = 1; x <= rating; x++)
                {
                    output.Append("<img src=\"../icons/star_yellow.gif\" border=0>");
                }
                output.Append("</td>");
                output.Append("</tr>");
                //standard
                output.Append("<tr>");
                output.Append("<td>Hotel Location</td>");
                output.Append("<td style=\"padding-left: 10px;\">");
                rating = (byte)ds.Tables[0].Rows[i]["location"];
                for (x = 1; x <= rating; x++)
                {
                    output.Append("<img src=\"../icons/star_yellow.gif\" border=0>");
                }
                output.Append("</td>");
                output.Append("</tr>");
                output.Append("</table></td></tr>");
                output.Append("<tr><td colspan=4 style=\"border-bottom: 1px dashed #666;padding-left: 30px;padding-top: 10px;padding-bottom:10px;\"><strong>Comments:</strong>&nbsp;");
                output.Append(ds.Tables[0].Rows[i]["review"]);

                output.Append("</td>");
                output.Append("</tr>");

                if ((decimal)ds.Tables[0].Rows[i]["amountpaid"] > 0)
                {
                    decimal totalPaid = (decimal)ds.Tables[0].Rows[i]["amountpaid"];

                    output.Append("<tr><td colspan=4 style=\"border-bottom: 1px dashed #666;padding-left: 30px;padding-top: 10px;padding-bottom:10px;\"><strong>Amount Paid:</strong>&nbsp;" + totalPaid.ToString("###,###,##0.00") + "</td></tr>");

                }



            }

            output.Append("</table>");

            return output.ToString();

        }

        public void addReview(int hotelid, byte rating, string review, byte displaytype, int employeeid, decimal amountpaid, byte standardrooms, byte hotelfacilities, byte valuemoney, byte performancestaff, byte location)
        {
            strsql = "insert into hotel_reviews (hotelid, rating, review, displaytype, employeeid, amountpaid, standardrooms, hotelfacilities, valuemoney, performancestaff, location) " + "values (@hotelid, @rating, @review, @displaytype, @employeeid, @amountpaid, @standardrooms, @hotelfacilities, @valuemoney, @performancestaff, @location)";
            expdata.sqlexecute.Parameters.AddWithValue("@hotelid", hotelid);
            expdata.sqlexecute.Parameters.AddWithValue("@rating", rating);
            if (review.Length > 4000)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@review", review.Substring(0, 3999));
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@review", review);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@displaytype", displaytype);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
            expdata.sqlexecute.Parameters.AddWithValue("@amountpaid", amountpaid);
            expdata.sqlexecute.Parameters.AddWithValue("@standardrooms", standardrooms);
            expdata.sqlexecute.Parameters.AddWithValue("@hotelfacilities", hotelfacilities);
            expdata.sqlexecute.Parameters.AddWithValue("@valuemoney", valuemoney);
            expdata.sqlexecute.Parameters.AddWithValue("@performancestaff", performancestaff);
            expdata.sqlexecute.Parameters.AddWithValue("@location", location);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public bool alreadyReviewed(int hotelid, int employeeid)
        {
            int count = 0;
            strsql = "select count(*) from hotel_reviews where hotelid = @hotelid and employeeid = @employeeid";
            expdata.sqlexecute.Parameters.AddWithValue("@hotelid", hotelid);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
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
    }

    public class cReview
    {
        private int nReviewid;

        private int nHotelid;

        private byte bRating;

        private string sReview;

        private int nEmployeeid;

        private byte bDisplaytype;

        private decimal dAmountpaid;

        private DateTime dtReviewdate;

        public cReview(int reviewid, int hotelid, byte rating, string review, int employeeid, byte displaytype, decimal amountpaid, DateTime reviewdate)
        {
            nReviewid = reviewid;
            nHotelid = hotelid;
            bRating = rating;
            sReview = review;
            nEmployeeid = employeeid;
            bDisplaytype = displaytype;
            dAmountpaid = amountpaid;
            dtReviewdate = reviewdate;
        }

        #region properties

        public int reviewid
        {
            get
            {
                return nReviewid;
            }
        }

        public int hotelid
        {
            get
            {
                return nHotelid;
            }
        }

        public byte rating
        {
            get
            {
                return bRating;
            }
        }

        public string review
        {
            get
            {
                return sReview;
            }
        }

        public int employeeid
        {
            get
            {
                return nEmployeeid;
            }
        }

        public byte displaytype
        {
            get
            {
                return bDisplaytype;
            }
        }

        public decimal amountpaid
        {
            get
            {
                return dAmountpaid;
            }
        }

        public DateTime reviewdate
        {
            get
            {
                return dtReviewdate;
            }
        }

        #endregion
    }
}