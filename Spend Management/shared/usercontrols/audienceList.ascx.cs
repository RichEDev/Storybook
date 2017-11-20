using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text;

namespace Spend_Management
{
    /// <summary>
    /// Audience List User Control
    /// </summary>
    public partial class audienceList : System.Web.UI.UserControl
    {
        private int nEntityIdentifier;
        private cTable clsBaseTable;
        private int nParentRecordId;
        private string sEntityKeyField;
        private bool bCanDelete;
        private bool bCanEdit;

        #region properties
        /// <summary>
        /// Gets or sets the 
        /// </summary>
        public int entityIdentifier
        {
            get { return nEntityIdentifier; }
            set { nEntityIdentifier = value; }
        }
        /// <summary>
        /// Gets or sets the base table that audiences are being applied to
        /// </summary>
        public cTable baseTable
        {
            get { return clsBaseTable; }
            set { clsBaseTable = value; }
        }
        /// <summary>
        /// Gets or sets the primary key field for the entity that audiences are being applied to
        /// </summary>
        public string entityKeyField
        {
            get { return sEntityKeyField; }
            set { sEntityKeyField = value; }
        }
        /// <summary>
        /// Gets or sets the parent record id that the audience is associated to
        /// </summary>
        public int parentRecordId
        {
            get { return nParentRecordId; }
            set { nParentRecordId = value; }
        }

        public bool DeletePermission
        {
            get { return bCanDelete; }
            set { bCanDelete = value; }
        }

        public bool EditPermission
        {
            get { return bCanEdit; }
            set { bCanEdit = value; }
        }
        #endregion properties

        /// <summary>
        /// Page Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Page.LoadControl("~/shared/usercontrols/audienceList.ascx");
            //lnkAddAudience.Attributes.Add("onclick", "javascript:SEL.Audience.addAudience(" + nEntityIdentifier.ToString() + ", '" + clsBaseTable.tablename + "');");
            CurrentUser user = cMisc.GetCurrentUser();
            cCustomEntities entities = new cCustomEntities(user);
            SerializableDictionary<string, object> audRecs = entities.GetAudienceRecords(nEntityIdentifier, user.EmployeeID, parentRecordId);
            if (audRecs.ContainsKey(parentRecordId.ToString()))
            {
                cAudienceRecordStatus audrec = (cAudienceRecordStatus)audRecs[parentRecordId.ToString()];
                bCanDelete = audrec.CanDelete;
                bCanEdit = audrec.CanEdit;
            }
            if (bCanEdit && bCanDelete)
            {
                lnkAddAudience.NavigateUrl = "javascript:SEL.Audience.addAudience(" + nEntityIdentifier.ToString() + ", '" + clsBaseTable.TableID.ToString() + "', " + parentRecordId.ToString() + ");";
            }
            else
            {
                lnkAddAudience.Visible = false;
            }

            createAudienceGrid();
        }

        /// <summary>
        /// Constructs and renders the audience grid
        /// </summary>
        private void createAudienceGrid()
        {
            svcAudiences svc = new svcAudiences();
            CurrentUser curUser = cMisc.GetCurrentUser();

            string[] gridData = svc.CreateAudienceGridUC(nParentRecordId, clsBaseTable.TableID, nEntityIdentifier, bCanEdit, bCanDelete);
            litAudienceGridUC.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AudListGridVars", "SEL.Audience.CanEdit = " + bCanEdit.ToString().ToLower() + ";\nSEL.Audience.CanDelete = " + bCanDelete.ToString().ToLower() + ";\n" + cGridNew.generateJS_init("AudListGridVars", new List<string>() { gridData[0] }, curUser.CurrentActiveModule), true);

            return;
        }
    }
}
