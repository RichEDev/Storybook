using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using AjaxControlToolkit;
using SpendManagementLibrary.Employees;

namespace Spend_Management
{
	public partial class TaskSummary : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
			{
				Title = "Task Summary";
                Master.title = Title;
                Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                switch (user.CurrentActiveModule)
                {
                    case Modules.contracts:
                        Master.helpid = 1223;
                        break;
                    default:
                        Master.helpid = 0;
                        break;
                }

                Accordion tsAcc = new Accordion();
                tsAcc.ID = "TaskSummary";
                tsAcc.HeaderCssClass = "accordianheader";
                tsAcc.HeaderSelectedCssClass = "accordianheader";
                AccordionPane accPane;
                Literal litPaneTitle;
                
				if (Request.QueryString["vc"] != null)
				{
					// view closed tasks
					accPane = new AccordionPane();
                    accPane.ID = "headerCompleted";
                    litPaneTitle = new Literal();
                    litPaneTitle.ID = "litCompleted";
                    litPaneTitle.Text = "Completed";
                    accPane.HeaderContainer.Controls.Add(litPaneTitle);
                    //accPane.HeaderContainer.ID = accPane.ID + "_headerID";
                    //accPane.ContentContainer.ID = accPane.ID + "_contentID";
					accPane.ContentContainer.Controls.Add(ShowSummary(TaskStatus.Completed));
                    tsAcc.Panes.Add(accPane);

					accPane = new AccordionPane();
                    accPane.ID = "headerCancelled";
                    litPaneTitle = new Literal();
                    litPaneTitle.ID = "litCancelled";
                    litPaneTitle.Text = "Cancelled";
                    accPane.HeaderContainer.Controls.Add(litPaneTitle);
                    //accPane.HeaderContainer.ID = accPane.ID + "_headerID";
                    //accPane.ContentContainer.ID = accPane.ID + "_contentID";
					accPane.ContentContainer.Controls.Add(ShowSummary(TaskStatus.Cancelled));
                    tsAcc.Panes.Add(accPane);

					lnkViewActive.Visible = true;
				}
				else
				{
					accPane = new AccordionPane();
                    accPane.ID = "headerNotStarted";
                    litPaneTitle = new Literal();
                    litPaneTitle.ID = "litNotStarted";
                    litPaneTitle.Text = "Not Started";
                    accPane.HeaderContainer.Controls.Add(litPaneTitle);
                    //accPane.HeaderContainer.ID = accPane.ID + "_headerID";
                    //accPane.ContentContainer.ID = accPane.ID + "_contentID";
					accPane.ContentContainer.Controls.Add(ShowSummary(TaskStatus.NotStarted));
                    tsAcc.Panes.Add(accPane);

					accPane = new AccordionPane();
                    accPane.ID = "headerInProgress";
                    litPaneTitle = new Literal();
                    litPaneTitle.ID = "litInProgress";
                    litPaneTitle.Text = "In Progress";
                    accPane.HeaderContainer.Controls.Add(litPaneTitle);
                    //accPane.HeaderContainer.ID = accPane.ID + "_headerID";
                    //accPane.ContentContainer.ID = accPane.ID + "_contentID";
					accPane.ContentContainer.Controls.Add(ShowSummary(TaskStatus.InProgress));
                    tsAcc.Panes.Add(accPane);

					accPane = new AccordionPane();
                    accPane.ID = "headerPostponed";
                    litPaneTitle = new Literal();
                    litPaneTitle.ID = "litPostponed";
                    litPaneTitle.Text = "Postponed";
                    accPane.HeaderContainer.Controls.Add(litPaneTitle);
                    //accPane.HeaderContainer.ID = accPane.ID + "_headerID";
                    //accPane.ContentContainer.ID = accPane.ID + "_contentID";
					accPane.ContentContainer.Controls.Add(ShowSummary(TaskStatus.Postponed));
                    tsAcc.Panes.Add(accPane);

					lnkViewActive.Visible = false;
				}
                phTaskSummary.Controls.Add(tsAcc);

				lnkViewClosed.Visible = !lnkViewActive.Visible;

				returnURL.Value = Server.UrlDecode(Request.QueryString["ret"]);

                Master.PageSubTitle = txtAppArea.Text;
			}
		}

        private Table ShowSummary(TaskStatus taskStatus)
        {
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
            string sql = "";
            int parentId = int.Parse(Request.QueryString["pid"]);
            AppliesTo parentAppArea = (AppliesTo)int.Parse(Request.QueryString["paa"]);
            DBConnection db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));
            db.sqlexecute.Parameters.AddWithValue("@parentId", parentId);

            switch (parentAppArea)
            {
                case AppliesTo.CONTRACT_DETAILS:
                case AppliesTo.CONTRACT_GROUPING:
                    txtAppArea.Text = "Contract Details";
                    sql = "select [contractDescription] from contract_details where [contractId] = @parentId";

                    txtRegarding.Text = db.getStringValue(sql);

                    break;
                case AppliesTo.CONPROD_GROUPING:
                case AppliesTo.CONTRACT_PRODUCTS:
                    txtAppArea.Text = "Contract Product Details";
                    cProducts prods = new cProducts(curUser.AccountID, curUser.CurrentSubAccountId);
                    sql = "select [productid] from contract_productdetails where [contractProductId] = @parentId";
                    int prodId = db.getIntSum(sql);
                    cProduct product = prods.GetProductById(prodId);
                    if (product != null)
                    {
                        txtRegarding.Text = product.ProductName;
                    }
                    else
                    {
                        txtRegarding.Text = "Unknown";
                    }
                    break;
                case AppliesTo.VENDOR_DETAILS:
                    txtAppArea.Text = properties.SupplierPrimaryTitle;

                    cSuppliers suppliers;
                    if (curUser.CurrentSubAccountId >= 0)
                    {
                        suppliers = new cSuppliers(curUser.AccountID, curUser.CurrentSubAccountId);
                    }
                    else
                    {
                        suppliers = new cSuppliers(curUser.AccountID, curUser.CurrentSubAccountId);
                    }
                    cSupplier supplier = suppliers.getSupplierById(parentId);
                    if (supplier != null)
                    {
                        txtRegarding.Text = supplier.SupplierName;
                    }
                    else
                    {
                        txtRegarding.Text = "Unknown";
                    }
                    break;
                case AppliesTo.PRODUCT_DETAILS:
                    txtAppArea.Text = "Product Details";

                    cProducts clsProducts = new cProducts(curUser.AccountID, curUser.CurrentSubAccountId);
                    cProduct oProduct = clsProducts.GetProductById(parentId);

                    if (oProduct != null)
                    {
                        txtRegarding.Text = oProduct.ProductName;
                    }
                    else
                    {
                        txtRegarding.Text = "Unknown";
                    }
                    break;
                default:
                    txtAppArea.Text = "Unknown Area";
                    txtRegarding.Text = "";
                    break;
            }

            cTasks tasks = new cTasks(curUser.AccountID, curUser.CurrentSubAccountId);
            cTeams teams = new cTeams(curUser.AccountID, curUser.CurrentSubAccountId);
            cEmployees emps = new cEmployees(curUser.AccountID);

            Dictionary<int, cTask> summaryTasks = tasks.GetTaskSummary(parentAppArea, parentId, taskStatus);

            Table taskTable = new Table();
            taskTable.CssClass = "datatbl";

            bool rowalt = false;
            string rowClass = "row1";
            string viewImgPath = "~/shared/images/icons/edit.png";

            TableHeaderRow headerRow = new TableHeaderRow();
            TableHeaderCell thcell;

            thcell = new TableHeaderCell();
            Image img = new Image();
            img.ImageUrl = viewImgPath;
            thcell.Controls.Add(img);
            thcell.Width = Unit.Pixel(25);
            headerRow.Cells.Add(thcell);

            thcell = new TableHeaderCell();
            thcell.Text = "Subject";
            headerRow.Cells.Add(thcell);

            thcell = new TableHeaderCell();
            thcell.Text = "Task Owner";
            headerRow.Cells.Add(thcell);

            thcell = new TableHeaderCell();
            thcell.Text = "Start Date";
            headerRow.Cells.Add(thcell);

            thcell = new TableHeaderCell();
            thcell.Text = "Due Date";
            headerRow.Cells.Add(thcell);

            thcell = new TableHeaderCell();
            thcell.Text = "End Date";
            headerRow.Cells.Add(thcell);

            taskTable.Rows.Add(headerRow);

            bool hasdata = false;

            foreach (KeyValuePair<int, cTask> i in summaryTasks)
            {
                cTask curTask = (cTask)i.Value;

                rowalt = (rowalt ^ true);
                if (rowalt)
                {
                    rowClass = "row1";
                }
                else
                {
                    rowClass = "row2";
                }

                TableRow trow = new TableRow();

                TableCell tcell;

                tcell = new TableCell();
                tcell.CssClass = rowClass;
                tcell.Attributes.Add("style", "text-align:center;");

                Image imgbtn = new Image();
                imgbtn.ID = "img" + curTask.TaskId.ToString();
                imgbtn.ImageUrl = viewImgPath;
                imgbtn.Attributes.Add("onmouseover", "window.status='View Task Details';return true;");
                imgbtn.Attributes.Add("onmouseout", "window.status='Done';");
                imgbtn.ToolTip = "View Task Details";
                imgbtn.Attributes.Add("onclick", "javascript:window.location.href='ViewTask.aspx?rid=" + parentId.ToString() + "&rtid=" + ((int)parentAppArea).ToString() + "&tid=" + curTask.TaskId.ToString() + "&tsret=" + Server.UrlEncode(Request.RawUrl) + "';");
                imgbtn.Attributes.Add("style", "cursor: hand;");
                tcell.Controls.Add(imgbtn);
                trow.Cells.Add(tcell);

                tcell = new TableCell();
                tcell.CssClass = rowClass;
                tcell.Text = curTask.Subject;
                trow.Cells.Add(tcell);

                tcell = new TableCell();
                tcell.CssClass = rowClass;

                switch (curTask.TaskOwner.OwnerType)
                {
                    case sendType.employee:
                        Employee empOwner = emps.GetEmployeeById(curTask.TaskOwner.OwnerId);
                        tcell.Text = empOwner.Forename + " " + empOwner.Surname;
                        break;
                    case sendType.team:
                        tcell.Text = teams.GetTeamById(curTask.TaskOwner.OwnerId).teamname;
                        break;
                    default:
                        break;
                }
                trow.Cells.Add(tcell);

                tcell = new TableCell();
                tcell.CssClass = rowClass;
                if (curTask.StartDate.HasValue)
                {
                    tcell.HorizontalAlign = HorizontalAlign.Center;
                    tcell.Text = curTask.StartDate.Value.ToShortDateString();
                }
                trow.Cells.Add(tcell);

                tcell = new TableCell();
                tcell.CssClass = rowClass;
                if (curTask.DueDate.HasValue)
                {
                    tcell.HorizontalAlign = HorizontalAlign.Center;
                    tcell.Text = curTask.DueDate.Value.ToShortDateString();
                }
                trow.Cells.Add(tcell);

                tcell = new TableCell();
                tcell.CssClass = rowClass;
                if (curTask.EndDate.HasValue)
                {
                    tcell.HorizontalAlign = HorizontalAlign.Center;
                    tcell.Text = curTask.EndDate.Value.ToShortDateString();
                }
                trow.Cells.Add(tcell);

                taskTable.Rows.Add(trow);

                hasdata = true;
            }

            if (!hasdata)
            {
                TableRow trow = new TableRow();
                trow.CssClass = "row1";

                TableCell tcell = new TableCell();
                tcell.ColumnSpan = 7;
                tcell.HorizontalAlign = HorizontalAlign.Center;
                tcell.Text = "No tasks currently for this status";
                trow.Cells.Add(tcell);

                taskTable.Rows.Add(trow);
            }

            return taskTable;
        }

		protected void cmdClose_Click(object sender, ImageClickEventArgs e)
		{
			if (returnURL.Value != "")
			{
				Response.Redirect(returnURL.Value, true);
			}
			else
			{
				Response.Redirect("~/Home.aspx", true);
			}
		}

		protected void lnkViewActive_Click(object sender, EventArgs e)
		{
			int parentId = int.Parse(Request.QueryString["pid"]);
			int parentAppArea = int.Parse(Request.QueryString["paa"]);

			Response.Redirect("~/shared/tasks/TaskSummary.aspx?pid=" + parentId.ToString() + "&paa=" + parentAppArea.ToString() + "&ret=" + Server.UrlEncode(returnURL.Value), true);
		}

		protected void lnkViewClosed_Click(object sender, EventArgs e)
		{
			int parentId = int.Parse(Request.QueryString["pid"]);
			int parentAppArea = int.Parse(Request.QueryString["paa"]);

			Response.Redirect("~/shared/tasks/TaskSummary.aspx?vc=1&pid=" + parentId.ToString() + "&paa=" + parentAppArea.ToString() + "&ret=" + Server.UrlEncode(returnURL.Value), true);
		}
	}
}
