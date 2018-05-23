namespace Spend_Management
{
    using System;
    using System.Collections;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using BusinessLogic;
    using BusinessLogic.Modules;

    using SpendManagementLibrary;

    using SQLDataAccess.ImportExport;

    public partial class importdatawizard : System.Web.UI.Page
    {
        /// <summary>
        /// Gets or sets the <see cref="ImportFileFactory"/> instance.
        /// </summary>
        [Dependency]
        public ImportFileFactory ImportFileFactory { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Import Data Wizard";
            Master.title = Title;
            Master.enablenavigation = false;

            if (IsPostBack == false)
            {
                wizimport.ActiveStepIndex = 0;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportDataWizard, true, true);

                var usingExpenses = user.CurrentActiveModule == Modules.Expenses;

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cTables clstables = new cTables(user.AccountID);
                cmbdestination.Items.AddRange(clstables.CreateDropDown(cTables.DropDownType.AllowImport, !usingExpenses, !user.Account.IsNHSCustomer).ToArray());
                ViewState["stepsVisited"] = new ArrayList();
                littimeline.Text = createTimeline(user.AccountID);

                switch (user.CurrentActiveModule)
                {
                    case Modules.SmartDiligence:
                    case Modules.SpendManagement:
                    case Modules.Contracts:
                        wizimport.CancelDestinationPageUrl = cMisc.Path + "/MenuMain.aspx?menusection=importsexports";
                        break;
                    default:
                        wizimport.CancelDestinationPageUrl = cMisc.Path + "/exportmenu.aspx";
                        break;
                }

            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gridmatching.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridmatching_InitializeDataSource);
            griddefaults.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(griddefaults_InitializeDataSource);
        }

        void griddefaults_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            if (ViewState["import"] != null)
            {
                cImport import = (cImport)ViewState["import"];
                griddefaults.DataSource = import.getColumnsForDefaultGrid();
                griddefaults.DataBind();
            }
            
        }

        void gridmatching_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            if (ViewState["import"] != null)
            {
                cImport import = (cImport)ViewState["import"];
                gridmatching.DataSource = import.getMatchingGrid();
                gridmatching.DataBind();
            }
        }
        protected void wizimport_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            cImport import;
            byte[] file = null;
            int headercount = 0;
            ImportType filetype;
            List<string> status;
            List<string> validated;
            cXMLFileImport xmlimport;
            switch (e.CurrentStepIndex)
            {
                case 0:
                    filetype = (ImportType)int.Parse(cmbfiletype.SelectedValue);
                    switch (filetype)
                    {
                        case ImportType.FlatFile:
                        
                            viewfiledetails.ActiveViewIndex = 0;
                            file = new byte[txtfilename.PostedFile.ContentLength];

                            txtfilename.PostedFile.InputStream.Read(file, 0, txtfilename.PostedFile.ContentLength);

                            import = new cFlatFileImport((int)ViewState["accountid"], file,",",0,0,false);

                            ((cFlatFileImport)import).extractDataFromFile(true);
                            if (!import.isvalidfile)
                            {
                                lblmsg.Text = "The file you have selected to import is not a valid " + cmbfiletype.SelectedItem.Text + " file.";
                                lblmsg.Visible = true;
                                e.Cancel = true;
                                return;
                            }
                            ViewState["import"] = import;
                            gridsample.DataSource = import.getSample();
                            gridsample.DataBind();
                            break;
                        case ImportType.Excel:
                            viewfiledetails.ActiveViewIndex = 2;
                            file = new byte[txtfilename.PostedFile.ContentLength];
                            if (cmbfirstrowheader.SelectedValue == "Yes")
                            {
                                
                                headercount = 1;
                            }
                            else
                            {
                                headercount = 0;
                            }
                            
                            txtfilename.PostedFile.InputStream.Read(file, 0, txtfilename.PostedFile.ContentLength);
                            cExcelFileImport excelimport = new cExcelFileImport((int)ViewState["accountid"], file, headercount,0, 0);
                            excelimport.extractDataFromFile(true);
                            if (!excelimport.isvalidfile)
                            {
                                lblmsg.Text = "The file you have selected to import is not a valid " + cmbfiletype.SelectedItem.Text + " file.";
                                lblmsg.Visible = true;
                                e.Cancel = true;
                                return;
                            }
                            cmbworksheet.Items.Clear();
                            cmbworksheet.Items.AddRange(excelimport.CreateWorkSheetDropDown().ToArray());

                            ViewState["import"] = excelimport;
                            gridsample.DataSource = excelimport.getSample();
                            gridsample.DataBind();
                            break;
                        case ImportType.XML:
                            viewfiledetails.ActiveViewIndex = 1;
                            file = new byte[txtfilename.PostedFile.ContentLength];

                            txtfilename.PostedFile.InputStream.Read(file, 0, txtfilename.PostedFile.ContentLength);
                            xmlimport = new cXMLFileImport((int)ViewState["accountid"], file);
                            xmlimport.extractDataFromFile(true);
                            if (!xmlimport.isvalidfile)
                            {
                                lblmsg.Text = "The file you have selected to import is not a valid " + cmbfiletype.SelectedItem.Text + " file.";
                                lblmsg.Visible = true;
                                e.Cancel = true;
                                return;
                            }
                            ViewState["import"] = xmlimport;
                            gridsample.DataSource = xmlimport.getSample();
                            gridsample.DataBind();
                            break;
                        case ImportType.ESREmployees:
                            //file = new byte[txtfilename.PostedFile.ContentLength];

                            //txtfilename.PostedFile.InputStream.Read(file, 0, txtfilename.PostedFile.ContentLength);
                            //cESRImport esrimport = new cESRImport((int)ViewState["accountid"], file);
                            //esrimport.extractDataFromFile(true);
                            //if (!esrimport.isvalidfile)
                            //{
                            //    lblmsg.Text = "The file you have selected to import is not a valid " + cmbfiletype.SelectedItem.Text + " file.";
                            //    lblmsg.Visible = true;
                            //    e.Cancel = true;
                            //    return;
                            //}
                            //ViewState["import"] = esrimport;
                            break;
                    }

                    break;
                case 1:
                    filetype = (ImportType)int.Parse(cmbfiletype.SelectedValue);
                    switch (filetype)
                    {
                        case ImportType.FlatFile:
                        case ImportType.Excel:
                        case ImportType.XML:
                            import = (cImport)ViewState["import"];
                            gridmatching.DataSource = import.getMatchingGrid();
                            gridmatching.DataBind();
                            break;
                            
                        case ImportType.ESREmployees:
                            wizimport.ActiveStepIndex = 3;
                            break;
                    }
                    break;
                case 2:

                    filetype = (ImportType)int.Parse(cmbfiletype.SelectedValue);
                    switch (filetype)
                    {
                        case ImportType.FlatFile:
                        case ImportType.Excel:
                        case ImportType.XML:

                            import = (cImport)ViewState["import"];
                            import.setMatchingGrid(new Guid(cmbdestination.SelectedValue), getMatchingGrid());
                            griddefaults.DataSource = import.getColumnsForDefaultGrid();
                            griddefaults.DataBind();
                            break;
                        
                    }
                    break;
                case 3:
                    import = (cImport)ViewState["import"];

                    filetype = (ImportType)int.Parse(cmbfiletype.SelectedValue);
                    switch (filetype)
                    {
                        case ImportType.FlatFile:

                            int headerRows = 0;

                            int.TryParse(txtheaderrowstoskip.Text, out headerRows);

                            import.headercount = headerRows;

                            ((cFlatFileImport)import).extractDataFromFile(false);
                            break;
                        case ImportType.Excel:
                            ((cExcelFileImport)import).extractDataFromFile(false);
                            break;
                        case ImportType.XML:
                            ((cXMLFileImport)import).extractDataFromFile(false);
                            break;
                        case ImportType.ESREmployees:
                            //((cESRImport)import).extractDataFromFile(false);
                            break;
                    }


                    import.setDefaultValues(getDefaultValues());
                    status = (List<string>)ViewState["status"];
                    if (status == null)
                    {
                        status = new List<string>();
                    }
                    status.Add("Validating file, please wait . . .");
                    ViewState["status"] = status;

                    displayStatusUpdate();
                    validated = import.validateImport();
                    foreach (string i in validated)
                    {
                        status.Add(i);
                    }
                    status.Add("File Validation Complete.");
                    if (status.Count > 1)
                    {
                        status.Add("The file cannot currently be imported. Please correct the errors and try again.");
                    }
                    ViewState["status"] = status;

                    if (validated.Count == 0)
                    {
                        status.Add("Importing data, please wait . . .");
                        validated = import.importData(this.ImportFileFactory);
                        foreach (string i in validated)
                        {
                            status.Add(i);
                        }
                        ViewState["status"] = status;
                    }
                    break;
                case 4:
                    filetype = (ImportType)int.Parse(cmbfiletype.SelectedValue);
                    switch (filetype)
                    {
                        case ImportType.FlatFile:
                        case ImportType.Excel:
                            
                            break;
                    }
                    break;
            }

        }

        private List<cImportField> getMatchingGrid()
        {
            string defaultvalue = string.Empty;
            Guid destinationcolumn;
            Guid lookupcolumn;
            string[] arrid;
            
            List<cImportField> grid = new List<cImportField>();
            foreach (Infragistics.WebUI.UltraWebGrid.UltraGridRow row in gridmatching.Rows)
            {
                if (row.Cells[2].Value != null)
                {
                    defaultvalue = (string)row.Cells[2].Value;
                }

                if (row.Cells[1].Value.ToString().Contains(","))
                {
                    arrid = row.Cells[1].Value.ToString().Split(',');
                    destinationcolumn = new Guid(arrid[0]);
                    lookupcolumn = new Guid(arrid[1]);
                }
                else
                {
                    lookupcolumn = Guid.Empty;
                    if ((string)row.Cells[1].Value == "-1" || (string)row.Cells[1].Value == "")
                    {
                        destinationcolumn = Guid.Empty;
                    }
                    else
                    {
                        destinationcolumn = new Guid(row.Cells[1].Value.ToString());
                    }
                }

                //if (hasData)
                //{
                    grid.Add(new cImportField(destinationcolumn, lookupcolumn, defaultvalue));
                    ((cImport)ViewState["import"]).usedColumnIndexes.Add(row.Index);
                //}
            }
            return grid;
        }

        private SortedList<Guid,string> getDefaultValues()
        {
            string defaultvalue;
            Guid destinationcolumn;
            
            SortedList<Guid,string> grid = new SortedList<Guid,string>();
            foreach (Infragistics.WebUI.UltraWebGrid.UltraGridRow row in griddefaults.Rows)
            {
                if (row.Cells[1].Value != null)
                {
                    if ((string)row.Cells[1].Value != "")
                    {
                        destinationcolumn = new Guid(row.Cells[0].Value.ToString());
                        defaultvalue = (string)row.Cells[1].Value;
                        grid.Add(destinationcolumn, defaultvalue);
                    }
                }
                
            }
            return grid;
        }

        protected void gridmatching_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {

            //create value list for fields

            Guid tableid = new Guid(cmbdestination.SelectedValue);
            cTables clsTables = new cTables((int)ViewState["accountid"]);
            cTable table = clsTables.GetTableByID(tableid);
            cFields clsfields = new cFields((int)ViewState["accountid"]);
            SortedList<string,string> fields = clsfields.getFieldsForImport(table,true);
            Infragistics.WebUI.UltraWebGrid.ValueList fieldlst = new Infragistics.WebUI.UltraWebGrid.ValueList();

            foreach (KeyValuePair<string, string> data in fields)
            {
                fieldlst.ValueListItems.Add(data.Value, data.Key);
            }
            fieldlst.ValueListItems.Add("-1","<ignore>");
            e.Layout.Bands[0].Columns.FromKey("sourcecolumn").Header.Caption = "Source Column";
            e.Layout.Bands[0].Columns.FromKey("destinationcolumn").Header.Caption = "Destination Column";
            e.Layout.Bands[0].Columns.FromKey("destinationcolumn").ValueList = fieldlst;
            e.Layout.Bands[0].Columns.FromKey("destinationcolumn").AllowUpdate = Infragistics.WebUI.UltraWebGrid.AllowUpdate.Yes;
            
            e.Layout.Bands[0].Columns.FromKey("destinationcolumn").Type = Infragistics.WebUI.UltraWebGrid.ColumnType.DropDownList;
            e.Layout.Bands[0].Columns.FromKey("defaultvalue").Header.Caption = "Default Value";
            e.Layout.Bands[0].Columns.FromKey("defaultvalue").AllowUpdate = Infragistics.WebUI.UltraWebGrid.AllowUpdate.Yes;
            e.Layout.CellClickActionDefault = Infragistics.WebUI.UltraWebGrid.CellClickAction.Edit;
            
        }

        protected void cmbdestination_SelectedIndexChanged(object sender, EventArgs e)
        {

            cImport import = (cImport)ViewState["import"];
            gridmatching.DataSource = import.getMatchingGrid();
            gridmatching.DataBind();
        }

        protected void gridmatching_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            //e.Row.Cells.FromKey("destinationcolumn").Value =-1;
        }

        protected void tmrimports_Tick(object sender, EventArgs e)
        {
            displayStatusUpdate();
        }

        private void displayStatusUpdate()
        {
            List<string> status = (List<string>)ViewState["status"];
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            foreach (string i in status)
            {
                output.Append("<li>" + i);
                if (i == "The file cannot currently be imported. Please correct the errors and try again." || i == "File imported successfully.")
                {
                    tmrimports.Enabled = false;
                }
            }
            
            status.Clear();
            ViewState["status"] = status;
            
            litimportstatus.Text += output.ToString();
        }

        protected void wizimport_ActiveStepChanged(object sender, EventArgs e)
        {
            if (ViewState["accountid"] != null)
            {
                littimeline.Text = createTimeline((int)ViewState["accountid"]);
            }
            switch (wizimport.ActiveStepIndex)
            {
                case 4:
                    List<string> status;
                    ImportType filetype = (ImportType)int.Parse(cmbfiletype.SelectedValue);
                    switch (filetype)
                    {
                        case ImportType.ESREmployees:
                            cESRImport esrimport = (cESRImport)ViewState["import"];
                            status = (List<string>)ViewState["status"];
                            if (status == null)
                            {
                                status = new List<string>();
                            }
                            status.Add("Validating file, please wait . . .");
                            ViewState["status"] = status;

                            displayStatusUpdate();
                            //validated = esrimport.validateImport();
                            
                            //BEN - This needs to incorporate logging in the future
                            //foreach (string i in validated)
                            //{
                            //    status.Add(i);
                            //}


                            ViewState["status"] = status;

                            //if (validated.Count == 0)
                            //{
                            //    status.Add("Importing data, please wait . . .");
                            //    esrimport.importData();
                            //}
                            break;
                    }
                    break;
            }
        }

        protected void cmbfirstrowheader_SelectedIndexChanged(object sender, EventArgs e)
        {
            int headercount;
            if (cmbfirstrowheader.SelectedValue == "Yes")
            {
                headercount = 1;
            }
            else
            {
                headercount = 0;
            }
            cExcelFileImport excelimport = (cExcelFileImport)ViewState["import"];
            excelimport.headercount = headercount;
            excelimport.extractDataFromFile(true);
            ViewState["import"] = excelimport;
            gridsample.DataSource = excelimport.getSample();
            gridsample.DataBind();
        }

        protected void cmbworksheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            cExcelFileImport excelimport = (cExcelFileImport)ViewState["import"];
            int worksheet = Convert.ToInt32(cmbworksheet.SelectedValue);
            excelimport.worksheetnumber = worksheet;
            excelimport.extractDataFromFile(true);
            ViewState["import"] = excelimport;
            gridsample.DataSource = excelimport.getSample();
            gridsample.DataBind();
        }

        public string createTimeline(int accountid)
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            wizardStep step;
            ArrayList steps = new ArrayList();
            int stepcount = 0;

            ArrayList stepsVisited = (ArrayList)ViewState["stepsVisited"];
            step = new wizardStep(0, stepcount, "Select a file to import");
            steps.Add(step);
            stepcount++;
            step = new wizardStep(1, stepcount, "File sample");
            steps.Add(step);
            stepcount++;
            step = new wizardStep(2, stepcount, "Match fields");
            steps.Add(step);
            stepcount++;
            step = new wizardStep(3, stepcount, "Import progress");
            steps.Add(step);
            stepcount++;




            float distance = (float)Math.Round(((float)100 / ((float)stepcount + 1)), 1, MidpointRounding.AwayFromZero);
            float left;
            for (int i = 0; i < steps.Count; i++)
            {
                step = (wizardStep)steps[i];
                left = distance * (i + 1);
                output.Append("<div class=\"timelineevent\" style=\"left: " + left + "%;\">");
                if (stepsVisited.Contains(step.Actualstep))
                {
                    output.Append("<a href=\"javascript:changeStep(" + step.Actualstep + ");\">");
                }
                if (wizimport.ActiveStepIndex == step.Actualstep)
                {
                    output.Append("<img id=\"activeStepCheck\" src=\"" + cMisc.Path + "/shared/images/buttons/timeline_event" + (i + 1) + "_sel.gif\" class=\"timelineimg\">");
                }
                else
                {
                    output.Append("<img src=\"" + cMisc.Path + "/shared/images/buttons/timeline_event" + (i + 1) + ".gif\" class=\"timelineimg\">");
                }
                if (stepsVisited.Contains(step.Actualstep))
                {
                    output.Append("</a>");
                }
                output.Append("<br>");
                output.Append("<span class=\"timeeventlabel\">" + step.Label + "</span>");
                output.Append("</div>\n");
            }
            return output.ToString();
        }

        protected void wizimport_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            switch (user.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.Contracts:
                    Response.Redirect(cMisc.Path + "/MenuMain.aspx?menusection=importsexports", true);
                    break;

                default:
                    Response.Redirect(cMisc.Path + "/exportmenu.aspx", true);
                    break;
            }
        }

        protected void griddefaults_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns.FromKey("column").Header.Caption = "Field";
            e.Layout.Bands[0].Columns.FromKey("column").Type = Infragistics.WebUI.UltraWebGrid.ColumnType.DropDownList;

            cTables clsTables = new cTables((int)ViewState["accountid"]);
            cTable table = clsTables.GetTableByID(new Guid(cmbdestination.SelectedValue));
            cFields clsfields = new cFields((int)ViewState["accountid"]);
            SortedList<string,string> fields = clsfields.getFieldsForImport(table,false);
            
            Infragistics.WebUI.UltraWebGrid.ValueList lst = new Infragistics.WebUI.UltraWebGrid.ValueList();
            foreach (KeyValuePair<string, string> field in fields)
            {
                lst.ValueListItems.Add(new Guid(field.Value), field.Key);
            }
            e.Layout.Bands[0].Columns.FromKey("column").ValueList = lst;
            e.Layout.Bands[0].Columns.FromKey("defaultvalue").Header.Caption = "Default Value";
            e.Layout.Bands[0].Columns.FromKey("defaultvalue").AllowUpdate = Infragistics.WebUI.UltraWebGrid.AllowUpdate.Yes;

        }

    }
}
