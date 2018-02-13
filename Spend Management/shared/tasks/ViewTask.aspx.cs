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
using System.Text;
using SpendManagementLibrary;
using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Helpers;

namespace Spend_Management
{
	public partial class ViewTask : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            // don't cache the ViewTask page to prevent pasting of url which if obtains cached page, could allow them to complete tasks using the TaskSummary Modal.
            Response.Expires = 0;
            Response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1);
            Response.AddHeader("pragma", "no-cache");
            Response.AddHeader("cache-control", "private");
            Response.CacheControl = "no-cache";

			if (!this.IsPostBack)
			{
				CurrentUser curUser = cMisc.GetCurrentUser();

                switch (curUser.CurrentActiveModule)
                {
                    case Modules.contracts:
                        Master.helpid = 1169;
                        break;
                    default:
                        Master.helpid = 0;
                        break;
                }

                hiddenEditId.Value = Request.QueryString["tid"];

                // check user has view access to tasks
                if (!curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Tasks, true))
                {
                    Response.Redirect(cMisc.Path + "/shared/restricted.aspx", true);
                }

                // check user is either the task creator or is in the task ownership (individual or team member)
                if (hiddenEditId.Value != "" && hiddenEditId.Value != "0")
                {
                    cTasks tasks = new cTasks(curUser.AccountID, curUser.CurrentSubAccountId);
                    cTask curTask = tasks.GetTaskById(int.Parse(hiddenEditId.Value));

                    if (!curTask.isTaskCreator(curUser.EmployeeID) && !curTask.isTaskOwner(curUser.EmployeeID))
                    {
                        Response.Redirect(cMisc.Path + "/shared/restricted.aspx", true);
                    }
                }

                if(curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Tasks, true) == false && curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, true) == false) 
                {
                    cmdUpdate.Visible = false;
                }

                cmdUpdate.Attributes.Add("onclick", "javascript:if(validateform('tasks') == false) { return false; }");

				Title = "View Tasks";
                Master.PageSubTitle = "Task Details";
				//Master.isSubFolder = true;
				Master.enablenavigation = false;

                if (Request.QueryString["ret"] != null)
                {
                    returnURL.Value = (Request.QueryString["ret"]).Base64Decode();

                    // check for a direct return link to a certain supplier
                    if (returnURL.Value.Contains("supplier_details.aspx") && returnURL.Value.Contains("sid="))
                    {
                        int tmpInt = 0;
                        int tmpInt2 = 0;
                        int.TryParse(Request.QueryString["rid"], out tmpInt);
                        int.TryParse(Request.QueryString["rtid"], out tmpInt2);

                        if (tmpInt > 0 && returnURL.Value.Contains("sid=" + tmpInt.ToString()) && (AppliesTo)tmpInt2 == AppliesTo.VENDOR_DETAILS)
                        {
                            ViewState["directReturnToURL"] = "~/shared/supplier_details.aspx?sid=" + tmpInt.ToString();
                        }
                    }
                }

				cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
				//cParams fwparams = new cParams(uinfo, fws, uinfo.ActiveLocation);
				cAccountProperties parameters;
				if (curUser.CurrentSubAccountId >= 0)
				{
					parameters = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
				}
				else
				{
					parameters = subaccs.getFirstSubAccount().SubAccountProperties;
				}

				if (parameters.TaskStartDateMandatory)
				{
					reqTaskStart.Enabled = true;
				}
				if (parameters.TaskEndDateMandatory)
				{
					reqTaskEnd.Enabled = true;
				}
				if (parameters.TaskDueDateMandatory)
				{
					reqTaskDue.Enabled = true;
				}

				DisplayForEdit();
			}
		}

		protected void cmdCancel_Click(object sender, ImageClickEventArgs e)
		{
			string retParams = "";

			if (hiddenTSReturnURL.Value != "")
			{
				Response.Redirect(hiddenTSReturnURL.Value, true);
            }
            else if (ViewState["directReturnToURL"] != null && (string)ViewState["directReturnToURL"] != string.Empty)
            {
                Response.Redirect((string)ViewState["directReturnToURL"], true);
            }
            else
            {
                if (returnURL.Value != "")
                {
                    retParams = "?ret=" + (returnURL.Value).Base64Encode();
                }
                Response.Redirect("MyTasks.aspx" + retParams, true);
            }
		}

		protected void cmdUpdate_Click(object sender, ImageClickEventArgs e)
		{
			CurrentUser curUser = cMisc.GetCurrentUser();
            if (hiddenEditId.Value == "0")
            {
                if (!curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Tasks, true))
                {
                    Response.Redirect(cMisc.Path + "/shared/restricted.aspx", true);
                }
            }
            else
            {
                if (!curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Tasks, true))
                {
                    Response.Redirect(cMisc.Path + "/shared/restricted.aspx", true);
                }
            }
			cTasks tasks = new cTasks(curUser.AccountID, curUser.CurrentSubAccountId);
			cEmployees employees = new cEmployees(curUser.AccountID);
			cTeams teams = new cTeams(curUser.AccountID, curUser.CurrentSubAccountId);
			cBaseDefinitions clsBaseDefs = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.TaskTypes);

			DateTime? start = null;
			if (dtTaskStart.Text != "")
			{
				start = DateTime.Parse(dtTaskStart.Text);
			}
			DateTime? due = null;
			if (dtTaskDue.Text != "")
			{
				due = DateTime.Parse(dtTaskDue.Text);
			}
			DateTime? end = null;
			if (dtTaskEnd.Text != "")
			{
				end = DateTime.Parse(dtTaskEnd.Text);
			}
			TaskStatus status = (TaskStatus)int.Parse(lstStatus.SelectedItem.Value);
			cTaskOwner ownership;
			cTaskType tasktype = null;
			if (lstTaskType.SelectedItem.Value != "0")
			{
				tasktype = (cTaskType)clsBaseDefs.GetDefinitionByID(int.Parse(lstTaskType.SelectedItem.Value));
			}

			if (lstOwner.SelectedItem.Value.StartsWith("TEAM_"))
			{
				// must be a team selection
				int ownerid = int.Parse(lstOwner.SelectedItem.Value.Replace("TEAM_", ""));
				cTeam taskTeam = teams.GetTeamById(ownerid);
				ownership = new cTaskOwner(ownerid, sendType.team, taskTeam);
			}
			else
			{
				int ownerid = int.Parse(lstOwner.SelectedItem.Value);
				ownership = new cTaskOwner(ownerid, sendType.employee, null);
			}
			int? subaccountid = curUser.CurrentSubAccountId;
			
			Employee employee = employees.GetEmployeeById(curUser.EmployeeID);
			cTask newTask = new cTask(0, subaccountid, TaskCommand.Standard, employee.EmployeeID, DateTime.Now, tasktype, int.Parse(hiddenRegardingId.Value), (AppliesTo)int.Parse(hiddenRegardingArea.Value), txtSubject.Text, txtDescription.Text, start, due, end, status, ownership, false, null);

			if (hiddenEditId.Value == "0")
			{
				tasks.AddTask(newTask, curUser.EmployeeID);
			}
			else
			{
                // check that user is either the task creator or in the permitted ownership for updating the task
                cTask curTask = tasks.GetTaskById(int.Parse(hiddenEditId.Value));
                if (!curTask.isTaskCreator(curUser.EmployeeID) && !curTask.isTaskOwner(curUser.EmployeeID))
                {
                    Response.Redirect(cMisc.Path + "/shared/restricted.aspx", true);
                }

				// must be editing an existing task
				newTask.TaskId = int.Parse(hiddenEditId.Value);
				tasks.UpdateTask(newTask, curUser.EmployeeID);
			}

			if (status == TaskStatus.Cancelled || status == TaskStatus.Postponed || status == TaskStatus.Completed)
			{
				// notify task ownership of change of status - at must have been set to cancelled/postponed/complete
				cPendingEmails pendingemail = new cPendingEmails(curUser.AccountID, curUser.EmployeeID);
				pendingemail.CreatePendingTaskEmail(PendingMailType.TaskStatusChange, newTask);
			}


			string retParams = "";

			if (hiddenTSReturnURL.Value != "")
			{
				Response.Redirect(hiddenTSReturnURL.Value, true);
            }
            else if (ViewState["directReturnToURL"] != null && (string)ViewState["directReturnToURL"] != string.Empty)
            {
                Response.Redirect((string)ViewState["directReturnToURL"], true);
            }
            else
            {
                if (returnURL.Value != "")
                {
                    retParams = "?ret=" + returnURL.Value;
                }

                Response.Redirect("~/shared/tasks/MyTasks.aspx" + retParams, true);
            }
		}

		private void DisplayForEdit()
		{
			CurrentUser curUser = cMisc.GetCurrentUser();
			DBConnection db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));
			cTasks tasks = new cTasks(curUser.AccountID, curUser.CurrentSubAccountId);
			cEmployees emps = new cEmployees(curUser.AccountID);
			string sql = "";
            cTask curTask = null;
            bool hideAttachments = false;

            int regardingId = 0;
            AppliesTo regardingArea = AppliesTo.CONTRACT_DETAILS;

            if (hiddenEditId.Value != "0")
            {
                curTask = tasks.GetTaskById(int.Parse(hiddenEditId.Value));
                regardingId = curTask.RegardingId;
                regardingArea = curTask.RegardingArea;
            }
            else
            {
                if (Request.QueryString["rid"] != null)
                {
                    regardingId = int.Parse(Request.QueryString["rid"]);
                }

                if(Request.QueryString["rtid"] != null)
                {
                    regardingArea = (AppliesTo)int.Parse(Request.QueryString["rtid"]);
                }
            }

			// retrieve what the task is regarding
			if (Request.QueryString["tsret"] != null)
			{
				hiddenTSReturnURL.Value = (Request.QueryString["tsret"]).Base64Decode();
			}
            
			hiddenRegardingId.Value = regardingId.ToString();
			hiddenRegardingArea.Value = ((int)regardingArea).ToString();

			//AppAreas regardingArea = (AppAreas)int.Parse(hiddenRegardingArea.Value);

			db.sqlexecute.Parameters.AddWithValue("@regardingId", regardingId);

			switch (regardingArea)
			{
                case AppliesTo.CONTRACT_DETAILS:
                case AppliesTo.CONTRACT_GROUPING:
					sql = "select [contractDescription] from contract_details where [contractId] = @regardingId";

					txtRegarding.Text = db.getStringValue(sql);
					break;
                case AppliesTo.PRODUCT_DETAILS:
					sql = "select [productName] from productDetails where [productId] = @regardingId";
					
					txtRegarding.Text = db.getStringValue(sql);
					break;
                case AppliesTo.Employee:
                case AppliesTo.STAFF_DETAILS:
					Employee employee = emps.GetEmployeeById(regardingId);
					if (employee != null)
					{
						txtRegarding.Text = employee.Username;
					}
					else
					{
						txtRegarding.Text = "Unknown";
					}
                    hideAttachments = true;
					break;
                case AppliesTo.VENDOR_DETAILS:
					cSuppliers suppliers = new cSuppliers(curUser.AccountID, curUser.CurrentSubAccountId);
					cSupplier supplier = suppliers.getSupplierById(regardingId);

					if(supplier != null)
					{
						txtRegarding.Text = supplier.SupplierName;
					}
					else
					{
						txtRegarding.Text = "Unknown";
					}
					break;
                case AppliesTo.CONTRACT_PRODUCTS:
                case AppliesTo.CONPROD_GROUPING:
					sql = "select [productId] from contract_productdetails where [contractProductId] = @regardingId";
					int prodId = db.getIntSum(sql);
 
					cProducts products = new cProducts(curUser.AccountID, curUser.CurrentSubAccountId);
					cProduct product = products.GetProductById(prodId);
					if (product != null)
					{
						txtRegarding.Text = product.ProductName;
					}
					else
					{
						txtRegarding.Text = "Unknown";
					}
					break;
                case AppliesTo.INVOICE_DETAILS:
					sql = "select [invoiceNumber] from invoices where [invoiceID] = @regardingId";

					txtRegarding.Text = "INV: " + db.getStringValue(sql);
					break;
                case AppliesTo.INVOICE_FORECASTS:
					sql = "select Convert(nvarchar(25), [forecastAmount]) from contract_forecastdetails where [contractForecastId] = @regardingId";
					
					txtRegarding.Text = "Forecast Amount: " + db.getStringValue(sql);
					break;
                case AppliesTo.Car:
                    var employees = new cEmployees(curUser.AccountID);
                    var employeId = employees.GetEmployeeIdFromCarId(curTask.RegardingId);
                    var employeesCars = new cEmployeeCars(curUser.AccountID, employeId);
                    var currentCar = employeesCars.GetCarByID(curTask.RegardingId);
                    txtRegarding.Text = string.Format("{0} {1}", currentCar.make, currentCar.model);
                    break;
				default:
					return;
			}

			if (hiddenEditId.Value != "0")
			{
				PopulateLists(false, curTask.StatusId);

				if (curTask != null)
				{
					txtDescription.Text = curTask.Description;
					txtSubject.Text = curTask.Subject;
					if (curTask.StartDate.HasValue)
					{
						dtTaskStart.Text = curTask.StartDate.Value.ToShortDateString();
					}
					if (curTask.DueDate.HasValue)
					{
						dtTaskDue.Text = curTask.DueDate.Value.ToShortDateString();
					}
					if (curTask.EndDate.HasValue)
					{
						dtTaskEnd.Text = curTask.EndDate.Value.ToShortDateString();
					}

                    Employee empCreator = emps.GetEmployeeById(curTask.TaskCreator);

                    //Show the task creator
                    divCreator.Style.Add(HtmlTextWriterStyle.Display, "");

                    if (empCreator != null)
                    {
                        txtCreator.Text = empCreator.Surname + ", " + empCreator.Title + " " + empCreator.Forename + " (" + empCreator.Username + ")";
                    }

					if (curTask.TaskType != null)
					{
						if (curTask.TaskType.ID != 0)
						{
							lstTaskType.Items.FindByValue(((int)curTask.TaskType.ID).ToString()).Selected = true;
						}
					}
                    lstStatus.SelectedIndex = lstStatus.Items.IndexOf(lstStatus.Items.FindByValue(((int)curTask.StatusId).ToString()));
					string ownerVal = "";
					if (curTask.TaskOwner.OwnerType == sendType.team)
					{
						cTeams teams = new cTeams(curUser.AccountID, curUser.CurrentSubAccountId);
						cTeam curTeam = teams.GetTeamById(curTask.TaskOwner.OwnerId);
						ownerVal = curTeam.teamname + " (Team)";
					}
					else
					{
						Employee editUser = emps.GetEmployeeById(curTask.TaskOwner.OwnerId);

						ownerVal = editUser.Surname + ", " + editUser.Title + " " + editUser.Forename + " (" + editUser.Username + ")";
					}

					if (lstOwner.Items.FindByText(ownerVal) != null)
					{
						lstOwner.Items.FindByText(ownerVal).Selected = true;
					}
                    Master.title = "Task: " + curTask.Subject;
				}

				if (!curTask.isTaskCreator(curUser.EmployeeID) && !curTask.isTaskOwner(curUser.EmployeeID))
				{
					cmdUpdate.Visible = false;
					cmdCancel.ImageUrl = "~/buttons/page_close.gif";
				}

				GetAttachmentList(curTask);
			}
			else
			{
				PopulateLists(true, TaskStatus.NotStarted);
				
				GetAttachmentList(null);

                Master.title = "Task: New";
			}

            if (hideAttachments)
            {
                string script = "if(document.getElementById('attSpan') != null) { document.getElementById('attSpan').style.display = 'none'; }";
                ClientScript.RegisterStartupScript(this.GetType(), "hideAtt", script, true);
            }
			return;
		}

		private void PopulateLists(bool isAdd, TaskStatus curStatus)
		{
			CurrentUser curUser = cMisc.GetCurrentUser();
			if (curStatus == TaskStatus.NotStarted)
			{
				lstStatus.Items.Add(new ListItem("Not Started", ((int)TaskStatus.NotStarted).ToString()));
			}

			if (!isAdd)
			{
				lstStatus.Items.Add(new ListItem("In Progress", ((int)TaskStatus.InProgress).ToString()));
				lstStatus.Items.Add(new ListItem("Completed", ((int)TaskStatus.Completed).ToString()));
				lstStatus.Items.Add(new ListItem("Postponed", ((int)TaskStatus.Postponed).ToString()));
				lstStatus.Items.Add(new ListItem("Cancelled", ((int)TaskStatus.Cancelled).ToString()));
			}

            cTeams teams = new cTeams(curUser.AccountID, curUser.CurrentSubAccountId);
            lstOwner.Items.AddRange(teams.GetCombinedEmployeeListItems(true));

			if (hiddenEditId.Value == "0")
			{
				if (lstOwner.Items.FindByText(curUser.Employee.BankAccountDetails.AccountHolderName) != null)
				{
                    lstOwner.Items.FindByText(curUser.Employee.BankAccountDetails.AccountHolderName).Selected = true;
				}
			}

            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.TaskTypes);

            if (!isAdd)
            {
                cTasks tasks = new cTasks(curUser.AccountID, curUser.CurrentSubAccountId);
                cTask curTask = tasks.GetTaskById(int.Parse(hiddenEditId.Value));

                if (curTask != null)
                {
                    if (curTask.TaskType != null)
                    {
                        lstTaskType.Items.AddRange(clsBaseDefs.CreateDropDown(true, curTask.TaskType.ID));

                        lstTaskType.SelectedIndex = lstTaskType.Items.IndexOf(lstTaskType.Items.FindByValue(curTask.TaskType.ID.ToString()));
                    }
                    else
                    {
                        lstTaskType.Items.AddRange(clsBaseDefs.CreateDropDown(true, 0));
                    }
                }
                else
                {
                    lstTaskType.Items.AddRange(clsBaseDefs.CreateDropDown(true, 0));
                }
            }
            else
            {
                lstTaskType.Items.AddRange(clsBaseDefs.CreateDropDown(true, 0));
            }

            if (lstTaskType.Items.Count <= 1)
            {
                lstTasks.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
		}

		protected void cmdAttachments_Click(object sender, ImageClickEventArgs e)
		{
			Response.Redirect("~/Attachments.aspx?attarea=" + ((int)AttachmentArea.TASKS).ToString() + "&ref=" + hiddenEditId.Value + "&ret=" + returnURL.Value, true);
		}

		private void GetAttachmentList(cTask curTask)
		{
            CurrentUser curUser = cMisc.GetCurrentUser();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));
            string sql = "";
            StringBuilder strHTML = new StringBuilder();

            strHTML.Append("<script language=\"javascript\" type=\"text/javascript\">\n");
            strHTML.Append("function OpenAttachment()\n{\n");
            strHTML.Append("var path;\n");
            strHTML.Append("var att_list = document.getElementById('attachments');\n");
            strHTML.Append("path = att_list.options[att_list.selectedIndex].value;\n");
            strHTML.Append("if(path != '0')\n{\n");
            strHTML.Append("var att_type = path.substring(0,1);\n");
            strHTML.Append("var att_path = path.substring(1);\n");
            strHTML.Append("if(att_type == '" + (int)AttachmentType.Open + "' || att_type == '" + (int)AttachmentType.Audience + "')\n{\n");
            strHTML.Append("window.open('" + cMisc.Path + "/ViewAttachment.aspx?id=' + att_path);\n");
            strHTML.Append("}\n");
            strHTML.Append("else if(att_type == '" + (int)AttachmentType.Hyperlink + "')\n");
            strHTML.Append("{\n");
            strHTML.Append("window.open(att_path);\n");
            strHTML.Append("}\n");
            strHTML.Append("}\n");
            strHTML.Append("}\n</script>\n");
            strHTML.Append("<select name=\"attachments\" id=\"attachments\" tabindex=\"10\" onchange=\"OpenAttachment();\">\n");

            if (curTask != null)
            {
                cSecureData crypt = new cSecureData();

                sql = "SELECT * FROM [attachments] WHERE [referenceNumber] = @RefNum AND [attachmentArea] = @Attachment_Area AND dbo.CheckAttachmentAccess([attachmentId],@userId) > 0";
                db.sqlexecute.Parameters.AddWithValue("@RefNum", curTask.TaskId);
                db.sqlexecute.Parameters.AddWithValue("@Attachment_Area", (int)AttachmentArea.TASKS);
                db.sqlexecute.Parameters.AddWithValue("@userId", curUser.EmployeeID);
                DataSet dset = db.GetDataSet(sql);

                if (dset.Tables[0].Rows.Count > 0)
                {
                    lblAttachments.Text = dset.Tables[0].Rows.Count.ToString() + " Attachments";
                    strHTML.Append("<option value=\"0\" selected>[select attachment]</option>\n");

                    foreach (DataRow drow in dset.Tables[0].Rows)
                    {
                        string tmpStr = "";
                        if (drow["Description"] == null)
                        {
                            if ((AttachmentType)drow["attachmentType"] == AttachmentType.Hyperlink)
                            {
                                tmpStr = (string)drow["Directory"];
                            }
                            else
                            {
                                tmpStr = (string)drow["Directory"] + (string)drow["Filename"];
                            }
                        }
                        else
                        {
                            if ((string)drow["Description"] == "")
                            {
                                if ((AttachmentType)drow["attachmentType"] == AttachmentType.Hyperlink)
                                {
                                    tmpStr = (string)drow["Directory"];
                                }
                                else
                                {
                                    tmpStr = (string)drow["Directory"] + (string)drow["Filename"];
                                }
                            }
                            else
                            {
                                tmpStr = (string)drow["Description"];
                            }
                        }

                        switch ((AttachmentType)drow["attachmentType"])
                        {
                            case AttachmentType.Hyperlink:
                                strHTML.Append("<option value=\"" + ((int)drow["attachmentType"]).ToString() + (string)drow["Directory"] + "\">" + tmpStr + "</option>\n");
                                break;
                            default:
                                strHTML.Append("<option value=\"" + ((int)drow["attachmentType"]).ToString() + (crypt.Encrypt(((int)drow["attachmentId"]).ToString())).Base64Encode() + "\">" + tmpStr + "</option>\n");
                                break;
                        }
                    }
                }
                else
                {
                    strHTML.Append("<option value=\"0\">[No Links]</option>\n");
                }
            }
            else
            {
                strHTML.Append("<option value=\"0\">[No Links]</option>\n");
                cmdAttachments.Visible = false;
            }
            strHTML.Append("</select>\n");
            
            litAttachments.Text = strHTML.ToString();
			return;
		}
	}
}
