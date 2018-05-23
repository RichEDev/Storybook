using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    public partial class TaskHistory : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
			{
				Title = "Task History";
                Master.PageSubTitle = Title;
				//Master.isSubFolder = true;

                CurrentUser user = cMisc.GetCurrentUser();
                switch (user.CurrentActiveModule)
                {
                    case Modules.Contracts:
                        Master.helpid = 1217;
                        break;
                    default:
                        Master.helpid = 0;
                        break;
                }

				int nTaskId = int.Parse(Request.QueryString["tid"]);

				ShowTaskHistory(nTaskId);
			}
		}

		private void ShowTaskHistory(int taskId)
		{
			CurrentUser curUser = cMisc.GetCurrentUser();
			StringBuilder strData = new StringBuilder();
			DBConnection db = new DBConnection(cAccounts.getConnectionString(curUser.AccountID));
			cTasks tasks = new cTasks(curUser.AccountID, curUser.CurrentSubAccountId);
			DataSet taskDSet = new DataSet();

			//cDataGrid taskGrid = new cDataGrid(curUser.UserFWS, curUser.currentUser.userInfo, taskDSet, true, false);
			//taskGrid.isSubFolder = true;
			//taskGrid.tblclass = "datatbl";

			Table taskTable = new Table();
			taskTable.CssClass = "datatbl";

			TableHeaderRow taskTHeader = new TableHeaderRow();
			TableHeaderCell taskTHCell = new TableHeaderCell();

			cTask task = tasks.GetTaskById(taskId);
			lblTaskTitle.Text = task.Subject;

			taskTHCell.Text = "Date";
			taskTHCell.Width = Unit.Pixel(80);
			taskTHeader.Cells.Add(taskTHCell);

			taskTHCell = new TableHeaderCell();
			taskTHCell.Text = "Change Details";
			taskTHCell.Width = Unit.Pixel(150);
			taskTHeader.Cells.Add(taskTHCell);

			taskTHCell = new TableHeaderCell();
			taskTHCell.Text = "Original Value";
			taskTHCell.Width = Unit.Pixel(100);
			taskTHCell.Wrap = true;
			taskTHeader.Cells.Add(taskTHCell);

			taskTHCell = new TableHeaderCell();
			taskTHCell.Text = "Changed Value";
			taskTHCell.Width = Unit.Pixel(100);
			taskTHeader.Cells.Add(taskTHCell);

			taskTable.Rows.Add(taskTHeader);

			TableRow trow;
			bool hasdata = false;
			string rowClass = "row1";
			bool rowalt = false;

			string sql = "SELECT datestamp, changeDetails, ISNULL([preVal],'') AS [preVal], ISNULL([postVal],'') AS [postVal] FROM task_history WHERE [taskId] = @taskId ORDER BY [datestamp] DESC";
			db.sqlexecute.Parameters.AddWithValue("@taskId", taskId);

		    using (System.Data.SqlClient.SqlDataReader reader = db.GetReader(sql))
		    {
		        while (reader.Read())
		        {
		            trow = new TableRow();

		            rowalt = (rowalt ^ true);
		            if (rowalt)
		            {
		                rowClass = "row1";
		            }
		            else
		            {
		                rowClass = "row2";
		            }

		            DateTime datestamp = reader.GetDateTime(reader.GetOrdinal("datestamp"));
		            string description = reader.GetString(reader.GetOrdinal("changeDetails"));
		            string preval = reader.GetString(reader.GetOrdinal("preVal"));
		            string postval = reader.GetString(reader.GetOrdinal("postVal"));

		            TableCell tcell = new TableCell();
		            tcell.CssClass = rowClass;
		            tcell.Text = datestamp.ToShortDateString();
		            trow.Cells.Add(tcell);

		            tcell = new TableCell();
		            tcell.CssClass = rowClass;
		            tcell.Text = description;
		            trow.Cells.Add(tcell);

		            tcell = new TableCell();
		            tcell.CssClass = rowClass;
		            tcell.Text = preval;
		            trow.Cells.Add(tcell);

		            tcell = new TableCell();
		            tcell.CssClass = rowClass;
		            tcell.Text = postval;
		            trow.Cells.Add(tcell);

		            taskTable.Rows.Add(trow);

		            hasdata = true;
		        }

		        reader.Close();
		    }

		    if (!hasdata)
			{
				TableCell tcell = new TableCell();
				tcell.ColumnSpan = 4;
				tcell.CssClass = "row1";
				tcell.HorizontalAlign = HorizontalAlign.Center;
				tcell.Text = "No history currently exists for this task";
				trow = new TableRow();
				trow.Cells.Add(tcell);
				taskTable.Rows.Add(trow);
			}
			historyPanel.Controls.Add(taskTable);

			return;
		}

		protected void cmdClose_Click(object sender, ImageClickEventArgs e)
		{
			Response.Redirect("~/shared/tasks/MyTasks.aspx", true);
		}
	}
}