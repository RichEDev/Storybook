using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using SpendManagementLibrary;

namespace Spend_Management
{
    using SpendManagementLibrary.Logic_Classes.Fields;

    public partial class printerfriendly : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                Title = "Printer Friendly";
                int requestnum = int.Parse(Request.QueryString["requestnum"]);
                cReportRequest printrequest = (cReportRequest)Session["request" + requestnum];

                cText clstext = new cText();

                calcman.RegisterUserDefinedFunction(clstext);

                lbltitle.Text = printrequest.report.reportname;
                litreport.Text = generatePrintout(printrequest);
            }
        }

        private string generatePrintout(cReportRequest request)
        {
            var output = new StringBuilder();
            cReportColumn column;
            cStandardColumn standard;
            cStaticColumn staticcol;
            cCalculatedColumn calculatedcol;
            decimal num;
            DateTime date;
            DataSet rcdstreport = null;
            ICurrentUser user = cMisc.GetCurrentUser();
            rcdstreport = (DataSet)Session[request.requestnum.ToString() + "_" + user.EmployeeID.ToString() + "_" + user.AccountID.ToString()];  //(DataSet)clsreports.getReportData(request);
            var lstColumns = new SortedList<int, object>();

            output.Append("<table>");
            output.Append("<tr>");
            for (int x = 0; x < request.report.columns.Count; x++)
            {
                column = (cReportColumn)request.report.columns[x];
                lstColumns.Add(x, 0);
                if (!column.hidden)
                {
                    switch (column.columntype)
                    {
                        case ReportColumnType.Standard:
                            standard = (cStandardColumn)column;
                            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(request.accountid);
                            cAccountProperties accountProperties = clsSubAccounts.getSubAccountById(request.SubAccountId).SubAccountProperties;
                            var relabler = new FieldRelabler(accountProperties);
                            string fieldDescription = relabler.Relabel(standard.field).Description;

                            if (standard.funcavg || standard.funccount || standard.funcmax || standard.funcmin || standard.funcsum)
                            {
                                if (standard.funcsum)
                                {
                                    output.Append("<th>");
                                    output.Append("SUM of " + fieldDescription);
                                    output.Append("</th>");
                                }

                                if (standard.funcavg)
                                {
                                    output.Append("<th>");
                                    output.Append("AVG of " + fieldDescription);
                                    output.Append("</th>");
                                }

                                if (standard.funccount)
                                {
                                    output.Append("<th>");
                                    output.Append("COUNT of " + fieldDescription);
                                    output.Append("</th>");
                                }

                                if (standard.funcmax)
                                {
                                    output.Append("<th>");
                                    output.Append("MAX of " + fieldDescription);
                                    output.Append("</th>");
                                }

                                if (standard.funcmin)
                                {
                                    output.Append("<th>");
                                    output.Append("MIN of " + fieldDescription);
                                    output.Append("</th>");
                                }
                            }
                            else
                            {
                                output.Append("<th>");
                                output.Append(fieldDescription);
                                output.Append("</th>");
                            }


                            break;
                        case ReportColumnType.Static:
                            staticcol = (cStaticColumn)column;
                            output.Append("<th>");
                            output.Append(staticcol.literalname);
                            output.Append("</th>");
                            break;
                        case ReportColumnType.Calculated:
                            calculatedcol = (cCalculatedColumn)column;
                            output.Append("<th>");
                            output.Append(calculatedcol.columnname);
                            output.Append("</th>");
                            break;
                    }

                }
            }
            output.Append("</tr>");

            var clsGlobalCurrencies = new cGlobalCurrencies();
            var clsCurrencies = new cCurrencies(request.accountid, request.SubAccountId);

            for (int i = 0; i < rcdstreport.Tables[0].Rows.Count; i++)
            {
                output.Append("<tr>");
                for (int x = 0; x < request.report.columns.Count; x++)
                {

                    column = (cReportColumn)request.report.columns[x];

                    if (!column.hidden)
                    {
                        switch (column.columntype)
                        {
                            case ReportColumnType.Standard:
                                standard = (cStandardColumn)column;
                                if (standard.field.ValueList == true)
                                {
                                    output.Append("<td>");
                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                    {
                                        output.Append(rcdstreport.Tables[0].Rows[i][x]);
                                    }
                                    output.Append("</td>");
                                }
                                else
                                {
                                    if (standard.funcavg || standard.funccount || standard.funcmax || standard.funcmin || standard.funcsum)
                                    {
                                        if (standard.funcsum)
                                        {
                                            output.Append("<td>");
                                            switch (standard.field.FieldType)
                                            {
                                                case "M":
                                                case "FD":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        num = Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                        output.Append(num.ToString("########0.00"));
                                                        lstColumns[x] = Convert.ToDecimal(lstColumns[x]) + Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                    }
                                                    break;
                                                case "C":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        num = Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                        output.Append(num.ToString("#######0.00"));
                                                        lstColumns[x] = Convert.ToDecimal(lstColumns[x]) + Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                    }
                                                    break;
                                                case "N":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        output.Append(rcdstreport.Tables[0].Rows[i][x].ToString());
                                                        lstColumns[x] = (int)lstColumns[x] + (int)rcdstreport.Tables[0].Rows[i][x];
                                                    }
                                                    break;
                                            }

                                            output.Append("</td>");
                                        }
                                        if (standard.funcavg)
                                        {
                                            output.Append("<td>");
                                            switch (standard.field.FieldType)
                                            {
                                                case "M":
                                                case "FD":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        num = Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                        output.Append(num.ToString("########0.00"));
                                                        lstColumns[x] = Convert.ToDecimal(lstColumns[x]) + Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                    }

                                                    break;
                                                case "C":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        num = Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                        output.Append(num.ToString("########0.00"));
                                                        lstColumns[x] = Convert.ToDecimal(lstColumns[x]) + Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                    }

                                                    break;
                                                case "N":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        output.Append(rcdstreport.Tables[0].Rows[i][x].ToString());
                                                        lstColumns[x] = (int)lstColumns[x] + (int)rcdstreport.Tables[0].Rows[i][x];
                                                    }

                                                    break;
                                            }

                                            output.Append("</td>");
                                        }

                                        if (standard.funccount)
                                        {
                                            output.Append("<td>");
                                            output.Append(rcdstreport.Tables[0].Rows[i][x].ToString());
                                            output.Append("</td>");
                                        }

                                        if (standard.funcmax)
                                        {
                                            output.Append("<td>");
                                            switch (standard.field.FieldType)
                                            {
                                                case "M":
                                                case "FD":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        num = Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                        output.Append(num.ToString("########0.00"));
                                                        lstColumns[x] = Convert.ToDecimal(lstColumns[x]) + Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                    }
                                                    break;
                                                case "C":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        num = Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                        output.Append(num.ToString("########0.00"));
                                                        lstColumns[x] = Convert.ToDecimal(lstColumns[x]) + Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                    }
                                                    break;
                                                case "N":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        output.Append(rcdstreport.Tables[0].Rows[i][x].ToString());
                                                        lstColumns[x] = (int)lstColumns[x] + (int)rcdstreport.Tables[0].Rows[i][x];
                                                    }
                                                    break;
                                                case "D":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)rcdstreport.Tables[0].Rows[i][x];
                                                        output.Append(date.ToShortDateString());

                                                    }
                                                    break;
                                                case "DT":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)rcdstreport.Tables[0].Rows[i][x];
                                                        output.Append(date);
                                                    }
                                                    break;
                                                case "T":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)rcdstreport.Tables[0].Rows[i][x];
                                                        output.Append(date.Hour + ":" + date.Minute);
                                                    }
                                                    break;
                                            }

                                            output.Append("</td>");
                                        }

                                        if (standard.funcmin)
                                        {
                                            output.Append("<td>");
                                            switch (standard.field.FieldType)
                                            {
                                                case "M":
                                                case "FD":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        num = Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                        output.Append(num.ToString("########0.00"));
                                                        lstColumns[x] = Convert.ToDecimal(lstColumns[x]) + Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                    }
                                                    break;
                                                case "C":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        num = Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                        output.Append(num.ToString("########0.00"));
                                                        lstColumns[x] = Convert.ToDecimal(lstColumns[x]) + Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                    }
                                                    break;
                                                case "N":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        output.Append(rcdstreport.Tables[0].Rows[i][x].ToString());
                                                        lstColumns[x] = (int)lstColumns[x] + (int)rcdstreport.Tables[0].Rows[i][x];
                                                    }
                                                    break;
                                                case "D":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)rcdstreport.Tables[0].Rows[i][x];
                                                        output.Append(date.ToShortDateString());
                                                    }
                                                    break;
                                                case "DT":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)rcdstreport.Tables[0].Rows[i][x];
                                                        output.Append(date);
                                                    }
                                                    break;
                                                case "T":
                                                    if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                    {
                                                        date = (DateTime)rcdstreport.Tables[0].Rows[i][x];
                                                        output.Append(date.Hour + ":" + date.Minute);
                                                    }
                                                    break;
                                            }

                                            output.Append("</td>");
                                        }
                                    }
                                    else
                                    {
                                        output.Append("<td>");
                                        switch (standard.field.FieldType)
                                        {
                                            case "S":
                                            case "FS":
                                            case "LT":
                                            case "R":
                                                if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                {
                                                    output.Append((string)rcdstreport.Tables[0].Rows[i][x]);
                                                }

                                                break;
                                            case "D":
                                                if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                {
                                                    date = (DateTime)rcdstreport.Tables[0].Rows[i][x];
                                                    output.Append(date.ToShortDateString());
                                                }

                                                break;
                                            case "DT":
                                                if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                {
                                                    date = (DateTime)rcdstreport.Tables[0].Rows[i][x];
                                                    output.Append(date);
                                                }

                                                break;
                                            case "T":
                                                if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                {
                                                    date = (DateTime)rcdstreport.Tables[0].Rows[i][x];
                                                    output.Append(date.Hour.ToString("00") + ":" + date.Minute.ToString("00"));
                                                }

                                                break;
                                            case "X":
                                                if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                {
                                                    output.Append(rcdstreport.Tables[0].Rows[i][x].ToString());
                                                }

                                                break;
                                            case "M":
                                            case "FD":
                                                if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                {
                                                    num = Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                    output.Append(num.ToString("########0.00"));
                                                    lstColumns[x] = Convert.ToDecimal(lstColumns[x]) + Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                }

                                                break;
                                            case "C":
                                                if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                {
                                                    num = Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);
                                                    output.Append(num.ToString("########0.00"));
                                                    lstColumns[x] = Convert.ToDecimal(lstColumns[x]) + Convert.ToDecimal(rcdstreport.Tables[0].Rows[i][x]);

                                                }

                                                break;
                                            case "N":
                                                if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                {
                                                    if (standard.field.FieldSource != cField.FieldSourceType.Metabase && (standard.field.IsForeignKey && standard.field.RelatedTableID != Guid.Empty))
                                                    {
                                                        output.Append(cReports.getRelationshipValueText(user, standard.field.FieldID, rcdstreport.Tables[0].Rows[i][x]));
                                                    }
                                                    else
                                                    {
                                                        output.Append(rcdstreport.Tables[0].Rows[i][x].ToString());
                                                        lstColumns[x] = (int)lstColumns[x] + (int)rcdstreport.Tables[0].Rows[i][x];
                                                    }
                                                }

                                                break;
                                            case "CL":
                                                if (rcdstreport.Tables[0].Rows[i][x] != DBNull.Value)
                                                {
                                                    cCurrency currency = clsCurrencies.getCurrencyById((int)rcdstreport.Tables[0].Rows[i][x]);

                                                    output.Append(currency == null ? "" : clsGlobalCurrencies.getGlobalCurrencyById(currency.globalcurrencyid).label);;
                                                }

                                                break;
                                        }

                                        output.Append("</td>");
                                    }
                                }
                                break;
                            case ReportColumnType.Static:
                                output.Append("<td>");
                                output.Append(rcdstreport.Tables[0].Rows[i][x].ToString());
                                output.Append("</td>");
                                break;
                            case ReportColumnType.Calculated:
                                calculatedcol = (cCalculatedColumn)column;
                                output.Append("<td>");
                                output.Append(rcdstreport.Tables[0].Rows[i][x].ToString());
                                output.Append("</td>");
                                break;
                        }
                    }
                }

                output.Append("</tr>");
            }

            output.Append("<tr>");
            for (int x = 0; x < request.report.columns.Count; x++)
            {
                column = (cReportColumn)request.report.columns[x];
 
                if (!column.hidden)
                {
                    switch (column.columntype)
                    {
                        case ReportColumnType.Standard:
                            standard = (cStandardColumn)column;
                            if (standard.funccount || standard.funcsum)
                            {
                                if (standard.funcsum)
                                {
                                    if (standard.field.FieldType == "M" || standard.field.FieldType == "C" || standard.field.FieldType == "FD")
                                    {
                                        num = Convert.ToDecimal(lstColumns[x]);
                                        output.Append("<td>");
                                        output.Append(num.ToString("###,###,##0.00"));
                                        output.Append("</td>");
                                    }
                                    else if (standard.field.FieldType == "N") 
                                    {
                                        num = (int)lstColumns[x];
                                        output.Append("<td>");
                                        output.Append(num.ToString("###,###,##0.00"));
                                        output.Append("</td>");
                                    }

                                }

                                if (standard.funccount)
                                {
                                    if (standard.field.FieldType == "M" || standard.field.FieldType == "C" || standard.field.FieldType == "FD")
                                    {
                                        num = Convert.ToDecimal(lstColumns[x]);
                                        output.Append("<td>");
                                        output.Append(num.ToString("###,###,##0.00"));
                                        output.Append("</td>");
                                    }
                                    else if (standard.field.FieldType == "N")
                                    {
                                        num = (int)lstColumns[x];
                                        output.Append("<td>");
                                        output.Append(num.ToString("###,###,##0.00")); 
                                        output.Append("</td>");
                                    }
                                }
                            }
                            else
                            {
                                output.Append("<td>");
                                output.Append("&nbsp;");
                                output.Append("</td>");
                            }

                            break;
                        case ReportColumnType.Static:
                            output.Append("<td>");
                            output.Append("&nbsp;");
                            output.Append("</td>");
                            break;
                        case ReportColumnType.Calculated:
                            output.Append("<td>");
                            output.Append("&nbsp;");
                            output.Append("</td>");
                            break;
                    }
                }
            }

            output.Append("</tr>");
            output.Append("</table>");

            return output.ToString();
        }
    }
}
