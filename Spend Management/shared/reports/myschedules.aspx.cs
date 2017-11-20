using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using expenses;
using SpendManagementLibrary;
using Spend_Management;
public partial class reports_myschedules : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (IsPostBack == false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;
				
            IScheduler clsscheduler = (IScheduler)Activator.GetObject(typeof(IScheduler), ConfigurationManager.AppSettings["SchedulerServicePath"] + "/scheduler.rem");


            if (Request.QueryString["action"] != null)
            {
                int action = int.Parse(Request.QueryString["action"]);
                switch (action)
                {
                    case 3://delete
                        clsscheduler.deleteSchedule(user.AccountID, int.Parse(Request.Form["scheduleid"]));
                        break;
                }
            }

            Title = "My Schedules";
            Master.title = "Scheduled Reports";
            Master.PageSubTitle = Title;

            switch (user.CurrentActiveModule)
            {
                case Modules.contracts:
                    Master.helpid = 1155;
                    break;
                default:
                    Master.helpid = 0;
                    break;
            }

            try
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(clsscheduler.getGrid(user.AccountID, user.EmployeeID));
                cGrid clsgrid = new cGrid(user.AccountID, ds, true, false, Grid.Schedules);

                cGridColumn newcol;
                cGridRow reqrow;


                newcol = new cGridColumn("Delete", "<img alt=\"Delete\" src=\"../images/icons/delete2.png\">", "S", "", false, true);
                clsgrid.gridcolumns.Insert(0, newcol);
                newcol = new cGridColumn("Edit", "<img alt=\"Edit\" src=\"../images/icons/edit.png\">", "S", "", false, true);
                clsgrid.gridcolumns.Insert(0, newcol);
                clsgrid.getColumn("scheduleid").hidden = true;
                clsgrid.getColumn("financialexportid").hidden = true;
                clsgrid.getColumn("reportid").description = "Report";
                cReports clsreports = new cReports(user.AccountID, user.CurrentSubAccountId);
                ArrayList reportlst = clsreports.getValueList();
                
                for (int i = 0; i < reportlst.Count; i++)
                {
                    clsgrid.getColumn("reportid").listitems.addItem(((object[])reportlst[i])[0], (string)((object[])reportlst[i])[1]);
                }
                clsgrid.getColumn("startdate").description = "Start Date";
                clsgrid.getColumn("startdate").fieldtype = "D";
                clsgrid.getColumn("enddate").description = "End Date";
                clsgrid.getColumn("enddate").fieldtype = "D";
                clsgrid.getColumn("scheduletype").description = "Schedule Type";
                clsgrid.getColumn("outputtype").description = "Output Type";
                clsgrid.getColumn("deliverymethod").description = "Delivery Method";
                clsgrid.tblclass = "datatbl";
                clsgrid.tableid = "schedules";
                clsgrid.emptytext = "No schedules to display";
                clsgrid.idcolumn = clsgrid.getColumn("scheduleid");
                clsgrid.getData();
                string sDeliveryMethod = "";
                for (int i = 0; i < clsgrid.gridrows.Count; i++)
                {

                    reqrow = (cGridRow)clsgrid.gridrows[i];
                    reqrow.getCellByName("Edit").thevalue = "<a href=\"aeschedule.aspx?action=2&returnto=2&reportid=" + reqrow.getCellByName("reportid").thevalue + "&financialexportid=" + reqrow.getCellByName("financialexportid").thevalue + "&scheduleid=" + reqrow.getCellByName("scheduleid").thevalue + "\"><img alt=\"Edit\" src=\"../images/icons/edit.gif\"></a>";
                    reqrow.getCellByName("Delete").thevalue = "<a href=\"javascript:deleteSchedule(" + reqrow.getCellByName("scheduleid").thevalue + ");\"><img alt=\"Delete\" src=\"../images/icons/delete2.gif\"></a>";

                    sDeliveryMethod = reqrow.getCellByName("deliverymethod").thevalue.ToString();
                    sDeliveryMethod = sDeliveryMethod[0].ToString().ToUpper() + sDeliveryMethod.Substring(1, sDeliveryMethod.Length - 1);

                    reqrow.getCellByName("deliverymethod").thevalue = sDeliveryMethod;

                    if ((DateTime)reqrow.getCellByName("enddate").thevalue == new DateTime(1900, 01, 01))
                    {
                        reqrow.getCellByName("enddate").thevalue = "";
                    }
                }
                litgrid.Text = clsgrid.CreateGrid();
            }
            catch
            {
                Response.Redirect(cMisc.Path + "/shared/404.aspx", true);
            }
        }
    }
}
