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
    /// Summary description for aep11d.
    /// </summary>
    public partial class aep11d : Page
    {
        private string action;

        private int pdcatid;

        protected System.Web.UI.WebControls.ImageButton cmdhelp;


        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Add / Edit P11D Category";
            Master.title = Title;
            Master.showdummymenu = true;
            Master.helpid = 1016;


            if (IsPostBack == false)
            {
                Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.P11D, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cP11dcats clspdcats = new cP11dcats(user.AccountID);
                action = Request.QueryString["action"];

                if (action == "2")
                {
                    sP11dCat reqpdcat;
                    txtaction.Text = "2";
                    pdcatid = int.Parse(Request.QueryString["pdcatid"]);
                    txtpdcatid.Text = pdcatid.ToString();
                    reqpdcat = clspdcats.getP11dCatById(pdcatid);
                    txtpdcat.Text = reqpdcat.pdname;

                    litsubcats.Text = createSubcatGrid(clspdcats.getSubCatList(pdcatid));

                }
                else
                {


                    litsubcats.Text = createSubcatGrid(clspdcats.getSubCatList(0));
                }


            }
        }

        private string createSubcatGrid(System.Data.DataSet ds)
        {
            int i;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.Append("<table class=datatbl>");
            output.Append("<tr><th>Expense Item</th><th>Tick to<br>select</th></tr>");
            for (i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                output.Append("<tr>");
                output.Append("<td>" + ds.Tables[0].Rows[i]["subcat"] + "</td>");
                output.Append("<td><input type=checkbox name=subcat id=\"subcat" + i + "\" value=\"" + ds.Tables[0].Rows[i]["subcatid"] + "\"");
                if (ds.Tables[0].Rows[i]["pdcatid"] != DBNull.Value)
                {
                    output.Append(" checked");
                }
                output.Append("></td>");
                output.Append("</tr>");
            }
            output.Append("</table>");
            return output.ToString();
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



        private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            cP11dcats clspdcats = new cP11dcats((int)ViewState["accountid"]);
            string pdname;
            string subcat = Request.Form["subcat"];
            string[] arrsubcat;
            int[] subcatids;

            int i = 0;


            action = txtaction.Text;

            pdname = txtpdcat.Text;


            if (subcat != null)
            {
                arrsubcat = subcat.Split(',');
                subcatids = new int[arrsubcat.Length];
            }
            else
            {
                arrsubcat = new string[0];
                subcatids = new int[0];
            }


            for (i = 0; i < arrsubcat.Length; i++)
            {


                subcatids[i] = int.Parse(arrsubcat[i]);


            }

            if (action == "2")
            {
                pdcatid = int.Parse(txtpdcatid.Text);
                if (clspdcats.updateP11dCat(pdname, pdcatid, subcatids) == 1)
                {
                    lblmsg.Text = "The P11D Category you have entered already exists.";
                    lblmsg.Visible = true;
                    return;
                }
            }
            else
            {
                if (clspdcats.addP11dCat(pdname, subcatids) == 1)
                {
                    lblmsg.Text = "The P11D Category you have entered already exists.";
                    lblmsg.Visible = true;
                    return;
                }
            }

            Response.Redirect("adminp11d.aspx", true);
        }

        private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("adminp11d.aspx", true);

        }
    }
}