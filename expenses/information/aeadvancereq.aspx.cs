namespace expenses.information
{
    using System;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    using Spend_Management;

    /// <summary>
	/// Summary description for aeadvancereq.
	/// </summary>
	public partial class aeadvancereq : Page
	{
		int floatid;

	    /// <summary>
	    /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
	    /// </summary>
        [Dependency]
	    public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        protected void Page_Load(object sender, EventArgs e)
		{
		    //Stops user accessing directly via url if no access
		    if (!(cMisc.GetCurrentUser().Employee.AdvancesSignOffGroup != 0 && cMisc.GetCurrentUser().Account.AdvancesEnabled))
		    {
		        Response.Redirect("http://" + Request.Url.Host + "/restricted.aspx");
		    }

            Title = "Add / Edit Advance Request";
            Master.title = Title;
			Master.showdummymenu = true;
            Master.helpid = 1162;

			
			if (IsPostBack == false)
			{
				int action = 0;
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

			    var clscurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
				
				if (Request.QueryString["action"] != null)
				{
					action = 2;
					decimal amount;
				    var clsfloats = new cFloats(user.AccountID);
					action = int.Parse(Request.QueryString["action"]);
					ViewState["action"] = action;
					floatid = int.Parse(Request.QueryString["floatid"]);
					ViewState["floatid"] = floatid;

					cFloat reqfloat = clsfloats.GetFloatById(this.floatid);
					txtname.Text = reqfloat.name;
					txtreason.Text = reqfloat.reason;

                    cmbcurrencies.Items.AddRange(clscurrencies.CreateDropDown(reqfloat.currencyid));

                    //if (reqfloat.currencyid == 0)
                    //{
                    //    amount = reqfloat.floatamount;
                    //}
                    //else
                    //{
                    //    if ((decimal)reqfloat.exchangerate != 0)
                    //    {
                    //        amount = reqfloat.floatamount / (decimal)reqfloat.exchangerate;
                    //    }
                    //    else
                    //    {
                    //        amount = reqfloat.floatamount;
                    //    }
                    //}

                    //txtamount.Text = amount.ToString("#,###,##0.00");

                    txtamount.Text = reqfloat.foreignAmount.ToString("#,###,##0.00");
				    if (reqfloat.requiredby == new DateTime(1900, 01, 01))
				    {
				        dtrequiredby.Text = string.Empty;
				    }
				    else
				    {
				        dtrequiredby.Text = reqfloat.requiredby.ToShortDateString();
				    }
                    this.comprequiredby.ValueToCompare = reqfloat.requiredby != new DateTime(01,01,01) ? reqfloat.requiredby.ToShortDateString() : DateTime.Today.ToShortDateString();

				}
				else
				{
					cmbcurrencies.Items.AddRange(clscurrencies.CreateDropDown(0));	

					comprequiredby.ValueToCompare = DateTime.Today.ToShortDateString();
				}
				ViewState["action"] = action;
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

		private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Response.Redirect("myadvances.aspx",true);
		}

		private void cmdok_Click(object sender, ImageClickEventArgs e)
		{
			var clsfloats = new cFloats((int)ViewState["accountid"]);
		    string requiredby = "";
			int basecurrency = 0;
			
			string name = this.txtname.Text;
			string reason = this.txtreason.Text;
			decimal amount = decimal.Parse(this.txtamount.Text);
			int currencyid = int.Parse(this.cmbcurrencies.SelectedValue);
			if (dtrequiredby.Text != "")
			{
				requiredby = dtrequiredby.Text;
			}

			var clsemployees = new cEmployees((int)ViewState["accountid"]);
            Employee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);

            if (reqemp.PrimaryCurrency != 0)
            {
                basecurrency = reqemp.PrimaryCurrency;
            }
            else
            {
                var generalOptions = this.GeneralOptionsFactory[cMisc.GetCurrentUser().CurrentSubAccountId].WithCurrency();

                if (generalOptions.Currency.BaseCurrency.HasValue)
                {
                    basecurrency = Convert.ToInt32(generalOptions.Currency.BaseCurrency);
                }
            }

            int saveFloatResponse;
			if ((int)ViewState["action"] == 2) //update
			{
                saveFloatResponse = clsfloats.updateFloat((int)ViewState["employeeid"], (int)ViewState["floatid"], name, reason, amount, currencyid, requiredby);
                if (saveFloatResponse == 1)
				{
					lblmsg.Text = "An exchange rate does not exist for the selected currency. Please consult your administrator";
					lblmsg.Visible = true;
					return;
				}

			    if (saveFloatResponse == 2)
			    {
			        this.lblmsg.Text = "An advance with this name already exists";
			        this.lblmsg.Visible = true;
			        return;
			    }
			}
			else
			{
                saveFloatResponse = clsfloats.requestFloat((int)ViewState["employeeid"], name, reason, amount, currencyid, requiredby, basecurrency);
                if (saveFloatResponse == 1)
				{
					lblmsg.Text = "An exchange rate does not exist for the selected currency. Please consult your administrator";
					lblmsg.Visible = true;
					return;
                }

			    if (saveFloatResponse == 2)
			    {
			        this.lblmsg.Text = "An advance with this name already exists";
			        this.lblmsg.Visible = true;
			        return;
			    }
			}
			

			Response.Redirect("myadvances.aspx",true);
		}
	}
}
