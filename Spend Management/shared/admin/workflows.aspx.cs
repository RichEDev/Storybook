using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Spend_Management;
using System.Text;
using SpendManagementLibrary;

namespace Spend_Management
{
    public partial class workflows : System.Web.UI.Page
    {
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            Title = "Workflows";
            Master.title = "Workflows";
            
            litWorkflows.Text = "";

            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Workflows, true, true);           
            
            cWorkflows clsWorkflows = new cWorkflows(currentUser);

            cGridNew reqGrid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "workflows", "SELECT workflowID, workflowName, description, canBeChildWorkflow, runOnCreation, runOnChange, runOnDelete FROM dbo.workflows");
            reqGrid.deletelink = "javascript:deleteWorkflow({workflowID});";
            reqGrid.enabledeleting = true;
            reqGrid.DisplayFilter = true;
            reqGrid.EmptyText = "There are current no workflows";
            reqGrid.enableupdating = true;
            reqGrid.editlink = "aeWorkflow.aspx?workflowID={workflowID}";
            reqGrid.getColumnByName("workflowID").hidden = true;
            reqGrid.KeyField = "workflowID";

            string[] gridData = reqGrid.generateGrid();

            litWorkflows.Text = gridData[1];

            // set the sel.grid javascript variables
            Page.ClientScript.RegisterStartupScript(this.GetType(), "WorkflowGridVars", cGridNew.generateJS_init("WorkflowGridVars", new System.Collections.Generic.List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
        
        }
    }    
}
