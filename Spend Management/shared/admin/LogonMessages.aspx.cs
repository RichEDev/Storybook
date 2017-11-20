
namespace Spend_Management.shared.admin
{
    using System;
    using System.Web.UI;

    /// <summary>
    /// Logon Message class
    /// </summary>
    public partial class LogonMessages : Page
    {
        /// <summary>
        /// Defines if the user is allowed to edit logon messages or not
        /// </summary>
        public bool AllowEdit { get; set; }

        /// <summary>
        /// Defines if the user is allowed to edit logon messages or not
        /// </summary>
        public bool AllowDelete { get; set; }

        /// <summary>
        /// Page load function
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = @"Marketing Information";
            this.Master.title = this.Title;

            var reqCurrentUser = cMisc.GetCurrentUser();
            reqCurrentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.LogonMessages, true, true);
            if (!reqCurrentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.LogonMessages, true, false))
            {
                this.lnkNewLogonMessages.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
            this.GenerateLogonMessageGrid(reqCurrentUser);
        }

        /// <summary>
        /// Generates the grid showing the users Logon Messages.
        /// </summary>
        private void GenerateLogonMessageGrid(ICurrentUser reqCurrentUser)
        {
            var gridLogonMessages = new cGridNew(reqCurrentUser.AccountID, reqCurrentUser.EmployeeID, "LogonMessages", "SELECT  messageid,CategoryTitle,dbo.GetModulesForMessage(LogonMessagesView.messageid),Archived,HeaderText FROM LogonMessagesView");

            if (reqCurrentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.LogonMessages, true, false))
            {
                this.AllowDelete = true;
            }

            if (reqCurrentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.LogonMessages, true, false))
            {
                this.AllowEdit = true;
            }

            gridLogonMessages.KeyField = "messageid";
            gridLogonMessages.getColumnByName("messageid").hidden = true;
            gridLogonMessages.getColumnByName("Archived").hidden = true;
            gridLogonMessages.editlink = "aeLogonMessages.aspx?messageid={messageid}";
            gridLogonMessages.deletelink = "javascript:SEL.LogonMessages.DeleteLogonMessage({messageid});";
            gridLogonMessages.archivelink = "javascript:SEL.LogonMessages.ChangeStatus({messageid});";
            gridLogonMessages.enableupdating = this.AllowEdit;
            gridLogonMessages.enabledeleting = this.AllowDelete;
            gridLogonMessages.enablearchiving = false;
            gridLogonMessages.ArchiveField = "archived";
            gridLogonMessages.addEventColumn("Active", "Active", "", "Active");
            gridLogonMessages.EmptyText = "There are currently no marketing information defined.";
            var gridInfo = new SerializableDictionary<string, object>();
            gridLogonMessages.InitialiseRowGridInfo = gridInfo;
            gridLogonMessages.InitialiseRow += this.LogonMessagesGrid_InitialiseRow;
            gridLogonMessages.ServiceClassForInitialiseRowEvent = "Spend_Management.shared.admin.LogonMessages";
            gridLogonMessages.ServiceClassMethodForInitialiseRowEvent = "LogonMessagesGrid_InitialiseRow";
            var gridData = gridLogonMessages.generateGrid();
            this.litLogonMessages.Text = gridData[1];
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "LogonMessages", cGridNew.generateJS_init("LogonMessages", new System.Collections.Generic.List<string>() { gridData[0] }, reqCurrentUser.CurrentActiveModule), true);

        }

        /// <summary>
        /// Grid initialise event to override rendering
        /// </summary>
        /// <param name="row">The row of the grid</param>
        /// <param name="gridInfo">Info about the grid</param>
        private void LogonMessagesGrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
        {
            if (row.getCellByID("archived").Value != DBNull.Value)
            {
                string isactive = (bool)row.getCellByID("archived").Value == false ? " checked " : "";
                string html = "<input type=\"checkbox\" enabled " + isactive + "onclick ='SEL.LogonMessages.ChangeStatus(" + row.getCellByID("messageid").Value + "," + Convert.ToInt32(!(bool)row.getCellByID("archived").Value) + ");'\"> ";
                row.getCellByID("Active").Value = html;
            }
        }
    }
}