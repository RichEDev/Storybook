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

public partial class reports_chartviewer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            

            int requestnum = int.Parse(Request.QueryString["requestnum"]);
            ViewState["requestnum"] = requestnum;

            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");


            cReportRequest clsrequest = (cReportRequest)Session["request" + requestnum];


            DataSet ds = null;// clsreports.createReport(clsrequest);
            clsreports.createReport(clsrequest);
            ViewState["ds"] = ds;
            createColumnList(clsrequest.report.columns);

            SortedList chartColumns = (SortedList)ViewState["chartcolumns"];
            cChartColumn chartcol;
            ListItem item;
            for (int i = 0; i < chartColumns.Count; i++)
            {
                chartcol = (cChartColumn)chartColumns.GetByIndex(i);
                item = new ListItem(chartcol.description, chartcol.columnid.ToString());
                if (chartcol.show)
                {
                    item.Selected = true;
                }
                chkcolumns.Items.Add(item);
            }

            createChart();
        }
    }
    protected void cmdcharttype_SelectedIndexChanged(object sender, EventArgs e)
    {
        changeChartType();
    }
    protected void chk3d_CheckedChanged(object sender, EventArgs e)
    {
        changeChartType();
    }

    private void changeChartType()
    {
        bool threeD = chk3d.Checked;

        switch (cmdcharttype.SelectedItem.Text)
        {
            case "Area Chart":
                if (threeD)
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart3D;
                }
                else
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.AreaChart;
                }
                break;
            case "Bar Chart":
                if (threeD)
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.BarChart3D;
                }
                else
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.BarChart;
                }
                break;
            case "Box Chart":
                if (threeD)
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.BarChart3D;
                }
                else
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.BoxChart;
                }
                break;
            case "Column Chart":
                if (threeD)
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart3D;
                }
                else
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.ColumnChart;
                }
                break;
            case "Doughnut Chart":
                if (threeD)
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart3D;
                }
                else
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.DoughnutChart;
                }
                break;
            case "Funnel Chart":
                if (threeD)
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.FunnelChart3D;
                }
                else
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.FunnelChart;
                }
                break;
            case "Line Chart":
                if (threeD)
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.LineChart3D;
                }
                else
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.LineChart;
                }
                break;
            case "Pie Chart":
                if (threeD)
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.PieChart3D;
                }
                else
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.PieChart;
                }
                break;
            case "Pyramid Chart":
                if (threeD)
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.PyramidChart3D;
                }
                else
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.PyramidChart;
                }
                break;
            case "Scatter Chart":

                chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.ScatterChart;

                break;
            case "Spline Chart":
                if (threeD)
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.SplineChart3D;
                }
                else
                {
                    chartrpt.ChartType = Infragistics.UltraChart.Shared.Styles.ChartType.SplineChart;
                }
                break;
        }

        createChart();
    }

    private void createColumnList(ArrayList columns)
    {
        cReportColumn column;
        cStandardColumn standardcol;
        cChartColumn chartcol;
        SortedList chartColumns = new SortedList();
        for (int i = 0; i < columns.Count; i++)
        {
            column = (cReportColumn)columns[i];
            switch (column.columntype)
            {
                case ReportColumnType.Standard:
                    standardcol = (cStandardColumn)column;
                    chartcol = new cChartColumn(standardcol.columnid, standardcol.field.Description, true, column.order);
                    chartColumns.Add(standardcol.columnid, chartcol);
                    break;
            }
        }

        ViewState["chartcolumns"] = chartColumns;
    }


    
    protected void chkcolumns_SelectedIndexChanged(object sender, EventArgs e)
    {
        cChartColumn chartcol;
        SortedList chartColumns = (SortedList)ViewState["chartcolumns"];
        for (int i = 0; i < chartColumns.Count; i++)
        {
            chartcol = (cChartColumn)chartColumns.GetByIndex(i);
            chartcol.show = chkcolumns.Items.FindByValue(chartcol.columnid.ToString()).Selected;
        }

        createChart();
    }

    private void createChart()
    {
        DataSet ds = (DataSet)ViewState["ds"];
        

        cChartColumn chartcol;
        SortedList chartColumns = (SortedList)ViewState["chartcolumns"];
        string[] labels;
        int colcount = chartColumns.Count;
        for (int i = 0; i < chartColumns.Count; i++)
        {
            chartcol = (cChartColumn)chartColumns.GetByIndex(i);
            
            if (chartcol.show == false)
            {
                chartrpt.Data.IncludeColumn(chartcol.order, false);
                colcount--;
            }
        }

        //set the labels
        labels = new string[colcount];
        colcount = 0;
        for (int i = 0; i < chartColumns.Count; i++)
        {
            chartcol = (cChartColumn)chartColumns.GetByIndex(i);
            
            if (chartcol.show == true)
            {
                labels[colcount] = chartcol.description;
                colcount++;
            }
        }
       //chartrpt.Data.SetColumnLabels(labels);
        //chartrpt.Axis.X.Labels.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.VerticalLeftFacing;
        
        chartrpt.DataSource = ds;
        chartrpt.DataBind();
    }
}

[Serializable()]
public class cChartColumn
{
    private int nColumnid;
    private string sDescription;
    private bool bShow;
    private int nOrder;
    public cChartColumn(int columnid, string description, bool show, int order)
    {
        nColumnid = columnid;
        sDescription = description;
        bShow = show;
        nOrder = order;
    }

    #region properties
    public int columnid
    {
        get { return nColumnid; }
    }
    public string description
    {
        get { return sDescription; }
    }
    public bool show
    {
        get
        {
            return bShow;
        }
        set { bShow = value; }
    }
    public int order
    {
        get { return nOrder; }
    }
    #endregion
}
