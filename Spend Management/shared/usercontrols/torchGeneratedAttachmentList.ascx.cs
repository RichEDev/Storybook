namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SpendManagementLibrary;

    /// <summary>
    /// Torch generated attachments control
    /// </summary>
    public partial class torchGeneratedAttachmentList : System.Web.UI.UserControl
    {
        #region properties
        /// <summary>
        /// Gets or sets the database table name for attachment storage
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the database ID field name
        /// </summary>
        public string IdField { get; set; }

        /// <summary>
        /// Gets or sets the ID of the parent record that "owns" the attachment
        /// </summary>
        public int[] RecordIds { get; set; }

        #endregion

        /// <summary>
        /// Generate the <see cref="cGridNew"/> for this control
        /// </summary>
        public void CreateGrid()
        {
            const string GridName = "gridTorchAttachments";

            CurrentUser user = cMisc.GetCurrentUser();
            var grid = new cGridNew(user.AccountID, user.EmployeeID, GridName, "select attachmentID, " + this.IdField + ", title, createdon, Published from [" + this.TableName + "]");
            var fields = new cFields(user.AccountID);
            var currentTable = new cTables(user.AccountID).GetTableByName(this.TableName);
            cField field = fields.GetBy(currentTable.TableID, this.IdField);
            grid.addFilter(field, ConditionType.Equals, this.RecordIds.Cast<object>().ToArray(), null, ConditionJoiner.None);
            cField torchGeneratedField = fields.GetBy(currentTable.TableID, "TorchGenerated");
            grid.addFilter(torchGeneratedField, ConditionType.Equals, new object[] { 1 }, null, ConditionJoiner.And);

            grid.KeyField = this.IdField;
            grid.enabledeleting = true;
            grid.enablearchiving = false;
            grid.enableupdating = false;
            grid.EmptyText = "There are currently no attachments to display.";

            var tableId = field.GetParentTable().TableID;
            grid.deletelink = string.Format("javascript:deleteAttachment({0}, '{1}', 0, 0, '{2}');", "{attachmentID}", tableId, GridName);

            var twoStateColumnJs = string.Format("javascript:toggleTorchAttachmentPublished({0}, '{1}', '{2}');", "{attachmentID}", tableId, GridName);
            grid.addTwoStateEventColumn("published", (cFieldColumn)grid.getColumnByName("Published"), false, true, "/static/icons/16/new-icons/document_up.png", twoStateColumnJs, "Publish", "Publish", "/static/icons/16/plain/document_down.png", twoStateColumnJs, "Unpublish", "Unpublish");
            
            grid.addEventColumn("viewattachment", "/shared/images/icons/16/plain/zoom_in.png", "javascript:viewAttachment({attachmentID},'" + field.GetParentTable().TableID + "');", "View", "View attachment");
            grid.getColumnByName("attachmentID").hidden = true;
            grid.getColumnByName(this.IdField).hidden = true;
            grid.getColumnByName("title").HeaderText = "Title";
            grid.getColumnByName("createdon").HeaderText = "Date Created";
            grid.getColumnByName("createdon").CustomDateFormat = "dd/MM/yyyy H:mm:ss";
            grid.KeyField = "attachmentID";

            string[] gridData = grid.generateGrid();
            this.torchAttachmentGridLiteral.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "attListGridVars", cGridNew.generateJS_init("attListGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
        }

        /// <summary>
        /// Primary page load event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

    }
}
