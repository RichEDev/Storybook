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

using System.Collections.Generic;
using SpendManagementLibrary;

using Spend_Management;
using System.Text;

namespace Spend_Management
{
    using SpendManagementLibrary.Interfaces;

    /// <summary>
	/// Summary description for aecostcode.
	/// </summary>
	public partial class aecostcode : Page
	{
		protected System.Web.UI.WebControls.ImageButton cmdhelp;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            Master.helpid = 1014;
			
            CurrentUser user = cMisc.GetCurrentUser();
			if (IsPostBack == false)
			{
                cmdok.Attributes.Add("onclick", "if (validateform(null) == false) {return;}");
                Master.enablenavigation = false;
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CostCodes, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

			    cCostcodes clscostcodes = new cCostcodes((int)ViewState["accountid"]);
                
                int costcodeid = 0;
                if (Request.QueryString["costcodeid"] != null)
                {
                    costcodeid = Convert.ToInt32(Request.QueryString["costcodeid"]);
                }
                ViewState["costcodeid"] = costcodeid;
                if (costcodeid > 0)
                {
                    cCostCode reqcode;

                    reqcode = clscostcodes.GetCostcodeById(costcodeid);
                    if (reqcode == null)
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }

                    txtcostcode.Text = reqcode.Costcode;
                    txtdescription.Text = reqcode.Description;
                    string ownerDescription;
                    IOwnership currentOwnership = Ownership.Parse(user.AccountID, user.CurrentSubAccountId, GenerateOwnershipID(reqcode));
                    if (currentOwnership == null)
                    {
                        this.txtcostcodeowner_ID.Text = string.Empty;
                        this.txtcostcodeowner.Text = string.Empty;
                    }
                    else
                    {
                        this.txtcostcodeowner_ID.Text = currentOwnership.CombinedItemKey;
                        this.txtcostcodeowner.Text = currentOwnership.ItemDefinition();
                    }

                    ViewState["record"] = reqcode.UserdefinedFields;

                    Master.title = "Cost Code: " + reqcode.Costcode;
                }
                else
                {
                    Master.title = "Cost Code: New";
                }

                Master.PageSubTitle = "Cost Code Details";
			}

            this.BindOwnerAutoComplete(user);

            cUserdefinedFields clsuserdefined = new cUserdefinedFields((int)ViewState["accountid"]);
            cTables clstables = new cTables((int)ViewState["accountid"]);
            cTable tbl = clstables.GetTableByID(new Guid("02009E21-AA1D-4E0D-908A-4E9D73DDFBDF"));
            StringBuilder udfscript;
            clsuserdefined.createFieldPanel(ref holderUserdefined, clstables.GetTableByID(tbl.UserDefinedTableID), "", out udfscript);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "udfscript", udfscript.ToString(), true);

            if (ViewState["record"] != null)
            {
                clsuserdefined.populateRecordDetails(ref holderUserdefined, clstables.GetTableByID(tbl.UserDefinedTableID), (SortedList<int, object>)ViewState["record"]);
            }
            
		}

        /// <summary>
        /// Binds the jQuery autocomplete to the cost code owner text box
        /// </summary>
        /// <param name="user">Current User object</param>
        private void BindOwnerAutoComplete(CurrentUser user)
        {
            // bind the jQuery auto complete to the txtUser field
            cTables clsTables = new cTables(user.AccountID);
            List<object> acParams = AutoComplete.getAutoCompleteQueryParams("signoffentities");
            string acBindStr = AutoComplete.createAutoCompleteBindString("txtcostcodeowner", 15, clsTables.GetTableByName("signoffentities").TableID, (Guid)acParams[0], (List<Guid>)acParams[1], 500, keyFieldIsString: true);

            ClientScriptManager scmgr = this.ClientScript;
            scmgr.RegisterStartupScript(this.GetType(), "autocompleteBind", AutoComplete.generateScriptRegisterBlock(new List<string>() { acBindStr }), true);
        }

        /// <summary>
	    /// Sets the Cost Code owner value and returns text definition for display
	    /// </summary>
	    /// <param name="reqcode">Cost code object being edited</param>
	    /// <returns></returns>
	    private string GenerateOwnershipID(cCostCode reqcode)
	    {
	        if (reqcode == null)
	        {
	            return string.Empty;
	        }

	        if (reqcode.OwnerEmployeeId.HasValue)
	        {
                return ((int)SpendManagementElement.Employees).ToString() + "," + reqcode.OwnerEmployeeId.ToString();
	        }

	        if (reqcode.OwnerTeamId.HasValue)
	        {
                return ((int)SpendManagementElement.Teams).ToString() + "," + reqcode.OwnerTeamId.ToString();
	        }

	        if (reqcode.OwnerBudgetHolderId.HasValue)
	        {
                return ((int)SpendManagementElement.BudgetHolders).ToString() + "," + reqcode.OwnerBudgetHolderId.ToString();
	        }

            return string.Empty;
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
            string costcode;
            string description;
            int? ownerEmpId = null;
            int? ownerTeamId = null;
            int? ownerBudgetHolderId = null;

            if(!string.IsNullOrEmpty(txtcostcodeowner_ID.Text.Trim()))
            {
                string[] ownerSelection = txtcostcodeowner_ID.Text.Split(',');
                int elementVal = 0;

                if (ownerSelection.Length != 2 || !int.TryParse(ownerSelection[0], out elementVal))
                {
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('Invalid owner selection detected.');", true);
                    return;
                }

                int tmpId;
                if (!int.TryParse(ownerSelection[1], out tmpId))
                {
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('Invalid owner selection detected.');", true);
                    return;
                }

                switch ((SpendManagementElement)elementVal)
                {
                    case SpendManagementElement.Employees:
                        ownerEmpId = tmpId;
                        break;
                    case SpendManagementElement.BudgetHolders:
                        ownerBudgetHolderId = tmpId;
                        break;
                    case SpendManagementElement.Teams:
                        ownerTeamId = tmpId;
                        break;

                    default:
                        Page.ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('Invalid cost code owner type selection.');", true);
                        return;
                }
            }

            int costcodeid = (int)ViewState["costcodeid"];
            costcode = txtcostcode.Text;
            description = txtdescription.Text;
            
            cCostcodes costcodes = new cCostcodes((int)ViewState["accountid"]);

            cUserdefinedFields clsuserdefined = new cUserdefinedFields((int)ViewState["accountid"]);
            cTables clstables = new cTables((int)ViewState["accountid"]);
            cTable tbl = clstables.GetTableByID(new Guid("02009E21-AA1D-4E0D-908A-4E9D73DDFBDF"));
            SortedList<int, object> udf = clsuserdefined.getItemsFromPanel(ref holderUserdefined, clstables.GetTableByID(tbl.UserDefinedTableID));
            
            DateTime createdon;
            int createdby;
            int? modifiedby;
            DateTime? modifiedon;
            bool archived;

            if (costcodeid > 0)
            {
                cCostCode oldcode = costcodes.GetCostcodeById(costcodeid);
                createdon = oldcode.CreatedOn;
                createdby = oldcode.CreatedBy;
                modifiedby = (int)ViewState["employeeid"];
                modifiedon = DateTime.Now;
                archived = oldcode.Archived;
            }
            else
            {
                createdon = DateTime.Now;
                createdby = (int)ViewState["employeeid"];
                modifiedby = null;
                modifiedon = null;
                archived = false;
            }

            cCostCode clscostcode = new cCostCode(costcodeid, costcode, description, archived, createdon, createdby, modifiedon, modifiedby, udf, ownerEmpId, ownerTeamId, ownerBudgetHolderId);
            costcodeid = costcodes.SaveCostcode(clscostcode);

            switch (costcodeid)
            {
                case -1:
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('The costcode name you have entered already exists.');", true);
                    break;
                case -2:
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('The description you have entered already exists.');", true);
                    break;
                case -3:
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('Only one owner type can be specified for saving.');", true);
                    break;
                default:
                    Response.Redirect("admincostcodes.aspx", true);
                    break;
            }
        }

		private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Response.Redirect("admincostcodes.aspx",true);
		}
	}
}
