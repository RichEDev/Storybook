using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Text;

namespace Spend_Management
{
    public partial class attachmentList : System.Web.UI.UserControl
    {
        private string sTableName;
        private string sIdField;
        private int nRecordID;
        private string sIFrameName;
        private bool bMultipleAttachment;
        private string sOnClickClick;

        #region properties
        /// <summary>
        /// Get or Set the database table name for attachment storage
        /// </summary>
        public string TableName
        {
            get { return sTableName; }
            set
            {
                sTableName = value;
                usrUpload.TableName = value;
            }
        }
        /// <summary>
        /// Get or Set the database ID field name
        /// </summary>
        public string IDField
        {
            get { return sIdField; }
            set
            {
                sIdField = value;
                usrUpload.IDField = value;
            }
        }
        /// <summary>
        /// Get or Set the ID of the parent record that "owns" the attachment
        /// </summary>
        public int RecordID
        {
            get { return nRecordID; }
            set
            {
                nRecordID = value;
                usrUpload.RecordID = value;
            }
        }

        public string iFrameName
        {
            get { return sIFrameName; }
            set
            {
                sIFrameName = value;
                usrUpload.iFrameName = value;
            }
        }

        public bool MultipleAttachments
        {
            get { return bMultipleAttachment; }
            set
            {
                bMultipleAttachment = value;
                usrUpload.MultipleAttachments = value;
            }
        }

        public string OnClickClick
        {
            get { return sOnClickClick; }
            set { sOnClickClick = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether "Publish" functionality should be included in the control
        /// </summary>
        public bool WithPublishing { get; set; }
        #endregion

        /// <summary>
        /// Primary page load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            usrUpload.Height = 230;
        }

        /// <summary>
        /// createGrid: generate the 
        /// </summary>
        /// <param name="entityRecordId">
        /// The entity Record Id. If equal to zero, this indicates that the user is creating a new greenlight record.
        /// </param>
        public void createGrid(int entityRecordId = -1)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            var sql = string.Format("select attachmentID, {0}, title, description, fileName{1} from [{2}]", this.IDField, this.WithPublishing ? ", Published" : string.Empty,  this.TableName);
            var clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridAttachments", sql);
            var clsfields = new cFields(user.AccountID);
            var currentTable = new cTables(user.AccountID).GetTableByName(this.TableName);
            cField field = clsfields.GetBy(currentTable.TableID, this.IDField);
            clsgrid.addFilter(field, ConditionType.Equals, new object[] { RecordID }, new object[] { }, ConditionJoiner.None);

            cField torchGeneratedField = clsfields.GetBy(currentTable.TableID, "TorchGenerated");
            if (torchGeneratedField != null)
            {
                clsgrid.addFilter(torchGeneratedField, ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);
            }

            if (entityRecordId == 0)
            {
                cField createdOnDate = clsfields.GetBy(currentTable.TableID, "createdon");
                cField createdBy = clsfields.GetBy(currentTable.TableID, "createdby");
                clsgrid.addFilter(createdOnDate, ConditionType.GreaterThan, new object[] { DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm") }, null, ConditionJoiner.And);
                clsgrid.addFilter(createdBy, ConditionType.Equals, new object[] { user.EmployeeID }, null, ConditionJoiner.And);
            }
            
            clsgrid.KeyField = this.IDField.ToString();

            switch (user.CurrentActiveModule)
            {
                case Modules.contracts:
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                    clsgrid.CssClass = "datatbl";
                    clsgrid.SortedColumn = clsgrid.getColumnByName("description");
                    break;
                default:
                    break;
            }
            
            clsgrid.enabledeleting = true;
            clsgrid.enablearchiving = false;
            clsgrid.enableupdating = false;
            clsgrid.EmptyText = "There are currently no attachments";

            var tableId = field.GetParentTable().TableID;
            clsgrid.deletelink = "javascript:deleteAttachment({attachmentID},'" + tableId + "',0,0);";

            if (this.WithPublishing)
            {
                var twoStateColumnJs = string.Format("javascript:toggleTorchAttachmentPublished({0}, '{1}', '{2}');", "{attachmentID}", tableId, "gridAttachments");
                var publishedColumn = clsgrid.getColumnByName("Published") as cFieldColumn;
                if (publishedColumn != null)
                {
                    clsgrid.addTwoStateEventColumn("published", publishedColumn,
                        false, true, "/static/icons/16/new-icons/document_up.png", twoStateColumnJs, "Publish", "Publish",
                        "/static/icons/16/plain/document_down.png", twoStateColumnJs, "Unpublish", "Unpublish");
                }
            }
            
            clsgrid.addEventColumn("viewattachment", "/shared/images/icons/16/plain/zoom_in.png", "javascript:viewAttachment({attachmentID},'" + field.GetParentTable().TableID.ToString() + "');", "View", "View uploaded attachment");
            clsgrid.getColumnByName("attachmentID").hidden = true;
            clsgrid.getColumnByName(IDField).hidden = true;
            clsgrid.KeyField = "attachmentID";

            string[] gridData = clsgrid.generateGrid();
            litgrid.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "attListGridVars", cGridNew.generateJS_init("attListGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
            
        }
    }
}
