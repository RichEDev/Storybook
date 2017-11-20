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

using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
    using SpendManagementLibrary.Employees;

    /// <summary>
	/// Summary description for aefloat.
	/// </summary>
	public partial class aefloat : Page
	{
		protected System.Web.UI.WebControls.RadioButton optall;
		protected System.Web.UI.WebControls.RadioButton optselected;
	
		
		string action;
		int floatid = 0;
		
		protected System.Web.UI.WebControls.ImageButton cmdhelp;
		cFloats clsfloats;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Add / Edit Advance";
            Master.title = Title;
			Master.showdummymenu = true;
            Master.helpid = 1055;

			CurrentUser user = cMisc.GetCurrentUser();
            cEmployees clsemployees = new cEmployees(user.AccountID);
            string varScript = clsemployees.createEmployeeControl(ref placeEmp, "floatemployeeid", "floatemployee", EmployeeAreaType.FloatEmployee, false);
            ClientScript.RegisterClientScriptBlock(this.GetType(), "script", varScript, true);

			if (IsPostBack == false)
			{
                Master.enablenavigation = false;
                
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Advances, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

				cCurrencies clscurrencies;

                clscurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);

				if (Request.QueryString["action"] != null)
				{
					action = Request.QueryString["action"];
		
				}


				if (action == "2") //update
				{
					txtaction.Text = "2";
					txtfloatid.Text = Request.QueryString["floatid"];
					cFloat reqfloat;

                    clsfloats = new cFloats(user.AccountID);
					
					
					floatid = int.Parse(Request.QueryString["floatid"]);
					txtfloatid.Text = floatid.ToString();
					reqfloat = clsfloats.GetFloatById(floatid);

					txtamount.Text = reqfloat.floatamount.ToString("######0.00");
					txtname.Text = reqfloat.name;
					txtreason.Text = reqfloat.reason;
					cmbcurrencies.Items.AddRange(clscurrencies.CreateDropDown(reqfloat.currencyid));
                    //cmbemployees.Items.AddRange(clsemployees.CreateDropDown(reqfloat.employeeid, false));

                    DropDownList ddlFloatEmp = (DropDownList)placeEmp.FindControl("cmbfloatemployee");

                    if (ddlFloatEmp != null)
                    {
                        if (ddlFloatEmp.Items.FindByValue(reqfloat.employeeid.ToString()) != null)
                        {
                            ddlFloatEmp.Items.FindByValue(reqfloat.employeeid.ToString()).Selected = true;
                        }
                    }

                    TextBox txtFloatEmp = (TextBox)placeEmp.FindControl("txtfloatemployee");

                    if (txtFloatEmp != null)
                    {
                        if (reqfloat.employeeid > 0)
                        {
                            Employee tempEmp = clsemployees.GetEmployeeById(reqfloat.employeeid);

                            txtFloatEmp.Text = tempEmp.Surname + ", " + tempEmp.Forename + " [" + tempEmp.Username + "]";
                        }
                    }

                    TextBox txtFloatEmpID = (TextBox)placeEmp.FindControl("txtfloatemployeeid");

                    if (txtFloatEmpID != null)
                    {
                        txtFloatEmpID.Text = reqfloat.employeeid.ToString();
                    }
				}
				else
				{
					cmbcurrencies.Items.AddRange(clscurrencies.CreateDropDown(0));
				}
			}
			
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
			this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);
			this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

		}
		#endregion

		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		    int employeeid = 0;

		    int basecurrency;

            DropDownList ddlFloatEmp = (DropDownList)placeEmp.FindControl("cmbfloatemployee");

            if (ddlFloatEmp != null)
            {
                employeeid = int.Parse(ddlFloatEmp.SelectedItem.Value);
            }

            TextBox txtFloatEmpID = (TextBox)placeEmp.FindControl("txtfloatemployeeid");

            if (txtFloatEmpID != null)
            {
                int.TryParse(txtFloatEmpID.Text, out employeeid);
            }

			int currencyid = int.Parse(this.cmbcurrencies.SelectedItem.Value);
			decimal amount = decimal.Parse(this.txtamount.Text);

			string name = this.txtname.Text;
			string reason = this.txtreason.Text;

			clsfloats = new cFloats((int)ViewState["accountid"]);

            cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
            Employee reqemp = clsemployees.GetEmployeeById(employeeid);

            if (reqemp.PrimaryCurrency != 0)
            {
                basecurrency = reqemp.PrimaryCurrency;
            }
            else
            {
                cMisc clsmisc = new cMisc((int)ViewState["accountid"]);
                cGlobalProperties clsproperties = clsmisc.GetGlobalProperties((int)ViewState["accountid"]);
                basecurrency = clsproperties.basecurrency;
            }
			
            clsfloats.requestFloat(employeeid,name,reason,amount,currencyid,"", basecurrency);

		    Response.Redirect("adminfloats.aspx");
		}

        private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Response.Redirect("adminfloats.aspx",true);
		}
	}
}
