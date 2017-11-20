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
using Spend_Management;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for aeliteral.
    /// </summary>
    public partial class aeliteral : Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Add / Edit Static Column";
            CurrentUser user = cMisc.GetCurrentUser();

            if (IsPostBack == false)
            {
                txtorder.Style.Add("display", "none");
                ViewState["accountid"] = user.AccountID;

                int action = 0;
                int reportid = 0;
                if (Request.QueryString["reportid"] != null)
                {
                    reportid = int.Parse(Request.QueryString["reportid"]);
                }
                ViewState["reportid"] = reportid;

                if (Request.QueryString["action"] != null)
                {
                    action = int.Parse(Request.QueryString["action"]);
                    ViewState["action"] = action;
                }

                if (action == 2)
                {
                    string name = Request.QueryString["name"];
                    string value = Request.QueryString["value"];
                    string order = Request.QueryString["order"];
                    bool runtime = Convert.ToBoolean(Request.QueryString["runtime"]);
                    txtname.Text = name;
                    if (!runtime)
                    {
                        txtvalue.Text = value;
                    }
                    chkruntime.Checked = runtime;
                    txtorder.Text = order;
                }
            }


            #region css inline edits


            cColours clscolours = new cColours((int)ViewState["accountid"], user.CurrentSubAccountId, user.CurrentActiveModule);
            System.Text.StringBuilder output = new System.Text.StringBuilder();
  

            output.Append(cColours.customiseStyles(false));

            //if (output.Length > 35)
            //{
                litstyles.Text = output.ToString();
            //}

            #endregion



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



        

    }
}
