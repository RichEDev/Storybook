namespace Spend_Management
{
    #region Using Directives

    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Infragistics.WebUI.UltraWebGrid;

    #endregion

    /// <summary>
    /// The tooltips.
    /// </summary>
    public partial class tooltips : Page
    {
        #region Methods

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.gridtooltips.InitializeDataSource += this.gridtooltips_InitializeDataSource;
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "Tooltips";
            this.Master.title = this.Title;

            CurrentUser currentUser = cMisc.GetCurrentUser();

            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tooltips, true, true);

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.contracts:
                    this.Master.helpid = 1182;
                    break;
                default:
                    this.Master.helpid = 0;
                    break;
            }

            if (this.IsPostBack == false)
            {
                this.ViewState["accountid"] = currentUser.AccountID;
                this.ViewState["employeeid"] = currentUser.EmployeeID;
            }

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.contracts:
                    this.hlClose.NavigateUrl = "~/MenuMain.aspx?menusection=tailoring";
                    break;
                default:
                    this.hlClose.NavigateUrl = "~/tailoringmenu.aspx";
                    break;
            }
        }

        /// <summary>
        /// The gridtooltips_ initialize layout.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void gridtooltips_InitializeLayout(object sender, LayoutEventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();            
            e.Layout.Bands[0].Columns.FromKey("description").Header.Caption = "Tooltip Description";
            e.Layout.Bands[0].Columns.FromKey("page").Header.Caption = "Page";
            e.Layout.Bands[0].Columns.FromKey("text").Header.Caption = "Tooltip Text";
            e.Layout.Bands[0].Columns.FromKey("tooltipID").Hidden = true;
            e.Layout.Bands[0].Columns.FromKey("tooltipArea").Hidden = true;
            if (e.Layout.Bands[0].Columns.FromKey("edit") == null
                && currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Tooltips, true))
            {
                e.Layout.Bands[0].Columns.Insert(
                    0, 
                    new UltraGridColumn(
                        "edit", "<img alt=\"Edit\" src=\"/shared/images/icons/edit.png\">", ColumnType.HyperLink, string.Empty));
                e.Layout.Bands[0].Columns.FromKey("edit").Width = Unit.Pixel(15);
            }

            e.Layout.Bands[0].Columns.FromKey("page").IsGroupByColumn = true;
        }

        /// <summary>
        /// The gridtooltips_ initialize row.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void gridtooltips_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells.FromKey("edit") != null)
            {
                e.Row.Cells.FromKey("edit").Value = "<a href=\"edittooltip.aspx?tooltipID="
                                                    + e.Row.Cells.FromKey("tooltipID").Value
                                                    + "\"><img src=\"/shared/images/icons/edit.png\" /></a>";
            }
        }

        /// <summary>
        /// The gridtooltips_ initialize data source.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void gridtooltips_InitializeDataSource(object sender, UltraGridEventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var clshelp = new cHelp((int)this.ViewState["accountid"]);
            this.gridtooltips.DataSource = clshelp.getGrid((int)currentUser.CurrentActiveModule);
        }

        #endregion
    }
}