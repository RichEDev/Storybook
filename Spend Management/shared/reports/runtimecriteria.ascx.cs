using System;
using System.Collections;
using System.Web.UI.WebControls;

using Infragistics.WebUI.WebDataInput;
using AjaxControlToolkit;

using SpendManagementLibrary;
using Spend_Management;

using System.Collections.Generic;

using SpendManagementLibrary.Logic_Classes.Fields;

public partial class reports_runtimecriteria : System.Web.UI.UserControl
{
    private int nAccountid;
    private int nSubAccountId;

    private cReport reqreport;
    private SortedList lstStaticCols;

    #region properties
    public int accountid
    {
        get { return nAccountid; }
        set { nAccountid = value; }
    }

    public int subAccountId
    {
        get { return nSubAccountId; }
        set { nSubAccountId = value; }
    }

    public cReport report
    {
        get { return reqreport; }
        set { reqreport = value; }
    }
    public SortedList staticColumns
    {
        get { return lstStaticCols; }
        set { lstStaticCols = value; }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {

        }
        holderStatic.Controls.Add(createStaticFieldTable());
        holderCriteria.Controls.Add(createCriteriaTable());
    }

    private Table createStaticFieldTable()
    {

        cReportColumn column;
        cStaticColumn staticcol;
        Table tbl = new Table();
        TableRow row;
        TableCell cell;
        TextBox txtbox;
        RequiredFieldValidator reqval;
        string rowclass = "row1";
        tbl.CssClass = "datatbl";

        for (int i = 0; i < report.columns.Count; i++)
        {
            column = (cReportColumn)report.columns[i];
            if (column.columntype == ReportColumnType.Static)
            {
                staticcol = (cStaticColumn)column;


                if (staticcol.runtime)
                {
                    row = new TableRow();
                    cell = new TableCell();
                    cell.Text = staticcol.literalname;
                    cell.CssClass = rowclass;
                    row.Cells.Add(cell);
                    cell = new TableCell();
                    cell.CssClass = rowclass;
                    txtbox = new TextBox();
                    txtbox.ID = "statictxt" + staticcol.order;
                    if (lstStaticCols != null && lstStaticCols.ContainsKey(staticcol.reportcolumnid))
                    {
                        txtbox.Text = lstStaticCols[staticcol.reportcolumnid].ToString();
                    }
                    cell.Controls.Add(txtbox);
                    row.Cells.Add(cell);
                    cell = new TableCell();
                    cell.CssClass = rowclass;
                    reqval = new RequiredFieldValidator();
                    reqval.ControlToValidate = "statictxt" + staticcol.order;
                    reqval.ErrorMessage = "Please enter a value for " + staticcol.literalname;
                    reqval.Text = "*";
                    cell.Controls.Add(reqval);
                    row.Cells.Add(cell);
                    tbl.Rows.Add(row);
                    if (rowclass == "row1")
                    {
                        rowclass = "row2";
                    }
                    else
                    {
                        rowclass = "row1";
                    }
                }
            }
        }
        return tbl;
    }
    private Table createCriteriaTable()
    {
        cReportCriterion criteria;

        Table tbl = new Table();
        TableRow row;
        TableCell cell;
        RequiredFieldValidator val1 = null;
        RequiredFieldValidator val2 = null;
        RequiredFieldValidator valtime1 = null;
        RequiredFieldValidator valtime2 = null;
        string rowclass = "row1";
        tbl.CssClass = "datatbl";

        cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(accountid);
        cAccountProperties accountProperties = clsSubAccounts.getSubAccountById(report.SubAccountID ?? cMisc.GetCurrentUser().CurrentSubAccountId).SubAccountProperties;
        var relabeler = new FieldRelabler(accountProperties);

        DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
        System.Data.SqlClient.SqlDataReader reader;
        cField field;
        string strsql;

        for (int i = 0; i < report.criteria.Count; i++)
        {
            criteria = (cReportCriterion)report.criteria[i];
            val1 = null;
            val2 = null;
            valtime1 = null;
            valtime2 = null;
            if (criteria.runtime)
            {
                row = new TableRow();
                cell = new TableCell { CssClass = rowclass, Text = relabeler.Relabel(criteria.field).Description };
                row.Cells.Add(cell);

                #region condition
                cell = new TableCell();
                cell.CssClass = rowclass;
                switch (criteria.condition)
                {
                    case ConditionType.Between:
                        cell.Text = "Between";
                        break;
                    case ConditionType.DoesNotEqual:

                        cell.Text = "Not Equal To";

                        break;
                    case ConditionType.NotOn:
                        cell.Text = "Not On";
                        break;
                    case ConditionType.Equals:


                        cell.Text = "Equal To";

                        break;
                    case ConditionType.On:
                        cell.Text = "On";
                        break;
                    case ConditionType.GreaterThan:

                        cell.Text = "Greater Than";

                        break;
                    case ConditionType.After:
                        cell.Text = "After";
                        break;
                    case ConditionType.GreaterThanEqualTo:

                        cell.Text = "Greater Than or Equal To";

                        break;
                    case ConditionType.OnOrAfter:
                        cell.Text = "On or After";
                        break;
                    case ConditionType.LastXDays:
                        cell.Text = "in the last (days)";
                        break;
                    case ConditionType.LastXMonths:
                        cell.Text = "in the last (months)";
                        break;
                    case ConditionType.LastXWeeks:
                        cell.Text = "in the last (weeks)";
                        break;
                    case ConditionType.LastXYears:
                        cell.Text = "in the last (years)";
                        break;
                    case ConditionType.LessThan:
                        cell.Text = "Less Than";
                        break;
                    case ConditionType.Before:
                        cell.Text = "Before";
                        break;
                    case ConditionType.LessThanEqualTo:

                        cell.Text = "Less Than or Equal To";

                        break;
                    case ConditionType.OnOrBefore:
                        cell.Text = "On or Before";
                        break;
                    case ConditionType.Like:
                        cell.Text = "Like";
                        break;
                    case ConditionType.NextXDays:
                        cell.Text = "in the next (days)";
                        break;
                    case ConditionType.NextXMonths:
                        cell.Text = "in the next (months)";
                        break;
                    case ConditionType.NextXWeeks:
                        cell.Text = "in the next (weeks)";
                        break;
                    case ConditionType.NextXYears:
                        cell.Text = "in the next (years)";
                        break;
                }
                row.Cells.Add(cell);
                #endregion

                #region criteria
                cell = new TableCell();
                cell.CssClass = rowclass;
                WebTextEdit txtbox;
                field = criteria.field;

                switch (criteria.field.FieldType)
                {
                    case "S":
                    case "FS":
                    case "LT":
                    case "R":
                        txtbox = new WebTextEdit();
                        txtbox.ID = "txtbox" + criteria.order;

                        if (criteria.value1 != null)
                        {
                            if (criteria.value1[0] != null)
                            {
                                if (criteria.field.FieldType == "R")
                                {
                                    strsql = "select [" + field.GetRelatedTable().TableName + "].[" + field.FieldName + "] from [" + field.GetRelatedTable().TableName + "] where [" + field.GetRelatedTable().TableName + "].[" + field.GetRelatedTable().GetPrimaryKey().FieldName + "] in (" + criteria.value1[0].ToString() + ")";
                                }
                                else
                                {
                                    strsql = "select [" + field.GetParentTable().TableName + "].[" + field.FieldName + "] from [" + field.GetParentTable().TableName + "] where [" + field.GetParentTable().TableName + "].[" + field.GetParentTable().GetPrimaryKey().FieldName + "] in (" + criteria.value1[0].ToString() + ")";
                                }

                                using (reader = expdata.GetReader(strsql))
                                {
                                    while (reader.Read())
                                    {
                                        txtbox.Value += reader.GetString(0) + ", ";
                                    }
                                    reader.Close();
                                }
                                if (txtbox.Text.Length != 0)
                                {
                                    txtbox.Text = txtbox.Text.Remove(txtbox.Text.Length - 2, 2);
                                }
                            }
                        }
                        cell.Controls.Add(txtbox);

                        //txtbox = (WebTextEdit)FindControl("txtbox" + criteria.order + "_values");

                        val1 = new RequiredFieldValidator();
                        val1.ControlToValidate = "txtbox" + criteria.order;

                        if (criteria.field.GenList)
                        {
                            txtbox.ReadOnly = true;
                            txtbox = new WebTextEdit();

                            txtbox.ID = "txtbox" + criteria.order + "_values";
                            txtbox.Style["display"] = "none";

                            txtbox.ReadOnly = true;

                            if (criteria.value1 != null)
                            {
                                if (criteria.value1[0] != null)
                                {
                                    txtbox.Value = criteria.value1[0].ToString();
                                }
                            }
                            cell.Controls.Add(txtbox);
                            Literal lit = new Literal();
                            lit.Text = "&nbsp;<input type=button onclick=\"selectListOption('" + criteria.field.FieldID + "'," + criteria.order + ");\" value=\". . .\">";
                            cell.Controls.Add(lit);
                        }
                        break;
                    case "D":
                        Image img;
                        CalendarExtender cal;
                        MaskedEditExtender maskededit;
                        TextBox txtdate;
                        switch (criteria.condition)
                        {
                            case ConditionType.Between:
                                txtdate = new TextBox();
                                txtdate.ID = "txtdate" + criteria.order;
                                if (criteria.value1 != null)
                                {
                                    if (criteria.value1.GetLength(0) > 0)
                                    {
                                        if (criteria.value1[0] != null)
                                        {
                                            txtdate.Text = DateTime.Parse((criteria.value1[0]).ToString()).ToShortDateString();
                                        }
                                    }
                                }
                                cell.Controls.Add(txtdate);
                                maskededit = new MaskedEditExtender();
                                maskededit.TargetControlID = txtdate.ID;
                                maskededit.Mask = "99/99/9999";
                                maskededit.MaskType = MaskedEditType.Date;
                                maskededit.CultureName = "en-GB";
                                maskededit.ID = "mskdate" + criteria.order;
                                img = new Image();
                                img.ImageUrl = "../images/icons/cal.gif";
                                img.ID = "imgcal" + criteria.order;
                                cell.Controls.Add(img);
                                cal = new CalendarExtender();
                                cal.TargetControlID = "txtdate" + criteria.order;
                                cal.Format = "dd/MM/yyyy";
                                cal.PopupButtonID = "imgcal" + criteria.order;
                                cal.ID = "caldate" + criteria.order;
                                cell.Controls.Add(cal);
                                cell.Controls.Add(maskededit);
                                val1 = new RequiredFieldValidator();
                                val1.ControlToValidate = "txtdate" + criteria.order;

                                txtdate = new TextBox();
                                txtdate.ID = "txtdate2" + criteria.order;
                                if (criteria.value2 != null)
                                {
                                    if (criteria.value2.GetLength(0) > 0)
                                    {
                                        if (criteria.value2[0] != null)
                                        {
                                            txtdate.Text = DateTime.Parse((criteria.value2[0]).ToString()).ToShortDateString();
                                        }
                                    }
                                }
                                cell.Controls.Add(txtdate);
                                maskededit = new MaskedEditExtender();
                                maskededit.TargetControlID = txtdate.ID;
                                maskededit.Mask = "99/99/9999";
                                maskededit.MaskType = MaskedEditType.Date;
                                maskededit.CultureName = "en-GB";
                                maskededit.ID = "mskdate2" + criteria.order;
                                img = new Image();
                                img.ImageUrl = "../images/icons/cal.gif";
                                img.ID = "imgcal2" + criteria.order;
                                cell.Controls.Add(img);
                                cal = new CalendarExtender();
                                cal.TargetControlID = "txtdate2" + criteria.order; ;
                                cal.Format = "dd/MM/yyyy";
                                cal.PopupButtonID = "imgcal2" + criteria.order;
                                cal.ID = "caldate2" + criteria.order;
                                cell.Controls.Add(cal);
                                cell.Controls.Add(maskededit);


                                val2 = new RequiredFieldValidator();
                                val2.ControlToValidate = "txtdate2" + criteria.order;
                                break;
                            case ConditionType.NotOn:
                            case ConditionType.On:
                            case ConditionType.After:
                            case ConditionType.OnOrAfter:
                            case ConditionType.Before:
                            case ConditionType.OnOrBefore:
                                txtdate = new TextBox();
                                txtdate.ID = "txtdate" + criteria.order;
                                if (criteria.value1 != null)
                                {
                                    if (criteria.value1.GetLength(0) > 0)
                                    {
                                        if (criteria.value1[0] != null)
                                        {
                                            txtdate.Text = DateTime.Parse((criteria.value1[0]).ToString()).ToShortDateString();
                                        }
                                    }
                                }
                                cell.Controls.Add(txtdate);
                                maskededit = new MaskedEditExtender();
                                maskededit.TargetControlID = txtdate.ID;
                                maskededit.Mask = "99/99/9999";
                                maskededit.MaskType = MaskedEditType.Date;
                                maskededit.CultureName = "en-GB";
                                maskededit.ID = "mskdate" + criteria.order;
                                img = new Image();
                                img.ImageUrl = "../images/icons/cal.gif";
                                img.ID = "imgcal" + criteria.order;
                                cell.Controls.Add(img);
                                cal = new CalendarExtender();
                                cal.TargetControlID = "txtdate" + criteria.order; ;
                                cal.Format = "dd/MM/yyyy";
                                cal.PopupButtonID = "imgcal" + criteria.order;
                                cell.Controls.Add(cal);
                                cell.Controls.Add(maskededit);
                                val1 = new RequiredFieldValidator();
                                val1.ControlToValidate = "txtdate" + criteria.order;

                                break;
                            case ConditionType.LastXDays:
                            case ConditionType.LastXMonths:
                            case ConditionType.LastXWeeks:
                            case ConditionType.LastXYears:
                            case ConditionType.NextXDays:
                            case ConditionType.NextXMonths:
                            case ConditionType.NextXWeeks:
                            case ConditionType.NextXYears:
                                WebNumericEdit txtdtnum = new WebNumericEdit();
                                txtdtnum.ID = "txt" + criteria.order;
                                txtdtnum.DataMode = NumericDataMode.Int;
                                if (criteria.value1 != null)
                                {
                                    if (criteria.value1[0] != null)
                                    {
                                        txtdtnum.Text = criteria.value1[0].ToString();
                                    }
                                }
                                cell.Controls.Add(txtdtnum);
                                val1 = new RequiredFieldValidator();
                                val1.ControlToValidate = "txt" + criteria.order;
                                break;

                        }
                        break;
                    case "DT":
                        Image imgdt;
                        CalendarExtender caldt;
                        MaskedEditExtender maskededitdt;
                        TextBox txtdatedt;
                        TextBox txttimedt;
                        MaskedEditExtender maskededittimedt;

                        string datepart = "", timepart = "";
                        switch (criteria.condition)
                        {
                            case ConditionType.Between:
                                txtdatedt = new TextBox();
                                txttimedt = new TextBox();
                                txtdatedt.ID = "txtdate" + criteria.order;
                                txttimedt.ID = "txttime" + criteria.order;
                                if (criteria.value1 != null)
                                {
                                    if (criteria.value1[0] != null)
                                    {
                                        string[] datetime = criteria.value1[0].ToString().Split(' ');
                                        int timepos = 0;
                                        if (datetime[0] != "00/00/0000")
                                        {
                                            datepart = DateTime.Parse(datetime[0]).ToShortDateString();
                                            timepos = 1;
                                        }
                                        timepart = datetime[timepos];
                                        timepart = String.IsNullOrEmpty(timepart) ? timepart : timepart.Substring(0, 5);
                                        txtdatedt.Text = datepart;
                                        txttimedt.Text = timepart;
                                    }
                                }
                                cell.Controls.Add(txtdatedt);
                                cell.Controls.Add(txttimedt);
                                maskededitdt = new MaskedEditExtender();
                                maskededitdt.TargetControlID = txtdatedt.ID;
                                maskededitdt.Mask = "99/99/9999";
                                maskededitdt.MaskType = MaskedEditType.Date;
                                maskededitdt.CultureName = "en-GB";
                                maskededitdt.ID = "mskdate" + criteria.order;
                                maskededittimedt = new MaskedEditExtender();
                                maskededittimedt.TargetControlID = txttimedt.ID;
                                maskededittimedt.Mask = "99:99";
                                maskededittimedt.MaskType = MaskedEditType.Time;
                                maskededittimedt.CultureName = "en-GB";
                                maskededittimedt.ID = "msktime" + criteria.order;
                                imgdt = new Image();
                                imgdt.ImageUrl = "../images/icons/cal.gif";
                                imgdt.ID = "imgcal" + criteria.order;
                                cell.Controls.Add(imgdt);
                                caldt = new CalendarExtender();
                                caldt.TargetControlID = txtdatedt.ID;
                                caldt.Format = "dd/MM/yyyy";
                                caldt.PopupButtonID = "imgcal" + criteria.order;
                                caldt.ID = "caldate" + criteria.order;
                                cell.Controls.Add(caldt);
                                cell.Controls.Add(maskededitdt);
                                cell.Controls.Add(maskededittimedt);
                                val1 = new RequiredFieldValidator();
                                val1.ControlToValidate = txtdatedt.ID;
                                valtime1 = new RequiredFieldValidator();
                                valtime1.ControlToValidate = txttimedt.ID;

                                txtdatedt = new TextBox();
                                txttimedt = new TextBox();
                                txtdatedt.ID = "txtdate2" + criteria.order;
                                txttimedt.ID = "txttimeend" + criteria.order;
                                datepart = null;
                                timepart = "";
                                if (criteria.value2 != null)
                                {
                                    if (criteria.value2[0] != null)
                                    {
                                        string[] datetime = criteria.value2[0].ToString().Split(' ');
                                        int timepos = 0;
                                        if (datetime[0] != "00/00/0000")
                                        {
                                            datepart = DateTime.Parse(datetime[0]).ToShortDateString();
                                            timepos = 1;
                                        }
                                        timepart = datetime[timepos];
                                        timepart = String.IsNullOrEmpty(timepart) ? timepart : timepart.Substring(0, 5);
                                        txtdatedt.Text = datepart;
                                        txttimedt.Text = timepart;
                                    }
                                }
                                cell.Controls.Add(txtdatedt);
                                cell.Controls.Add(txttimedt);
                                maskededitdt = new MaskedEditExtender();
                                maskededitdt.TargetControlID = txtdatedt.ID;
                                maskededitdt.Mask = "99/99/9999";
                                maskededitdt.MaskType = MaskedEditType.Date;
                                maskededitdt.CultureName = "en-GB";
                                maskededitdt.ID = "mskdate2" + criteria.order;
                                maskededittimedt = new MaskedEditExtender();
                                maskededittimedt.TargetControlID = txttimedt.ID;
                                maskededittimedt.Mask = "99:99";
                                maskededittimedt.MaskType = MaskedEditType.Time;
                                maskededittimedt.CultureName = "en-GB";
                                maskededittimedt.ID = "msktime2" + criteria.order;
                                imgdt = new Image();
                                imgdt.ImageUrl = "../images/icons/cal.gif";
                                imgdt.ID = "imgcal2" + criteria.order;
                                cell.Controls.Add(imgdt);
                                caldt = new CalendarExtender();
                                caldt.TargetControlID = txtdatedt.ID;
                                caldt.Format = "dd/MM/yyyy";
                                caldt.PopupButtonID = "imgcal2" + criteria.order;
                                caldt.ID = "caldate2" + criteria.order;
                                cell.Controls.Add(caldt);
                                cell.Controls.Add(maskededitdt);
                                cell.Controls.Add(maskededittimedt);
                                val2 = new RequiredFieldValidator();
                                val2.ControlToValidate = txtdatedt.ID;
                                valtime2 = new RequiredFieldValidator();
                                valtime2.ControlToValidate = txttimedt.ID;
                                break;
                            case ConditionType.DoesNotEqual:
                            case ConditionType.Equals:
                            case ConditionType.On:
                            case ConditionType.After:
                            case ConditionType.OnOrAfter:
                            case ConditionType.Before:
                            case ConditionType.OnOrBefore:
                                txtdatedt = new TextBox();
                                txttimedt = new TextBox();
                                txtdatedt.ID = "txtdate" + criteria.order;
                                txttimedt.ID = "txttime" + criteria.order;
                                datepart = null;
                                timepart = "";
                                if (criteria.value1 != null)
                                {
                                    if (criteria.value1.GetLength(0) > 0)
                                    {
                                        if (criteria.value1[0] != null)
                                        {
                                            string[] datetime = criteria.value1[0].ToString().Split(' ');
                                            int timepos = 0;
                                            if (datetime[0] != "00/00/0000")
                                            {
                                                datepart = DateTime.Parse(datetime[0]).ToShortDateString();
                                                timepos = 1;
                                            }
                                            timepart = datetime[timepos];
                                            timepart = String.IsNullOrEmpty(timepart) ? timepart : timepart.Substring(0, 5);
                                            txtdatedt.Text = datepart;
                                            txttimedt.Text = timepart;
                                        }
                                    }
                                }
                                cell.Controls.Add(txtdatedt);
                                cell.Controls.Add(txttimedt);
                                maskededitdt = new MaskedEditExtender();
                                maskededitdt.TargetControlID = txtdatedt.ID;
                                maskededitdt.Mask = "99/99/9999";
                                maskededitdt.MaskType = MaskedEditType.Date;
                                maskededitdt.CultureName = "en-GB";
                                maskededitdt.ID = "mskdate" + criteria.order;
                                maskededittimedt = new MaskedEditExtender();
                                maskededittimedt.TargetControlID = txttimedt.ID;
                                maskededittimedt.Mask = "99:99";
                                maskededittimedt.MaskType = MaskedEditType.Time;
                                maskededittimedt.CultureName = "en-GB";
                                maskededittimedt.ID = "msktime" + criteria.order;
                                imgdt = new Image();
                                imgdt.ImageUrl = "../images/icons/cal.gif";
                                imgdt.ID = "imgcal" + criteria.order;
                                cell.Controls.Add(imgdt);
                                caldt = new CalendarExtender();
                                caldt.TargetControlID = txtdatedt.ID;
                                caldt.Format = "dd/MM/yyyy";
                                caldt.PopupButtonID = "imgcal" + criteria.order;
                                caldt.ID = "caldate" + criteria.order;
                                cell.Controls.Add(caldt);
                                cell.Controls.Add(maskededitdt);
                                cell.Controls.Add(maskededittimedt);
                                val1 = new RequiredFieldValidator();
                                val1.ControlToValidate = txtdatedt.ID;
                                valtime1 = new RequiredFieldValidator();
                                valtime1.ControlToValidate = txttimedt.ID;
                                break;
                            case ConditionType.LastXDays:
                            case ConditionType.LastXMonths:
                            case ConditionType.LastXWeeks:
                            case ConditionType.LastXYears:
                            case ConditionType.NextXDays:
                            case ConditionType.NextXMonths:
                            case ConditionType.NextXWeeks:
                            case ConditionType.NextXYears:
                                WebNumericEdit txtdtnum = new WebNumericEdit();
                                txtdtnum.ID = "txt" + criteria.order;
                                txtdtnum.DataMode = NumericDataMode.Int;
                                if (criteria.value1 != null)
                                {
                                    if (criteria.value1[0] != null)
                                    {
                                        txtdtnum.Text = criteria.value1[0].ToString();
                                    }
                                }
                                cell.Controls.Add(txtdtnum);
                                val1 = new RequiredFieldValidator();
                                val1.ControlToValidate = txtdtnum.ID;
                                break;

                        }
                        break;
                    case "T":
                        TextBox txttime;
                        MaskedEditExtender maskededittime;

                        switch (criteria.condition)
                        {
                            case ConditionType.Between:
                                txttime = new TextBox();
                                txttime.ID = "txttime" + criteria.order;
                                if (criteria.value1 != null)
                                {
                                    if (criteria.value1[0] != null)
                                    {
                                        txttime.Text = criteria.value1[1].ToString();
                                    }
                                }
                                cell.Controls.Add(txttime);
                                maskededittime = new MaskedEditExtender();
                                maskededittime.TargetControlID = txttime.ID;
                                maskededittime.Mask = "99:99";
                                maskededittime.MaskType = MaskedEditType.Time;
                                maskededittime.CultureName = "en-GB";
                                maskededittime.ID = "msktime" + criteria.order;
                                cell.Controls.Add(maskededittime);
                                valtime1 = new RequiredFieldValidator();
                                valtime1.ControlToValidate = txttime.ID;

                                txttime = new TextBox();
                                txttime.ID = "txttimeend" + criteria.order;
                                if (criteria.value2 != null)
                                {
                                    if (criteria.value2[0] != null)
                                    {
                                        txttime.Text = criteria.value2[1].ToString();
                                    }
                                }
                                cell.Controls.Add(txttime);
                                maskededittime = new MaskedEditExtender();
                                maskededittime.TargetControlID = txttime.ID;
                                maskededittime.Mask = "99:99";
                                maskededittime.MaskType = MaskedEditType.Time;
                                maskededittime.CultureName = "en-GB";
                                maskededittime.ID = "msktime2" + criteria.order;
                                cell.Controls.Add(maskededittime);
                                valtime2 = new RequiredFieldValidator();
                                valtime2.ControlToValidate = txttime.ID;
                                break;
                            case ConditionType.DoesNotEqual:
                            case ConditionType.Equals:
                            case ConditionType.On:
                            case ConditionType.After:
                            case ConditionType.OnOrAfter:
                            case ConditionType.Before:
                            case ConditionType.OnOrBefore:
                                txttime = new TextBox();
                                txttime.ID = "txttime" + criteria.order;
                                if (criteria.value1 != null)
                                {
                                    if (criteria.value1[0] != null)
                                    {
                                        txttime.Text = criteria.value1[1].ToString();
                                    }
                                }
                                cell.Controls.Add(txttime);
                                maskededittime = new MaskedEditExtender();
                                maskededittime.TargetControlID = txttime.ID;
                                maskededittime.Mask = "99:99";
                                maskededittime.MaskType = MaskedEditType.Time;
                                maskededittime.CultureName = "en-GB";
                                maskededittime.ID = "msktime" + criteria.order;
                                cell.Controls.Add(maskededittime);
                                valtime1 = new RequiredFieldValidator();
                                valtime1.ControlToValidate = txttime.ID;
                                break;
                        }
                        break;
                    case "N":
                        bool isNtoOneValueList = (criteria.field.FieldSource != cField.FieldSourceType.Metabase && criteria.field.GetRelatedTable() != null && criteria.field.IsForeignKey);
                        if (criteria.field.ValueList || isNtoOneValueList)
                        {
                            Dictionary<object, object> values = null;
                            if (isNtoOneValueList)
                            {
                                values = cReports.getRelationshipValues(cMisc.GetCurrentUser(), criteria.field.FieldID);
                            }
                            txtbox = new WebTextEdit();
                            txtbox.ID = "txtbox" + criteria.order;
                            if (criteria.value1 != null)
                            {
                                if (criteria.value1[0] != null)
                                {
                                    object[] listitems = (object[])criteria.value1[0].ToString().Split(',');

                                    for (int x = 0; x < listitems.GetLength(0); x++)
                                    {
                                        if (!isNtoOneValueList)
                                        {
                                            foreach (KeyValuePair<object, string> kvp in field.ListItems)
                                            {
                                                if (kvp.Key.ToString() == listitems[x].ToString())
                                                {
                                                    txtbox.Text += kvp.Value + ", ";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (KeyValuePair<object, object> kvp in values)
                                            {
                                                if (kvp.Key.ToString() == listitems[x].ToString())
                                                {
                                                    txtbox.Text += kvp.Value + ", ";
                                                }
                                            }
                                        }
                                    }
                                    if (txtbox.Text.Length != 0)
                                    {
                                        txtbox.Text = txtbox.Text.Remove(txtbox.Text.Length - 2, 2);
                                    }
                                }
                            }
                            cell.Controls.Add(txtbox);

                            val1 = new RequiredFieldValidator();
                            val1.ControlToValidate = "txtbox" + criteria.order;
                            txtbox.ReadOnly = true;

                            txtbox = new WebTextEdit();
                            txtbox.ID = "txtbox" + criteria.order + "_values";
                            txtbox.Style["display"] = "none";
                            if (criteria.value1 != null)
                            {
                                if (criteria.value1[0] != null)
                                {
                                    txtbox.Value = criteria.value1[0].ToString();
                                }
                            }
                            cell.Controls.Add(txtbox);
                            Literal lit = new Literal();
                            lit.Text = "&nbsp;<input type=\"button\" onclick=\"selectListOption('" + criteria.field.FieldID + "'," + criteria.order + ");\" value=\". . .\">";
                            cell.Controls.Add(lit);
                        }
                        else
                        {
                            var txtnum = new WebNumericEdit { ID = string.Format("txt{0}", criteria.order) };
                            if (criteria.value1 != null)
                            {
                                if (criteria.value1[0] != null)
                                {
                                    txtnum.Text = criteria.value1[0].ToString();
                                }
                            }

                            txtnum.DataMode = NumericDataMode.Int;
                            cell.Controls.Add(txtnum);
                            val1 = new RequiredFieldValidator { ControlToValidate = string.Format("txt{0}", criteria.order) };

                            if (criteria.condition == ConditionType.Between)
                            {
                                txtnum = new WebNumericEdit { ID = string.Format("txt{0}_2", criteria.order) };
                                if (criteria.value2 != null)
                                {
                                    if (criteria.value2[0] != null)
                                    {
                                        txtnum.Text = criteria.value2[0].ToString();
                                    }
                                }

                                txtnum.DataMode = NumericDataMode.Int;
                                cell.Controls.Add(txtnum);
                                val2 = new RequiredFieldValidator { ControlToValidate = string.Format("txt{0}_2", criteria.order) };
                            }
                        }

                        break;
                    case "M":
                    case "FD":
                        var txtmon = new WebNumericEdit
                                         {
                                             ID = "txt" + criteria.order,
                                             DataMode = NumericDataMode.Double
                                         };
                        if (criteria.value1 != null)
                        {
                            if (criteria.value1[0] != null)
                            {
                                txtmon.Text = criteria.value1[0].ToString();
                            }
                        }

                        cell.Controls.Add(txtmon);
                        val1 = new RequiredFieldValidator { ControlToValidate = "txt" + criteria.order };

                        if (criteria.condition == ConditionType.Between)
                        {
                            txtmon = new WebNumericEdit
                            {
                                ID = "txt" + criteria.order + "_2",
                                DataMode = NumericDataMode.Double
                            };
                            if (criteria.value2 != null)
                            {
                                if (criteria.value2[0] != null)
                                {
                                    txtmon.Text = criteria.value2[0].ToString();
                                }
                            }

                            txtmon.DataMode = NumericDataMode.Double;
                            cell.Controls.Add(txtmon);
                            val2 = new RequiredFieldValidator { ControlToValidate = string.Format("txt{0}_2", criteria.order) };
                        }

                        break;
                    case "C":
                        var txtcur = new WebCurrencyEdit { ID = string.Format("txt{0}", criteria.order) };
                        if (criteria.value1 != null)
                        {
                            if (criteria.value1[0] != null)
                            {
                                txtcur.Text = criteria.value1[0].ToString();
                            }
                        }
                        txtcur.DataMode = NumericDataMode.Decimal;
                        cell.Controls.Add(txtcur);
                        val1 = new RequiredFieldValidator
                                   {
                                       ControlToValidate = string.Format("txt{0}", criteria.order)
                                   };

                        if (criteria.condition == ConditionType.Between)
                        {
                            txtcur = new WebCurrencyEdit { ID = string.Format("txt{0}_2", criteria.order) };
                            if (criteria.value2 != null)
                            {
                                if (criteria.value2[0] != null)
                                {
                                    txtcur.Text = criteria.value2[0].ToString();
                                }
                            }

                            txtcur.DataMode = NumericDataMode.Decimal;
                            cell.Controls.Add(txtcur);
                            val2 = new RequiredFieldValidator { ControlToValidate = string.Format("txt{0}_2", criteria.order) };
                        }

                        break;
                    case "X":
                        DropDownList cmb = new DropDownList();
                        cmb.Items.Add(new ListItem("Yes", "1"));
                        cmb.Items.Add(new ListItem("No", "0"));
                        cmb.ID = "cmb" + criteria.order;
                        if (criteria.value1 != null)
                        {
                            if (criteria.value1[0] != null)
                            {
                                if (cmb.Items.FindByValue(criteria.value1[0].ToString()) != null)
                                {
                                    cmb.Items.FindByValue(criteria.value1[0].ToString()).Selected = true;
                                }
                            }
                        }
                        cell.Controls.Add(cmb);
                        val1 = new RequiredFieldValidator();
                        val1.ControlToValidate = "cmb" + criteria.order;
                        break;
                    default:
                        System.Web.UI.LiteralControl blank = new System.Web.UI.LiteralControl();
                        blank.Text = "&nbsp;";
                        cell.Controls.Add(blank);
                        break;
                }
                row.Cells.Add(cell);
                #endregion

                #region validator
                cell = new TableCell();

                if (val1 != null)
                {
                    val1.ErrorMessage = "Please enter a value for " + criteria.field.Description;
                    val1.Text = "*";
                    cell.Controls.Add(val1);
                }

                if (val2 != null)
                {

                    val2.ErrorMessage = "Please enter a value for " + criteria.field.Description;
                    val2.Text = "*";
                    cell.Controls.Add(val2);
                }

                if (valtime1 != null)
                {
                    valtime1.ErrorMessage = "Please enter a time for " + criteria.field.Description;
                    valtime1.Text = "*";
                    cell.Controls.Add(valtime1);
                }

                if (valtime2 != null)
                {

                    valtime2.ErrorMessage = "Please enter a time for " + criteria.field.Description;
                    valtime2.Text = "*";
                    cell.Controls.Add(valtime2);
                }

                cell.CssClass = rowclass;
                row.Cells.Add(cell);
                #endregion
                tbl.Rows.Add(row);

                if (rowclass == "row1")
                {
                    rowclass = "row2";
                }
                else
                {
                    rowclass = "row1";
                }
            }
        }

        return tbl;
    }

    public void getRuntimeValues(ref cReport rpt)
    {
        cReportColumn column;
        cStaticColumn staticcol;
        cReportCriterion criteria;
        object[] value1 = null;
        object[] value2 = null;

        TextBox txtstatic;
        for (int i = 0; i < rpt.columns.Count; i++)
        {
            column = (cReportColumn)rpt.columns[i];
            if (column.columntype == ReportColumnType.Static)
            {
                staticcol = (cStaticColumn)column;
                if (staticcol.runtime)
                {
                    txtstatic = (TextBox)FindControl("statictxt" + staticcol.order);
                    staticcol.setValue(txtstatic.Text);
                }
            }
        }
        WebTextEdit txtbox;
        TextBox txtdate;
        TextBox txttime;
        for (int i = 0; i < rpt.criteria.Count; i++)
        {
            criteria = (cReportCriterion)rpt.criteria[i];
            if (criteria.runtime)
            {
                switch (criteria.field.FieldType)
                {
                    case "S":
                    case "FS":
                    case "LT":
                    case "R":
                        value1 = new object[1];

                        if (criteria.field.GenList)
                        {
                            txtbox = (WebTextEdit)FindControl("txtbox" + criteria.order + "_values");
                            txtbox.ReadOnly = true;
                        }
                        else
                        {
                            txtbox = (WebTextEdit)FindControl("txtbox" + criteria.order);

                        }
                        value1[0] = txtbox.Text;
                        break;
                    case "N":
                    case "M":
                    case "FD":
                    case "C":
                        if (criteria.field.ValueList || (criteria.field.FieldSource != cField.FieldSourceType.Metabase && criteria.field.FieldType == "N" && criteria.field.GetRelatedTable() != null && criteria.field.IsForeignKey))
                        {
                            txtbox = (WebTextEdit)FindControl("txtbox" + criteria.order + "_values");
                            value1 = new object[1];
                            value1[0] = txtbox.Text;
                        }
                        else
                        {
                            var txtnum = (WebNumericEdit)FindControl(string.Format("txt{0}", criteria.order));
                            value1 = new object[1];
                            value1[0] = txtnum.Value;

                            if (criteria.condition == ConditionType.Between)
                            {
                                txtnum = (WebNumericEdit)FindControl(string.Format("txt{0}_2", criteria.order));
                                value2 = new object[1];
                                value2[0] = txtnum.Value; 
                            }
                        }
                        break;
                    case "X":
                        DropDownList cmb = (DropDownList)FindControl("cmb" + criteria.order);
                        value1 = new object[1];
                        value1[0] = byte.Parse(cmb.SelectedValue);
                        break;
                    case "D":
                        switch (criteria.condition)
                        {
                            case ConditionType.Between:
                                txtdate = (TextBox)FindControl("txtdate" + criteria.order);
                                value1 = new object[1];

                                value1[0] = DateTime.ParseExact(txtdate.Text, "dd/MM/yyyy", null);
                                txtdate = (TextBox)FindControl("txtdate2" + criteria.order);
                                value2 = new object[1];

                                value2[0] = DateTime.ParseExact(txtdate.Text, "dd/MM/yyyy", null);
                                break;
                            case ConditionType.NotOn:
                            case ConditionType.On:
                            case ConditionType.After:
                            case ConditionType.OnOrAfter:
                            case ConditionType.Before:
                            case ConditionType.OnOrBefore:
                                txtdate = (TextBox)FindControl("txtdate" + criteria.order);
                                value1 = new object[1];

                                value1[0] = DateTime.ParseExact(txtdate.Text, "dd/MM/yyyy", null);
                                break;
                            case ConditionType.LastXDays:
                            case ConditionType.LastXMonths:
                            case ConditionType.LastXWeeks:
                            case ConditionType.LastXYears:
                            case ConditionType.NextXDays:
                            case ConditionType.NextXMonths:
                            case ConditionType.NextXWeeks:
                            case ConditionType.NextXYears:
                                WebNumericEdit txtdtnum = (WebNumericEdit)FindControl("txt" + criteria.order);
                                value1 = new object[1];
                                value1[0] = txtdtnum.Value;
                                break;
                        }
                        break;
                    case "DT":
                        switch (criteria.condition)
                        {
                            case ConditionType.Between:
                                txtdate = (TextBox)FindControl("txtdate" + criteria.order);
                                txttime = (TextBox)FindControl("txttime" + criteria.order);
                                value1 = new object[1];
                                value1[0] = DateTime.ParseExact(txtdate.Text + " " + txttime.Text + ":00", "dd/MM/yyyy hh:mm:ss", null);

                                txtdate = (TextBox)FindControl("txtdate2" + criteria.order);
                                txttime = (TextBox)FindControl("txttimeend" + criteria.order);
                                value2 = new object[1];
                                value2[0] = DateTime.ParseExact(txtdate.Text + " " + txttime.Text + ":00", "dd/MM/yyyy hh:mm:ss", null);

                                break;
                            case ConditionType.DoesNotEqual:
                            case ConditionType.Equals:
                            case ConditionType.After:
                            case ConditionType.OnOrAfter:
                            case ConditionType.Before:
                            case ConditionType.OnOrBefore:
                                txtdate = (TextBox)FindControl("txtdate" + criteria.order);
                                txttime = (TextBox)FindControl("txttime" + criteria.order);
                                value1 = new object[1];
                                value1[0] = DateTime.ParseExact(txtdate.Text + " " + txttime.Text + ":00", "dd/MM/yyyy hh:mm:ss", null);
                                break;
                            case ConditionType.LastXDays:
                            case ConditionType.LastXMonths:
                            case ConditionType.LastXWeeks:
                            case ConditionType.LastXYears:
                            case ConditionType.NextXDays:
                            case ConditionType.NextXMonths:
                            case ConditionType.NextXWeeks:
                            case ConditionType.NextXYears:
                                WebNumericEdit txtdtnum = (WebNumericEdit)FindControl("txt" + criteria.order);
                                value1 = new object[1];
                                value1[0] = txtdtnum.Value;
                                break;
                        }
                        break;
                    case "T":
                        switch (criteria.condition)
                        {
                            case ConditionType.Between:
                                txttime = (TextBox)FindControl("txttime" + criteria.order);
                                value1 = new object[1];
                                value1[0] = txttime.Text;
                                txttime = (TextBox)FindControl("txttimeend" + criteria.order);
                                value2 = new object[1];
                                value2[0] = txttime.Text;
                                break;
                            case ConditionType.DoesNotEqual:
                            case ConditionType.Equals:
                            case ConditionType.After:
                            case ConditionType.OnOrAfter:
                            case ConditionType.Before:
                            case ConditionType.OnOrBefore:
                                txttime = (TextBox)FindControl("txttime" + criteria.order);
                                value1 = new object[1];
                                value1[0] = txttime.Text;
                                break;
                        }
                        break;
                }

                criteria.updateValues(value1, value2);
            }
        }

        rpt.runtimecriteriaset = true;
    }
}
