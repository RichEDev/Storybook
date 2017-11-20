using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SpendManagementLibrary;
using AjaxControlToolkit;
using System.Text;

namespace Spend_Management
{
	public partial class MyTasks : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Title = "My Tasks";
            Master.PageSubTitle = Title;
			//Master.isSubFolder = true;
            CurrentUser user = cMisc.GetCurrentUser();
            switch (user.CurrentActiveModule)
            {
                case Modules.contracts:
                    Master.helpid = 1175;
                    break;
                default:
                    Master.helpid = 0;
                    break;
            }

			if (!this.IsPostBack)
			{
				switch (Request.QueryString["action"])
				{
					case "delete":
						DeleteTask(int.Parse(Request.QueryString["tid"]));
						break;
					default:
						break;
				}

				bool viewActive = true;
				if (Request.QueryString["vc"] == "1")
				{
					lnkViewActive.Visible = true;
					lnkViewCompleted.Visible = false;
					viewActive = false;
				}

				if (Request.QueryString["ret"] != null)
				{
                    returnURL.Value = Request.QueryString["ret"];
				}
                Literal litPaneTitle;
                Accordion MyTasksAccordian = new Accordion();
                MyTasksAccordian.ID = "MyTasks";
                MyTasksAccordian.HeaderCssClass = "accordianheader";
                MyTasksAccordian.HeaderSelectedCssClass = "accordianheader";

                AccordionPane accPane;
                accPane = new AccordionPane();
                accPane.ID = "accCurrent";
                litPaneTitle = new Literal();
                litPaneTitle.ID = "litCurrentTitle";
                if (!viewActive)
                {
                    litPaneTitle.Text = "My Previous Tasks";
                }
                else
                {
                    litPaneTitle.Text = "My Current Tasks";
                }
                accPane.HeaderContainer.Controls.Add(litPaneTitle);
                accPane.ContentContainer.Style.Add(HtmlTextWriterStyle.MarginBottom, "5px");

                
                StringBuilder js = new StringBuilder();
                Literal lit = new Literal();
                string[] grid = this.createUserTaskGrid(viewActive, false);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "gridTasks", cGridNew.generateJS_init("taskGridVars", new System.Collections.Generic.List<string>() { grid[0] }, user.CurrentActiveModule), true);
                
                lit.Text = grid[1];
                accPane.ContentContainer.Controls.Add(lit);

                MyTasksAccordian.Panes.Add(accPane);

                accPane = new AccordionPane();
                accPane.ID = "accDelegated";
                litPaneTitle = new Literal();
                litPaneTitle.ID = "litDelegatedTitle";
                litPaneTitle.Text = "Delegated Tasks";
                accPane.HeaderContainer.Controls.Add(litPaneTitle);
                accPane.ContentContainer.Style.Add(HtmlTextWriterStyle.MarginBottom, "5px");

                lit = new Literal();
                grid = this.createUserTaskGrid(viewActive, true);
                lit.Text = grid[1];
                
                accPane.ContentContainer.Controls.Add(lit);

                
                MyTasksAccordian.Panes.Add(accPane);

                phMyTasks.Controls.Add(MyTasksAccordian);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "griddelegates", cGridNew.generateJS_init("delegateGridVars", new System.Collections.Generic.List<string>() { grid[0] }, user.CurrentActiveModule), true);
                
			}
		}

        private string[] createUserTaskGrid(bool viewActive, bool viewDelegates)
        {
            string id;
            CurrentUser curUser = cMisc.GetCurrentUser();
            cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
            cAccountProperties properties;
            if (curUser.CurrentSubAccountId >= 0)
            {
                properties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
            }
            else
            {
                properties = subaccs.getFirstSubAccount().SubAccountProperties;
            }


            cTables tables = new cTables(curUser.AccountID);
            cFields clsfields = new cFields(curUser.AccountID);
            cTable basetable = tables.GetTableByName("employeeTasks");
            List<cNewGridColumn> columns = new List<cNewGridColumn>();

            columns.Add(new cFieldColumn(clsfields.GetBy(basetable.TableID, "taskid")));
            columns.Add(new cFieldColumn(clsfields.GetBy(basetable.TableID, "regardingId")));
            columns.Add(new cFieldColumn(clsfields.GetBy(basetable.TableID, "regardingArea")));
            columns.Add(new cFieldColumn(clsfields.GetBy(basetable.TableID, "regarding")));
            columns.Add(new cFieldColumn(clsfields.GetBy(basetable.TableID, "subject")));
            columns.Add(new cFieldColumn(clsfields.GetBy(basetable.TableID, "startdate")));
            columns.Add(new cFieldColumn(clsfields.GetBy(basetable.TableID, "duedate")));
            columns.Add(new cFieldColumn(clsfields.GetBy(basetable.TableID, "enddate")));
            columns.Add(new cFieldColumn(clsfields.GetBy(basetable.TableID, "taskOwner")));
            columns.Add(new cFieldColumn(clsfields.GetBy(basetable.TableID, "statusId")));

            if (viewDelegates)
            {
                id = "gridDelegates";
            }
            else
            {
                id = "gridUserTasks";
            }
            cGridNew clsgrid = new cGridNew(curUser.AccountID, curUser.EmployeeID, id, basetable, columns);
            clsgrid.KeyField = "taskId";
            clsgrid.getColumnByName("taskId").hidden = true;
            clsgrid.getColumnByName("regardingId").hidden = true;
            clsgrid.enableupdating = true;
            if (viewDelegates)
            {
                clsgrid.EmptyText = "There are currently no tasks created by you for others.";
            }
            else
            {
                clsgrid.EmptyText = "You currently have no tasks assigned by you to others.";
            }
            clsgrid.editlink = "ViewTask.aspx?action=edit&tid={taskId}&rid={regardingId}&rtid={regardingArea}&ret=" + returnURL.Value;
            
            clsgrid.addEventColumn("viewHistory","/shared/images/icons/16/plain/history.png", "TaskHistory.aspx?tid={taskId}&ret=" + returnURL.Value, "Task History","");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(1, "Contract");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(5, "Contract");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(7, "Contract Product");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(2, "Contract Product");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(3, "Product");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(8, "Employee");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(13, "Employee");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(6, "Recharge");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(9, "Invoice Details");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(10, "Invoice Forecast");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(17, "Car");
            ((cFieldColumn)clsgrid.getColumnByName("regardingArea")).addValueListItem(4, properties.SupplierPrimaryTitle);
            ((cFieldColumn)clsgrid.getColumnByName("statusId")).addValueListItem((int)TaskStatus.NotStarted, "Not Started");
            ((cFieldColumn)clsgrid.getColumnByName("statusId")).addValueListItem((int)TaskStatus.InProgress, "In Progress");
            ((cFieldColumn)clsgrid.getColumnByName("statusId")).addValueListItem((int)TaskStatus.Completed, "Completed");
            ((cFieldColumn)clsgrid.getColumnByName("statusId")).addValueListItem((int)TaskStatus.Cancelled, "Cancelled");
            ((cFieldColumn)clsgrid.getColumnByName("statusId")).addValueListItem((int)TaskStatus.Postponed, "Postponed");


            if (viewDelegates)
            {
                clsgrid.WhereClause = "taskCreatorId = @taskCreatorId and ((taskOwnerType = 1 and teamemployeeid <> @teamemployeeid) or (taskOwnerType = 3 and taskOwnerid <> @taskOwnerId))";
                clsgrid.addFilter(clsfields.GetBy(basetable.TableID, "taskCreatorId"), "@taskCreatorId", curUser.EmployeeID);
                //clsgrid.addFilter(clsfields.GetFieldByTableAndFieldName(basetable.TableID, "taskOwnerType"), ConditionType.Equals, new object[] { 1 }, null, ConditionJoiner.And);
                clsgrid.addFilter(clsfields.GetBy(basetable.TableID, "teamemployeeid"), "@teamemployeeid", curUser.EmployeeID);
                //clsgrid.addFilter(clsfields.GetFieldByTableAndFieldName(basetable.TableID, "taskOwnerType"), ConditionType.Equals, new object[] { 3 }, null, ConditionJoiner.Or);
                clsgrid.addFilter(clsfields.GetBy(basetable.TableID, "taskOwnerId"), "taskOwnerId", curUser.EmployeeID);
            }
            else
            {
                clsgrid.WhereClause = "((taskOwnerType = 1 and teamemployeeid = @teamemployeeid) or (taskOwnerType = 3 and taskOwnerid = @taskownerid))";
                //clsgrid.addFilter(clsfields.GetFieldByTableAndFieldName(basetable.TableID, "taskOwnerType"), ConditionType.Equals, new object[] { 1 }, null, ConditionJoiner.None);
                //clsgrid.addFilter(clsfields.GetFieldByTableAndFieldName(basetable.TableID, "teamemployeeid"), ConditionType.Equals, new object[] { curUser.EmployeeID }, null, ConditionJoiner.And);
                clsgrid.addFilter(clsfields.GetBy(basetable.TableID, "teamemployeeid"), "@teamemployeeid",curUser.EmployeeID);
                clsgrid.addFilter(clsfields.GetBy(basetable.TableID, "taskOwnerId"), "@taskownerid", curUser.EmployeeID);
                //clsgrid.addFilter(clsfields.GetFieldByTableAndFieldName(basetable.TableID, "taskOwnerType"), ConditionType.Equals, new object[] { 3 }, null, ConditionJoiner.Or);
                //clsgrid.addFilter(clsfields.GetFieldByTableAndFieldName(basetable.TableID, "taskOwnerId"), ConditionType.Equals, new object[] { curUser.EmployeeID }, null, ConditionJoiner.And);
            }
            if (viewActive)
            {
                clsgrid.WhereClause += " and (statusId = " + (int)TaskStatus.InProgress + " or statusId = " + (int)TaskStatus.NotStarted + " or statusId = " + (int)TaskStatus.Postponed + ")";
                
            }
            else
            {
                clsgrid.WhereClause += " and (statusId = " + (int)TaskStatus.Cancelled + " or statusId = " + (int)TaskStatus.Completed + ")";
                //clsgrid.addFilter(clsfields.GetFieldByTableAndFieldName(basetable.TableID, "statusId"), ConditionType.Equals, new object[] { (int)TaskStatus.Cancelled, (int)TaskStatus.Completed }, null, ConditionJoiner.And);
            }
            if (curUser.CurrentSubAccountId > -1)
            {
                clsgrid.WhereClause += " and ((subaccountid is null and  regardingarea = @regardingArea) or subaccountid = @subaccountid)";
                clsgrid.addFilter(clsfields.GetBy(basetable.TableID, "subaccountid"), "@subaccountid", curUser.CurrentSubAccountId);
                clsgrid.addFilter(clsfields.GetBy(basetable.TableID, "regardingArea"), "@regardingArea", (int)AppliesTo.Employee);
            }
            return clsgrid.generateGrid();
        }
		

        /// <summary>
        /// Action the clicking of the Close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected void cmdClose_Click(object sender, ImageClickEventArgs e)
		{
            CurrentUser curUser = cMisc.GetCurrentUser();

			if (returnURL.Value != "")
			{
				string retURL = Server.UrlDecode(returnURL.Value);
				Response.Redirect(retURL, true);
			}
			else
			{
                switch (curUser.CurrentActiveModule)
                {
                    case Modules.SmartDiligence:
                    case Modules.SpendManagement:
                    case Modules.contracts:
                        Response.Redirect("~/MenuMain.aspx?menusection=mydetails", true);
                        break;
                    default:
                        Response.Redirect("~/mydetailsmenu.aspx", true);
                        break;
                }
			}
		}

		

		private void DeleteTask(int taskid)
		{
			CurrentUser curUser = cMisc.GetCurrentUser();
            if (curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Tasks, true))
            {
                    Response.Redirect(cMisc.Path + "/shared/restricted.aspx", true);                
            }
			cTasks tasks = new cTasks(curUser.AccountID, curUser.CurrentSubAccountId);

			tasks.DeleteTask(taskid, curUser.EmployeeID);

            //IScheduler clsscheduler = (IScheduler)Activator.GetObject(typeof(IScheduler), "tcp://localhost:7887/scheduler.rem");
            //try
            //{
            //    clsscheduler.DeleteTask(uinfo, fws, taskid);
            //}
            //catch { }

			return;
		}

		protected void lnkViewCompleted_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/shared/tasks/MyTasks.aspx?vc=1&ret=" + returnURL.Value, true);
		}

		protected void lnkViewActive_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/shared/tasks/MyTasks.aspx?ret=" + returnURL.Value, true);
		}
	}
}
