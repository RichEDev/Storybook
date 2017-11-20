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

namespace expenses.information
{
    /// <summary>
    /// Summary description for directory.
    /// </summary>
    public partial class directory : Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Employee Directory";
            Master.title = Title;
            Master.helpid = 1024;
            if (IsPostBack == false)
            {


                string letter;
                int i;
                cGridRow reqrow;
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cEmployees clsemployees = new cEmployees(user.AccountID);

                if (Request.QueryString["letter"] != null)
                {
                    letter = Request.QueryString["letter"];
                    cGrid clsgrid = new cGrid(clsemployees.getDirectoryGrid(user.AccountID, letter), true, false);
                    clsgrid.align = "center";
                    clsgrid.getColumn("employeeid").hidden = true;
                    clsgrid.getColumn("title").description = "Title";
                    clsgrid.getColumn("firstname").description = "First Name";
                    clsgrid.getColumn("surname").description = "Surname";
                    clsgrid.getColumn("extension").description = "Extension";
                    clsgrid.getColumn("username").description = "Username";
                    clsgrid.tblclass = "datatbl";
                    clsgrid.getData();
                    for (i = 0; i < clsgrid.gridrows.Count; i++)
                    {
                        reqrow = (cGridRow)clsgrid.gridrows[i];
                        reqrow.getCellByName("username").thevalue = "<a href=\"direntry.aspx?employeeid=" + reqrow.getCellByName("employeeid").thevalue + "\">" + reqrow.getCellByName("username").thevalue + "</a>";
                    }
                    litnames.Text = clsgrid.CreateGrid();


                }
            }
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
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

        protected void LinkButton1_Click(object sender, System.EventArgs e)
        {
            string search;
            int i;
            cGridRow reqrow;
            cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);

            search = txtsearch.Text;

            cGrid clsgrid = new cGrid(clsemployees.getDirectoryGrid((int)ViewState["accountid"], search), true, false);
            clsgrid.align = "center";
            clsgrid.getColumn("employeeid").hidden = true;
            clsgrid.getColumn("title").description = "Title";
            clsgrid.getColumn("firstname").description = "Firstname";
            clsgrid.getColumn("surname").description = "Surname";
            clsgrid.getColumn("extension").description = "Extension";
            clsgrid.getColumn("username").description = "Username";
            clsgrid.tblclass = "datatbl";
            for (i = 0; i < clsgrid.gridrows.Count; i++)
            {
                reqrow = (cGridRow)clsgrid.gridrows[i];
                reqrow.getCellByName("username").thevalue = "<a href=\"direntry.aspx?employeeid=" + reqrow.getCellByName("employeeid").thevalue + "\">" + reqrow.getCellByName("username").thevalue + "</a>";
            }
            litnames.Text = clsgrid.CreateGrid();
        }


        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string sPreviousURL = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            Response.Redirect(sPreviousURL, true);
        }
    }
}