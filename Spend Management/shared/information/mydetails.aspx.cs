using System.IO;
using Spend_Management.shared.code;

namespace Spend_Management
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Web.UI;

	using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using System.Web.Services;
    using SpendManagementLibrary.Helpers;
    using System.Data;

	/// <summary>
	/// Summary description for mydetails.
	/// </summary>
	public partial class mydetails : Page
	{
        /// <summary>
        /// Help image button
        /// </summary>
		protected System.Web.UI.WebControls.ImageButton cmdhelp;
	
        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Title = "Change My Details";
            Master.PageSubTitle = Title;
            Master.UseDynamicCSS = true;
            if (IsPostBack == false)
            {
                List<string> jsGridObjects = new List<string>();

                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                ViewState["subAccountID"] = user.CurrentSubAccountId;

                switch (user.CurrentActiveModule)
                {
                    case Modules.contracts:
                        Master.helpid = 1161;
                        break;
                    default:
                        Master.helpid = 1167;
                        break;
                }

				if (user.CurrentActiveModule != Modules.expenses)
				{
                    var suScript = new StringBuilder();
					suScript.Append("if(document.getElementById('divCCBreakdown') != null) {document.getElementById('divCCBreakdown').style.display = 'none'; }");
                    suScript.Append("if(document.getElementById('divEmailNotification') != null) {document.getElementById('divEmailNotification').style.display = 'none'; }");
                    
                    if (user.CurrentActiveModule == Modules.Greenlight || user.CurrentActiveModule == Modules.GreenlightWorkforce)
					{						
						suScript.Append("if(document.getElementById('divClaimApproval') != null) {document.getElementById('divClaimApproval').style.display = 'none'; }");
					}
					else
					{
						suScript.Append("if(document.getElementById('divEmpDetails') != null) {document.getElementById('divEmpDetails').style.display = 'none'; }");
						suScript.Append("if(document.getElementById('divExpensesDetails') != null) {document.getElementById('divExpensesDetails').style.display = 'none'; }");
					}

					ClientScriptManager smgr = this.ClientScript;
					smgr.RegisterStartupScript(this.GetType(), "hidesections", suScript.ToString(), true);
				}

                cEmployees clsemployees = new cEmployees(user.AccountID);

                cGroups clsgroups = new cGroups(user.AccountID);

                cAccountSubAccounts subaccs = new cAccountSubAccounts(user.AccountID);
                cAccountProperties accProperties = subaccs.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;

                lblusername.Text = user.Employee.Username;
                lblusername.Enabled = false; // user can never modify their username
                txtTitle.Text = user.Employee.Title;
                txtfirstname.Text = user.Employee.Forename;
                txtsurname.Text = user.Employee.Surname;
                txttelno.Text = user.Employee.TelephoneNumber;
                txtemail.Text = user.Employee.EmailAddress;
                txtcreditor.Value = user.Employee.Creditor;
                txtpayroll.Value = user.Employee.PayrollNumber;
                txtposition.Value = user.Employee.Position;
                txtmileage.Text = clsemployees.getMileageTotal(user.Employee.EmployeeID, DateTime.Today).ToString();
                txtpersonalmiles.Text = clsemployees.getPersonalMiles(user.Employee.EmployeeID).ToString();
                txtextension.Text = user.Employee.TelephoneExtensionNumber;
                txtmobileno.Text = user.Employee.MobileTelephoneNumber;
                txtemailhome.Text = user.Employee.HomeEmailAddress;
                txtpagerno.Text = user.Employee.PagerNumber;
                chkNotifyUnsubmission.Checked = user.Employee.NotifyClaimUnsubmission;
               if (!user.isDelegate)
                {
                    litbankdetails.Text = PopulateBankList(user.Employee);
                }

                if (accProperties.EditMyDetails == false)
                {
                    txtfirstname.Enabled = false;
                    txtsurname.Enabled = false;
                    txttelno.Enabled = false;
                    txtemail.Enabled = false;
                    txtTitle.Enabled = false;
                    txtextension.Enabled = false;
                    txtmobileno.Enabled = false;
                    txtemailhome.Enabled = false;
                    txtpagerno.Enabled = false;
                    lblnamemsg.Visible = false;
                    if (user.CurrentActiveModule != Modules.expenses)
                    {
                        cmdok.Visible = false;
                    }
                }

                cGroup reqgroup = clsgroups.GetGroupById(user.Employee.SignOffGroupID);
                if (reqgroup != null)
                {
                    lblsignoff.Text = reqgroup.groupname;
                    createStagesGrid(user.AccountID, reqgroup);
                }
                else
                {
                    lblsignoff.Text = "No Sign-off Group";
                }
                
                string[] gridData = clsemployees.CreateMyDetailsCarGrid(user);
                litCars2.Text = gridData[2];

                // set the sel.grid javascript variables                
                jsGridObjects.Add(gridData[1]);
                                
                if (accProperties.MainAdministrator != 0 && accProperties.EmailServerAddress != "" && accProperties.AllowEmployeeToNotifyOfChangeOfDetails && !user.isDelegate)
                {
                    litnotifyadmin.Text = "If you believe that any of the details below are incorrect <a href=\"changeofdetails.aspx\">click here</a> to notify your administrator.";
                }

                ccb.EmptyValuesEnabled = true;

                foreach (cDepCostItem depCostItem in user.Employee.GetCostBreakdown())
                {
                    ccb.AddCostCentreBreakdownRow(depCostItem.departmentid, depCostItem.costcodeid, depCostItem.projectcodeid, depCostItem.percentused);
                }

                ccb.ReadOnly = true;

                pnlCCB.Controls.Add(ccb);

                if (user.Account.IsNHSCustomer)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div class=\"sectiontitle\">ESR Assignment Numbers</div>");
                    cESRAssignments assignments = new cESRAssignments(user.AccountID, user.EmployeeID);
                    string[] ESRAssignmentGridData = assignments.getCurrentAssignmentsGrid(false);
                    jsGridObjects.Add(ESRAssignmentGridData[1]);
                    
                    sb.Append(ESRAssignmentGridData[2]);

                    litESRAssignments.Text = sb.ToString();
                }
                if (!user.isDelegate)
                {
                    string[] homeAddrGrid = createHomeAddressesGrid();
                    lithomeaddresses.Text = homeAddrGrid[2];
                    jsGridObjects.Add(homeAddrGrid[1]);
                }
                divHomeAddress.Visible = !user.isDelegate;

                string[] workAddrGrid = createWorkAddressesGrid();
                litworkaddresses.Text = workAddrGrid[2];
                jsGridObjects.Add(workAddrGrid[1]);

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "MyDetailsGridVars", cGridNew.generateJS_init("MyDetailsGridVars", jsGridObjects, user.CurrentActiveModule), true);
            }
		}

        /// <summary>
        /// Creates the bank details panel
        /// </summary>
        /// <param name="reqemp">cEmployee record whose bank details are to be displayed</param>
        /// <returns>HTML data to display</returns>
		private string createBankDetails(Employee reqemp)
		{
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.Append("<div class=\"sectiontitle\">Employee Bank Details</div>");

            output.Append("<div class=\"twocolumn\">");
            output.Append("<label for=\"txtBankName\">Name</label><span class=\"inputs\"><input id=\"txtBankName\" type=\"text\" disabled=\"disabled\" value=\"" + reqemp.BankAccountDetails.AccountHolderName + "\" class=\"fillspan\" /></span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"></span>");
            output.Append("<label for=\"txtBankAccountNumber\">Account Number</label><span class=\"inputs\"><input id=\"txtBankAccountNumber\" type=\"text\" disabled=\"disabled\" value=\"" + reqemp.BankAccountDetails.AccountNumber + "\" class=\"fillspan\" /></span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"></span>");
            output.Append("</div>");
            output.Append("<div class=\"twocolumn\">");
            output.Append("<label for=\"txtBankAccountType\">Account Type</label><span class=\"inputs\"><input id=\"txtBankAccountType\" type=\"text\" disabled=\"disabled\" value=\"" + reqemp.BankAccountDetails.AccountType + "\" class=\"fillspan\" /></span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"></span>");
            output.Append("<label for=\"txtBankSortCode\">Sort Code</label><span class=\"inputs\"><input id=\"txtBankSortCode\" type=\"text\" disabled=\"disabled\" value=\"" + reqemp.BankAccountDetails.SortCode + "\" class=\"fillspan\" /></span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"></span>");
            output.Append("</div>");
            output.Append("<div class=\"twocolumn\">");
            output.Append("<label for=\"txtBankReference\">Reference</label><span class=\"inputs\"><input id=\"txtBankReference\" type=\"text\" disabled=\"disabled\" value=\"" + reqemp.BankAccountDetails.AccountReference + "\" class=\"fillspan\" /></span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"></span>");
            output.Append("</div>");

			return output.ToString();
		}

        /// <summary>
        /// Fill bank details in grid
        /// </summary>
        /// <param name="reqemp"></param>
        /// <returns>grid in form of html</returns>
        private string PopulateBankList(Employee reqemp)
        {
            CurrentUser reqCurrentUser = cMisc.GetCurrentUser();
            var clsFields = new cFields(reqemp.AccountID);

            var gridBankAccounts = new cGridNew(reqCurrentUser.AccountID, reqCurrentUser.EmployeeID, "myBankAccounts", "SELECT dbo.getDecryptedValue(BankAccounts.AccountName),dbo.getDecryptedValue(BankAccounts.AccountNumber),dbo.getAccountType(BankAccounts.AccountType),dbo.getCurrencyLabel(BankAccounts.CurrencyId),dbo.getDecryptedValue(BankAccounts.SortCode),dbo.getDecryptedValue(BankAccounts.Reference),dbo.getCountryLabel(BankAccounts.CountryId), BankAccountId,EmployeeId FROM BankAccounts");
            gridBankAccounts.addFilter(clsFields.GetFieldByID(new Guid("33873935-C9BC-4436-AD4C-3CF2120C7D4D")), ConditionType.Equals, new object[] { reqemp.EmployeeID }, null, ConditionJoiner.None);

            gridBankAccounts.KeyField = "EmployeeId";
            gridBankAccounts.getColumnByName("BankAccountId").hidden = true;
            gridBankAccounts.getColumnByName("EmployeeId").hidden = true;
            gridBankAccounts.editlink = "javascript:SEL.BankAccounts.LoadBankAccountModal(SEL.BankAccounts.LoadType.Edit, {BankAccountId});";
            gridBankAccounts.enableupdating = false;
            gridBankAccounts.enabledeleting = false;
            gridBankAccounts.EmptyText = "There are currently no bank accounts defined.";
            gridBankAccounts.SortedColumn = gridBankAccounts.getColumnByName("dbo.getDecryptedValue(BankAccounts.AccountName)");

            var output = new StringBuilder();
            output.Append("<div class=\"sectiontitle\">Employee Bank Details</div>");

            string[] gridData = gridBankAccounts.generateGrid();

            Page.ClientScript.RegisterStartupScript(this.GetType(), "BankGridVars",  "\n" + cGridNew.generateJS_init("BankAccountGridVars", new List<string> { gridData[0] }, reqCurrentUser.CurrentActiveModule), true);

            return gridData[1].ToString();
        }

        /// <summary>
        /// Creates the Home Address data panel
        /// </summary>
        /// <returns>HTML data to display</returns>
        private string[] createHomeAddressesGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>
            {
                new cFieldColumn(clsfields.GetFieldByID(new Guid("C1E0C8F9-4A1E-4194-BD1D-148CF222D958"))), // EmployeeHomeAddressId
                new cFieldColumn(clsfields.GetFieldByID(new Guid("E1DF7EB9-B119-4A58-8485-FC901F3D5440"))), // StartDate
                new cFieldColumn(clsfields.GetFieldByID(new Guid("F5883185-4BA8-4EC7-A78C-EE114AFEC52F"))), // EndDate
                new cFieldColumn(clsfields.GetFieldByID(new Guid("76AED3E5-33BC-4547-BD30-A5E8758AB653"))), // Line1
                new cFieldColumn(clsfields.GetFieldByID(new Guid("E76A3C68-833A-4E6F-85B5-29C8EF357C50"))), // City
                new cFieldColumn(clsfields.GetFieldByID(new Guid("6A6F8977-2D38-4BFA-ADA3-A6BCA273F3ED"))) // Postcode
            };

            cGridNew grid = new cGridNew(user.AccountID, user.EmployeeID, "gridHomeAddresses", clstables.GetTableByID(new Guid("F999A9A6-3F66-43D7-A8F7-6B92C1649825")), columns);
            grid.addFilter(clsfields.GetFieldByID(new Guid("730BE26D-FDF4-40EC-BB6E-2148AC1F674C")), ConditionType.Equals, new object[] { user.EmployeeID }, null, ConditionJoiner.None);
            grid.KeyField = "EmployeeHomeAddressId";
            grid.getColumnByName("EmployeeHomeAddressId").hidden = true;
            List<string> retVals = new List<string>();
            retVals.Add(grid.GridID);
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();                
        }

        /// <summary>
        /// Creates the Work Address data panel
        /// </summary>
        /// <returns>HTML data to display</returns>
        private string[] createWorkAddressesGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cTables clstables = new cTables(user.AccountID);
            cFields clsfields = new cFields(user.AccountID);
            List<cNewGridColumn> columns = new List<cNewGridColumn>
            {
                new cFieldColumn(clsfields.GetFieldByID(new Guid("f5acae20-9dd6-4e64-9b1e-740013e7e586"))), // EmployeeWorkAddressId
                new cFieldColumn(clsfields.GetFieldByID(new Guid("39cf4e42-97a6-442d-bd00-28b271f04057"))), // StartDate
                new cFieldColumn(clsfields.GetFieldByID(new Guid("66cb2f1a-0b37-41b9-ba41-c6b6c76e72a2"))), // EndDate
                new cFieldColumn(clsfields.GetFieldByID(new Guid("F35B6561-911A-4BFE-AE2C-85BC2B031920"))), // Line1
                new cFieldColumn(clsfields.GetFieldByID(new Guid("C3F899E2-6E27-4E35-82E8-2A72F1A437FB"))), // City 
                new cFieldColumn(clsfields.GetFieldByID(new Guid("3CC0D44F-DE41-4C24-9990-1E24A35799DB"))) // Postcode
            };

            cGridNew grid = new cGridNew(user.AccountID, user.EmployeeID, "gridWorkAddresses", clstables.GetTableByID(new Guid("2330AED0-18C0-4F69-AFF7-77BE9F041C92")), columns);
            grid.addFilter(clsfields.GetFieldByID(new Guid("0cb32ebc-aaa5-4b96-bb04-1de4e3985b70")), ConditionType.Equals, new object[] { user.EmployeeID }, null, ConditionJoiner.None);
            grid.KeyField = "EmployeeWorkAddressId";
            grid.getColumnByName("EmployeeWorkAddressId").hidden = true;
            List<string> retVals = new List<string>();
            retVals.Add(grid.GridID);
            retVals.AddRange(grid.generateGrid());
            return retVals.ToArray();
        }

        /// <summary>
        /// Creates the sign off stage data panel
        /// </summary>
        /// <param name="reqgroup">Sign-off group to display stages for</param>
		private void createStagesGrid(int accountID, cGroup reqgroup)
		{
            cBudgetholders clsbudgetholders = new cBudgetholders(accountID);
            cGroups clsgroups = new cGroups(accountID);
            cEmployees clsemployees = new cEmployees(accountID);
			int i;
            cGrid stagesgrid = new cGrid(accountID, clsgroups.getStagesGrid(reqgroup), true, false, Grid.UserStages);

            const string signoffTypeColum = "signofftype";

            stagesgrid.tblclass = "cGrid";
			stagesgrid.getColumn("signoffid").hidden = true;
			stagesgrid.getColumn("stage").description = "Stage";
            stagesgrid.getColumn(signoffTypeColum).description = "Type";
			stagesgrid.getColumn("relid").description = "User/Team/Budget<br>Holder";
			stagesgrid.getColumn("include").description = "Include Type";
			stagesgrid.getColumn("notify").description = "Action";
			stagesgrid.getColumn("amount").hidden = true;
            stagesgrid.getColumn("cleanupstage").hidden = true;
            stagesgrid.getColumn("allocateforpayment").hidden = true;

            stagesgrid.getColumn(signoffTypeColum).listitems.addItem((byte)1, SignoffType.CostCodeOwner.GetDisplayValue());
            stagesgrid.getColumn(signoffTypeColum).listitems.addItem((byte)2, SignoffType.Employee.GetDisplayValue());
            stagesgrid.getColumn(signoffTypeColum).listitems.addItem((byte)3, SignoffType.Team.GetDisplayValue());
            stagesgrid.getColumn(signoffTypeColum).listitems.addItem((byte)4, SignoffType.LineManager.GetDisplayValue());
            stagesgrid.getColumn(signoffTypeColum).listitems.addItem((byte)5, SignoffType.ClaimantSelectsOwnChecker.GetDisplayValue());
            stagesgrid.getColumn(signoffTypeColum).listitems.addItem((byte)6, SignoffType.ApprovalMatrix.GetDisplayValue());
            stagesgrid.getColumn(signoffTypeColum).listitems.addItem((byte)7, SignoffType.DeterminedByClaimantFromApprovalMatrix.GetDisplayValue());
            stagesgrid.getColumn(signoffTypeColum).listitems.addItem((byte)8, SignoffType.CostCodeOwner.GetDisplayValue());
            stagesgrid.getColumn(signoffTypeColum).listitems.addItem((byte)9, SignoffType.AssignmentSignOffOwner.GetDisplayValue());
            stagesgrid.getColumn(signoffTypeColum).listitems.addItem((byte)100, SignoffType.SELScanAttach.GetDisplayValue());
            stagesgrid.getColumn(signoffTypeColum).listitems.addItem((byte)101, SignoffType.SELValidation.GetDisplayValue());

            stagesgrid.getColumn("notify").listitems.addItem((int)1, "Stage is notified of claim");
            stagesgrid.getColumn("notify").listitems.addItem((int)2, "Stage is to check claim");
						
			for (i = 0; i < stagesgrid.gridrows.Count; i++)
			{
				cGridRow reqrow = (cGridRow)stagesgrid.gridrows[i];
				int relid = (int)reqrow.getCellByName("relid").thevalue;
			    Employee reqemployee;
				switch ((byte)reqrow.getCellByName("signofftype").thevalue)
				{
				    case 1:
				        int employeeid = 0;

				        cBudgetHolder reqholder = clsbudgetholders.getBudgetHolderById(relid);
				        employeeid = reqholder.employeeid;
				        reqemployee = clsemployees.GetEmployeeById(employeeid);
				        reqrow.getCellByName("relid").thevalue = reqholder.budgetholder + " (" + reqemployee.Surname + ", " +
				                                                 reqemployee.Title + " " + reqemployee.Forename + ")";
				        break;
				    case 2:
				        reqemployee = clsemployees.GetEmployeeById(relid);
				        reqrow.getCellByName("relid").thevalue = reqemployee.Surname + ", " + reqemployee.Title + " " +
				                                                 reqemployee.Forename;
				        break;
				    case 3:

				        cTeams clsteams;
                        clsteams = new cTeams((int)ViewState["accountid"]);
				
				        cTeam reqteam = clsteams.GetTeamById(relid);
				        reqrow.getCellByName("relid").thevalue = reqteam.teamname;
				        break;
                    case 4:
                        var currentUser = cMisc.GetCurrentUser();
                        var currentEmployeeLineManager = clsemployees.GetEmployeeById(currentUser.EmployeeID).LineManager;
                        if(currentEmployeeLineManager != 0)
                        {
                            reqemployee = clsemployees.GetEmployeeById(currentEmployeeLineManager);
                            reqrow.getCellByName("relid").thevalue =string.Format("{0}, {1} {2}", reqemployee.Surname,reqemployee.Title,reqemployee.Forename) ;
                        }
                        else { reqrow.getCellByName("relid").thevalue = string.Empty; }
                            break;
                }
			    switch ((int)reqrow.getCellByName("include").thevalue)
				{
					case 1:
						
						reqrow.getCellByName("include").thevalue = "Always include stage";
						break;
					case 2:
						reqrow.getCellByName("include").thevalue = "Only include stage if claim amount is > " + reqrow.getCellByName("amount").thevalue;
						break;
					case 3:
						reqrow.getCellByName("include").thevalue = "Only if an item exceeds allowed amount";
						break;
					case 4:
						reqrow.getCellByName("include").thevalue = "Only if claim includes specified cost code";
						break;
                    case 5:
                        reqrow.getCellByName("include").thevalue = "Only include stage if claim amount is < " + reqrow.getCellByName("amount").thevalue;
                        break;
                    case 6:
                        reqrow.getCellByName("include").thevalue = "Only if claim include specified expense item";
                        break;
                    case 7:
                        reqrow.getCellByName("include").thevalue = "Only if claim includes an expense item older than " + reqrow.getCellByName("amount").thevalue + " days";
                        break;
				}
			}
            
            litstages.Text = stagesgrid.CreateGrid();
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

        /// <summary>
        /// Update button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            CurrentUser user = cMisc.GetCurrentUser();
            cAccountSubAccounts subaccs = new cAccountSubAccounts((int)ViewState["accountid"]);
            cAccountProperties accProperties = subaccs.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;

			if (accProperties.EditMyDetails == false && user.CurrentActiveModule != Modules.expenses)
			{
                return;               
			}
			
            user.Employee.Title = txtTitle.Text.Trim();
            user.Employee.Forename = txtfirstname.Text;
            user.Employee.Surname = txtsurname.Text;
            user.Employee.TelephoneNumber = txttelno.Text;
            user.Employee.EmailAddress = txtemail.Text;
            user.Employee.HomeEmailAddress = txtemailhome.Text;
            user.Employee.MobileTelephoneNumber = txtmobileno.Text;
            user.Employee.PagerNumber = txtpagerno.Text;
            user.Employee.TelephoneExtensionNumber = txtextension.Text;
            user.Employee.NotifyClaimUnsubmission = chkNotifyUnsubmission.Checked;
            user.Employee.Save(user);
         
			lblmsg.Text = "Your details have been updated successfully.";
			lblmsg.Visible = true;

            switch (user.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=mydetails", true);
                    break;
                default:
                    Response.Redirect("~/mydetailsmenu.aspx", true);
                    break;
            }
        }

        /// <summary>
        /// Change password link event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected void cmbchangep_Click(object sender, System.EventArgs e)
		{
            Response.Redirect("~/shared/changepassword.aspx?returnto=2&employeeid=" + ViewState["employeeid"], true);
		}

        /// <summary>
        /// Cancel button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            CurrentUser user = cMisc.GetCurrentUser();
            switch (user.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=mydetails", true);
                    break;
                default:
                    Response.Redirect("~/mydetailsmenu.aspx", true);
                    break;
            }
		}
	}
}
