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
    /// <summary>
    /// Summary description for aecategory.
    /// </summary>
    /// 

    public partial class aecategory : Page
    {
        private string action;

        private int categoryid;

        protected System.Web.UI.WebControls.ImageButton cmdhelp;

        private cCategories clscategory;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Add / Edit Expense Category";
            Master.title = Title;
            Master.helpid = 1022;
            Master.showdummymenu = true;


            if (IsPostBack == false)
            {
                Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ExpenseCategories, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                clscategory = new cCategories(user.AccountID);
                if (Request.QueryString["action"] != null)
                {
                    action = Request.QueryString["action"];
                    categoryid = int.Parse(Request.QueryString["categoryid"]);

                    cCategory reqCategory = clscategory.FindById(categoryid);

                    txtcategory.Text = reqCategory.category;
                    txtdescription.Text = reqCategory.description;
                    txtcategoryid.Text = categoryid.ToString();
                    txtaction.Text = action;



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
            this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.ImageButton1_Click);
            this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

        }

        #endregion

        private void ImageButton1_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string category;
            string description;



            clscategory = new cCategories((int)ViewState["accountid"]);

            category = txtcategory.Text;
            description = txtdescription.Text;

            if (category.Length > 49)
            {
                category = category.Substring(0, 48);
            }

            if (description.Length > 3998)
            {
                description = description.Substring(0, 3998);
            }

            if (txtaction.Text != "") //update
            {
                categoryid = int.Parse(txtcategoryid.Text);
                if (clscategory.updateCategory(categoryid, category, description, (int)ViewState["employeeid"]) == 1)
                {
                    lblmsg.Text = "The category name you have entered already exists";
                    lblmsg.Visible = true;
                    return;
                }
            }
            else
            {
                if (clscategory.addCategory(category, description, (int)ViewState["employeeid"]) == 1)
                {
                    lblmsg.Text = "The category name you have entered already exists";
                    lblmsg.Visible = true;
                    return;
                }
            }



            Response.Redirect("admincategories.aspx");

        }

        private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("admincategories.aspx", true);
        }
    }
}