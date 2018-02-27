namespace expenses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Services;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using AjaxControlToolkit;
    using Spend_Management;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using Syncfusion.XlsIO;
    using SpendManagementLibrary.Employees.DutyOfCare;
    using Spend_Management.shared.code;

    public partial class qeform : Page
    {
        public int quickentryid;
        public IWorkbook workbook;
        public string todaysDate
        {
            get
            {
                string dt = DateTime.Today.Year.ToString("0000") + "/" + DateTime.Today.Month.ToString("00") + "/" + DateTime.Today.Day.ToString("00");
                return dt;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;
            ViewState["subAccountID"] = user.CurrentSubAccountId;
           
            
            cAccounts clsAccounts = new cAccounts();
            cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);

            if (reqAccount.QuickEntryFormsEnabled == false)
            {
                Response.Redirect("home.aspx?", true);
            }

            if (Request.QueryString["quickentryid"] == null || Request.QueryString["quickentryid"] == "")
            {
                Title = "Quick Entry Form Upload";
                mvQEForm.ActiveViewIndex = 0;
                pnlLinks.Visible = false;
            }
            else
            {
                Title = "Quick Entry Form";
                mvQEForm.ActiveViewIndex = 1;
                pnlLinks.Visible = true;
            }
            
            Master.title = Title;

            if (IsPostBack == false)
            {
                if (Request.QueryString["quickentryid"] != null && Request.QueryString["quickentryid"] != "")
                {
                    quickentryid = int.Parse(Request.QueryString["quickentryid"]);
                    ViewState["quickentryid"] = quickentryid;
                }
            }

            if (IsPostBack == false)
            {
                cQeForms clsforms = new cQeForms((int)ViewState["accountid"]);
                cQeForm reqform = clsforms.getFormById(quickentryid);
                ViewState["reqform"] = reqform;
                //select default month
                cmbmonth.Items.Add(new ListItem("January", "1"));
                cmbmonth.Items.Add(new ListItem("February", "2"));
                cmbmonth.Items.Add(new ListItem("March", "3"));
                cmbmonth.Items.Add(new ListItem("April", "4"));
                cmbmonth.Items.Add(new ListItem("May", "5"));
                cmbmonth.Items.Add(new ListItem("June", "6"));
                cmbmonth.Items.Add(new ListItem("July", "7"));
                cmbmonth.Items.Add(new ListItem("August", "8"));
                cmbmonth.Items.Add(new ListItem("September", "9"));
                cmbmonth.Items.Add(new ListItem("October", "10"));
                cmbmonth.Items.Add(new ListItem("November", "11"));
                cmbmonth.Items.Add(new ListItem("December", "12"));
                cmbmonth.Items.FindByValue(DateTime.Today.Month.ToString()).Selected = true;

                ClientScript.RegisterClientScriptBlock(this.GetType(), "empIDforcar", "var carEmployeeID = " + user.EmployeeID + ";",true);
            }

            if (ViewState["quickentryid"] != null && ViewState["quickentryid"].ToString() != "")
            {
                bool spreadsheet = false;

                if (Request.QueryString["spreadsheet"] != null && Request.QueryString["spreadsheet"] != "")
                {
                    spreadsheet = true;
                }

                generateForm((cQeForm)ViewState["reqform"], null, spreadsheet);
            }
        }

        private void generateForm(cQeForm form, IWorkbook Workbook, bool Spreadsheet)
        {
            var user = cMisc.GetCurrentUser();
            tbl.Rows.Clear();
            int accountId = (int)ViewState["accountid"];
            cQeFieldColumn fieldcol;
            cQeSubcatColumn subcatcol;
            TableRow tblrow;
            TableCell cell;
            TableHeaderCell header;
            cMisc clsmisc = new cMisc((int)ViewState["accountid"]);
            cFieldToDisplay reason = clsmisc.GetGeneralFieldByCode("reason");
            cFieldToDisplay country = clsmisc.GetGeneralFieldByCode("country");
            cFieldToDisplay currency = clsmisc.GetGeneralFieldByCode("currency");
            cFieldToDisplay otherdetails = clsmisc.GetGeneralFieldByCode("otherdetails");

            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            var accountProperties = clsSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties.Clone();

            cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);

            int employeeid = (int)this.ViewState["employeeid"];
            Employee reqemp = clsemployees.GetEmployeeById(employeeid);
            cEmployeeCars clsEmployeeCars = new cEmployeeCars((int)ViewState["accountid"], employeeid);
            var documentExpiryResult = new DutyOfCareDocuments().PassesDutyOfCare(user.AccountID, clsEmployeeCars.GetActiveCars(includePoolCars: false), employeeid, Convert.ToDateTime(this.todaysDate), user.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect), accountProperties, new VehicleValidatorCheck(cMisc.GetCurrentUser(), accountProperties, clsEmployeeCars));
            #region header


            int colspan = 0;

            tblrow = new TableRow();

            foreach (cQeColumn column in form.columns)
            {

                if (column.GetType() == typeof(cQeFieldColumn))
                {
                    header = new TableHeaderCell();
                    fieldcol = (cQeFieldColumn)column;
                    switch (fieldcol.field.FieldID.ToString())
                    {
                        case "ec527561-dfee-48c7-a126-0910f8e031b0": //country
                            header.Text = country.description;
                            break;
                        case "1ee53ae2-2cdf-41b4-9081-1789adf03459": //currency
                            header.Text = currency.description;
                            break;
                        case "7cf61909-8d25-4230-84a9-f5701268f94b": //otherdetails
                            header.Text = otherdetails.description;
                            break;
                        case "af839fe7-8a52-4bd1-962c-8a87f22d4a10": //reasons
                            header.Text = reason.description;
                            break;
                        default:
                            header.Text = fieldcol.field.Description;
                            break;
                    }

                    tblrow.Cells.Add(header);

                }
                else
                {
                    colspan = 1;
                    header = new TableHeaderCell();

                    subcatcol = (cQeSubcatColumn)column;
                    switch (subcatcol.subcat.calculation)
                    {
                        case CalculationType.NormalItem:
                        case CalculationType.Meal:
                            if (subcatcol.subcat.receiptapp == true)
                            {
                                colspan++;

                            }
                            if (subcatcol.subcat.vatapp == true && subcatcol.subcat.vatreceipt == true)
                            {
                                colspan++;

                            }
                            break;
                        case CalculationType.PencePerMile:
                            if (getCarCount() > 1)
                            {
                                colspan++;

                                List<cCar> cars = clsEmployeeCars.Cars;

                                foreach (cCar car in cars)
                                {
                                    if (car.mileagecats.Count > 1)
                                    {
                                        colspan++;
                                        break;
                                    }
                                }
                            }
                            break;
                    }
                    if (colspan != 1)
                    {
                        header.ColumnSpan = colspan;
                    }
                    header.Text = subcatcol.subcat.subcat;
                    tblrow.Cells.Add(header);
                }

            }
            tbl.Rows.Add(tblrow);


            tblrow = new TableRow();
            foreach (cQeColumn column in form.columns)
            {

                if (column.GetType() == typeof(cQeFieldColumn))
                {
                    header = new TableHeaderCell();
                    tblrow.Cells.Add(header);
                }
                else
                {
                    colspan = 1;

                    subcatcol = (cQeSubcatColumn)column;
                    switch (subcatcol.subcat.calculation)
                    {
                        case CalculationType.NormalItem:
                        case CalculationType.Meal:
                            header = new TableHeaderCell();

                            if (subcatcol.subcat.calculation != CalculationType.FixedAllowance)
                            {
                                if (subcatcol.subcat.addasnet == true)
                                {
                                    header.Text = "NET";
                                }
                                else
                                {
                                    header.Text = "Gross";
                                }
                            }
                            tblrow.Cells.Add(header);
                            if (subcatcol.subcat.receiptapp == true)
                            {
                                header = new TableHeaderCell();
                                header.Text = "R";
                                tblrow.Cells.Add(header);
                            }
                            if (subcatcol.subcat.vatapp == true && subcatcol.subcat.vatreceipt == true)
                            {
                                header = new TableHeaderCell();
                                header.Text = "VR";
                                tblrow.Cells.Add(header);
                            }
                            break;
                        case CalculationType.PencePerMile:
                            if (getCarCount() > 1)
                            {
                                header = new TableHeaderCell();
                                header.Text = "No Miles";
                                tblrow.Cells.Add(header);

                                header = new TableHeaderCell();
                                header.Text = "Cars";
                                tblrow.Cells.Add(header);


                                List<cCar> cars = clsEmployeeCars.Cars;

                                foreach (cCar car in cars)
                                {
                                    if (car.mileagecats.Count > 1)
                                    {
                                        header = new TableHeaderCell();
                                        header.Text = "Vehicle Journey Rate Categories";
                                        tblrow.Cells.Add(header);
                                        break;
                                    }
                                }

                            }
                            else
                            {
                                header = new TableHeaderCell();
                                header.Text = "No Miles";
                                tblrow.Cells.Add(header);
                            }
                            break;
                        default:
                            header = new TableHeaderCell();
                            header.Text = "No Miles";
                            tblrow.Cells.Add(header);
                            break;
                    }

                }

            }
            tbl.Rows.Add(tblrow);



            #endregion

            StringBuilder totalboxes = new StringBuilder();
            #region body
            int row = 5;
            int col = 1;
            bool spreadsheet = false;
            ExcelEngine exceleng;
            IApplication app;

            //IWorkbook workbook = ExcelUtils.CreateWorkbook(1);
            IWorksheet worksheet;

            if (Spreadsheet ==true && Workbook != null)
            {
                spreadsheet = true;
                exceleng = new ExcelEngine();
                app = exceleng.Excel;
                workbook = Workbook;
                //workbook = (IWorkbook)Session["qef" + quickentryid];
                //Session.Remove("qef"+quickentryid);
                
                worksheet = workbook.Worksheets[0];
            }
            else
            {
                worksheet = null;
            }
            int cursubcol;
            
            cReasons clsreasons = new cReasons((int)ViewState["accountid"]);
            cCountries clscountries = new cCountries((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
            cCurrencies clscurrencies = new cCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
            cMileagecats clsmileagecats = new cMileagecats((int)ViewState["accountid"]);

            string rowclass = "row1";
            string colclass = "col1";
            CalendarExtender cal;
            DropDownList ddlst;
            CompareValidator compval;
            HiddenField hdn;
            Image img;
            int numrows;
            int month = 0;
            DateTime date;
            if (form.genmonth == true)
            {


                month = int.Parse(cmbmonth.SelectedValue);

                numrows = getDaysInMonth(month);
            }
            else
            {
                numrows = form.numrows;
            }
            ClientScript.RegisterHiddenField("numrows", numrows.ToString());
            cGlobalProperties clsproperties = clsmisc.GetGlobalProperties((int)ViewState["accountid"]);
            DateTime earliestdate = new DateTime();

            List<string> javaScriptBinds = new List<string>();
            List<object> autoCompleteParameters = AutoComplete.getAutoCompleteQueryParams("organisations");
            SortedList<byte, FieldFilter> autoCompleteFilters = new SortedList<byte, FieldFilter> { { 0, new FieldFilter(new cFields(accountId).GetFieldByID(new Guid("4B7873D6-8EDC-44D4-94F7-B8ABCBD87692")), ConditionType.Equals, "0", "", 0, null) } };

            for (int i = 0; i < numrows; i++)
            {
                cursubcol = 0;
                colclass = "col1";
                tblrow = new TableRow();
                col = 1;
                foreach (cQeColumn column in form.columns)
                {
                    TextBox txtbox;
                    if (column.GetType() == typeof(cQeFieldColumn))
                    {
                        fieldcol = (cQeFieldColumn)column;
                        AutoCompleteExtender autocomp;
                        switch (fieldcol.field.FieldID.ToString())
                        {
                            case "a52b4423-c766-47bb-8bf3-489400946b4c": //date
                                cell = new TableCell();
                                cell.CssClass = rowclass;
                                txtbox = new TextBox();
                                txtbox.Width = Unit.Pixel(69);
                                txtbox.ID = "txtdate" + i;
                                if (spreadsheet == true)
                                {
                                    if (worksheet.Range[row, col].Value != "")
                                    {
                                        date = DateTime.Parse(worksheet.Range[row, col].Value);
                                        txtbox.Text = date.ToShortDateString();
                                    }
                                }
                                else
                                {
                                    if (form.genmonth == true)
                                    {
                                        date = new DateTime(DateTime.Today.Year, month, (i + 1));
                                        if (date > DateTime.Today)
                                        {
                                            date = DateTime.Today;
                                        }
                                        txtbox.Text = date.ToShortDateString();
                                    }
                                }
                                cell.Controls.Add(txtbox);
                                //compval = new CompareValidator();
                                //compval.ControlToValidate = "txtdate" + i;
                                //compval.Text = "*";
                                //compval.ErrorMessage = "The latest date an expense can be claimed for is the " + DateTime.Today.ToShortDateString();
                                //compval.Type = ValidationDataType.Date;
                                //compval.Operator = ValidationCompareOperator.LessThanEqual;
                                //compval.ID = "comptxtdatemax" + i;
                                //compval.ValueToCompare = DateTime.Today.ToShortDateString();
                                //cell.Controls.Add(compval);
                                if (earliestdate != new DateTime(1900, 01, 01))
                                {

                                    compval = new CompareValidator();
                                    compval.ControlToValidate = "txtdate" + i;
                                    compval.Text = "*";
                                    compval.ErrorMessage = "The earliest date an expense can be claimed for is the " + earliestdate.ToShortDateString();
                                    compval.Type = ValidationDataType.Date;
                                    compval.Operator = ValidationCompareOperator.GreaterThanEqual;
                                    compval.ID = "comptxtdatemin" + i;
                                    compval.ValueToCompare = earliestdate.ToShortDateString();
                                    cell.Controls.Add(compval);
                                }
                                MaskedEditExtender maskededit = new MaskedEditExtender();
                                maskededit.TargetControlID = "txtdate" + i;
                                maskededit.Mask = "99/99/9999";
                                maskededit.MaskType = MaskedEditType.Date;
                                maskededit.CultureName = "en-GB";
                                maskededit.ID = "txtdatemask" + i;
                                img = new Image();
                                img.ImageUrl = "icons/cal.gif";
                                img.ID = "imgcal" + i;
                                cell.Controls.Add(img);
                                cal = new CalendarExtender();
                                cal.TargetControlID = "txtdate" + i;
                                cal.Format = "dd/MM/yyyy";
                                cal.PopupButtonID = "imgcal" + i;
                                cal.ID = "txtdatecal" + i;

                                cell.Controls.Add(cal);
                                cell.Controls.Add(maskededit);
                                tblrow.Cells.Add(cell);
                                //output.Append("><a href=\"javascript:calendar_onclick('date" + i + "');\"><img src=\"icons/cal.gif\" border=\"0\"></a>");
                                break;

                            case "ec527561-dfee-48c7-a126-0910f8e031b0": //dd //country
                                cell = new TableCell();
                                cell.Text = rowclass;
                                ddlst = new DropDownList();
                                ddlst.ID = "cmbcountry" + i;
                                ddlst.Items.AddRange(clscountries.CreateDropDown().ToArray());

                                if (spreadsheet == true)
                                {
                                    if (worksheet.Range[row, col].Text != "")
                                    {
                                        if (ddlst.Items.FindByText(worksheet.Range[row, col].Text) != null)
                                        {
                                            ddlst.Items.FindByText(worksheet.Range[row, col].Text).Selected = true;
                                        }

                                    }
                                }
                                cell.Controls.Add(ddlst);

                                tblrow.Cells.Add(cell);
                                break;
                            case "1ee53ae2-2cdf-41b4-9081-1789adf03459": //currencies
                                cell = new TableCell();
                                cell.CssClass = rowclass;
                                ddlst = new DropDownList();
                                ddlst.ID = "cmbcurrency" + i;
                                ddlst.Items.AddRange(clscurrencies.CreateDropDown().ToArray());


                                if (spreadsheet == true)
                                {
                                    if (worksheet.Range[row, col].Text != "")
                                    {
                                        if (ddlst.Items.FindByText(worksheet.Range[row, col].Text) != null)
                                        {
                                            ddlst.Items.FindByText(worksheet.Range[row, col].Text).Selected = true;
                                        }

                                    }
                                }
                                cell.Controls.Add(ddlst);
                                tblrow.Cells.Add(cell);
                                break;
                            case "7cf61909-8d25-4230-84a9-f5701268f94b": //other details
                                cell = new TableCell();
                                cell.CssClass = rowclass;

                                txtbox = new TextBox();
                                txtbox.ID = "txtotherdetails" + i;
                                txtbox.TextMode = TextBoxMode.MultiLine;
                                if (spreadsheet == true)
                                {
                                    txtbox.Text = worksheet.Range[row, col].Text;
                                }
                                cell.Controls.Add(txtbox);
                                tblrow.Cells.Add(cell);
                                break;
                            case "af839fe7-8a52-4bd1-962c-8a87f22d4a10": //reasons
                                cell = new TableCell();
                                cell.CssClass = rowclass;

                                ddlst = new DropDownList();
                                ddlst.ID = "cmbreasons" + i;
                                ddlst.Items.AddRange(clsreasons.CreateDropDown().ToArray());


                                if (spreadsheet == true)
                                {
                                    if (worksheet.Range[row, col].Text != "")
                                    {
                                        if (ddlst.Items.FindByText(worksheet.Range[row, col].Text) != null)
                                        {
                                            ddlst.Items.FindByText(worksheet.Range[row, col].Text).Selected = true;
                                        }

                                    }
                                }
                                cell.Controls.Add(ddlst);
                                tblrow.Cells.Add(cell);
                                break;

                        }

                    }
                    else
                    {
                        subcatcol = (cQeSubcatColumn)column;
                        CheckBox chkbox;
                        switch (subcatcol.subcat.calculation)
                        {
                            case CalculationType.FixedAllowance:
                                cell = new TableCell();
                                cell.CssClass = rowclass + " " + colclass;
                                chkbox = new CheckBox();
                                chkbox.ID = "chkallowance" + i;
                                cell.Controls.Add(chkbox);
                                tblrow.Cells.Add(cell);
                                break;
                            case CalculationType.NormalItem: //Normal
                            case CalculationType.Meal: //lunch
                                    cell = new TableCell();
                                    cell.CssClass = rowclass + " " + colclass;
                                    txtbox = new TextBox();
                                    txtbox.Width = Unit.Pixel(30);
                                    txtbox.ID = "subcat" + subcatcol.subcat.subcatid + i;
                                    if (i == 0)
                                    {
                                        totalboxes.Append("'subcat" + subcatcol.subcat.subcatid + "',");
                                    }
                                    if (spreadsheet == true)
                                    {
                                        txtbox.Text = worksheet.Range[row, col].Value;
                                    }
                                    cell.Controls.Add(txtbox);
                                    tblrow.Cells.Add(cell);
                                    if (subcatcol.subcat.receiptapp == true)
                                    {
                                        col++;
                                        cell = new TableCell();
                                        cell.CssClass = rowclass + " " + colclass;
                                        chkbox = new CheckBox();
                                        chkbox.ID = "subcatreceipt" + subcatcol.subcat.subcatid + i;

                                        if (spreadsheet == true)
                                        {
                                            if (worksheet.Range[row, col].Text == "Y")
                                            {
                                                chkbox.Checked = true;
                                            }
                                        }
                                        cell.Controls.Add(chkbox);
                                        tblrow.Cells.Add(cell);
                                    }
                                    if (subcatcol.subcat.vatapp == true && subcatcol.subcat.vatreceipt == true)
                                    {
                                        col++;
                                        cell = new TableCell();
                                        cell.CssClass = rowclass + " " + colclass;
                                        chkbox = new CheckBox();
                                        chkbox.ID = "subcatvatreceipt" + subcatcol.subcat.subcatid + i;

                                        if (spreadsheet == true)
                                        {
                                            if (worksheet.Range[row, col].Text == "Y")
                                            {
                                                chkbox.Checked = true;
                                            }
                                        }
                                        cell.Controls.Add(chkbox);
                                        tblrow.Cells.Add(cell);
                                    }
                                
                                if (colclass == "col1")
                                {
                                    colclass = "col2";
                                }
                                else
                                {
                                    colclass = "col1";
                                }
                                break;
                            case CalculationType.PencePerMile: //fuel
                                cell = new TableCell();
                                cell.CssClass = rowclass;
                                txtbox = new TextBox();
                                txtbox.ID = "txtmileage" + i;
                                txtbox.Width = Unit.Pixel(30);

                                if (documentExpiryResult.Count >0)
                                {
                                    txtbox.Enabled = false;
                                    txtbox.ToolTip = "Mileage cannot be claimed because your car does not meet the required duty of care policy.";
                                }
                                if (clsEmployeeCars.GetActiveCars(includePoolCars: false).Count == 1)
                                {
                                    if (documentExpiryResult.Count >0)
                                    {
                                        txtbox.Enabled = false;
                                        txtbox.ToolTip = "Mileage cannot be claimed because your car does not meet the required duty of care policy.";
                                    }
                                }
                                if (i == 0)
                                {
                                    totalboxes.Append("'subcat" + subcatcol.subcat.subcatid + "',");
                                }
                                txtbox.Width = Unit.Pixel(50);
                                if (spreadsheet == true)
                                {
                                    txtbox.Text = worksheet.Range[row, col].Value;
                                }
                                cell.Controls.Add(txtbox);
                                tblrow.Cells.Add(cell);
                                if (getCarCount() > 1)
                                {
                                    bool showMileageCats = false;
                                    cell = new TableCell();
                                    cell.CssClass = rowclass;
                                    ddlst = new DropDownList();
                                    ddlst.ID = "subcat" + subcatcol.subcat.subcatid + "cars" + i;
                                    ddlst.Attributes.Add("onchange", "popCats(" + i + ")");
                                    ddlst.Items.AddRange(clsEmployeeCars.CreateCurrentValidCarDropDown(DateTime.Now).ToArray());
                                    col++;
                                    cell.Controls.Add(ddlst);
                                    tblrow.Cells.Add(cell);

                                    List<cCar> cars = clsEmployeeCars.Cars;

                                    foreach (cCar car in cars)
                                    {
                                        if (car.mileagecats.Count > 1)
                                        {
                                            showMileageCats = true;
                                            break;
                                        }
                                    }

                                    cCar empCar = clsEmployeeCars.GetCarByID(int.Parse(ddlst.SelectedValue));

                                    if (showMileageCats)
                                    {
                                        cell = new TableCell();
                                        cell.CssClass = rowclass;
                                        ddlst = new DropDownList();
                                        ddlst.ID = "subcat" + subcatcol.subcat.subcatid + "mileage" + i;

                                        cMileageCat milecat;

                                        foreach (int val in empCar.mileagecats)
                                        {
                                            milecat = clsmileagecats.GetMileageCatById(val);
                                            ddlst.Items.Add(new ListItem(milecat.carsize, milecat.mileageid.ToString()));
                                        }
                                        col++;
                                        cell.Controls.Add(ddlst);
                                        tblrow.Cells.Add(cell);
                                    }

                                }
                                cell = new TableCell();
                                txtbox = new TextBox();
                                txtbox.Text = subcatcol.subcat.subcatid.ToString();
                                txtbox.Style.Add("display", "none");
                                txtbox.ID = "txtsubcatid" + i;
                                cell.Controls.Add(txtbox);
                                cell.Style.Add("display", "none");
                                tblrow.Cells.Add(cell);

                                break;
                        }
                        cursubcol++;

                    }
                    col++;
                }
                tbl.Rows.Add(tblrow);
                if (rowclass == "row1")
                {
                    rowclass = "row2";
                }
                else
                {
                    rowclass = "row1";
                }
                row++;
            }

            if (totalboxes.Length > 0)
            {
                totalboxes.Remove(totalboxes.Length - 1, 1);
                totalboxes.Insert(0, "var arrTotalboxes = new Array(");
                totalboxes.Append(");");
                ClientScript.RegisterClientScriptBlock(this.GetType(), "totals", totalboxes.ToString(), true);
            }
            #endregion
        }

        #region Utility Functions
        private int getCarCount()
        {
            var user = cMisc.GetCurrentUser();
            cEmployeeCars clsEmployeeCars = new cEmployeeCars(user.AccountID, user.EmployeeID);
            int carcount;
            carcount = clsEmployeeCars.GetActiveCars().Count;
            return carcount;

        }
        private int getDaysInMonth(int month)
        {
            DateTime date = new DateTime(DateTime.Today.Year, month, 01);
            date = date.AddMonths(1);
            date = date.AddDays(-1);
            return date.Day;
        }
        #endregion

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            int accountId = (int)ViewState["accountid"];
            cQeForms clsforms = new cQeForms(accountId);
            cQeForm reqform = clsforms.getFormById((int)ViewState["quickentryid"]);
            cQeSubcatColumn subcatcol;
            DateTime date;
            DropDownList ddlst;
            TextBox txtbox;
            CheckBox chkbox;
            List<cExpenseItem> items = new List<cExpenseItem>();

            cMisc clsmisc = new cMisc(accountId);
            cGlobalProperties clsproperties = clsmisc.GetGlobalProperties(accountId);

            cEmployees clsemployees = new cEmployees(accountId);
            Employee reqemp = clsemployees.GetEmployeeById((int)ViewState["employeeid"]);
            cEmployeeCars clsEmployeeCars = new cEmployeeCars(accountId, (int)ViewState["employeeid"]);

            cClaims clsclaims = new cClaims(accountId);
            cMileagecats clsmileage = new cMileagecats(accountId);

            int claimid = clsclaims.getDefaultClaim(ClaimStage.Current, reqemp.EmployeeID);
            if (claimid == 0)
            {
                claimid = clsclaims.insertDefaultClaim(reqemp.EmployeeID);
            }
            int country = 0;
            int currency = 0;
            int reasonid = 0;
            decimal miles = 0;
            int carid = 0;
            int mileageid = 0;
            string otherdetails = "";
            decimal total;
            bool receipt, vatreceipt;
            MileageUOM journeyuom;
            cExpenseItem newitem;

            SortedList<int, cJourneyStep> listJourneySteps;

            List<cDepCostItem> breakdown = new List<cDepCostItem>();

            cDepCostItem[] lstCostCodes = reqemp.GetCostBreakdown().ToArray();
            foreach (cDepCostItem costitem in lstCostCodes)
            {
                breakdown.Add(costitem);
            }

            for (int i = 2; i < tbl.Rows.Count; i++)
            {
                txtbox = (TextBox)tbl.FindControl("txtdate" + (i-2));
                if (txtbox.Text != "")
                {
                    date = DateTime.Parse(txtbox.Text);
                }
                else
                {
                    date = new DateTime(1900, 01, 01);
                }

                if (date != new DateTime(1900, 01, 01))
                {
                    //get general details;
                    foreach (cQeColumn column in reqform.columns)
                    {
                        if (column.GetType() == typeof(cQeFieldColumn))
                        {
                            cQeFieldColumn fieldcol = (cQeFieldColumn)column;
                            switch (fieldcol.field.FieldID.ToString())
                            {
                                case "ec527561-dfee-48c7-a126-0910f8e031b0": //country
                                    ddlst = (DropDownList)tbl.FindControl("cmbcountry" + (i-2));
                                    if (ddlst != null)
                                    {
                                        if (ddlst.SelectedItem != null)
                                        {
                                            country = int.Parse(ddlst.SelectedValue);
                                        }
                                        else
                                        {
                                            country = 0;
                                        }
                                    }
                                    else
                                    {
                                        country = 0;
                                    }
                                    break;
                                case "1ee53ae2-2cdf-41b4-9081-1789adf03459": //currency
                                    ddlst = (DropDownList)tbl.FindControl("cmbcurrency" + (i-2));
                                    if (ddlst != null)
                                    {
                                        if (ddlst.SelectedItem != null)
                                        {
                                            currency = int.Parse(ddlst.SelectedValue);
                                        }
                                        else
                                        {
                                            currency = 0;
                                        }
                                    }
                                    else
                                    {
                                        currency = 0;
                                    }
                                    break;
                                case "7cf61909-8d25-4230-84a9-f5701268f94b": //otherdetails
                                    txtbox = (TextBox)tbl.FindControl("txtotherdetails" + (i-2));
                                    otherdetails = txtbox.Text;
                                    break;
                                case "af839fe7-8a52-4bd1-962c-8a87f22d4a10": //reasons
                                    ddlst = (DropDownList)tbl.FindControl("cmbreasons" + (i-2));
                                    if (ddlst != null)
                                    {
                                        if (ddlst.SelectedItem != null)
                                        {
                                            reasonid = int.Parse(ddlst.SelectedValue);
                                        }
                                        else
                                        {
                                            reasonid = 0;
                                        }
                                    }
                                    else
                                    {
                                        reasonid = 0;
                                    }
                                    break;
                            }
                        }
                    }

                    //add items
                    foreach (cQeColumn column in reqform.columns)
                    {
                        miles = 0;
                        total = 0;
                        receipt = false;
                        vatreceipt = false;
                        carid = 0;
                        mileageid = 0;
                        if (column.GetType() == typeof(cQeSubcatColumn))
                        {
                            subcatcol = (cQeSubcatColumn)column;

                            switch (subcatcol.subcat.calculation)
                            {
                                case CalculationType.FixedAllowance:
                                    chkbox = (CheckBox)tbl.FindControl("chkallowance" + (i - 2));
                                    break;
                                case CalculationType.NormalItem: //Normal
                                case CalculationType.Meal: //lunch
                                    miles = 0;
                                    carid = 0;
                                    
                                   
                                        
                                        txtbox = (TextBox)tbl.FindControl("subcat" + subcatcol.subcat.subcatid + (i-2));
                                        if (txtbox != null)
                                        {
                                            if (txtbox.Text != "")
                                            {
                                                total = decimal.Parse(txtbox.Text);
                                            }
                                            else
                                            {
                                                total = 0;
                                            }
                                        }
                                        else
                                        {
                                            total = 0;
                                        }

                                        if (subcatcol.subcat.receiptapp == true)
                                        {
                                            chkbox = (CheckBox)tbl.FindControl("subcatreceipt" + subcatcol.subcat.subcatid + (i-2));
                                            receipt = chkbox.Checked;
                                        }
                                        else
                                        {
                                            receipt = false;
                                        }
                                        if (subcatcol.subcat.vatapp == true && subcatcol.subcat.vatreceipt == true)
                                        {

                                            chkbox = (CheckBox)tbl.FindControl("subcatvatreceipt" + subcatcol.subcat.subcatid + (i-2));

                                            vatreceipt = chkbox.Checked;

                                        }
                                        else
                                        {
                                            vatreceipt = false;
                                        }
                                    

                                    break;
                                case CalculationType.PencePerMile: //fuel
                                    total = 0;
                                    receipt = false;
                                    vatreceipt = false;
                                    txtbox = (TextBox)tbl.FindControl("txtmileage" + (i - 2));
                                    if (txtbox.Text != "")
                                    {
                                        miles = decimal.Parse(txtbox.Text);
                                        if (getCarCount() > 1)
                                        {

                                            ddlst = (DropDownList)tbl.FindControl("subcat" + subcatcol.subcat.subcatid + "cars" + (i - 2));
                                            if (ddlst.SelectedItem != null)
                                            {
                                                carid = int.Parse(ddlst.SelectedValue);
                                            }
                                            else
                                            {
                                                carid = 0;
                                            }
                                        }
                                        else //get default car
                                        {
                                            carid = clsEmployeeCars.GetDefaultCarID(clsproperties.blocktaxexpiry, clsproperties.blockmotexpiry, clsproperties.blockinsuranceexpiry,clsproperties.BlockBreakdownCoverExpiry, false);
                                        }
                                        ddlst = (DropDownList)tbl.FindControl("subcat" + subcatcol.subcat.subcatid + "mileage" + (i - 2));
                                        if (ddlst != null)
                                        {
                                            if (ddlst.SelectedValue != null)
                                            {
                                                mileageid = int.Parse(ddlst.SelectedValue);
                                            }
                                            else
                                            {
                                                mileageid = 0;
                                            }
                                        }
                                        else
                                        {
                                            if (carid > 0)
                                            {
                                                mileageid = clsEmployeeCars.GetCarByID(carid).mileagecats[0];
                                            }
                                            else
                                            {
                                                lblmsg.Text = "< br />Valid car details are required on your account in order to submit a mileage claim.";
                                                lblmsg.Visible = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        miles = 0;
                                        carid = 0;
                                    }


                                    break;
                            }

                            cMileageCat reqmileage = clsmileage.GetMileageCatById(mileageid);

                            journeyuom = reqmileage == null ? MileageUOM.Mile : reqmileage.mileUom;

                            if (total > 0 || (miles > 0 && carid != 0)) //add the item
                            {
                                if (currency == 0) //get the default
                                {
                                    currency = clsproperties.basecurrency;
                                }
                                if (country == 0)
                                {
                                    country = clsproperties.homecountry;
                                }

                                newitem = new cExpenseItem(0, ItemType.Cash, 0, 0, otherdetails, receipt, total, 0, total, subcatcol.subcat.subcatid, date, 0, 0, 0, false, false, string.Empty, claimid, 0, 0, currency, string.Empty, 0, country, 0, 0, 0, false, reasonid, receipt, new DateTime(1900, 01, 01), new DateTime(1900, 01, 01), carid, 0, 0, 0, 0, 0, 0, 0, true, 0, "", 0, 0, "", 0, 0, 0, 0, 0, false, false, 0, new DateTime(1900, 01, 01), 0, DateTime.Now, (int)ViewState["employeeid"], mileageid, journeyuom, 0, subcatcol.subcat.HomeToLocationType);
                                newitem.costcodebreakdown = breakdown;
                                newitem.journeysteps = new SortedList<int, cJourneyStep>();
                                newitem.journeysteps.Add(1, new cJourneyStep(0, null, null, miles, 0, 0, 1, miles, false));

                                items.Add(newitem);
                            }
                        }
                    }
                }
            }

            //additems here
            cExpenseItems clsexpitems = new cExpenseItems(accountId);

            foreach (cExpenseItem item in items)
            {
                
                clsexpitems.addItem(item, reqemp);
            }

            Response.Redirect("expenses/claimViewer.aspx?claimid=" + claimid, true);
        }
        

        protected void cmdUpload_Click(object sender, ImageClickEventArgs e)
        {
            string filename = qeFileUpload.PostedFile.FileName;
            int quickentryid;

            ExcelEngine exceleng = new ExcelEngine();
            IApplication app = exceleng.Excel;

            workbook = ExcelUtils.Open(qeFileUpload.PostedFile.InputStream);
            IWorksheet worksheet = workbook.Worksheets[0];

            //see if the sheet is valid
            if (worksheet.Range[1, 12].Text == "" || worksheet.Range[1, 12].Text == null)
            {
                lblmsg.Text = "<br />The spreadsheet you have uploaded is not a valid quick entry form.";
                lblmsg.Visible = true;
                return;
            }

            try
            {
                quickentryid = int.Parse(worksheet.Range[1, 12].Text);
            }
            catch (System.FormatException)
            {
                lblmsg.Text = "<br />The spreadsheet you have uploaded is not a valid quick entry form.";
                lblmsg.Visible = true;
                return;
            }
            ViewState["quickentryid"] = quickentryid;

            //Session.Add("qef" + quickentryid, workbook);
            //Response.Redirect("qeform.aspx?quickentryid=" + quickentryid + "&spreadsheet=1", true);

            //ViewState["qef_" + quickentryid] = workbook;
            //ViewState["quickentryid"] = quickentryid;

            cQeForms clsforms = new cQeForms((int)ViewState["accountid"]);
            cQeForm reqform = clsforms.getFormById(quickentryid);
            ViewState["reqform"] = reqform;
            //select default month
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("January", "1"));
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("February", "2"));
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("March", "3"));
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("April", "4"));
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("May", "5"));
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("June", "6"));
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("July", "7"));
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("August", "8"));
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("September", "9"));
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("October", "10"));
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("November", "11"));
            cmbmonth.Items.Add(new System.Web.UI.WebControls.ListItem("December", "12"));
            cmbmonth.Items.FindByValue(DateTime.Today.Month.ToString()).Selected = true;

            mvQEForm.ActiveViewIndex = 1;
            ViewState["quickentryid"] = quickentryid;
            generateForm(reqform, workbook, true);
        }

        protected void cmdexport_Click(object sender, System.EventArgs e)
        {

            cQeForms clsforms = new cQeForms((int)ViewState["accountid"]);
            cQeForm reqform = clsforms.getFormById((int)ViewState["quickentryid"]);
            ExcelEngine exceleng = new ExcelEngine();
            IApplication app = exceleng.Excel;
            IWorkbook workbook = ExcelUtils.CreateWorkbook(1);
            //IWorkbook workbook = ExcelUtils.CreateWorkbook(1);
            IWorksheet worksheet = workbook.Worksheets[0];
            workbook.Activate();

            worksheet.Range[1, 1, 1, 10].Merge();
            worksheet.Range[1, 1].Text = reqform.name;
            IStyle style;
            style = worksheet.Range[1, 1].CellStyle;
            style.Font.Bold = true;
            style.Font.Size = 10;
            style = worksheet.Range[1, 12].CellStyle;
            worksheet.Range[1, 12].Text = reqform.quickentryid.ToString();
            style.Font.Color = ExcelKnownColors.White;

            generateExcelHeader(reqform, ref worksheet);
            generateExcelBody(reqform, ref worksheet);



            //worksheet.Range["a1:a15"].sty

            worksheet.Protect("p3ngu1ns");
            workbook.SaveAs("qe.xls", Response, ExcelDownloadType.PromptDialog);


        }

        private void generateExcelHeader(cQeForm reqform, ref IWorksheet worksheet)
        {
            IStyle style;
            int row = 3;
            int col;
            col = 1;

            cQeFieldColumn fieldcol;
            cQeSubcatColumn subcatcol;
            cMisc clsmisc = new cMisc((int)ViewState["accountid"]);
            cFieldToDisplay reason = clsmisc.GetGeneralFieldByCode("reason");
            cFieldToDisplay country = clsmisc.GetGeneralFieldByCode("country");
            cFieldToDisplay currency = clsmisc.GetGeneralFieldByCode("currency");
            cFieldToDisplay otherdetails = clsmisc.GetGeneralFieldByCode("otherdetails");
            int colspan;
            foreach (cQeColumn column in reqform.columns)
            {
                
                if (column.GetType() == typeof(cQeFieldColumn))
                {


                    fieldcol = (cQeFieldColumn)column;
                    switch (fieldcol.field.FieldID.ToString())
                    {
                        case "ec527561-dfee-48c7-a126-0910f8e031b0": //country
                            worksheet.Range[row, col].Text = country.description;
                            break;
                        case "1ee53ae2-2cdf-41b4-9081-1789adf03459": //currency
                            worksheet.Range[row, col].Text = currency.description;
                            break;
                        case "7cf61909-8d25-4230-84a9-f5701268f94b": //otherdetails
                            worksheet.Range[row, col].Text = otherdetails.description;
                            break;
                        case "af839fe7-8a52-4bd1-962c-8a87f22d4a10": //reasons
                            worksheet.Range[row, col].Text = reason.description;
                            break;
                        default:
                            worksheet.Range[row, col].Text = fieldcol.field.Description;
                            break;
                    }

                    worksheet.Columns[col - 1].ColumnWidth = 15;
                    style = worksheet.Range[row, col].CellStyle;
                    style.Font.Bold = true;
                    style.Font.Size = 10;
                    style.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    style.Borders.LineStyle = ExcelLineStyle.Thin;
                    style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                    style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;


                }
                else
                {

                    colspan = 0;

                    subcatcol = (cQeSubcatColumn)column;
                    switch (subcatcol.subcat.calculation)
                    {
                        case CalculationType.NormalItem:
                        case CalculationType.Meal:
                            if (subcatcol.subcat.receiptapp == true)
                            {
                                colspan++;

                            }
                            if (subcatcol.subcat.vatapp == true && subcatcol.subcat.vatreceipt == true)
                            {
                                colspan++;

                            }
                            break;
                        case CalculationType.PencePerMile:

                            if (getCarCount() > 1)
                            {
                                colspan++;
                            }
                            break;
                    }
                    if (colspan != 0)
                    {
                        worksheet.Range[row, col, row, col + colspan].Merge();

                    }
                    worksheet.Range[row, col].Text = subcatcol.subcat.subcat;
                    style = worksheet.Range[row, col].CellStyle;
                    style.Font.Bold = true;
                    style.Font.Size = 10;
                    style.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    style.Borders.LineStyle = ExcelLineStyle.Thin;
                    style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                    style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                    if (colspan != 0)
                    {
                        col = col + colspan;
                    }

                }
                col++;
            }



            row++;
            col = 1;
            foreach (cQeColumn column in reqform.columns)
            {


                if (column.GetType() == typeof(cQeFieldColumn))
                {
                    style = worksheet.Range[row, col].CellStyle;
                    style.Borders.LineStyle = ExcelLineStyle.Thin;
                    style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                    style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                    col++;
                }
                else
                {
                    colspan = 1;

                    subcatcol = (cQeSubcatColumn)column;
                    switch (subcatcol.subcat.calculation)
                    {
                        case CalculationType.FixedAllowance:
                            if (subcatcol.subcat.addasnet == true)
                            {
                                worksheet.Range[row, col].Text = "Total (NET)";
                            }
                            else
                            {
                                worksheet.Range[row, col].Text = "Total (Gross)";
                            }
                            break;
                        case CalculationType.NormalItem:
                        case CalculationType.Meal:
                            style = worksheet.Range[row, col].CellStyle;
                            style.Borders.LineStyle = ExcelLineStyle.Thin;
                            style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                            style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                            style.WrapText = true;

                            

                            if (subcatcol.subcat.receiptapp == true)
                            {
                                col++;
                                style = worksheet.Range[row, col].CellStyle;
                                style.Borders.LineStyle = ExcelLineStyle.Thin;
                                style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                                style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                                worksheet.Range[row, col].Text = "R";
                                //worksheet.AutofitColumn(col);
                                worksheet.Columns[col - 1].ColumnWidth = 5;

                            }
                            if (subcatcol.subcat.vatapp == true && subcatcol.subcat.vatreceipt == true)
                            {
                                col++;
                                style = worksheet.Range[row, col].CellStyle;
                                style.Borders.LineStyle = ExcelLineStyle.Thin;
                                style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                                style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                                worksheet.Range[row, col].Text = "VR";
                                worksheet.Columns[col - 1].ColumnWidth = 5;
                            }
                            break;
                        case CalculationType.PencePerMile:
                            if (getCarCount() > 1)
                            {
                                worksheet.Range[row, col].Text = "No Miles";
                                style = worksheet.Range[row, col].CellStyle;
                                style.Borders.LineStyle = ExcelLineStyle.Thin;
                                style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                                style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                                col++;
                            }
                            else
                            {
                                worksheet.Range[row, col].Text = "No Miles";
                                style = worksheet.Range[row, col].CellStyle;
                                style.Borders.LineStyle = ExcelLineStyle.Thin;
                                style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                                style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                            }
                            break;
                        default:
                            worksheet.Range[row, col].Text = "No Miles";
                            style = worksheet.Range[row, col].CellStyle;
                            style.Borders.LineStyle = ExcelLineStyle.Thin;
                            style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                            style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                            break;
                    }
                    col++;
                }

            }

        }

        private void generateExcelBody(cQeForm reqform, ref IWorksheet worksheet)
        {
            IDataValidation val;
            IStyle style;


            string[] reasons = new string[0];
            string[] countries = new string[0];
            string[] currencies = new string[0];
            string[] yesno = new string[2];
            int row;
            int cursubcol;
            cReasons clsreasons = new cReasons((int)ViewState["accountid"]);
            cCountries clscountries = new cCountries((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
            cCurrencies clscurrencies = new cCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
            cQeFieldColumn fieldcol;
            cQeSubcatColumn subcatcol;

            int numrows;
            int month = 0;
            DateTime date;
            if (reqform.genmonth == true)
            {

                if (Request.QueryString["month"] != null)
                {
                    month = int.Parse(Request.QueryString["month"]);
                }
                else
                {
                    month = int.Parse(cmbmonth.SelectedValue);
                    //month = DateTime.Today.Month;
                }
                numrows = getDaysInMonth(month);

            }
            else
            {
                numrows = reqform.numrows;
            }

            row = 5;

            yesno[0] = "Y";
            yesno[1] = "N";
            for (int i = 0; i < numrows; i++)
            {
                cursubcol = 0;
                int col = 1;
                foreach (cQeColumn column in reqform.columns)
                {

                    if (column.GetType() == typeof(cQeFieldColumn))
                    {
                        fieldcol = (cQeFieldColumn)column;
                        switch (fieldcol.field.FieldID.ToString())
                        {
                            case "a52b4423-c766-47bb-8bf3-489400946b4c":
                                if (reqform.genmonth == true)
                                {
                                    date = new DateTime(DateTime.Today.Year, month, (i + 1));
                                    worksheet.Range[row, col].NumberFormat = "dd/MM/yyyy";
                                    worksheet.Range[row, col].DateTime = date;
                                }

                                break;
                            case "ec527561-dfee-48c7-a126-0910f8e031b0": //dd
                                if (countries.Length == 0)
                                {
                                    countries = clscountries.getArray();
                                }
                                val = worksheet.Range[row, col].DataValidation;
                                //val.ListOfValues = countries;
                                val.DataRange = worksheet.Range[1, 102, countries.Length, 102];
                                worksheet.Range[1, 102, countries.Length, 102].CellStyle.Font.Color = ExcelKnownColors.White;
                                break;
                            case "1ee53ae2-2cdf-41b4-9081-1789adf03459": //dd+
                                if (currencies.Length == 0)
                                {
                                    currencies = clscurrencies.getArray();
                                }
                                val = worksheet.Range[row, col].DataValidation;
                                //val.ListOfValues = currencies;
                                val.DataRange = worksheet.Range[1, 103, currencies.Length, 103];
                                worksheet.Range[1, 103, currencies.Length, 103].CellStyle.Font.Color = ExcelKnownColors.White;
                                break;
                            case "7cf61909-8d25-4230-84a9-f5701268f94b":

                                break;
                            case "af839fe7-8a52-4bd1-962c-8a87f22d4a10": //dd
                                if (reasons.Length == 0)
                                {
                                    reasons = clsreasons.getArray();
                                }
                                val = worksheet.Range[row, col].DataValidation;
                                val.DataRange = worksheet.Range[1, 105, reasons.Length, 105];
                                worksheet.Range[1, 105, reasons.Length, 105].CellStyle.Font.Color = ExcelKnownColors.White;
                                break;
                        }
                        style = worksheet.Range[row, col].CellStyle;
                        style.Borders.LineStyle = ExcelLineStyle.Thin;
                        style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                        style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                        style.Locked = false;
                    }
                    else
                    {
                        subcatcol = (cQeSubcatColumn)column;
                        style = worksheet.Range[row, col].CellStyle;
                        style.Borders.LineStyle = ExcelLineStyle.Thin;
                        style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                        style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                        style.Locked = false;
                        switch (subcatcol.subcat.calculation)
                        {
                            case CalculationType.FixedAllowance:
                                val = worksheet.Range[row, col].DataValidation;
                                val.ListOfValues = yesno;
                                break;
                            case CalculationType.NormalItem: //Normal
                            case CalculationType.Meal: //lunch

                                    val = worksheet.Range[row, col].DataValidation;
                                    val.AllowType = Syncfusion.XlsIO.ExcelDataType.Decimal;
                                    val.ShowErrorBox = true;
                                    style = worksheet.Range[row, col].CellStyle;
                                    style.FillBackground = ExcelKnownColors.Red2;
                                    if (subcatcol.subcat.receiptapp == true)
                                    {
                                        col++;
                                        style = worksheet.Range[row, col].CellStyle;
                                        style.Borders.LineStyle = ExcelLineStyle.Thin;
                                        style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                                        style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                                        style.Locked = false;
                                        val = worksheet.Range[row, col].DataValidation;
                                        val.ListOfValues = yesno;
                                    }
                                    if (subcatcol.subcat.vatapp == true && subcatcol.subcat.vatreceipt == true)
                                    {
                                        col++;
                                        style = worksheet.Range[row, col].CellStyle;
                                        style.Borders.LineStyle = ExcelLineStyle.Thin;
                                        style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                                        style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                                        style.Locked = false;
                                        val = worksheet.Range[row, col].DataValidation;
                                        val.ListOfValues = yesno;

                                    }
                                

                                break;
                            case CalculationType.PencePerMile: //fuel

                                val = worksheet.Range[row, col].DataValidation;
                                val.AllowType = ExcelDataType.Integer;
                                val.ShowErrorBox = true;

                                if (getCarCount() > 1)
                                {
                                    col++;
                                    style = worksheet.Range[row, col].CellStyle;
                                    style.Borders.LineStyle = ExcelLineStyle.Thin;
                                    style.Borders[ExcelBordersIndex.DiagonalDown].LineStyle = ExcelLineStyle.None;
                                    style.Borders[ExcelBordersIndex.DiagonalUp].LineStyle = ExcelLineStyle.None;
                                    style.Locked = false;


                                }


                                break;
                        }
                        cursubcol++;
                    }
                    col++;
                }
                row++;
            }

            if (countries.Length != 0)
            {
                for (int i = 0; i < countries.Length; i++)
                {
                    worksheet.Range[i + 1, 102].Text = countries[i];
                }
            }
            if (currencies.Length != 0)
            {
                for (int i = 0; i < currencies.Length; i++)
                {
                    worksheet.Range[i + 1, 103].Text = currencies[i];
                }
            }
            if (reasons.Length != 0)
            {
                for (int i = 0; i < reasons.Length; i++)
                {
                    worksheet.Range[i + 1, 105].Text = reasons[i];
                }
            }

        }

        protected void cmbmonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            generateForm((cQeForm)ViewState["reqform"], null, false);
        }

        [WebMethod(EnableSession = true)]
        public static object[] getMileageCats(int carid, int accountid, int employeeID)
        {
            cMileagecats clsmileagecats = new cMileagecats(accountid);
            ArrayList objlst = new ArrayList();
            cEmployeeCars clsEmployeeCars = new cEmployeeCars(accountid, employeeID);

            cCar car = clsEmployeeCars.GetCarByID(carid);

            cMileageCat milecat;

            foreach (int val in car.mileagecats)
            {
                milecat = clsmileagecats.GetMileageCatById(val);
                objlst.Add(new object[] { milecat.carsize, milecat.mileageid.ToString() });
            }

            return objlst.ToArray();
        }
    }
}
