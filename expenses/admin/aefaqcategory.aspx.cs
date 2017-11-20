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

namespace expenses.admin
{
    /// <summary>
    /// Summary description for aefaqcategory.
    /// </summary>
    public partial class aefaqcategory : Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Add / Edit FAQ Category";
            Master.title = Title;
            Master.showdummymenu = true;
            Master.helpid = 1028;

            if (IsPostBack == false)
            {
                Master.enablenavigation = false;
                int action = 0;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FAQS, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                if (Request.QueryString["action"] != null)
                {
                    action = int.Parse(Request.QueryString["action"]);
                    ViewState["action"] = action;
                }

                if (action == 2)
                {
                    int faqcatid = int.Parse(Request.QueryString["faqcategoryid"]);
                    ViewState["faqcatid"] = faqcatid;
                    cFaqCategories clsfaqcats = new cFaqCategories(user.AccountID);
                    cFaqCategory reqfaqcat = clsfaqcats.getFaqCategoryById(faqcatid, false);
                    txtcategory.Text = reqfaqcat.category;
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
            this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);
            this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

        }

        #endregion

        private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("adminfaqs.aspx", true);
        }

        private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            cFaqCategories clsfaqcats = new cFaqCategories((int)ViewState["accountid"]);
            string category;
            int action = 0;

            category = txtcategory.Text;

            if (ViewState["action"] != null)
            {
                action = (int)ViewState["action"];
            }

            if (action == 2) //update
            {
                if (clsfaqcats.updateCategory((int)ViewState["faqcatid"], category) == 1)
                {
                    lblmsg.Text = "The FAQ category you have entered already exists.";
                    lblmsg.Visible = true;
                    return;
                }
            }
            else
            {
                if (clsfaqcats.addCategory(category) == 1)
                {
                    lblmsg.Text = "The FAQ category you have entered already exists.";
                    lblmsg.Visible = true;
                    return;
                }
            }

            Response.Redirect("adminfaqs.aspx", true);
        }
    }
}