using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using SpendManagementLibrary;
using Spend_Management;
using System.Configuration;
using System.Text;


namespace Spend_Management
{
    using BusinessLogic.Modules;

    /// <summary>
	/// Summary description for adminuserdefined.
	/// </summary>
	public partial class adminuserdefined : Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = 60;
			Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
			Response.AddHeader ("pragma","no-cache");
			Response.AddHeader ("cache-control","private");
			Response.CacheControl = "no-cache";


            Title = "Userdefined Fields";
            Master.title = Title;
           
			if (IsPostBack == false)
			{
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserDefinedFields, true, true);
                switch (user.CurrentActiveModule)
                {
                    case Modules.Contracts:
                        Master.helpid = 1148;
                        break;
                    default:
                        Master.helpid = 1007;
                        break;
                }
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;


                if (!user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.UserDefinedFields, true, false))
                {
                    lnkAddUDF.Visible = false;
                }

                string[] gridData = createGrid();
                litgrid.Text = gridData[1];

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "udfGridVars", cGridNew.generateJS_init("udfGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule) , true);
			}


		}

        private string[] createGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cUserdefinedFields ufields = new cUserdefinedFields((int)ViewState["accountid"]);

            cGridNew newgrid = new cGridNew((int)ViewState["accountid"], (int)ViewState["employeeid"], "gridFields", ufields.GetGrid());

            newgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.UserDefinedFields, true);
            newgrid.enableupdating = user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.UserDefinedFields, true);
            newgrid.editlink = "aeuserdefined.aspx?userdefineid={userdefineid}";
            newgrid.deletelink = "javascript:deleteUserdefined({userdefineid});";
            newgrid.getColumnByName("userdefineid").hidden = true;
            newgrid.KeyField = "userdefineid";
            newgrid.CssClass = "datatbl";
            newgrid.EmptyText = "There are currently no user defined fields.";

            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Text, "Text");
            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Number, "Number");
            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Currency, "Currency");
            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.List, "List");
            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.TickBox, "Tick Box");
            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.DateTime, "Date");
            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Integer, "Integer");
            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Hyperlink, "Hyperlink");
            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.DynamicHyperlink, "Dynamic Hyperlink");
            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.Relationship, "Relationship");
            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.RelationshipTextbox, "Relationship TextBox");
            ((cFieldColumn)newgrid.getColumnByName("fieldtype")).addValueListItem((byte)FieldType.LargeText, "Large Text");
            
            return newgrid.generateGrid();
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


        [WebMethod(EnableSession = true)]
        public static int deleteUserDefined(int userdefineid)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cUserdefinedFields clsuserdefined = new cUserdefinedFields(user.AccountID);
            
            return clsuserdefined.DeleteUserDefined(userdefineid);
        }


        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            switch (user.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.Contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=tailoring", true);
                    break;
                default:
                    Response.Redirect("~/tailoringmenu.aspx", true);
                    break;
            }
        }
	

		
	}
}
