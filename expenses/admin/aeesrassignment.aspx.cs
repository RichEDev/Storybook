using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Spend_Management;

namespace expenses.admin
{
    public partial class aeesrassignment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Add/Edit ESR Assignment Number";
            Master.title = Title;
			Master.showdummymenu = true;

            if (IsPostBack == false)
            {
                Master.enablenavigation = false;

                CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
                cEmployees clsEmployees = new cEmployees(user.accountid);

                int employeeid = int.Parse(Request.QueryString["employeeid"]);
                ViewState["accountid"] = user.accountid;
                ViewState["employeeid"] = employeeid;

                expenses.Action action = expenses.Action.Add;
                string assignmentNum = "";
                if (Request.QueryString["action"] != null)
                {
                    action = (expenses.Action)int.Parse(Request.QueryString["action"]);
                }

                ViewState["action"] = action;

                if (action == expenses.Action.Edit)
                {
                    lblESRAssignments.Text = "Edit ESR Assignment Number";
                    assignmentNum = Request.QueryString["assignmentnum"];
                    ViewState["assignmentnum"] = assignmentNum;
                    txtESRNum.Text = assignmentNum;
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
            this.cmdok.Click += new ImageClickEventHandler(cmdok_Click);
            this.cmdcancel.Click += new ImageClickEventHandler(cmdcancel_Click);
        }

        #endregion

        void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("aeemployee.aspx?action=2&employeeid=" + ViewState["employeeid"], true);
        }

        void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            cEmployees clsemps = new cEmployees((int)ViewState["accountid"]);
            bool checkedit = false;

            if ((Action)ViewState["action"] == Action.Edit)
            {
                if ((string)ViewState["assignmentnum"] != txtESRNum.Text)
                {
                    checkedit = true;
                }
            }

            string message = clsemps.saveESRAssignment((int)ViewState["employeeid"], txtESRNum.Text, (Action)ViewState["action"], checkedit, (string)ViewState["assignmentnum"]);

            if (message != "")
            {
                lblmsg.Text = message;
                lblmsg.Visible = true;
                return;
            }

            Response.Redirect("aeemployee.aspx?action=2&employeeid=" + ViewState["employeeid"]);
        }
    }
}
