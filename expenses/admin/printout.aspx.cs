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

using System.Collections.Generic;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses.admin
{
    /// <summary>
    /// Summary description for printout.
    /// </summary>
    public partial class printout : Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {

            Title = "Print-Out";
            Master.title = Title;
            Master.helpid = 1107;
            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DefaultPrintView, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                gridprintout.DataSource = getPrintoutGrid();
                gridprintout.DataBind();
            }
        }

        private DataTable getPrintoutGrid()
        {
            cMisc clsmisc = new cMisc((int)ViewState["accountid"]);
            cFields clsfields = new cFields((int)ViewState["accountid"]);
            object[] values;
            List<cField> fields = clsfields.getPrintoutFields();

            List<Guid> selectedfields = clsmisc.GetPrintoutFields();
            DataTable tbl = new DataTable();
            tbl.Columns.Add("selected", typeof(System.Boolean));
            tbl.Columns.Add("fieldid", typeof(System.Guid));
            tbl.Columns.Add("description", typeof(System.String));

            foreach (cField field in fields)
            {
                values = new object[3];
                if (selectedfields.Contains(field.FieldID))
                {
                    values[0] = true;
                }
                else
                {
                    values[0] = false;
                }
                values[1] = field.FieldID;
                values[2] = field.Description;
                tbl.Rows.Add(values);
            }
            return tbl;
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
            this.gridprintout.InitializeLayout += new Infragistics.WebUI.UltraWebGrid.InitializeLayoutEventHandler(this.gridprintout_InitializeLayout);
            this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);

        }

        #endregion

        private void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            int count, x;
            cMisc clsmisc = new cMisc((int)ViewState["accountid"]);
            int i;
            count = 0;
            for (i = 0; i < gridprintout.Rows.Count; i++)
            {
                if ((bool)gridprintout.Rows[i].Cells.FromKey("selected").Value == true)
                {
                    count++;
                }
            }

            Guid[] fieldids = new Guid[count];
            x = 0;
            for (i = 0; i < gridprintout.Rows.Count; i++)
            {
                if ((bool)gridprintout.Rows[i].Cells.FromKey("selected").Value == true)
                {
                    fieldids[x] = (Guid)gridprintout.Rows[i].Cells.FromKey("fieldid").Value;
                    x++;
                }
            }

            clsmisc.UpdatePrintOut(fieldids);
            Response.Redirect("../tailoringmenu.aspx", true);
        }

        private void gridprintout_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.FromKey("fieldid").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("fieldid").Width = 0;
            e.Layout.Bands[0].Columns.FromKey("description").HeaderText = "Description";
            e.Layout.Bands[0].Columns.FromKey("description").Width = 200;
            e.Layout.Bands[0].Columns.FromKey("selected").Type = Infragistics.WebUI.UltraWebGrid.ColumnType.CheckBox;
            e.Layout.Bands[0].Columns.FromKey("selected").Width = 75;
            e.Layout.Bands[0].Columns.FromKey("selected").HeaderText = "Selected";
            e.Layout.Bands[0].Columns.FromKey("selected").AllowUpdate = Infragistics.WebUI.UltraWebGrid.AllowUpdate.Yes;
            e.Layout.Bands[0].Columns.FromKey("selected").CellStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect(cMisc.Path + "/tailoringmenu.aspx", true);
        }
    }
}