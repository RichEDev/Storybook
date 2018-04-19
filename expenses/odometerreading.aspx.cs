namespace expenses
{
    using System;
    using System.Web.UI;
    using System.Linq;

    using SpendManagementLibrary;

    using Spend_Management;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    /// <summary>
    /// Summary description for odometerreading.
    /// </summary>
    public partial class odometerreading : Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Monthly Odometer Readings";
            Master.title = Title;
			Master.showdummymenu = true;

            cmdok.Attributes.Add("onclick", "return ok_onclick();");
			if (IsPostBack == false)
			{
                byte odotype = byte.Parse(Request.QueryString["odotype"]);

                ViewState["odotype"] = odotype;
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cEmployeeCars clsEmployeeCars = new cEmployeeCars(user.AccountID, user.EmployeeID);

			    var generalOptions = this.GeneralOptionsFactory[user.CurrentSubAccountId].WithMileage();

                if (odotype == 2)
                {
                    int claimid;
                    int filter = int.Parse(this.Request.QueryString["filter"]);
                    claimid = int.Parse(Request.QueryString["claimid"]);
                    ViewState["filter"] = filter;
                    ViewState["claimid"] = claimid;
                    lblday.Text = "Please enter your current odometer reading before submitting your claim. \n\n";

                    
                    

                    cClaims claims = new cClaims(user.AccountID);
                    if (claims.IncludesFuelCardMileage(claimid))
                    {
                        lblReimburse.Style.Add("display", "");
                        lblReimburse.Text = "Please note that the value of your business mileage and personal mileage will now be calculated and displayed correctly once you’ve entered your closing odometer reading and submitted your claim.  The value of your business mileage will be shown for information only and the value of your personal mileage will be deducted from the amount payable, reducing your claim.";
                    }
                        

                    
                }
                else
                {
                    lblday.Text = "Please enter your odometer reading as of the " + generalOptions.Mileage.OdometerDay + CreateOrdinal(generalOptions.Mileage.OdometerDay) + " of the month.";
                }
				
				


				litcars.Text = generateOdoGrid();
				
				ClientScript.RegisterClientScriptBlock(this.GetType(), "cars", clsEmployeeCars.CreateClientOdometerArray(), true);
				
			}
		}

		private string generateOdoGrid()
		{
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			string rowclass = "row1";
            cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
            //cEmployee reqemp;
            //reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
            cEmployeeCars clsEmployeeCars = new cEmployeeCars((int)ViewState["accountid"], (int)ViewState["employeeid"]);
			int i;
            var cars = clsEmployeeCars.GetActiveCars().Where(e => e.fuelcard == true).ToList();

            cOdometerReading lastodo;
            DateTime startdate = DateTime.Today;
			output.Append("<table class=datatbl>");
			output.Append("<tr><th>Car</th><th>Last Odometer Reading</th><th>New Odometer Reading</th></tr>");
			for (i = 0; i < cars.Count; i++)
			{
				lastodo = cars[i].getLastOdometerReading();
				if (lastodo == null)
				{
                    lastodo = new cOdometerReading(0, 0, DateTime.Today, 0, 0, new DateTime(1900, 01, 01), 0);
				}
				output.Append("<tr>");
				output.Append("<td class=\"" + rowclass + "\">" + cars[i].make + " " + cars[i].model + " (" + cars[i].registration + ")</td>");
				output.Append("<td class=\"" + rowclass + "\">" + lastodo.newreading + "(" + lastodo.datestamp.ToShortDateString() + ")</td>");
                output.Append("<td class=\"" + rowclass + "\" style=\"width:50px;\"><input type=text id=\"newodo" + cars[i].carid + "\" name=\"newodo" + cars[i].carid + "\" style=\"width:50px;\"></td>");
				output.Append("</tr>");
                if (lastodo.datestamp < startdate)
                {
                    startdate = lastodo.datestamp;
                }
			}
			output.Append("</table>");
            if (clsemployees.getBusinessMiles((int)ViewState["employeeid"], startdate, DateTime.Today) == 0)
            {
                output.Append("<table>");
                output.Append("<tr><td class=\"labeltd\">Have you incurred business mileage since your last reading?</td><td class=\"inputtd\">Yes&nbsp;<input type=radio name=businessmileage id=\"businessmilesno\" value=\"0\"></td><td class=\"inputtd\">No&nbsp;<input type=radio name=businessmileage id=\"businessmilesyes\" value=\"1\"></td></tr>");
                output.Append("</table>");
            }
			return output.ToString();
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

		}
		#endregion

		protected void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            if ((byte)ViewState["odotype"] == 2)
            {
                Response.Redirect("expenses/claimViewer.aspx?claimid=" + ViewState["claimid"], true);
            }
            else
            {
                Response.Redirect("logon.aspx", true);
            }
		}

		protected void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
			//cEmployee reqemp;
			//reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
            cEmployeeCars clsEmployeeCars = new cEmployeeCars((int)ViewState["accountid"], (int)ViewState["employeeid"]);
			cOdometerReading reading;
			int i;
			int newreading;
            byte businessmiles = 2;
            var cars = clsEmployeeCars.GetActiveCars().Where(f => f.fuelcard == true).ToList();

            if (Request.Form["businessmileage"] != null)
            {
                businessmiles = byte.Parse(Request.Form["businessmileage"]);
            }
            for (i = 0; i < cars.Count; i++)
            {
				reading = cars[i].getLastOdometerReading();
				newreading = int.Parse(Request.Form["newodo" + cars[i].carid]);
				if (reading == null)
				{
                    clsemployees.saveOdometerReading(0, (int)ViewState["employeeid"], cars[i].carid, null, 0, newreading, businessmiles);
				}
				else
				{
                    clsemployees.saveOdometerReading(0, (int)ViewState["employeeid"], cars[i].carid, null, reading.newreading, newreading, businessmiles);
				}
			}

            if ((byte)ViewState["odotype"] == 1)
            {
                Session.Remove("OdometerReadingsOnLogon");
                Response.Redirect("home.aspx", true);
            }
            else
            {
                Response.Redirect("submitclaim.aspx?odotype=2&claimid=" + ViewState["claimid"] + "&filter=" + ViewState["filter"], true);
            }
		}

		public string CreateOrdinal (byte number)
		{
			string suffix;
			if (number >= 21)
			{
				switch (int.Parse(number.ToString().Substring(number.ToString().Length-1,1)))
				{
					case 1:
						suffix = "st";
						break;
					case 2:
						suffix = "nd";
						break;
					case 3:
						suffix = "rd";
						break;
					default:
						suffix = "th";
						break;
				}
			}
			else
			{
				switch (number)
				{
					case 1:
						suffix = "st";
						break;
					case 2:
						suffix = "nd";
						break;
					case 3:
						suffix = "rd";
						break;
					default:
						suffix = "th";
						break;
				}
			}

			return suffix;
		}

        

        
}
}
