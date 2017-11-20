using System;
using System.Collections.Generic;
using System.Web.UI;

using SpendManagementLibrary;

using Spend_Management;
using Spend_Management.expenses.code.Claims;

namespace expenses
{
	using System.Globalization;

	/// <summary>
	/// Summary description for aeclaim.
	/// </summary>
	public partial class aeclaim : Page
	{

		string action = "";
		protected System.Web.UI.WebControls.ImageButton ImageButton2;
		int claimId;

		protected void Page_Load(object sender, EventArgs e)
		{

			Title = "Add / Edit Claim";
			Master.title = Title;
			Master.helpid = 1173;

			if (IsPostBack == false)
			{

				Response.Buffer = true;
				Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
				Response.Expires = 0;
				Response.CacheControl = "no-cache";

				CurrentUser user = cMisc.GetCurrentUser();
				ViewState["accountid"] = user.AccountID;
				ViewState["employeeid"] = user.EmployeeID;

				var claims = new cClaims(user.AccountID);

				if (Request.QueryString["action"] != null)
				{
					action = Request.QueryString["action"];
					ViewState["action"] = action;
				}
				if (Request.QueryString["returnto"] != null)
				{
					ViewState["returnto"] = int.Parse(Request.QueryString["returnto"]);
				}
				else
				{
					ViewState["returnto"] = 0;
				}

				if (action == "2") //edit
				{
					claimId = int.Parse(Request["claimid"]);
					cClaim claim = claims.getClaimById(this.claimId);

					if (claim == null)
					{
						Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
					}

					ViewState["claimid"] = claimId;
					txtaction.Text = "2";
					txtclaimid.Text = claimId.ToString(CultureInfo.InvariantCulture);
					if (claim != null)
					{
						this.txtname.Text = claim.name;
						this.txtdescription.Text = claim.description;
					}
				}
				else
				{
				    txtname.Text = claims.GenerateNewClaimName(user);
				}
			}

			var userdefinedFields = new cUserdefinedFields((int)ViewState["accountid"]);
			var table = new cTables((int)ViewState["accountid"]);
			cTable userdefinedClaimsTable = table.GetTableByID(new Guid("f70d6e0d-8e38-4a1d-a681-cc9d310c2ae9"));
			var fields = new cFields((int)ViewState["accountid"]);
			var clsnewuserdefined = new cUserdefinedFields((int)ViewState["accountid"]);
			SortedList<int, object> userdefinedList = userdefinedFields.GetRecord(userdefinedClaimsTable, claimId, table, fields);

			if (ViewState["action"] != null)
			{
				clsnewuserdefined.addItemsToPage(ref tbludf, userdefinedClaimsTable, false, "", userdefinedList, null, string.Empty);

			}
			else
			{
				clsnewuserdefined.addItemsToPage(ref tbludf, userdefinedClaimsTable, false, "", null, null, string.Empty);

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
			this.ImageButton1.Click += new System.Web.UI.ImageClickEventHandler(this.ImageButton1_Click);

		}
		#endregion

		private void cmdok_Click(object sender, ImageClickEventArgs e)
		{
			string name = this.txtname.Text;
			string description = this.txtdescription.Text;

			action = txtaction.Text;

			var claims = new cClaims((int)ViewState["accountid"]);
			var userdefinedFields = new cUserdefinedFields((int)ViewState["accountid"]);
			var tables = new cTables((int)ViewState["accountid"]);
			cTable userdefinedClaimsTable = tables.GetTableByID(new Guid("f70d6e0d-8e38-4a1d-a681-cc9d310c2ae9"));
			SortedList<int, object> userdefinedFieldsFromPage = userdefinedFields.getItemsFromPage(ref tbludf, userdefinedClaimsTable, false, "");

			if (action == "2")
			{
				claimId = int.Parse(txtclaimid.Text);

				if (claims.updateClaim(claimId, name, description, userdefinedFieldsFromPage, (int)ViewState["employeeid"]) == -1)
				{
					lblduplicate.Visible = true;
					return;
				}
			}
			else
			{
				var claimSubmission = new ClaimSubmission(cMisc.GetCurrentUser());
				claimId = claimSubmission.addClaim((int)ViewState["employeeid"], name, description, userdefinedFieldsFromPage);

				if (claimId == -1)
				{
					lblduplicate.Visible = true;
					return;
				}
			}

			switch ((int)ViewState["returnto"])
			{
				case 1:
					Response.Redirect("aeexpense.aspx?claimid=" + claimId, true);
					break;
				case 2:
					Response.Redirect("aeexpense.aspx?claimid=" + claimId, true);
					break;
			}

			Response.Redirect("expenses/claimsummary.aspx?claimtype=1", true);
		}

		private void ImageButton1_Click(object sender, ImageClickEventArgs e)
		{

			Response.Redirect("claimsmenu.aspx", true);
		}
	}
}
