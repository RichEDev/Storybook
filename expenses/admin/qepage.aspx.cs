namespace expenses.admin
{
    using System;
    using System.Web.UI;

    using SpendManagementLibrary;

    using Spend_Management;

    /// <summary>
    /// Summary description for qepage.
    /// </summary>
    public partial class qepage : Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Master.showdummymenu = true;
            Title = "Quick Entry Form Page Set-Up";
            Master.title = Title;
            Master.onloadfunc = "window_onload();";
            Master.helpid = 1045;
            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.QuickEntryForms, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;


                cAccounts clsAccounts = new cAccounts();
                cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);

                if (reqAccount.QuickEntryFormsEnabled == false)
                {
                    Response.Redirect("../home.aspx?", true);
                }

                int quickentryid = int.Parse(this.Request.QueryString["quickentryid"]);
                ViewState["quickentryid"] = quickentryid;
                cQeForms clsforms = new cQeForms(user.AccountID);
                cQeForm reqform = clsforms.getFormById(quickentryid);

                litavailfields.Text = getAvailableFields(clsforms.getAvailableFieldsForPrintout(quickentryid));
                createJavaFields(reqform.printout);
            }
        }

        private string getAvailableFields(object[,] fields)
        {
            int i;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.Append("<select name=availfields id=availfields size=10>\n");
            for (i = 0; i < fields.GetLength(0); i++)
            {
                output.Append("<option value=\"" + fields[i, 0] + "\">" + fields[i, 1] + "</option>");
            }
            output.Append("</select>");
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
            cQeForms clsforms = new cQeForms((int)ViewState["accountid"]);
            int i;
            string pos = "";
            string fieldid = "";
            string fieldtext = "";

            string[] arrpos = new string[0];
            string[] arrfieldid;
            string[] arrfieldtext;

            object[,] fields;
            if (Request.Form["pos"] != null)
            {
                pos = Request.Form["pos"];
            }

            if (pos != "")
            {
                arrpos = pos.Split(',');
            }

            cQeForm reqform = clsforms.getFormById((int)ViewState["quickentryid"]);
            if (arrpos.Length != 0)
            {

                fieldid = Request.Form["fieldid"];
                fieldtext = Request.Form["fieldtext"];

                arrfieldid = fieldid.Split(',');
                arrfieldtext = fieldtext.Split(',');

                fields = new object[arrpos.Length,3];
                for (i = 0; i < arrpos.Length; i++)
                {
                    fields[i, 0] = int.Parse(arrpos[i]);
                    fields[i, 1] = arrfieldid[i];
                    fields[i, 2] = arrfieldtext[i];

                }
                reqform.updatePrintout(fields);

            }
            else
            {
                fields = new object[0,0];
                reqform.updatePrintout(fields);
            }

            Response.Redirect("aeqeform.aspx?action=2&quickentryid=" + ViewState["quickentryid"], true);
        }

        private void createJavaFields(cQePrintoutField[] fields)
        {
            int i;
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.Append("<script type=\"text/javascript\">\n");
            output.Append("function window_onload()\n");
            output.Append("{\n");
            for (i = 0; i < fields.Length; i++)
            {
                output.Append("fields[" + i + "] = new Array(" + i + "," + (int)fields[i].pos + ",");
                if (fields[i].field == null)
                {
                    output.Append("0,'" + fields[i].freetext + "'");
                }
                else
                {
                    output.Append("'" + fields[i].field.FieldID + "','" + fields[i].field.Description + "'");
                }
                output.Append(");\n");
            }
            output.Append("createTable(1);\n");
            output.Append("createTable(2);\n");
            output.Append("createTable(3);\n");
            output.Append("createTable(4);\n");
            output.Append("createTable(5);\n");
            output.Append("createTable(6);\n");
            output.Append("}\n");

            output.Append("</script>");

            this.RegisterClientScriptBlock("windowonload", output.ToString());
        }

        private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("aeqeform.aspx?action=2&quickentryid=" + ViewState["quickentryid"], true);
        }
    }
}