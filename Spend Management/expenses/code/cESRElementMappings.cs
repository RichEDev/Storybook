using System;
using System.Data;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using SpendManagementLibrary;

namespace Spend_Management
{
    using System.Linq;

    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    public class cESRElementMappings : IESRElementMappings
    {
        private int nAccountID;
        private int nNHSTrustID;
        private string strsql;

        protected SortedList<int, cGlobalESRElement> listGlobals;
        protected SortedList<int, cESRElement> list;

        public cESRElementMappings(int AccountID, int NHSTrustID)
        {
            nAccountID = AccountID;
            nNHSTrustID = NHSTrustID;

            InitialiseData();
        }

        #region properties
        public int AccountID
        {
            get { return nAccountID; }
        }
        public int NHSTrustID
        {
            get { return nNHSTrustID; }
        }
        #endregion

        private void InitialiseData()
        {
            if (this.listGlobals == null)
            {
                this.listGlobals = CacheGlobalESRElements();
            }

            if (list == null)
            {
                list = CacheList();
            }
        }

        /// <summary>
        /// Clear the cache on the current server
        /// </summary>
        private void ClearCache()
        {
            list = null;
            InitialiseData();
        }

        /// <summary>
        /// Cache the list of elements for the specific trust
        /// </summary>
        /// <returns>A List of ESR Elements</returns>
        private SortedList<int, cESRElement> CacheList()
        {
            int ElementID;
            int GlobalElementID;
            SortedList<int, cESRElement> lstElements;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                lstElements = new SortedList<int, cESRElement>();

                this.strsql = "SELECT elementID, globalElementID FROM dbo.ESRElements WHERE NHSTrustID = @NHSTrustID";
                expdata.sqlexecute.Parameters.AddWithValue("@NHSTrustID", this.NHSTrustID);
                expdata.sqlexecute.CommandText = this.strsql;

                using (IDataReader reader = expdata.GetReader(this.strsql))
                {
                    expdata.sqlexecute.Parameters.Clear();

                    while (reader.Read())
                    {
                        ElementID = reader.GetInt32(reader.GetOrdinal("elementID"));
                        GlobalElementID = reader.GetInt32(reader.GetOrdinal("globalElementID"));

                        lstElements.Add(ElementID, new cESRElement(ElementID, GlobalElementID, this.getElementFields(ElementID), this.getElementSubcats(ElementID), this.NHSTrustID));
                    }
                    reader.Close();
                }
            }

            return lstElements;
        }

        /// <summary>
        /// Get a list of element fields associated to the element from the database
        /// </summary>
        /// <param name="ElementID">Unique ID of the element</param>
        /// <returns>A list of element fields</returns>
        private List<cESRElementField> getElementFields(int ElementID)
        {
            List<cESRElementField> lst;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                lst = new List<cESRElementField>();
                int GlobalElementFieldID;
                Guid ReportColumnID;
                int ElementFieldID;
                Aggregate Aggregate;
                byte Order;

                this.strsql = "SELECT * from ESRElementFields where elementID = @elementID";
                expdata.sqlexecute.Parameters.AddWithValue("@elementID", ElementID);
                using (IDataReader reader = expdata.GetReader(this.strsql))
                {
                    expdata.sqlexecute.Parameters.Clear();

                    while (reader.Read())
                    {
                        ElementFieldID = reader.GetInt32(reader.GetOrdinal("elementFieldID"));
                        GlobalElementFieldID = reader.GetInt32(reader.GetOrdinal("globalElementFieldID"));
                        ReportColumnID = reader.GetGuid(reader.GetOrdinal("reportColumnID"));
                        if (reader.IsDBNull(reader.GetOrdinal("aggregate")) == true)
                        {
                            Aggregate = Aggregate.None;
                        }
                        else
                        {
                            Aggregate = (Aggregate)reader.GetByte(reader.GetOrdinal("aggregate"));
                        }

                        Order = reader.GetByte(reader.GetOrdinal("order"));

                        lst.Add(new cESRElementField(ElementFieldID, ElementID, GlobalElementFieldID, ReportColumnID, Order, Aggregate));
                    }
                    reader.Close();
                }
            }

            return lst;

        }

        /// <summary>
        /// Get a list of element subcats associated to the element from the database
        /// </summary>
        /// <param name="ElementID">Unique ID of the element</param>
        /// <returns>A list of element subcats</returns>
        private List<int> getElementSubcats(int ElementID)
        {
            List<int> lstSubcats;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                int SubcatID;
                lstSubcats = new List<int>();

                this.strsql = "SELECT subcatID FROM ESRElementSubcats WHERE elementID = @elementID";

                expdata.sqlexecute.Parameters.AddWithValue("@elementID", ElementID);
                using (IDataReader reader = expdata.GetReader(this.strsql))
                {
                    expdata.sqlexecute.Parameters.Clear();

                    while (reader.Read())
                    {
                        SubcatID = reader.GetInt32(reader.GetOrdinal("subcatID"));

                        lstSubcats.Add(SubcatID);
                    }
                    reader.Close();
                }
            }
            return lstSubcats;
        }


        /// <summary>
        /// Get the ESR element by the passed in ID from the global list
        /// </summary>
        /// <param name="ElementID">Unique ID of the element</param>
        /// <returns>An element object</returns>
        public cESRElement getESRElementByID(int ElementID)
        {
            cESRElement element = null;
            list.TryGetValue(ElementID, out element);
            return element;
        }

        public SortedList<int, cESRElement> listElements()
        {
            return list;
        }

        /// <summary>
        /// Returns a list of all the unmapped expense items for this trust
        /// </summary>
        /// <returns></returns>
        public List<cSubcat> GetUnMappedExpenseItems()
        {
            List<cSubcat> lstUnMappedExpenseItems = new List<cSubcat>();
            using (var db = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                db.sqlexecute.Parameters.AddWithValue("@NHSTrustID", this.NHSTrustID);
                using (IDataReader reader = db.GetReader("SELECT subcatid FROM subcats WHERE subcats.subcatid NOT IN (SELECT ESRElementSubcats.subcatID FROM ESRElementSubcats INNER JOIN ESRElements ON ESRElements.elementID=ESRElementSubcats.elementID AND ESRElements.NHSTrustID=@NHSTrustID) ORDER BY subcat"))
                {
                    cSubcats clsSubcats = new cSubcats(this.AccountID);
                    cSubcat tmpSubcat = null;
                    while (reader.Read())
                    {
                        tmpSubcat = clsSubcats.GetSubcatById(reader.GetInt32(0));
                        lstUnMappedExpenseItems.Add(tmpSubcat);
                    }

                    reader.Close();
                }
            }

            return lstUnMappedExpenseItems;
        }

        /// <summary>
        /// Get all elements associated to the subcat passed in
        /// </summary>
        /// <param name="SubcatID">Unique ID of the expenses subcat</param>
        /// <returns>A list of elements</returns>
        public List<cESRElement> getESRElementsBySubcatID(int SubcatID)
        {
            List<cESRElement> lstElements = new List<cESRElement>();

            foreach (cESRElement element in list.Values)
            {
                foreach (int ID in element.Subcats)
                {
                    if (ID == SubcatID)
                    {
                        lstElements.Add(element);
                        break;
                    }
                }
            }

            return lstElements;
        }

        /// <summary>
        /// Save the ESR element and its corresponding fields and subcats to the database
        /// </summary>
        /// <param name="element">Element object to save</param>
        /// <returns>The ID of the element</returns>
        public int saveESRElement(cESRElement element)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int ElementID;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                int order = 1;

                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@elementID", element.ElementID);
                expdata.sqlexecute.Parameters.AddWithValue("@globalElementID", element.GlobalElementID);
                expdata.sqlexecute.Parameters.AddWithValue("@NHSTrustID", element.NHSTrustID);
                expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
                expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
                expdata.ExecuteProc("dbo.saveESRElement");
                ElementID = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                expdata.sqlexecute.Parameters.Clear();

                this.deleteESRElementFields(ElementID);

                //Save all associated fields
                foreach (cESRElementField field in element.Fields)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate == true)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                    expdata.sqlexecute.Parameters.AddWithValue("@elementID", ElementID);
                    expdata.sqlexecute.Parameters.AddWithValue("@elementFieldID", field.ElementFieldID);
                    expdata.sqlexecute.Parameters.AddWithValue("@globalElementFieldID", field.GlobalElementFieldID);
                    expdata.sqlexecute.Parameters.AddWithValue("@reportColumnID", field.ReportColumnID);
                    expdata.sqlexecute.Parameters.AddWithValue("@aggregate", (byte)field.Aggregate);
                    expdata.sqlexecute.Parameters.AddWithValue("@order", order);
                    expdata.ExecuteProc("dbo.saveESRElementField");

                    expdata.sqlexecute.Parameters.Clear();
                    order++;
                }

                //Delete all subcats from the element before re adding
                this.deleteESRElementSubcats(ElementID);

                //Save all associated subcats
                foreach (int ID in element.Subcats)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate == true)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                    expdata.sqlexecute.Parameters.AddWithValue("@elementSubcatID", 0);
                    expdata.sqlexecute.Parameters.AddWithValue("@elementID", ElementID);
                    expdata.sqlexecute.Parameters.AddWithValue("@subcatID", ID);
                    expdata.ExecuteProc("dbo.saveESRElementSubcat");

                    expdata.sqlexecute.Parameters.Clear();
                }
            }

            ClearCache();

            return ElementID;
        }

        /// <summary>
        /// Delete all element subcats from the database associated to the element ID passed in
        /// </summary>
        /// <param name="ElementID">ID of the element</param>
        private void deleteESRElementSubcats(int ElementID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@elementID", ElementID);
                expdata.ExecuteProc("deleteESRElementSubcats");
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        private void deleteESRElementFields(int ElementID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@elementID", ElementID);
                expdata.ExecuteProc("deleteESRElementFields");
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Delete the element from the database. The associated element fields and subcats will cascade delete in the databa.se
        /// </summary>
        /// <param name="ElementID">Unique ID of the element</param>
        public void deleteESRElement(int ElementID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }
                expdata.sqlexecute.Parameters.AddWithValue("@elementID", ElementID);
                expdata.ExecuteProc("deleteESRElement");

                expdata.sqlexecute.Parameters.Clear();
            }
            ClearCache();
        }

        /// <summary>
        /// Get all the fields required for an ESR Inbound export for the report column dropdown
        /// </summary>
        /// <returns></returns>
        public List<ListItem> CreateReportColumnDropDown()
        {
            cFinancialExports clsexports = new cFinancialExports(AccountID);
            cFinancialExport export = clsexports.getESRExport(NHSTrustID);

            IReports clsreports = (IReports)Activator.GetObject(typeof(IReports), ConfigurationManager.AppSettings["ReportsServicePath"] + "/reports.rem");
            cReport rpt = clsreports.getReportById(AccountID, export.reportid);
            if (rpt == null)
            {
                return new List<ListItem>();
            }

            cStandardColumn standardcol;
            cStaticColumn staticcol;
            cCalculatedColumn calculatedcol;
            List<ListItem> ddlst = new List<ListItem>();
            foreach (cReportColumn column in rpt.columns)
            {
                switch (column.columntype)
                {
                    case ReportColumnType.Standard:
                        standardcol = (cStandardColumn)column;
                        if (standardcol.funcavg || standardcol.funccount || standardcol.funcmax || standardcol.funcmin || standardcol.funcsum)
                        {
                            if (standardcol.funcavg)
                            {
                                ddlst.Add(new ListItem("AVG of " + standardcol.field.Description, standardcol.reportcolumnid + "_" + (int)Aggregate.AVG));
                            }
                            if (standardcol.funccount)
                            {
                                ddlst.Add(new ListItem("COUNT of " + standardcol.field.Description, standardcol.reportcolumnid + "_" + (int)Aggregate.COUNT));
                            }
                            if (standardcol.funcmin)
                            {
                                ddlst.Add(new ListItem("MIN of " + standardcol.field.Description, standardcol.reportcolumnid + "_" + (int)Aggregate.MIN));
                            }
                            if (standardcol.funcmax)
                            {
                                ddlst.Add(new ListItem("MAX of " + standardcol.field.Description, standardcol.reportcolumnid + "_" + (int)Aggregate.MAX));
                            }
                            if (standardcol.funcsum)
                            {
                                ddlst.Add(new ListItem("SUM of " + standardcol.field.Description, standardcol.reportcolumnid + "_" + (int)Aggregate.SUM));
                            }
                        }
                        else
                        {
                            ddlst.Add(new ListItem(standardcol.field.Description, standardcol.reportcolumnid.ToString()));
                        }
                        break;
                    case ReportColumnType.Static:
                        staticcol = (cStaticColumn)column;
                        ddlst.Add(new ListItem(staticcol.literalname, staticcol.reportcolumnid.ToString()));
                        break;
                    case ReportColumnType.Calculated:
                        calculatedcol = (cCalculatedColumn)column;
                        ddlst.Add(new ListItem(calculatedcol.columnname, calculatedcol.reportcolumnid.ToString()));
                        break;
                }


            }

            return ddlst;
        }

        /// <summary>
        /// Get a list of the global elements and their fields from the database
        /// </summary>
        /// <returns></returns>
        private SortedList<int, cGlobalESRElement> CacheGlobalESRElements()
        {
            SortedList<int, cGlobalESRElement> lstGlobalESRElements;
            using (var expdata = new DatabaseConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString))
            {
                lstGlobalESRElements = new SortedList<int, cGlobalESRElement>();
                string GlobalESRElementName;
                int GlobalESRElementID;
                SortedList<string, object> parameters = new SortedList<string, object>();
                this.strsql = "SELECT globalESRElementID, ESRElementName FROM dbo.globalESRElements";

                using (IDataReader reader = expdata.GetReader(this.strsql))
                {
                    while (reader.Read())
                    {
                        GlobalESRElementID = reader.GetInt32(reader.GetOrdinal("globalESRElementID"));
                        GlobalESRElementName = reader.GetString(reader.GetOrdinal("ESRElementName"));

                        lstGlobalESRElements.Add(GlobalESRElementID,
                            new cGlobalESRElement(GlobalESRElementID, GlobalESRElementName,
                                this.GetGlobalESRElementFields(GlobalESRElementID)));
                    }
                    reader.Close();
                }
            }

            return lstGlobalESRElements;
        }

        /// <summary>
        /// Get a list of global element fields associated to the element from the database
        /// </summary>
        /// <param name="GlobalElementID">Unique ID of the element</param>
        /// <returns>A list of global element fields</returns>
        private List<cGlobalESRElementField> GetGlobalESRElementFields(int GlobalESRElementID)
        {
            List<cGlobalESRElementField> lstGlobalESRElementFields;
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(AccountID)))
            {
                lstGlobalESRElementFields = new List<cGlobalESRElementField>();
                string GlobalESRElementFieldName;
                int GlobalESRElementFieldID;
                bool isMandatory;
                bool controlColumn;

                this.strsql =
                    "SELECT globalESRElementFieldID, globalESRElementID, ESRElementFieldName, isMandatory, isControlColumn, isSummaryColumn, isRounded FROM globalESRElementFields WHERE globalESRElementID = @globalESRElementID";
                expdata.sqlexecute.Parameters.AddWithValue("@globalESRElementID", GlobalESRElementID);
                using (IDataReader reader = expdata.GetReader(this.strsql))
                {
                    expdata.sqlexecute.Parameters.Clear();
                    var elementFieldIdOrd = reader.GetOrdinal("globalESRElementFieldID");
                    var elementFieldNameOrd = reader.GetOrdinal("ESRElementFieldName");
                    var mandatoryOrd = reader.GetOrdinal("isMandatory");
                    var controlColumnOrd = reader.GetOrdinal("isControlColumn");
                    var summaryOrd = reader.GetOrdinal("isSummaryColumn");
                    var roundedOrd = reader.GetOrdinal("isRounded");
                    while (reader.Read())
                    {
                        GlobalESRElementFieldID = reader.GetInt32(elementFieldIdOrd);
                        GlobalESRElementFieldName = reader.GetString(elementFieldNameOrd);
                        isMandatory = reader.GetBoolean(mandatoryOrd);
                        controlColumn = reader.GetBoolean(controlColumnOrd);
                        var summary = reader.GetBoolean(summaryOrd);
                        var rounded = reader.GetBoolean(roundedOrd);

                        lstGlobalESRElementFields.Add(
                            new cGlobalESRElementField(
                                GlobalESRElementFieldID,
                                GlobalESRElementID,
                                GlobalESRElementFieldName,
                                isMandatory,
                                controlColumn,
                                summary,
                                rounded));
                    }
                    reader.Close();
                }

                return lstGlobalESRElementFields;
            }
        }


        /// <summary>
        /// Get the Global Elements for a dropdown
        /// </summary>
        /// <returns></returns>
        public List<ListItem> CreateGlobalElementDropDown()
        {
            List<ListItem> lstOptions = new List<ListItem>();
            ListItem tempOpt;
            foreach (KeyValuePair<int, cGlobalESRElement> kvp in listGlobals)
            {
                tempOpt = new ListItem();
                tempOpt.Value = kvp.Value.GlobalElementID.ToString();
                tempOpt.Text = kvp.Value.GlobalElementName;

                lstOptions.Add(tempOpt);
            }
            return lstOptions;
        }

        /// <summary>
        /// Return the global element for the ID
        /// </summary>
        /// <param name="ElementID">Global Element ID</param>
        /// <returns>Global Element from Cache</returns>
        public cGlobalESRElement GetGlobalESRElementByID(int ElementID)
        {
            cGlobalESRElement element = null;
            listGlobals.TryGetValue(ElementID, out element);
            return element;
        }

        public SortedList<int, cGlobalESRElement> lstGlobalElements()
        {
            return listGlobals;
        }

        public List<Panel> CreateESRElementFieldPanels(cGlobalESRElement GlobalElement, cESRElementMappings ElementMappings, cESRElement Element)
        {
            List<Panel> lstPanels = new List<Panel>();
            Panel pnlTwoColumn;

            Label lblESRFieldName;
            Label spanInput;
            HiddenField hfFieldID;
            TextBox tbFieldName;
            Label spanValidator;
            Label spanIcon;
            Label spanToolTip;

            Label lblReportColumns;
            Label spanInput2;
            DropDownList ddlReportColumns;
            Label spanValidator2;
            Label spanIcon2;
            Label spanToolTip2;

            Guid reportColumnID = Guid.Empty;
            Aggregate reportColumnAgg = Aggregate.None;


            foreach (cGlobalESRElementField field in GlobalElement.Fields)
            {
                ListItem[] lstReportColumns = ElementMappings.CreateReportColumnDropDown().ToArray();
                reportColumnID = Guid.Empty;
                reportColumnAgg = Aggregate.None;

                if (Element != null)
                {
                    foreach (cESRElementField val in Element.Fields)
                    {
                        if (field.globalElementFieldID == val.GlobalElementFieldID)
                        {
                            reportColumnID = val.ReportColumnID;
                            reportColumnAgg = val.Aggregate;
                            break;
                        }
                    }
                }

                pnlTwoColumn = new Panel();
                pnlTwoColumn.CssClass = "twocolumn";

                lblESRFieldName = new Label();
                spanInput = new Label();
                hfFieldID = new HiddenField();
                tbFieldName = new TextBox();
                spanValidator = new Label();
                spanIcon = new Label();
                spanToolTip = new Label();

                lblESRFieldName.ID = "field_" + field.globalElementFieldID.ToString();
                lblESRFieldName.Text = "ESR Field Name";
                if (field.Mandatory)
                {
                    lblESRFieldName.Text = lblESRFieldName.Text + " *";
                    lblESRFieldName.CssClass = "mandatory";
                }

                spanInput.CssClass = "inputs";
                hfFieldID.ID = "globalfieldid_" + field.globalElementFieldID.ToString();
                hfFieldID.Value = field.globalElementFieldID.ToString();
                tbFieldName.ID = "fieldname_" + field.globalElementFieldID.ToString();
                tbFieldName.CssClass = "fillspan";
                tbFieldName.ReadOnly = true;
                tbFieldName.Text = field.globalElementFieldName;
                spanInput.Controls.Add(hfFieldID);
                spanInput.Controls.Add(tbFieldName);
                lblESRFieldName.AssociatedControlID = tbFieldName.ID;

                spanValidator.CssClass = "inputvalidatorfield";
                spanValidator.Text = "&nbsp;";

                spanIcon.CssClass = "inputicon";
                spanIcon.Text = "&nbsp;";

                spanToolTip.CssClass = "inputtooltipfield";
                spanToolTip.Text = "&nbsp;";

                pnlTwoColumn.Controls.Add(lblESRFieldName);
                pnlTwoColumn.Controls.Add(spanInput);
                pnlTwoColumn.Controls.Add(spanValidator);
                pnlTwoColumn.Controls.Add(spanIcon);
                pnlTwoColumn.Controls.Add(spanToolTip);

                lblReportColumns = new Label();
                spanInput2 = new Label();
                ddlReportColumns = new DropDownList();
                spanValidator2 = new Label();
                spanIcon2 = new Label();
                spanToolTip2 = new Label();

                lblReportColumns.ID = "reportcolumn_" + field.globalElementFieldID.ToString();
                lblReportColumns.Text = "Report Column";

                spanInput2.CssClass = "inputs";
                ddlReportColumns.ID = "reportcolumn_option_" + field.globalElementFieldID.ToString();
                ddlReportColumns.CssClass = "inputs";
                ddlReportColumns.Items.Add(new ListItem("[None]", "0"));
                ddlReportColumns.Items.AddRange(lstReportColumns);
                if (reportColumnID != Guid.Empty)
                {
                    string sReportColumnID = (reportColumnAgg == Aggregate.None) ? reportColumnID.ToString() : reportColumnID.ToString() + "_" + ((int)reportColumnAgg).ToString();
                    if (ddlReportColumns.Items.FindByValue(sReportColumnID) != null)
                    {
                        int temp = ddlReportColumns.Items.IndexOf(ddlReportColumns.Items.FindByValue(sReportColumnID));
                        ddlReportColumns.SelectedIndex = temp;
                    }
                    else
                    {
                        ddlReportColumns.SelectedIndex = 0;
                    }
                }
                else
                {
                    ddlReportColumns.SelectedIndex = 0;
                }

                spanInput2.Controls.Add(ddlReportColumns);

                lblReportColumns.AssociatedControlID = ddlReportColumns.ID;

                spanValidator2.CssClass = "inputvalidatorfield";
                spanValidator2.Text = "&nbsp;";
                if (field.Mandatory)
                {
                    CompareValidator cv = new CompareValidator();
                    cv.ControlToValidate = ddlReportColumns.ID;
                    cv.ID = "compval_" + field.globalElementFieldID.ToString();
                    cv.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                    cv.Text = "*";
                    cv.ErrorMessage = "Please make sure you have mapped columns for your required fields";
                    cv.ValueToCompare = "0";
                    cv.Operator = ValidationCompareOperator.GreaterThan;
                    spanValidator2.Controls.Add(cv);
                }

                spanIcon2.CssClass = "inputicon";
                spanIcon2.Text = "&nbsp;";

                spanToolTip2.CssClass = "inputtooltipfield";
                spanToolTip2.Text = "&nbsp;";

                pnlTwoColumn.Controls.Add(lblReportColumns);
                pnlTwoColumn.Controls.Add(spanInput2);
                pnlTwoColumn.Controls.Add(spanValidator2);
                pnlTwoColumn.Controls.Add(spanIcon2);
                pnlTwoColumn.Controls.Add(spanToolTip2);

                lstPanels.Add(pnlTwoColumn);
            }
            return lstPanels;
        }
        /// <summary>
        /// Get the global element that matches the given string.
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public cGlobalESRElement GetGlobalESRElement(string elementName)
        {
            return this.listGlobals.Values.FirstOrDefault(globalEsrElement => globalEsrElement.GlobalElementName == elementName);
        }
    }
}
