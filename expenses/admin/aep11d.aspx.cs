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
using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.P11DCategories;
using SpendManagementLibrary;
using SpendManagementLibrary.Helpers;
using Spend_Management;

namespace expenses
{
    /// <summary>
    /// Summary description for aep11d.
    /// </summary>
    public partial class aep11d : Page
    {
        [Dependency]
        public IDataFactory<IP11DCategory, int> P11DCategoriesRepository { get; set; }

        private string action;

        private int pdcatid;

        protected System.Web.UI.WebControls.ImageButton cmdhelp;

        /// <summary>
        /// Page Load events
        /// </summary>
        /// <param name="sender"><see cref="Object"/></param>
        /// <param name="e"><see cref="EventArgs"/></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Add / Edit P11D Category";
            this.Master.title = Title;
            this.Master.showdummymenu = true;
            this.Master.helpid = 1016;


            if (this.IsPostBack == false)
            {
                this.Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.P11D, true, true);
                this.ViewState["accountid"] = user.AccountID;
                this.ViewState["employeeid"] = user.EmployeeID;
                this.action = this.Request.QueryString["action"];
                var subcats = new cSubcats(user.AccountID);

                if (this.action == "2")
                {
                    this.pdcatid = int.Parse(this.Request.QueryString["pdcatid"]);
                    var p11DCategory = this.P11DCategoriesRepository.Get(p11D => p11D.Id == this.pdcatid);
                    if (p11DCategory.Count > 0)
                    {
                        this.txtaction.Text = "2";
                        this.txtpdcatid.Text = this.pdcatid.ToString();
                        this.txtpdcat.Text = p11DCategory[0].Name;
                        this.litsubcats.Text = this.CreateSubcatGrid(subcats.GetSubCatList(this.pdcatid));
                    }
                    else
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }
                }
                else
                {
                    this.litsubcats.Text = this.CreateSubcatGrid(subcats.GetSubCatList(0));
                }
            }
        }

        /// <summary>
        /// Create the subcat grid
        /// </summary>
        /// <param name="ds"><see cref="DataSet"/> of a list of subcats</param>
        /// <returns>Html to create subcat list</returns>
        private string CreateSubcatGrid(DataSet ds)
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
        
        /// <summary>
        /// Ok button click events
        /// </summary>
        /// <param name="sender"><see cref="object"/></param>
        /// <param name="e"><see cref="ImageClickEventArgs"/></param>
        private void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            var subcats = new cSubcats((int)this.ViewState["accountid"]);
            string subcat = this.Request.Form["subcat"];
            string[] arrsubcat;
            int[] subcatids;
            int i = 0;
            this.action = this.txtaction.Text;
            var pdname = this.txtpdcat.Text;
            
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

            if (this.action == "2")
            {
                var createdP11DCategory = this.P11DCategoriesRepository.Add(new P11DCategory(int.Parse(this.txtpdcatid.Text), pdname));
                if (createdP11DCategory.Id == -1)
                {
                    this.DisplayDuplicateMessage();

                    return;
                }

                subcats.AssignP11DToSubcats(subcatids, createdP11DCategory.Id);
            }
            else
            {
                var createdP11DCategory= this.P11DCategoriesRepository.Add(new P11DCategory(0, pdname));
                if (createdP11DCategory.Id == -1)
                {
                    this.DisplayDuplicateMessage();

                    return;
                }

                subcats.AssignP11DToSubcats(subcatids, createdP11DCategory.Id);
            }

            this.Response.Redirect("adminp11d.aspx", true);
        }

        /// <summary>
        /// Displays duplicate P11D Category message
        /// </summary>
        private void DisplayDuplicateMessage()
        {
            this.lblmsg.Text = "The P11D Category you have entered already exists.";
            this.lblmsg.Visible = true;
        }

        /// <summary>
        /// Cancel button click events
        /// </summary>
        /// <param name="sender"><see cref="object"/></param>
        /// <param name="e"><see cref="ImageClickEventArgs"/></param>
        private void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            this.Response.Redirect("adminp11d.aspx", true);
        }
    }
}