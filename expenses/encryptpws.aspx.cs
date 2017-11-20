using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using expenses.Old_App_Code;
using Spend_Management;
using SpendManagementLibrary;

namespace expenses
{
	/// <summary>
	/// Summary description for encryptpws.
	/// </summary>
	public partial class encryptpws : System.Web.UI.Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		protected void Button1_Click(object sender, System.EventArgs e)
		{
			DBConnection expdata;
			string companyid = "";
			int accountid = 0;
			int i = 0;
			int employeeid = 0;
			string oldpw = "";
			string newpw = "";
			string strsql;
            expdata = new DBConnection(cAccounts.getConnectionString(0));
			System.Data.DataSet rcdstpws = new DataSet();
			companyid = txtcompanyid.Text;
			strsql = "select accountid from registeredusers where companyid = '" + companyid + "'";
			accountid = expdata.getcount(strsql);

			strsql = "select password, employeeid from employees where accountid = " + accountid;
			rcdstpws = expdata.GetDataSet(strsql);

			for (i = 0; i < rcdstpws.Tables[0].Rows.Count; i++)
			{
				if (rcdstpws.Tables[0].Rows[i]["password"] != DBNull.Value)
				{
					oldpw = (string)rcdstpws.Tables[0].Rows[i]["password"];
					employeeid = (int)rcdstpws.Tables[0].Rows[i]["employeeid"];
					newpw = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(oldpw,"md5");
					strsql = "update employees set password = '" + newpw + "' where employeeid = " + employeeid;
					expdata.ExecuteSQL(strsql);
				}
			}

			Response.Write("Passwords encrypted");

		}
	}
}
