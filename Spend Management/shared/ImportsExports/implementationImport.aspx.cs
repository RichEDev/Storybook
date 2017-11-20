using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using Syncfusion.XlsIO;
using System.IO;
using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.ProjectCodes;

namespace Spend_Management
{
    public partial class implementationImport : System.Web.UI.Page
    {
        [Dependency]
        public IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> ProjectCodesRepository { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Implementation Excel Spreadsheet Import";
            Master.title = Title;

            cImportInfo importInfo;
            int rowNum = 1;

            
            //cmdok.Attributes.Add("onclick", "javascript:getImportProgress();");

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cAccounts clsAccounts = new cAccounts();
                cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);

                ddlViewType.Items.Add(new ListItem(LogViewType.All.ToString(), ((int)LogViewType.All).ToString()));
                ddlViewType.Items.Add(new ListItem(LogViewType.Successes.ToString(), ((int)LogViewType.Successes).ToString()));
                ddlViewType.Items.Add(new ListItem(LogViewType.Added.ToString(), ((int)LogViewType.Added).ToString()));
                ddlViewType.Items.Add(new ListItem(LogViewType.Updated.ToString(), ((int)LogViewType.Updated).ToString()));
                ddlViewType.Items.Add(new ListItem(LogViewType.Failures.ToString(), ((int)LogViewType.Failures).ToString()));
                ddlViewType.Items.Add(new ListItem(LogViewType.Warnings.ToString(), ((int)LogViewType.Warnings).ToString()));
                ddlViewType.Items.Add(new ListItem(LogViewType.Other.ToString(), ((int)LogViewType.Other).ToString()));
                ddlViewType.Items.Add(new ListItem(LogViewType.Summary.ToString(), ((int)LogViewType.Summary).ToString()));

                ddlElementType.Items.Add(new ListItem("All", "0"));
            }
            else
            {
                byte[] file = txtfilename.FileBytes;//new byte[txtfilename.PostedFile.ContentLength];

                //txtfilename.PostedFile.InputStream.Read(file, 0, txtfilename.PostedFile.ContentLength);

                cLogging clsLogging = new cLogging((int)ViewState["accountid"]);
                int logID = clsLogging.saveLog(0, LogType.SpreadsheetImport, 0, 0, 0, 0, 0);
                ViewState["logID"] = logID;

                cCustomerDataImport clsDataImport = new cCustomerDataImport((int)ViewState["accountid"], (int)ViewState["employeeid"], logID);

                ImportProcessData procData = clsDataImport.ReadExcelTemplate(file);

                ViewState["ProcessData"] = procData;

                //if (Session["SpreadsheetImport" + (int)ViewState["accountid"]] != null)
                //{
                if (HttpRuntime.Cache["SpreadsheetImport" + (int)ViewState["accountid"]] != null)
                {
                    //importInfo = (cImportInfo)Session["SpreadsheetImport" + (int)ViewState["accountid"]];
                importInfo = (cImportInfo)HttpRuntime.Cache["SpreadsheetImport" + (int)ViewState["accountid"]];

                    if (importInfo.worksheets != null)
                    {
                        if (importInfo.worksheets.Count > 0)
                        {
                            TableRow row;
                            TableCell cell;
                            TableHeaderCell header;

                            
                            row = new TableRow();
                            header = new TableHeaderCell();
                            header.CssClass = "th";
                            header.Text = "Worksheet";
                            row.Cells.Add(header);

                            header = new TableHeaderCell();
                            header.CssClass = "th";
                            header.Text = "Status";
                            row.Cells.Add(header);

                            tblProgress.Rows.Add(row);

                            foreach (KeyValuePair<int, ImportStatusValues> kp in importInfo.worksheets)
                            {
                                row = new TableRow();

                                cell = new TableCell();

                                cell.ID = "sheetName" + rowNum;
                                cell.Text = kp.Value.sheetName;
                                row.Cells.Add(cell);

                                cell = new TableCell();

                                cell.ID = "sheetStatus" + rowNum;
                                cell.Text = kp.Value.status.ToString();
                                row.Cells.Add(cell);
                                rowNum++;

                                tblProgress.Rows.Add(row);
                            }
                        }
                    }

                    //Get the ID of the log and store it client side so the log can be shown when the import is done
                    ClientScript.RegisterClientScriptBlock(typeof(System.String), "SpreadsheetImport", "var logid = " + logID, true);

                    ClientScript.RegisterStartupScript(typeof(System.String), "SpreadsheetImport", "getImportProgress();", true);
                }
            }
        }

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            cCustomerDataImport clsDataImport = new cCustomerDataImport((int)ViewState["accountid"], (int)ViewState["employeeid"], (int)ViewState["logID"]);

            ImportProcessData procData = new ImportProcessData();

            ExcelEngine exceleng = new ExcelEngine();
            byte[] file = txtfilename.FileBytes;//new byte[txtfilename.PostedFile.ContentLength];
            //txtfilename.PostedFile.InputStream.Read(file, 0, txtfilename.PostedFile.ContentLength);
            MemoryStream stream = new MemoryStream(file);
            IApplication app = exceleng.Excel;

            IWorkbook workbook = app.Workbooks.Open(stream);


            procData = (ImportProcessData)ViewState["ProcessData"];
            procData.workbook = workbook;

            clsDataImport.startImport(cMisc.GetCurrentUser(), procData);

            //ClientScript.RegisterStartupScript(typeof(System.String), "SpreadsheetImport", "javascript:getImportProgress();");

        }
    }
}
