using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using SpendManagementLibrary;
using SpendManagementHelpers;

namespace Spend_Management
{
    /// <summary>
    /// Shows options to handle a decision step at present in the workflow
    /// </summary>
    public partial class WorkflowUserControl : System.Web.UI.UserControl
    {
        /// <summary>
        /// The current workflow being used
        /// </summary>
        public cWorkflow reqWorkflow;
        /// <summary>
        /// The workflowID currently being used
        /// </summary>
        int? nWorkflowID = null;
        /// <summary>
        /// The entityID currently being used
        /// </summary>
        int? nEntityID = null;
        /// <summary>
        /// If the content is to be displayed in a modal, the id can be set here allowing it to be closed when an action is clicked.
        /// </summary>
        string sModalPopupID = string.Empty;




        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (nWorkflowID.HasValue == true && nEntityID.HasValue == true)
            {
                PlaceHolder phWorkflows = new PlaceHolder();
                phWorkflows.ID = "phWorkflows";
                this.Controls.Add(phWorkflows);

                cWorkflows clsWorkflows = new cWorkflows(currentUser);
                cWorkflowEntityDetails reqWorkflowDetails = clsWorkflows.GetCurrentEntityStatus(nEntityID.Value, nWorkflowID.Value);

                if (reqWorkflowDetails != null)
                {
                    cWorkflow reqWorkflow = clsWorkflows.GetWorkflowByID(nWorkflowID.Value);

                    Panel pnlContent = new Panel();
                    pnlContent.ID = "pnlContent";
                    Panel pnl = null;
                    Literal lit;
                    CSSButton cssBtn;

                    if (reqWorkflow.Steps[reqWorkflowDetails.StepNumber].Action == WorkFlowStepAction.Decision)
                    {
                        cDecisionStep reqDecisionStep = (cDecisionStep)reqWorkflow.Steps[reqWorkflowDetails.StepNumber];

                        pnl = new Panel();

                        lit = new Literal();
                        lit.ID = "litQuestion";
                        lit.Text = reqDecisionStep.Question;

                        pnl.Controls.Add(lit);
                        pnlContent.Controls.Add(pnl);

                        pnl = new Panel();
                        pnl.Style.Add(HtmlTextWriterStyle.MarginTop, "15px");

                        cssBtn = new CSSButton();
                        cssBtn.Text = reqDecisionStep.TrueOption;
                        cssBtn.Attributes.Add("onclick", "javscript:UpdateDecisionStep(" + EntityID + ", " + WorkflowID + ", true); $find('" + sModalPopupID + "').hide(); return false;");
                        pnl.Controls.Add(cssBtn);

                        //lit = new Literal();
                        //lit.ID = "litTrueAnswer";
                        //lit.Text = "<a href=\"javascript:UpdateDecisionStep(" + EntityID + ", " + WorkflowID + ", true);\">" + reqDecisionStep.TrueOption + "</a> - ";
                        //pnl.Controls.Add(lit);

                        cssBtn = new CSSButton();
                        cssBtn.Text = reqDecisionStep.FalseOption;
                        cssBtn.Attributes.Add("onclick", "javscript:UpdateDecisionStep(" + EntityID + ", " + WorkflowID + ", false); $find('" + sModalPopupID + "').hide(); return false;");
                        pnl.Controls.Add(cssBtn);

                        //lit = new Literal();
                        //lit.ID = "litFalseAnswer";
                        //lit.Text = "<a href=\"javascript:UpdateDecisionStep(" + EntityID + ", " + WorkflowID + ", false);\">" + reqDecisionStep.FalseOption + "</a>";
                        //pnl.Controls.Add(lit);

                        pnlContent.Controls.Add(pnl);

                    }
                    else if(reqWorkflow.Steps[reqWorkflowDetails.StepNumber].Action == WorkFlowStepAction.Approval) 
                    {
                        cApprovalStep reqApprovalStep = (cApprovalStep)reqWorkflow.Steps[reqWorkflowDetails.StepNumber];

                        pnl = new Panel();
                        pnl.ID = "pnlApprovalHolder";
                        lit = new Literal();
                        lit.Text = "<div class=\"sectiontitle\">Approval Required</div>";
                        pnl.Controls.Add(lit);
                        pnlContent.Controls.Add(pnl);


                            pnl = new Panel();
                            lit = new Literal(); // accept
                            lit.ID = "litTrueAnswer";
                            lit.Text = "<a href=\"javascript:UpdateApprovalStep(" + EntityID + ", " + WorkflowID + ", true);\">" + reqApprovalStep.TrueOption + "</a> - ";
                            pnl.Controls.Add(lit);

                            lit = new Literal(); // reject
                            lit.ID = "litFalseAnswer";
                            lit.Text = "<a href=\"javascript:UpdateApprovalStep(" + EntityID + ", " + WorkflowID + ", false);\">" + reqApprovalStep.FalseOption + "</a>";
                            pnl.Controls.Add(lit);

                            pnlContent.Controls.Add(pnl);

                    }

                    phWorkflows.Controls.Add(pnlContent);


                }
            }
             
        }

        /// <summary>
        /// The WorkflowID currently being used
        /// </summary>
        public int? WorkflowID
        {
            get { return nWorkflowID; }
            set { nWorkflowID = value; }
        }

        /// <summary>
        /// The EntityID currently being used
        /// </summary>
        public int? EntityID
        {
            get { return nEntityID; }
            set { nEntityID = value; }
        }

        /// <summary>
        /// Sets the modal popup the content is to be used with.
        /// </summary>
        public string ModalPopupID
        {
            get { return sModalPopupID; }
            set { sModalPopupID = value; }
        }
    }
}