using System;
using System.Collections.Generic;
using SpendManagementLibrary;


namespace Spend_Management
{
    using System.Data.SqlClient;

    /// <summary>
	/// Summary description for cP11dcats.
	/// </summary>
	public class cP11dcats
	{
		int accountid = 0;
		string strsql;
		DBConnection expdata;
		public cP11dcats(int nAccountid)
		{
			accountid = nAccountid;
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			InitialiseData();
		}
		private void InitialiseData()
		{
//			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;	
//			accountid = (int)appinfo.Session["accountid"];
		}
		

		public System.Data.DataSet getGrid()
		{
			
			System.Data.DataSet rcdsttemp = new System.Data.DataSet();
			strsql = "select pdcatid, pdname from pdcats order by pdname";
			rcdsttemp = expdata.GetDataSet(strsql);
			expdata.sqlexecute.Parameters.Clear();
			return rcdsttemp;
		}

        /// <summary>
        /// Returns all the pdCategories in a list.
        /// </summary>
        /// <returns></returns>
        public List<sP11dCat> GetAll()
        {
            var ds = getGrid();
            List<sP11dCat> list = new List<sP11dCat>();

            using (var reader = ds.CreateDataReader())
            {
                while (reader.Read())
                {
                    int idIndex = reader.GetOrdinal("pdcatid"),
                        nameIndex = reader.GetOrdinal("pdname");

                    var item = new sP11dCat
                    {
                        pdcatid = reader.GetInt32(idIndex),
                        pdname = reader.GetString(nameIndex)
                    };
                    list.Add(item);
                }
            }
            return list;
        }


		public System.Web.UI.WebControls.ListItem[] CreateDropDown(int pdcatid)
		{
			int count = 0;
			int i = 0;
			
			strsql = "select count(*) from pdcats";
			count = expdata.getcount(strsql);
			System.Web.UI.WebControls.ListItem[] items = new System.Web.UI.WebControls.ListItem[count + 1];

			strsql = "select * from pdcats order by pdname";
			items[0] = new System.Web.UI.WebControls.ListItem();
			i = 1;
		    using (SqlDataReader pdcatreader = expdata.GetReader(strsql))
		    {
		        while (pdcatreader.Read())
		        {
		            items[i] = new System.Web.UI.WebControls.ListItem();
		            items[i].Text = pdcatreader.GetString(pdcatreader.GetOrdinal("pdname"));
		            items[i].Value = pdcatreader.GetInt32(pdcatreader.GetOrdinal("pdcatid")).ToString();
		            if (pdcatreader.GetInt32(pdcatreader.GetOrdinal("pdcatid")) == pdcatid)
		            {
		                items[i].Selected = true;
		            }
		            i++;
		        }
		        pdcatreader.Close();
		    }
		    expdata.sqlexecute.Parameters.Clear();
			return items;
		}

		public int addP11dCat(string pdname, int[] subcatids)
		{
			if (checkExistance(pdname,0,0) == true)
			{
				return 1;
			}
			int pdcatid = 0;
			
			expdata.sqlexecute.Parameters.AddWithValue("@pdname",pdname);
			strsql = "insert into pdcats (pdname) " + 
				"values (@pdname)";
			expdata.ExecuteSQL(strsql);

			//asign to subcats
			strsql = "select pdcatid from pdcats where pdname = @pdname";
			pdcatid = expdata.getcount(strsql);
			assignToSubcats(subcatids,pdcatid);
			expdata.sqlexecute.Parameters.Clear();
			return 0;
		}

		public int updateP11dCat(string pdname, int pdcatid, int[] subcatids)
		{
			if (checkExistance(pdname,pdcatid,2) == true)
			{
				return 1;
			}
			expdata.sqlexecute.Parameters.AddWithValue("@pdname",pdname);
			expdata.sqlexecute.Parameters.AddWithValue("@pdcatid",pdcatid);
			strsql = "update pdcats set pdname = @pdname where pdcatid = @pdcatid";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
			assignToSubcats(subcatids,pdcatid);
			
			return 0;
		}

		private bool checkExistance (string pdname, int pdcatid, int action)
		{
			int count = 0;
			expdata.sqlexecute.Parameters.AddWithValue("@pdname",pdname);
			
			expdata.sqlexecute.Parameters.AddWithValue("@pdcatid",pdcatid);
			if (action == 2)
			{
				strsql = "select count(*) from pdcats where pdname = @pdname and pdcatid <> @pdcatid";
			}
			else
			{
				strsql = "select count(*) from pdcats where pdname = @pdname";
			}
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
		public int deleteP11dCat(int pdcatid)
		{
            cTables clsTables = new cTables(accountid);
            cTable p11dTable = clsTables.GetTableByName("pdcats");
            int retCode = 0;
            expdata.sqlexecute.Parameters.AddWithValue("@currentTableID", p11dTable.TableID);
            expdata.sqlexecute.Parameters.AddWithValue("@currentRecordID", pdcatid);
            expdata.sqlexecute.Parameters.Add("@retCode", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@retCode"].Direction = System.Data.ParameterDirection.ReturnValue;
            expdata.ExecuteProc("checkReferencedBy");
            retCode = (int)expdata.sqlexecute.Parameters["@retCode"].Value;

            if (retCode == 0)
            {
                expdata.sqlexecute.Parameters.Clear();

                expdata.sqlexecute.Parameters.AddWithValue("@pdcatid", pdcatid);
                strsql = "update subcats set pdcatid = null where pdcatid = @pdcatid";
                expdata.ExecuteSQL(strsql);

                strsql = "delete from pdcats where pdcatid = @pdcatid";
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }
            return retCode;
		}
											 
		private void assignToSubcats(int[] subcatids, int pdcatid)
		{
			expdata.sqlexecute.Parameters.AddWithValue("@pdcatid",pdcatid);
			strsql = "update subcats set pdcatid = null where pdcatid = @pdcatid";
			expdata.ExecuteSQL(strsql);

			if (subcatids.Length == 0) //no subcats, exit function
			{
				return;
			}

			int i = 0;
			
			strsql = "update subcats set pdcatid = " + pdcatid + " where ";
			for (i = 0; i < subcatids.Length; i++)
			{
				strsql = strsql + "subcatid = @subcatid" + i + " or ";
				expdata.sqlexecute.Parameters.AddWithValue("@subcatid" + i,subcatids[i]);
			}
			strsql = strsql.Remove(strsql.Length - 4,4);
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		public sP11dCat getP11dCatById(int pdcatid)
		{
			sP11dCat temp = new sP11dCat();
			expdata.sqlexecute.Parameters.AddWithValue("@pdcatid",pdcatid);
			strsql = "select * from pdcats where pdcatid = @pdcatid";
		    using (SqlDataReader pdcatreader = expdata.GetReader(strsql))
		    {
		        while (pdcatreader.Read())
		        {
		            temp.pdcatid = pdcatid;
		            temp.pdname = pdcatreader.GetString(pdcatreader.GetOrdinal("pdname"));
		        }
		        pdcatreader.Close();
		    }
		    expdata.sqlexecute.Parameters.Clear();

			return temp;

		}

        public sP11dCat getP11DByName(string name)
        {
            sP11dCat temp = new sP11dCat();
            expdata.sqlexecute.Parameters.AddWithValue("@pdname", name);
            strsql = "select pdname, pdcatid from pdcats where pdname = @pdname";
            using (SqlDataReader pdcatreader = expdata.GetReader(strsql))
            {
                while (pdcatreader.Read())
                {
                    temp.pdcatid = pdcatreader.GetInt32(pdcatreader.GetOrdinal("pdcatid"));
                    temp.pdname = pdcatreader.GetString(pdcatreader.GetOrdinal("pdname"));
                }
                pdcatreader.Close();
            }
            expdata.sqlexecute.Parameters.Clear();

            return temp;
        }

		public System.Data.DataSet getSubCatList(int pdcatid)
		{
			System.Data.DataSet temp = new System.Data.DataSet();
			
			expdata.sqlexecute.Parameters.AddWithValue("@pdcatid",pdcatid);
			if (pdcatid == 0)
			{
				strsql = "select subcat, subcatid, pdcatid from subcats where pdcatid is null order by subcat";
			}
			else
			{
				strsql = "select subcat, subcatid, pdcatid from subcats where (pdcatid = @pdcatid or pdcatid is null) order by subcat";
			}
			temp = expdata.GetDataSet(strsql);
			expdata.sqlexecute.Parameters.Clear();
			return temp;
		}
	}

	public struct sP11dCat
	{
		public int pdcatid;
		public string pdname;
	}
}
