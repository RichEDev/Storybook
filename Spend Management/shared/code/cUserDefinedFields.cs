namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.UI.WebControls;

    using AjaxControlToolkit;

    using SpendManagementLibrary;

    using Spend_Management.shared.code;

    public class cUserdefinedFields : cUserDefinedFieldsBase
    {
        private cFields clsFields;

        public cUserdefinedFields(int accountID)
            : base(accountID)
        {
            this.AccountID = accountID;

            this.MetabaseConnectionString = ConfigurationManager.ConnectionStrings["metabase"].ConnectionString;

            if (this.AccountID > 0)
            {
                long lastUpdatedAllServers = GetLastUpdatedFromCache(accountID);
                long lastReadFromDatabaseThisServer = 0;
                lastReadFromDatabaseTicks.TryGetValue(this.AccountID, out lastReadFromDatabaseThisServer);
                bool forceRefreshFromDatabase = lastUpdatedAllServers > lastReadFromDatabaseThisServer;
                if (forceRefreshFromDatabase)
                {
                    SortedList<int, cUserDefinedField> oldValue;
                    AllUserDefinedFields.TryRemove(accountID, out oldValue);
                }
            }
            this.clsFields = new cFields(this.AccountID);

            InitialiseData();
        }

        private void InitialiseData()
        {
            this.UserdefinedFields = AllUserDefinedFields.GetOrAdd(this.AccountID, CacheList);
        }

        private SortedList<int, cUserDefinedField> CacheList(int accountId)
        {
            if (accountId != this.AccountID) throw new ArgumentException("Incorrect account ID", "accountId");
            cUserdefinedFieldGroupings clsGroupings = new cUserdefinedFieldGroupings(accountId);
            cTables clsTables = new cTables(accountId);
            return GetCollection(clsTables, clsGroupings);
        }

        private void ResetCache(DateTime lastUpdated)
        {
            SaveLastUpdatedToCache(this.AccountID, lastUpdated);
            InitialiseData();
        }

        /// <summary>
        /// Adds the user defined field panel to a page. Redundant in spend management, use createFieldPanel instead
        /// </summary>
        /// <param name="tbl"></param>
        /// <param name="table"></param>
        /// <param name="itemspecific"></param>
        /// <param name="subcatid"></param>
        /// <param name="values"></param>
        /// <param name="udfs"></param>
        public void addItemsToPage(ref Table tbl, cTable table, bool itemspecific, string subcatid, SortedList<int, object> values, List<int> udfs, string validationGroup)
        {
            bool debugMode = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["debug"]) ? false : true;

            StringBuilder sbDebug = new StringBuilder();


            /*
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             */
            if (debugMode == true)
            {
                sbDebug.AppendLine("cUserdefinedFields.addItemsToPage(tbl, table, itemspecific, subcatid, values, udfs, validationGroup) called.");
                sbDebug.AppendLine("=============================================================");
                sbDebug.AppendLine("PARAMETERS");
                sbDebug.AppendLine("=============================================================");

                if (tbl == null)
                {
                    sbDebug.AppendLine("tbl: NULL");
                }
                else
                {
                    sbDebug.AppendLine("tbl: " + tbl.ID);
                }

                if (table == null)
                {
                    sbDebug.AppendLine("table: NULL");
                }
                else
                {
                    sbDebug.AppendLine("table: " + table.TableName);
                }

                sbDebug.AppendLine("subcatid: " + subcatid);

                if (values == null)
                {
                    sbDebug.AppendLine("values: NULL");
                }
                else if (values.Count == 0)
                {
                    sbDebug.AppendLine("values: count = 0");
                }
                else
                {
                    foreach (KeyValuePair<int, object> kvp in values)
                    {
                        sbDebug.AppendLine("UDF ID: " + kvp.Key + " - UDF Value: " + kvp.Value);
                    }
                }

                if (udfs == null)
                {
                    sbDebug.AppendLine("udfs: NULL");
                }
                else if (udfs.Count == 0)
                {
                    sbDebug.AppendLine("udfs: count = 0");
                }
                else
                {
                    foreach (int udfID in udfs)
                    {
                        sbDebug.AppendLine("UDF ID: " + udfID);
                    }
                }

                sbDebug.AppendLine("validationGroup: " + validationGroup);
                sbDebug.AppendLine("=============================================================");
                sbDebug.AppendLine("END PARAMETERS");
                sbDebug.AppendLine("=============================================================");
            }

            TableRow row;
            TableCell cell;
            TextBox txtbox;
            cTextAttribute txtatt;
            AjaxControlToolkit.HtmlEditorExtender txtrichtext;
            RequiredFieldValidator reqval;
            CompareValidator compval;
            CalendarExtender cal;
            MaskedEditExtender maskededit;
            CustomValidator custval;
            RegularExpressionValidator regexval;
            DropDownList ddlst;
            object value;
            string valid = "";
            string controlID = "";

            if (udfs == null)
            {
                udfs = new List<int>();
            }

            string id;
            SortedList<int, cUserDefinedField> sortedUFields = new SortedList<int, cUserDefinedField>();
            List<cUserDefinedField> duplicates = new List<cUserDefinedField>();

            foreach (cUserDefinedField field in this.UserdefinedFields.Values)
            {
                /*
                 *  THIS IS JUST DEBUG INFO
                 *  THIS IS JUST DEBUG INFO
                 *  THIS IS JUST DEBUG INFO
                 *  THIS IS JUST DEBUG INFO
                 *  THIS IS JUST DEBUG INFO
                 *  THIS IS JUST DEBUG INFO
                 *  THIS IS JUST DEBUG INFO
                 *  THIS IS JUST DEBUG INFO
                 *  THIS IS JUST DEBUG INFO
                 *  THIS IS JUST DEBUG INFO
                 *  THIS IS JUST DEBUG INFO
                 */
                if (debugMode == true)
                {

                    sbDebug.AppendLine(field.table + "==" + table + "&& (" + table.TableName + "!= \"userdefinedExpenses\" || (" + table.TableName + " == \"userdefinedExpenses\" && " + field.Specific + " == false && " + itemspecific + " == false) || (" + table.TableName + " == \"userdefinedExpenses\" && " + field.Specific + " == true && " + udfs.Contains(field.userdefineid) + ")");
                    sbDebug.AppendLine("Additional info:");
                    sbDebug.AppendLine("field.table.TableID: " + field.table.TableID);
                    sbDebug.AppendLine("table.TableID: " + table.TableID);
                }


                if (field.table.TableID == table.TableID && (table.TableName != "userdefinedExpenses" || (table.TableName == "userdefinedExpenses" && field.Specific == false && itemspecific == false) || (table.TableName == "userdefinedExpenses" && field.Specific == true && udfs.Contains(field.userdefineid))))
                {
                    if (!sortedUFields.ContainsKey(field.order))
                    {
                        sortedUFields.Add(field.order, field);
                    }
                    else
                    {
                        duplicates.Add(field);
                    }
                }
            }

            /*
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             *  THIS IS JUST DEBUG INFO
             */
            if (debugMode == true)
            {
                sbDebug.AppendLine("sortedUFields:");
                foreach (KeyValuePair<int, cUserDefinedField> kvp in sortedUFields)
                {
                    sbDebug.AppendLine("UDF Order: " + kvp.Key + " - UDF Label: " + kvp.Value.label);
                }

                sbDebug.AppendLine("duplicates:");
                foreach (cUserDefinedField udf in duplicates)
                {
                    sbDebug.AppendLine("UDF Order: " + udf.order + " - UDF Label: " + udf.label);
                }

                cEventlog.LogEntry(sbDebug.ToString(), true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);
            }

            if (duplicates.Count > 0)
            {
                // just append duplicates on the end of the list
                int orderIdx = 1000;
                foreach (cUserDefinedField field in duplicates)
                {
                    sortedUFields.Add(orderIdx++, field);
                }
            }

            foreach (KeyValuePair<int, cUserDefinedField> kvp in sortedUFields)
            {
                if (debugMode == true)
                {
                    cEventlog.LogEntry("Starting generation of " + kvp.Value.label + " field", false, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);
                }
                cUserDefinedField field = (cUserDefinedField)kvp.Value;

                id = "";
                valid = "";
                row = new TableRow();
                cell = new TableCell();
                cell.CssClass = "labeltd";
                cell.Text = field.label;
                if (field.label.Substring(field.label.Length - 1, 1) != ":")
                {
                    cell.Text += ":";
                }

                if (field.attribute.mandatory)
                {
                    cell.Text += "*";
                }

                row.Cells.Add(cell);
                cell = new TableCell();
                cell.CssClass = "inputtd";
                switch (field.fieldtype)
                {
                    case FieldType.Text:
                    case FieldType.Number:
                    case FieldType.Integer:
                    case FieldType.Relationship:
                    case FieldType.DynamicHyperlink:
                        txtbox = new TextBox();
                        id = "txtudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += "_" + subcatid;
                        }
                        txtbox.ID = id;

                        if (field.attribute.GetType() == typeof(cTextAttribute))
                        {
                            txtatt = (cTextAttribute)field.attribute;
                            if (txtatt.maxlength.HasValue == true && txtatt.maxlength.Value > 0)
                            {
                                txtbox.MaxLength = txtatt.maxlength.Value;
                            }

                            if (txtatt.format == AttributeFormat.MultiLine)
                            {
                                txtbox.TextMode = TextBoxMode.MultiLine;
                            }
                        }

                        if (values != null)
                        {
                            values.TryGetValue(field.userdefineid, out value);
                            if (value != null)
                            {
                                txtbox.Text = value.ToString();
                            }
                        }

                        cell.Controls.Add(txtbox);
                        controlID = txtbox.ClientID;

                        break;
                    case FieldType.Currency:
                        txtbox = new TextBox();
                        id = "txtudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += "_" + subcatid;
                        }
                        txtbox.ID = id;

                        if (values != null)
                        {
                            values.TryGetValue(field.userdefineid, out value);
                            if (value != null)
                            {
                                if (value.ToString() != "")
                                {
                                    txtbox.Text = ((decimal)value).ToString("########0.00");
                                }
                            }
                        }
                        cell.Controls.Add(txtbox);
                        break;
                    case FieldType.LargeText:
                        txtatt = (cTextAttribute)field.attribute;
                        if (txtatt.format == AttributeFormat.MultiLine)
                        {
                            txtbox = new TextBox();
                            id = "txtudf" + field.userdefineid;
                            if (itemspecific)
                            {
                                id += "_" + subcatid;
                            }

                            txtbox.TextMode = TextBoxMode.MultiLine;
                            txtbox.ID = id;

                            txtbox.ValidationGroup = validationGroup;

                            if (values != null)
                            {
                                values.TryGetValue(field.userdefineid, out value);
                                if (value != null)
                                {
                                    if (value.ToString() != "")
                                    {
                                        txtbox.Text = value.ToString();
                                    }
                                }
                            }

                            cell.Controls.Add(txtbox);
                        }
                        else if (txtatt.format == AttributeFormat.FormattedText)
                        {

                            txtrichtext = new AjaxControlToolkit.HtmlEditorExtender();
                            txtrichtext.ID = "editor" + field.userdefineid;
                            txtrichtext.EnableSanitization = false;
                            id = "txt" + field.userdefineid;

                            if (itemspecific)
                            {
                                id += "_" + subcatid;
                            }

                            TextBox txtContent = new TextBox();
                            txtContent.ID = id;
                            txtContent.Height = Unit.Pixel(260);
                            txtContent.Width = Unit.Pixel(579);
                            txtrichtext.TargetControlID = txtContent.ClientID;

                            if (values != null)
                            {
                                values.TryGetValue(field.userdefineid, out value);
                                if (value != null)
                                {
                                    txtContent.Text = value.ToString();
                                }
                            }
                            cell.Height = Unit.Pixel(290);
                            cell.Controls.Add(txtContent);
                            cell.Controls.Add(txtrichtext);
                        }

                        break;
                    case FieldType.List:
                        ddlst = new DropDownList();
                        id = "cmbudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += "_" + subcatid;
                        }
                        ddlst.ID = id;
                        ddlst.Items.Add(new ListItem("[None]", "0"));

                        List<cListAttributeElement> lstLI = new List<cListAttributeElement>();
                        foreach (KeyValuePair<int, cListAttributeElement> listitem in field.items)
                        {
                            cListAttributeElement listelement = (cListAttributeElement)listitem.Value;
                            
                            if (!listelement.Archived)
                            {  
                                lstLI.Add(listelement);                               
                            }  

                        }

                        // sort the list by element order
                        lstLI.Sort(delegate(cListAttributeElement liAttEle1, cListAttributeElement liAttEle2) { return liAttEle1.elementOrder.CompareTo(liAttEle2.elementOrder); });

                        foreach (cListAttributeElement lae in lstLI)
                        {
                            ddlst.Items.Add(new ListItem(lae.elementText, lae.elementValue.ToString()));
                        }

                        if (values != null)
                        {
                            values.TryGetValue(field.userdefineid, out value);
                            if (value != null)
                            {
                                if (ddlst.Items.FindByValue(value.ToString()) != null)
                                {
                                    ddlst.Items.FindByValue(value.ToString()).Selected = true;
                                }
                            }
                        }

                        //cFilterRules clsfilterrules = new cFilterRules(accountid);

                        //if (field.table.tablename == "savedexpenses" && field.itemspecific == false)
                        //{
                        //    clsfilterrules.filterDropdown(ref ddlst, FilterType.Userdefined, "");
                        //}
                        //else if (field.table.tablename == "savedexpenses" && field.itemspecific)
                        //{
                        //    clsfilterrules.filterDropdown(ref ddlst, FilterType.Userdefined, subcatid);
                        //}

                        cell.Controls.Add(ddlst);
                        break;
                    case FieldType.DateTime:
                        txtbox = new TextBox();
                        id = "txtudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += "_" + subcatid;
                        }
                        txtbox.ID = id;

                        DateTime date = new DateTime();
                        bool validDate = false;

                        if (values != null)
                        {
                            values.TryGetValue(field.userdefineid, out value);
                            if (value != null)
                            {
                                validDate = DateTime.TryParse(value.ToString(), out date);
                            }
                        }

                        if (validDate)
                        {
                            cDateTimeAttribute dateatt = (cDateTimeAttribute)field.attribute;
                            switch (dateatt.format)
                            {
                                case AttributeFormat.DateOnly:
                                    txtbox.Text = date.ToShortDateString();
                                    break;
                                case AttributeFormat.TimeOnly:
                                    txtbox.Text = date.ToShortTimeString();
                                    break;
                                case AttributeFormat.DateTime:
                                    txtbox.Text = date.ToShortDateString() + " " + date.ToShortTimeString();
                                    break;
                            }
                        }

                        cell.Controls.Add(txtbox);
                        break;
                    case FieldType.TickBox:
                        ddlst = new DropDownList();
                        id = "cmbudf" + field.userdefineid;

                        if (itemspecific)
                        {
                            id += "_" + subcatid;
                        }
                        ddlst.ID = id;
                        ddlst.Items.Add(new ListItem("Yes", "1"));
                        ddlst.Items.Add(new ListItem("No", "0"));
                        if (values != null && values.Count != 0)
                        {
                            values.TryGetValue(field.userdefineid, out value);

                            if (value != null)
                            {
                                string val;

                                if (value.ToString().ToLower() == "false" || value.ToString().ToLower() == "0")
                                {
                                    val = "0";
                                }
                                else
                                {
                                    val = "1";
                                }

                                if (ddlst.Items.FindByValue(val) != null)
                                {
                                    ddlst.Items.FindByValue(val).Selected = true;
                                }
                            }
                        }
                        else if (((cTickboxAttribute)field.attribute).defaultvalue != null)
                        {
                            if (ddlst.Items.FindByText(((cTickboxAttribute)field.attribute).defaultvalue) != null)
                            {
                                ddlst.Items.FindByText(((cTickboxAttribute)field.attribute).defaultvalue).Selected = true;
                            }

                        }
                        cell.Controls.Add(ddlst);
                        break;
                    case FieldType.Hyperlink:

                        HyperLink lnk = new HyperLink();
                        id = "lnk" + field.attribute.attributeid;
                        if (itemspecific)
                        {
                            id += "_" + subcatid;
                        }
                        lnk.ID = id;
                        cHyperlinkAttribute hyperlinkAtt = (cHyperlinkAttribute)field.attribute;
                        lnk.NavigateUrl = hyperlinkAtt.hyperlinkPath;
                        lnk.Target = "_blank";
                        lnk.Text = hyperlinkAtt.hyperlinkText;
                        cell.Controls.Add(lnk);
                        break;
                }
                row.Cells.Add(cell);

                cell = new TableCell();

                if (field.attribute.fieldtype == FieldType.DynamicHyperlink)
                {
                    // output the image link that will open whatever link is in the textbox
                    Image img = new Image();
                    img.ImageUrl = "~/shared/images/icons/16/Plain/earth.png";
                    img.ID = "img" + field.attribute.attributeid;
                    img.Attributes.Add("onclick", "launchURL(contentID + '" + controlID + "');");
                    img.CssClass = "btn";
                    cell.Controls.Add(img);
                }

                if (field.mandatory && (field.fieldtype != FieldType.TickBox && field.fieldtype != FieldType.Hyperlink))
                {
                    if (field.mandatory)
                    {

                        if (itemspecific)
                        {
                            custval = new CustomValidator();

                            string uniqueKey = Guid.NewGuid().ToString().Replace('-', '_');
                            custval.ID = "custmand" + uniqueKey + subcatid;
                            custval.ControlToValidate = id;
                            custval.ClientValidationFunction = "checkMandatory";
                            custval.ErrorMessage = field.label + " is a mandatory field. Please enter a value in the box provided.";
                            custval.Text = "*";
                            custval.ValidationGroup = validationGroup;
                            custval.ValidateEmptyText = true;
                            cell.Controls.Add(custval);

                        }
                        else
                        {
                            if (field.fieldtype == FieldType.List)
                            {
                                compval = new CompareValidator();
                                compval.ID = "req" + field.userdefineid;
                                compval.ControlToValidate = id;
                                compval.Operator = ValidationCompareOperator.GreaterThan;
                                compval.ValueToCompare = "0";
                                compval.Text = "*";
                                compval.ErrorMessage = field.attribute.displayname + " is a mandatory field. Please select a value from the list provided";

                                compval.ValidationGroup = validationGroup;

                                cell.Controls.Add(compval);
                            }
                            else
                            {
                                reqval = new RequiredFieldValidator();
                                valid = "requdf" + field.userdefineid;
                                reqval.ID = valid;

                                reqval.ControlToValidate = id;
                                reqval.ValidationGroup = validationGroup;
                                reqval.ErrorMessage = field.label + " is a mandatory field. Please enter a value in the box provided.";
                                reqval.Text = "*";
                                cell.Controls.Add(reqval);
                            }
                        }
                    }
                }
                switch (field.fieldtype)
                {
                    case FieldType.Currency:
                        compval = new CompareValidator();
                        valid = "compudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            valid += "_" + subcatid;
                        }
                        compval.ID = valid;
                        compval.ControlToValidate = id;
                        compval.Type = ValidationDataType.Currency;
                        compval.ErrorMessage = "The value you have entered for " + field.label + " is invalid. Valid characters are the numbers 0-9 and a full stop (.)";
                        compval.Text = "*";
                        compval.ValidationGroup = validationGroup;
                        compval.Operator = ValidationCompareOperator.DataTypeCheck;
                        cell.Controls.Add(compval);
                        break;
                    case FieldType.Number:
                        compval = new CompareValidator();
                        valid = "compudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            valid += "_" + subcatid;
                        }
                        compval.ID = valid;
                        compval.ControlToValidate = id;
                        compval.Type = ValidationDataType.Double;
                        compval.ErrorMessage = "The value you have entered for " + field.label + " is invalid. Valid characters are the numbers 0-9 and a full stop (.)";
                        compval.Text = "*";
                        compval.ValidationGroup = validationGroup;
                        compval.Operator = ValidationCompareOperator.DataTypeCheck;
                        cell.Controls.Add(compval);
                        break;
                    case FieldType.Integer:
                        compval = new CompareValidator();
                        valid = "compudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            valid += "_" + subcatid;
                        }
                        compval.ID = valid;
                        compval.ControlToValidate = id;
                        compval.Type = ValidationDataType.Integer;
                        compval.ErrorMessage = "The value you have entered for " + field.label + " is invalid. Valid characters are the numbers 0-9";
                        compval.Text = "*";
                        compval.Operator = ValidationCompareOperator.DataTypeCheck;

                        compval.ValidationGroup = validationGroup;

                        cell.Controls.Add(compval);
                        break;

                    case FieldType.DateTime:
                        switch (((cDateTimeAttribute)field.attribute).format)
                        {
                            case AttributeFormat.DateOnly:
                                Image img = new Image();
                                img.ImageUrl = "~/shared/images/icons/cal.gif";
                                img.ID = "img" + field.userdefineid;

                                if (itemspecific == true)
                                {
                                    img.ID += "_" + subcatid;
                                }

                                cell.Controls.Add(img);

                                maskededit = new MaskedEditExtender();
                                maskededit.TargetControlID = id;
                                maskededit.Mask = "99/99/9999";
                                maskededit.MaskType = MaskedEditType.Date;
                                maskededit.ID = "mskstartdate" + field.userdefineid;

                                if (itemspecific)
                                {
                                    maskededit.ID += "_" + subcatid;
                                }

                                cal = new CalendarExtender();
                                cal.TargetControlID = id;
                                cal.Format = "dd/MM/yyyy";
                                cal.PopupButtonID = "img" + field.userdefineid;

                                if (itemspecific)
                                {
                                    cal.PopupButtonID += "_" + subcatid;
                                }

                                cal.ID = "cal" + field.userdefineid;

                                if (itemspecific)
                                {
                                    cal.ID += "_" + subcatid;
                                }

                                cell.Controls.Add(cal);
                                cell.Controls.Add(maskededit);

                                compval = new CompareValidator();
                                valid = "compmax" + field.userdefineid;
                                if (itemspecific)
                                {
                                    valid += "_" + subcatid;
                                }
                                compval.ID = valid;
                                compval.ControlToValidate = id;
                                compval.ValidationGroup = validationGroup;
                                compval.Type = ValidationDataType.Date;
                                compval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid. Date must be earlier than 31/12/3000";
                                compval.Text = "*";
                                compval.Operator = ValidationCompareOperator.LessThan;
                                compval.ValueToCompare = "31/12/3000";

                                cell.Controls.Add(compval);

                                compval = new CompareValidator();
                                valid = "compmin" + field.userdefineid;
                                if (itemspecific)
                                {
                                    valid += "_" + subcatid;
                                }
                                compval.ID = valid;
                                compval.ControlToValidate = id;
                                compval.ValidationGroup = validationGroup;
                                compval.Type = ValidationDataType.Date;
                                compval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid. Date must be later than 01/01/1900";
                                compval.Text = "*";
                                compval.Operator = ValidationCompareOperator.GreaterThanEqual;
                                compval.ValueToCompare = "01/01/1900";

                                cell.Controls.Add(compval);

                                compval = new CompareValidator();
                                valid = "compudf" + field.userdefineid;
                                if (itemspecific)
                                {
                                    valid += "_" + subcatid;
                                }
                                compval.ID = valid;
                                compval.ControlToValidate = id;
                                compval.ValidationGroup = validationGroup;
                                compval.Type = ValidationDataType.Date;
                                compval.ErrorMessage = "The value you have entered for " + field.label + "is invalid. Valid characters are the numbers 0-9 and a forward slash (/)";
                                compval.Text = "*";
                                compval.Operator = ValidationCompareOperator.DataTypeCheck;
                                cell.Controls.Add(compval);
                                break;
                            case AttributeFormat.TimeOnly:
                                maskededit = new MaskedEditExtender();
                                maskededit.TargetControlID = id;
                                maskededit.Mask = "99:99";
                                maskededit.UserTimeFormat = MaskedEditUserTimeFormat.TwentyFourHour;
                                maskededit.MaskType = MaskedEditType.Time;
                                maskededit.ID = "msk" + field.attribute.attributeid;

                                if (itemspecific)
                                {
                                    maskededit.ID += "_" + subcatid;
                                }

                                cell.Controls.Add(maskededit);

                                regexval = new RegularExpressionValidator();
                                valid = "regextime" + field.attribute.attributeid;
                                if (itemspecific)
                                {
                                    valid += "_" + subcatid;
                                }
                                regexval.ID = valid;
                                regexval.ControlToValidate = id;
                                regexval.ValidationExpression = @"([0-1]\d|2[0-3]):([0-5]\d)";
                                regexval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid. Valid time format is 00:00 to 23:59";
                                regexval.Text = "*";
                                regexval.ValidationGroup = validationGroup;
                                cell.Controls.Add(regexval);
                                break;
                            case AttributeFormat.DateTime:
                                maskededit = new MaskedEditExtender();
                                maskededit.TargetControlID = id;
                                maskededit.Mask = "99/99/9999 99:99";
                                maskededit.UserDateFormat = MaskedEditUserDateFormat.DayMonthYear;
                                maskededit.MaskType = MaskedEditType.DateTime;
                                maskededit.ID = "msk" + field.attribute.attributeid;

                                if (itemspecific)
                                {
                                    maskededit.ID += "_" + subcatid;
                                }

                                cell.Controls.Add(maskededit);

                                regexval = new RegularExpressionValidator();
                                valid = "regexdatetime" + field.attribute.attributeid;
                                if (itemspecific)
                                {
                                    valid += "_" + subcatid;
                                }
                                regexval.ID = valid;
                                regexval.ControlToValidate = id;
                                regexval.ValidationExpression = "^(?=\\d)(?:(?:31(?!.(?:0?[2469]|11))|(?:30|29)(?!.0?2)|29(?=.0?2.(?:(?:(?:1[6-9]|[2-9]\\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(?:\x20|$))|(?:2[0-8]|1\\d|0?[1-9]))([-./])(?:1[012]|0?[1-9])\\1(?:1[6-9]|[2-9]\\d)?\\d\\d(?:(?=\x20\\d)\x20|$))?(((0?[1-9]|1[012])(:[0-5]\\d){0,2}(\x20[AP]M))|([01]\\d|2[0-3])(:[0-5]\\d){1,2})?$";
                                regexval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid.";
                                regexval.Text = "*";
                                regexval.ValidationGroup = validationGroup;
                                cell.Controls.Add(regexval);
                                break;
                        }
                        break;
                    case FieldType.DynamicHyperlink:
                        regexval = new RegularExpressionValidator();
                        valid = "regexlink" + field.attribute.attributeid;
                        if (itemspecific)
                        {
                            valid += "_" + subcatid;
                        }
                        regexval.ValidationExpression = @"^(https?|ftps?)://(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.*|^mailto\:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
                        regexval.ID = valid;
                        regexval.ControlToValidate = id;
                        regexval.Text = "*";
                        regexval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid.";
                        regexval.ValidationGroup = validationGroup;
                        cell.Controls.Add(regexval);
                        break;
                }

                row.Cells.Add(cell);
                if (field.tooltip != "")
                {
                    Literal lit = new Literal();
                    StringBuilder litTooltip = new StringBuilder();
                    litTooltip.AppendFormat("<td class=\"inputtd udf_tooltips\"><img id=\"imgtooltipudf{0}\" onclick=\"SEL.Tooltip.UDTooltip(this,{0}, {1});\" src=\"../static/icons/16/new-icons/tooltip.png\" alt=\"\" class=\"tooltipicon\"/></td>", field.userdefineid, AccountID);
                    lit.Text = litTooltip.ToString();

                    cell.Controls.Add(lit);
                    row.Cells.Add(cell);
                }
                tbl.Rows.Add(row);

                if (debugMode == true)
                {
                    cEventlog.LogEntry("Finished generation of " + kvp.Value.label + " field", true, System.Diagnostics.EventLogEntryType.Information, cEventlog.ErrorCode.DebugInformation);
                }
            }


        }

        /// <summary>
        /// Gets the values from the user defined field panel for saving. Redundant in the new release, use getItemsFromPanel instead
        /// </summary>
        /// <param name="tbl"></param>
        /// <param name="table"></param>
        /// <param name="itemspecific"></param>
        /// <param name="subcatid"></param>
        /// <returns></returns>
        public SortedList<int, object> getItemsFromPage(ref Table tbl, cTable table, bool itemspecific, string subcatid)
        {
            var values = new SortedList<int, object>();
            TextBox txtbox;
            cTextAttribute textatt;
            DropDownList ddlst;

            if (tbl == null)
            {
                return values;
            }

            string id;
            foreach (cUserDefinedField field in this.UserdefinedFields.Values)
            {
                
                id = "";

                switch (field.fieldtype)
                {
                    case FieldType.DynamicHyperlink:
                    case FieldType.Text:
                        id = "txtudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += "_" + subcatid;
                        }
                        txtbox = (TextBox)tbl.FindControl(id);
                        if (txtbox != null)
                        {
                            values.Add(field.userdefineid, txtbox.Text);
                        }
                        break;
                    case FieldType.Currency:
                    case FieldType.Number:
                        id = "txtudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += "_" + subcatid;
                        }
                        txtbox = (TextBox)tbl.FindControl(id);

                        if (txtbox != null)
                        {
                            if (txtbox.Text != "")
                            {
                                values.Add(field.userdefineid, Convert.ToDecimal(txtbox.Text));
                            }
                        }
                        break;
                    case FieldType.Integer:
                        id = "txtudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += "_" + subcatid;
                        }
                        txtbox = (TextBox)tbl.FindControl(id);
                        if (txtbox != null)
                        {
                            if (txtbox.Text != "")
                            {
                                values.Add(field.userdefineid, Convert.ToInt32(txtbox.Text));
                            }
                        }
                        break;
                    case FieldType.DateTime:
                        id = "txtudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += "_" + subcatid;
                        }
                        txtbox = (TextBox)tbl.FindControl(id);

                        if (txtbox != null)
                        {
                            if (txtbox.Text != "")
                            {
                                values.Add(field.userdefineid, DateTime.Parse(txtbox.Text));
                            }
                        }
                        break;

                    case FieldType.LargeText:
                        textatt = (cTextAttribute)field.attribute;
                        if (textatt.format == AttributeFormat.FormattedText)
                        {
                            id = "txt" + field.userdefineid;
                            if (itemspecific)
                            {
                                id += "_" + subcatid;
                            }

                            var textbox = (TextBox)tbl.FindControl(id);

                            if (textbox != null)
                            {
                                if (textbox.Text != "")
                                {
                                    string htmlDecode = WebUtility.HtmlDecode(textbox.Text);
                                    values.Add(field.userdefineid, htmlDecode);
                                }
                            }
                            //txtrichtxt = (RichTextEditor)tab.FindControl("txt" + field.attribute.attributeid);
                            //if (txtrichtxt.Text != "")
                            //{
                            //    clsquery.addColumn(clsfields.getFieldById(field.attribute.fieldid), txtrichtxt.Text);
                            //}
                        }
                        else
                        {
                            id = "txtudf" + field.userdefineid;
                            if (itemspecific)
                            {
                                id += "_" + subcatid;
                            }
                            txtbox = (TextBox)tbl.FindControl(id);
                            if (txtbox != null)
                            {
                                if (txtbox.Text != "")
                                {
                                    values.Add(field.userdefineid, txtbox.Text);
                                }
                            }
                        }
                        break;
                    case FieldType.List:
                    case FieldType.TickBox:
                        id = "cmbudf" + field.userdefineid;
                        if (itemspecific)
                        {
                            id += "_" + subcatid;
                        }
                        ddlst = (DropDownList)tbl.FindControl(id);
                        if (ddlst != null)
                        {
                            values.Add(field.userdefineid, ddlst.SelectedValue);
                        }
                        break;
                }

                //}
            }

            return values;
        }

        /// <summary>
        /// Updates the user defined field panel with the values from the supplied record
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="record"></param>
        /// <param name="form"></param>
        public void populateRecordDetails(ref PlaceHolder holder, cTable tbl, SortedList<int, object> record, GroupingOutputType groupType = GroupingOutputType.All, bool excludeAdminFields = false)
        {
            List<cUserDefinedField> fields = GetFieldsByTable(tbl);

            cTextAttribute textatt;
            TextBox txtbox;
            DropDownList cmbbox;
            cDateTimeAttribute dateatt;
            DateTime date;
            decimal currency;
            //RichTextEditor txtrichtext;
            AjaxControlToolkit.HTMLEditor.Editor txtrichtext;
            //relationshipTextbox relTxt;

            foreach (cUserDefinedField field in fields)
            {
                if (excludeAdminFields && field.table.TableName.ToLower() == "userdefinedcars" && !field.AllowEmployeeToPopulate)
                {
                    continue;
                }

                if (groupType == GroupingOutputType.All || (groupType == GroupingOutputType.GroupedOnly && field.Grouping != null) || (groupType == GroupingOutputType.UnGroupedOnly && field.Grouping == null))
                {
                    switch (field.attribute.fieldtype)
                    {
                        case FieldType.Relationship:
                            txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid.ToString());
                            TextBox txtID = (TextBox)holder.FindControl("txt" + field.attribute.attributeid.ToString() + "_ID");
                            if (txtbox != null && txtID != null)
                            {
                                if (record.ContainsKey(field.attribute.attributeid) && record[field.attribute.attributeid] != DBNull.Value)
                                {
                                  string txtvalue = GetOtmActualText(clsFields, field, record[field.attribute.attributeid]);

                                    if (txtvalue != string.Empty)
                                    {
                                        //relTxt.Panel.Style.Add(HtmlTextWriterStyle.Display, "");
                                        //Image img = new Image();

                                        //img.ImageUrl = cMisc.path + "/shared/images/icons/edit.png";
                                        //img.ID = "img_" + relTxt.ID;
                                        //img.Style.Add("align", "absmiddle");

                                        //HyperLink hypLink = new HyperLink();
                                        //hypLink.ID = "lnk_" + relTxt.ID;
                                        //hypLink.Text = txtvalue;
                                        //hypLink.NavigateUrl = "javascript:void(0);";

                                        //relTxt.Panel.Controls.Add(img);
                                        //Literal lit = new Literal();
                                        //lit.Text = " ";
                                        //relTxt.Panel.Controls.Add(lit);
                                        //relTxt.Panel.Controls.Add(hypLink);
                                        txtID.Text = record[field.attribute.attributeid].ToString();
                                        txtbox.Text = txtvalue;
                                    }
                                }
                                else
                                {
                                    txtID.Text = "0";
                                    txtbox.Text = "";
                                }
                            }
                            break;
                        case FieldType.DynamicHyperlink:
                        case FieldType.Text:
                            txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                            if (txtbox != null)
                            {
                                if (record.ContainsKey(field.attribute.attributeid))
                                {
                                    if (record[field.attribute.attributeid] != DBNull.Value)
                                    {
                                        txtbox.Text = (string)record[field.attribute.attributeid];
                                    }
                                }
                                else
                                {
                                    txtbox.Text = "";
                                }
                            }
                            break;
                        case FieldType.LargeText:
                            textatt = (cTextAttribute)field.attribute;
                            if (textatt.format == AttributeFormat.FormattedText)
                            {

                                var textbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                if (textbox != null)
                                {
                                    if (record.ContainsKey(field.attribute.attributeid))
                                    {
                                        if (record[field.attribute.attributeid] != DBNull.Value)
                                        {
                                            textbox.Text = (string)record[field.attribute.attributeid];
                                        }
                                    }
                                    else
                                    {
                                        textbox.Text = string.Empty;
                                    }
                                }
                                //txtrichtext = (RichTextEditor)tab.FindControl("txt" + field.attribute.attributeid);
                                //if (txtrichtext != null)
                                //{
                                //    if (record.ContainsKey(field.attribute.attributeid))
                                //    {
                                //        if (record[field.attribute.attributeid] != DBNull.Value)
                                //        {
                                //            txtrichtext.Text = (string)record[field.attribute.attributeid];
                                //        }
                                //    }
                                //}
                            }
                            else
                            {
                                txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                if (txtbox != null)
                                {
                                    if (record.ContainsKey(field.attribute.attributeid))
                                    {
                                        if (record[field.attribute.attributeid] != DBNull.Value)
                                        {
                                            txtbox.Text = (string)record[field.attribute.attributeid];
                                        }
                                    }
                                    else
                                    {
                                        txtbox.Text = "";
                                    }
                                }

                            }
                            break;
                        case FieldType.Currency:
                            txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                            if (txtbox != null)
                            {
                                if (record.ContainsKey(field.attribute.attributeid))
                                {
                                    if (record[field.attribute.attributeid] != DBNull.Value)
                                    {
                                        currency = (decimal)record[field.attribute.attributeid];
                                        txtbox.Text = currency.ToString("########0.00");
                                    }
                                }
                                else
                                {
                                    txtbox.Text = "";
                                }
                            }
                            break;
                        case FieldType.Integer:
                        case FieldType.Number:
                            txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                            if (txtbox != null)
                            {
                                if (record.ContainsKey(field.attribute.attributeid))
                                {
                                    if (record[field.attribute.attributeid] != DBNull.Value)
                                    {
                                        txtbox.Text = record[field.attribute.attributeid].ToString();
                                    }
                                }
                                else
                                {
                                    txtbox.Text = "";
                                }
                            }
                            break;
                        case FieldType.TickBox:
                            cmbbox = (DropDownList)holder.FindControl("cmb" + field.attribute.attributeid);
                            if (cmbbox != null)
                            {
                                if (record.ContainsKey(field.attribute.attributeid))
                                {
                                    if (record[field.attribute.attributeid] != DBNull.Value)
                                    {
                                        //foreach (ListItem item in cmbbox.Items)
                                        //{
                                        //    item.Selected = false;
                                        //}

                                        object tempVal = true;
                                        record.TryGetValue(field.attribute.attributeid, out tempVal);

                                        if ((bool)tempVal)
                                        {
                                            cmbbox.ClearSelection();
                                            cmbbox.Items.FindByValue("1").Selected = true;
                                        }
                                        else
                                        {
                                            cmbbox.ClearSelection();
                                            cmbbox.Items.FindByValue("0").Selected = true;
                                        }
                                    }
                                }
                                else
                                {
                                    cTickboxAttribute tmp = (cTickboxAttribute)field.attribute;
                                    if (tmp.defaultvalue != string.Empty && cmbbox.Items.FindByText(tmp.defaultvalue) != null)
                                    {
                                        cmbbox.SelectedIndex = cmbbox.Items.IndexOf(cmbbox.Items.FindByText(tmp.defaultvalue));
                                    }
                                    else
                                    {
                                        cmbbox.ClearSelection();
                                        cmbbox.Items.FindByValue("0").Selected = true;
                                    }
                                }
                            }
                            break;
                        case FieldType.List:
                            cmbbox = (DropDownList)holder.FindControl("cmb" + field.attribute.attributeid);
                            if (cmbbox != null)
                            {
                                if (record.ContainsKey(field.attribute.attributeid))
                                {
                                    //foreach (ListItem item in cmbbox.Items)
                                    //{
                                    //    item.Selected = false;
                                    //}

                                    if (record[field.attribute.attributeid] != DBNull.Value)
                                    {
                                        if (cmbbox.Items.FindByValue(record[field.attribute.attributeid].ToString()) != null)
                                        {
                                            cmbbox.ClearSelection();
                                            var currentItem = cmbbox.Items.FindByValue(record[field.attribute.attributeid].ToString());
                                            currentItem.Selected = true;
                                            if (!currentItem.Enabled)
                                            {
                                                currentItem.Enabled = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (cmbbox.Items.Count > 0)
                                    {
                                        cmbbox.ClearSelection();
                                        cmbbox.Items[0].Selected = true;
                                    }
                                }
                            }
                            break;
                        case FieldType.DateTime:
                            txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                            if (txtbox != null)
                            {
                                if (record.ContainsKey(field.attribute.attributeid))
                                {
                                    if (record[field.attribute.attributeid] != DBNull.Value)
                                    {
                                        dateatt = (cDateTimeAttribute)field.attribute;
                                        if (record[field.attribute.attributeid] != DBNull.Value)
                                        {
                                            date = (DateTime)record[field.attribute.attributeid];
                                            switch (dateatt.format)
                                            {
                                                case AttributeFormat.DateOnly:
                                                    txtbox.Text = date.ToShortDateString();
                                                    break;
                                                case AttributeFormat.TimeOnly:
                                                    txtbox.Text = date.ToShortTimeString();
                                                    break;
                                                case AttributeFormat.DateTime:
                                                    txtbox.Text = date.ToShortDateString() + " " + date.ToShortTimeString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    txtbox.Text = "";
                                }
                            }
                            break;
                    }
                }
            }
        }

        public void createFieldPanel(ref PlaceHolder holder, cTable tbl, string validationGroup, out StringBuilder javascript, int? categoryID = null, bool tooltipOnHover = false, GroupingOutputType groupType = GroupingOutputType.All, bool outJavascriptIncludesAutocompleteBindingsOnly = false, List<int> pkExcludeList = null, bool excludeAdminFields = false)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            CurrentValidationGroup = validationGroup;

            javascript = new StringBuilder();
            if (!outJavascriptIncludesAutocompleteBindingsOnly)
            {
                javascript.Append(CreateJavaScriptArray());
            }
            List<cUserDefinedField> fields = GetFieldsByTable(tbl);
            SortedList<int, cUserdefinedFieldGrouping> groupings = new SortedList<int, cUserdefinedFieldGrouping>();
            SortedList<int, cUserDefinedField> sortedFields;

            if (groupType != GroupingOutputType.UnGroupedOnly)
            {
                foreach (cUserDefinedField field in fields)
                {
                    if (field.Grouping != null && groupings.ContainsKey(field.Grouping.Order) == false)
                    {
                        bool addGrouping = false;

                        // all groupings should have filtercategories though they may be empty
                        switch (field.Grouping.AssociatedTable.TableID.ToString().ToUpper())
                        {
                            case ReportTable.ContractDetails:
                            case ReportTable.ContractProductDetails:
                            case ReportTable.SupplierDetails:
                                // check if filter category permits the display of the grouping
                                if (field.Grouping.FilterCategories != null && categoryID.HasValue)
                                {
                                    if (field.Grouping.FilterCategories.ContainsKey(currentUser.CurrentSubAccountId) && field.Grouping.FilterCategories[currentUser.CurrentSubAccountId].Count > 0)
                                    {
                                        if (field.Grouping.FilterCategories[currentUser.CurrentSubAccountId].Contains(categoryID.Value))
                                        {
                                            addGrouping = true;
                                        }
                                    }
                                }
                                break;
                            default:
                                // display all groupings for non filtered areas
                                addGrouping = true;
                                break;
                        }

                        if (addGrouping)
                        {
                            groupings.Add(field.Grouping.Order, field.Grouping);
                        }

                        //if (field.Grouping.FilterCategories != null)
                        //{
                        //    if (categoryID == null || field.Grouping.FilterCategories.ContainsKey(currentUser.CurrentSubAccountId) == false || field.Grouping.FilterCategories.Count == 0 || (field.Grouping.FilterCategories.ContainsKey(currentUser.CurrentSubAccountId) && field.Grouping.FilterCategories[currentUser.CurrentSubAccountId].Count > 0))
                        //    {
                        //        groupings.Add(field.Grouping.Order, field.Grouping);
                        //    }
                        //    else if (field.Grouping.FilterCategories.ContainsKey(currentUser.CurrentSubAccountId) && field.Grouping.FilterCategories[currentUser.CurrentSubAccountId].Contains(categoryID.Value)) // if it's filtered, then display it if this cat id is in the list
                        //    {
                        //        groupings.Add(field.Grouping.Order, field.Grouping);
                        //    }
                        //}
                    }
                }

                foreach (cUserdefinedFieldGrouping grouping in groupings.Values)
                {
                    sortedFields = GetFieldsByTableAndGrouping(tbl, grouping, excludeAdminFields);
                    if (sortedFields.Count > 0)
                    {
                        createSection(ref holder, grouping, sortedFields, ref javascript, tooltipOnHover, outJavascriptIncludesAutocompleteBindingsOnly, pkExcludeList, excludeAdminFields);
                    }
                }
            }

            if (groupType != GroupingOutputType.GroupedOnly)
            {
                sortedFields = GetFieldsByTableAndGrouping(tbl, null, excludeAdminFields);
                if (sortedFields.Count > 0)
                {
                    createSection(ref holder, null, sortedFields, ref javascript, tooltipOnHover, outJavascriptIncludesAutocompleteBindingsOnly, pkExcludeList, excludeAdminFields);
                }
            }
        }

        private void createSection(ref PlaceHolder holder, cUserdefinedFieldGrouping grouping, SortedList<int, cUserDefinedField> fields, ref StringBuilder javascript, bool tooltipOnHover = false, bool outJavascriptIncludesAutocompleteBindingsOnly = false, List<int> pkExcludeList = null, bool excludeAdminFields = false)
        {
            Literal lit = new Literal();
            if (grouping == null)
            {
                lit.Text = "<div class=\"sectiontitle\">Other Information</div>";
            }
            else
            {
                lit.Text = "<div class=\"sectiontitle\">" + grouping.GroupName + "</div>";
            }
            holder.Controls.Add(lit);

            generateFields(ref holder, fields, ref javascript, tooltipOnHover, outJavascriptIncludesAutocompleteBindingsOnly, pkExcludeList, excludeAdminFields);

            //lit = new Literal();
            //holder.Controls.Add(lit);
        }

        public SortedList<int, object> GetUserDefinedFieldsFromPage(ref PlaceHolder holder, cTable tbl, GroupingOutputType groupType = GroupingOutputType.All, bool excludeAdminFields = false)
        {
            SortedList<int, object> data = new SortedList<int, object>();
            List<cUserDefinedField> fields = GetFieldsByTable(tbl);
            foreach (cUserDefinedField field in fields)
            {
                if (excludeAdminFields && field.table.TableName.ToLower() == "userdefinedcars" && !field.AllowEmployeeToPopulate)
                {
                    continue;
                }

                if (groupType == GroupingOutputType.All || (groupType == GroupingOutputType.GroupedOnly && field.Grouping != null) || (groupType == GroupingOutputType.UnGroupedOnly && field.Grouping == null))
                {
                    TextBox txtbox;
                    DropDownList dropDownList;
                    switch (field.attribute.fieldtype)
                    {
                        case FieldType.DynamicHyperlink:
                        case FieldType.Text:
                            if (holder.FindControl("txt" + field.attribute.attributeid) != null)
                            {
                                txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                data.Add(field.userdefineid, string.IsNullOrWhiteSpace(txtbox.Text) ? null : txtbox.Text);
                            }
                            break;
                        case FieldType.LargeText:
                            if (holder.FindControl("txt" + field.attribute.attributeid) != null)
                            {
                                cTextAttribute textAttribute = (cTextAttribute)field.attribute;
                                object value = null;

                                var textbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);

                                if (string.IsNullOrWhiteSpace(textbox.Text) == false)
                                {
                                    value = textAttribute.format == AttributeFormat.FormattedText ? WebUtility.HtmlDecode(textbox.Text) : textbox.Text;
                                }

                                data.Add(field.userdefineid, value);
                            }
                            break;
                        case FieldType.Currency:
                        case FieldType.Number:
                            if (holder.FindControl("txt" + field.attribute.attributeid) != null)
                            {
                                txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                data.Add(field.userdefineid, string.IsNullOrWhiteSpace(txtbox.Text) ? (decimal?)null :  Convert.ToDecimal(txtbox.Text));
                            }
                            break;
                        case FieldType.Integer:
                            if (holder.FindControl("txt" + field.attribute.attributeid) != null)
                            {
                                txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                data.Add(field.userdefineid, string.IsNullOrWhiteSpace(txtbox.Text) ? (int?)null : Convert.ToInt32(txtbox.Text));
                            }
                            break;
                        case FieldType.TickBox:
                            if (holder.FindControl("cmb" + field.attribute.attributeid) != null)
                            {
                                dropDownList = (DropDownList)holder.FindControl("cmb" + field.attribute.attributeid);
                                if (dropDownList != null)
                                {
                                    if (dropDownList.SelectedItem.Value != "-1")
                                    {
                                        int nVal;
                                        int.TryParse(dropDownList.SelectedItem.Value, out nVal);
                                        data.Add(field.userdefineid, nVal);
                                    }
                                    else
                                    {
                                        data.Add(field.userdefineid, DBNull.Value);
                                    }
                                }
                            }
                            break;
                        case FieldType.DateTime:
                            if (holder.FindControl("txt" + field.attribute.attributeid) != null)
                            {
                                txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                data.Add(field.userdefineid, string.IsNullOrWhiteSpace(txtbox.Text) ? (DateTime?) null : DateTime.Parse(txtbox.Text));
                            }
                            break;
                        case FieldType.List:
                            if (holder.FindControl("cmb" + field.attribute.attributeid) != null)
                            {
                                dropDownList = (DropDownList)holder.FindControl("cmb" + field.attribute.attributeid);
                                if (dropDownList != null)
                                {
                                    data.Add(field.userdefineid, dropDownList.SelectedValue);
                                }
                            }
                            break;
                        case FieldType.Relationship:
                            if (holder.FindControl("txt" + field.attribute.attributeid + "_ID") != null)
                            {
                                if (field.attribute.GetType() == typeof(cManyToOneRelationship))
                                {
                                    txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid + "_ID");
                                    int IdValue;
                                    data.Add(field.userdefineid, txtbox.Text != "" && int.TryParse(txtbox.Text, out IdValue) ? IdValue : (int?) null);
                                }
                            }
                            break;
                    }
                }
            }
            return data;
        }
        
        public SortedList<int, object> getItemsFromPanel(ref PlaceHolder holder, cTable tbl, GroupingOutputType groupType = GroupingOutputType.All, bool excludeAdminFields = false)
        {
            SortedList<int, object> data = new SortedList<int, object>();
            TextBox txtbox;
            AjaxControlToolkit.HtmlEditorExtender txtrichtxt;
            DropDownList cmbbox;
            List<cUserDefinedField> fields = GetFieldsByTable(tbl);
            foreach (cUserDefinedField field in fields)
            {
                if (excludeAdminFields && field.table.TableName.ToLower() == "userdefinedcars" && !field.AllowEmployeeToPopulate)
                {
                    continue;
                }

                if (groupType == GroupingOutputType.All || (groupType == GroupingOutputType.GroupedOnly && field.Grouping != null) || (groupType == GroupingOutputType.UnGroupedOnly && field.Grouping == null))
                {
                    switch (field.attribute.fieldtype)
                    {
                        case FieldType.DynamicHyperlink:
                        case FieldType.Text:
                            if (holder.FindControl("txt" + field.attribute.attributeid) != null)
                            {
                                txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                if (txtbox.Text != "")
                                {
                                    data.Add(field.userdefineid, txtbox.Text);
                                }
                            }
                            break;
                        case FieldType.LargeText:
                            if (holder.FindControl("txt" + field.attribute.attributeid) != null)
                            {
                                cTextAttribute textatt = (cTextAttribute)field.attribute;
                                if (textatt.format == AttributeFormat.FormattedText)
                                {
                                    var textbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                    if (textbox.Text != "")
                                    {
                                        string htmlDecode = WebUtility.HtmlDecode(textbox.Text);
                                        data.Add(field.userdefineid, htmlDecode);
                                    }
                                }
                                else
                                {
                                    txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                    if (txtbox.Text != "")
                                    {
                                        data.Add(field.userdefineid, txtbox.Text);
                                    }
                                }
                            }
                            break;
                        case FieldType.Currency:
                        case FieldType.Number:
                            if (holder.FindControl("txt" + field.attribute.attributeid) != null)
                            {
                                txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                if (txtbox.Text != "")
                                {
                                    data.Add(field.userdefineid, Convert.ToDecimal(txtbox.Text));
                                }
                            }
                            break;
                        case FieldType.Integer:
                            if (holder.FindControl("txt" + field.attribute.attributeid) != null)
                            {
                                txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                if (txtbox.Text != "")
                                {
                                    data.Add(field.userdefineid, Convert.ToInt32(txtbox.Text));
                                }
                            }
                            break;
                        case FieldType.TickBox:
                            if (holder.FindControl("cmb" + field.attribute.attributeid) != null)
                            {
                                cmbbox = (DropDownList)holder.FindControl("cmb" + field.attribute.attributeid);
                                if (cmbbox != null)
                                {
                                    if (cmbbox.SelectedItem.Value != "-1")
                                    {
                                        int nVal = 0;
                                        int.TryParse(cmbbox.SelectedItem.Value, out nVal);
                                        data.Add(field.userdefineid, nVal);
                                    }
                                    else
                                    {
                                        data.Add(field.userdefineid, DBNull.Value);
                                    }
                                }
                            }
                            break;
                        case FieldType.DateTime:
                            if (holder.FindControl("txt" + field.attribute.attributeid) != null)
                            {
                                txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid);
                                if (txtbox.Text != "")
                                {
                                    data.Add(field.userdefineid, DateTime.Parse(txtbox.Text));
                                }
                            }
                            break;
                        case FieldType.List:
                            if (holder.FindControl("cmb" + field.attribute.attributeid) != null)
                            {
                                cmbbox = (DropDownList)holder.FindControl("cmb" + field.attribute.attributeid);
                                if (cmbbox != null)
                                {
                                    //if (cmbbox.SelectedValue != "0")
                                    //{
                                    data.Add(field.userdefineid, cmbbox.SelectedValue);
                                    //}
                                }
                            }
                            break;
                        case FieldType.Relationship:
                            if (holder.FindControl("txt" + field.attribute.attributeid + "_ID") != null)
                            {
                                if (field.attribute.GetType() == typeof(cManyToOneRelationship))
                                {
                                    txtbox = (TextBox)holder.FindControl("txt" + field.attribute.attributeid + "_ID");
                                    int ID_val = 0;
                                    if (txtbox.Text != "" && int.TryParse(txtbox.Text, out ID_val))
                                    {
                                        data.Add(field.userdefineid, ID_val);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return data;
        }
        
        public void generateFields(ref PlaceHolder holder, SortedList<int, cUserDefinedField> lstFields, ref StringBuilder javascript, bool tooltipOnHover = false, bool outJavascriptIncludesAutocompleteBindingsOnly = false, List<int> pkExcludeList = null, bool excludeAdminFields = false)
        {
            Literal cell;
            cTextAttribute txtatt;
            cListAttribute listatt;
            CompareValidator compval;
            RegularExpressionValidator regexval;
            TextBox txtbox;
            string controlid;
            string controlClientID = string.Empty;
            Label lbl;
            Literal row;
            //WebHtmlEditor txtrichtext;
            AjaxControlToolkit.HtmlEditorExtender txtrichtext;
            DropDownList cmbbox;
            Image img;
            CalendarExtender cal;
            MaskedEditExtender maskededit;
            int column = 0;
            bool isFirstFieldRendered = true;
            List<string> autocompBindStr = new List<string>();

            foreach (cUserDefinedField field in lstFields.Values)
            {
                if (excludeAdminFields && field.table.TableName.ToLower() == "userdefinedcars" && !field.AllowEmployeeToPopulate)
                {
                    continue;
                }

                if (!outJavascriptIncludesAutocompleteBindingsOnly)
                {
                    javascript.Append("userdefinedField = new Array();\n");
                    javascript.Append("userdefinedField.push(" + field.userdefineid + ");\n");
                    javascript.Append("userdefinedField.push('" + field.fieldtype + "');\n");
                }

                switch (field.attribute.fieldtype)
                {
                    case FieldType.TickBox:
                    case FieldType.List:
                        controlid = "cmb" + field.attribute.attributeid;
                        break;
                    case FieldType.Hyperlink:
                        controlid = "lnk" + field.attribute.attributeid;
                        break;
                    case FieldType.Text:
                    case FieldType.Currency:
                    case FieldType.Number:
                    case FieldType.Integer:
                    case FieldType.DateTime:
                    case FieldType.LargeText:
                    case FieldType.Relationship:
                    case FieldType.DynamicHyperlink:
                    case FieldType.RelationshipTextbox:
                    default:
                        controlid = "txt" + field.attribute.attributeid;
                        break;
                }
                if (column == 0 || (field.attribute.GetType() == typeof(cTextAttribute) && ((cTextAttribute)field.attribute).format == AttributeFormat.FormattedText) || (field.attribute.GetType() == typeof(cTextAttribute) && ((cTextAttribute)field.attribute).format == AttributeFormat.MultiLine))
                {
                    row = new Literal();
                    if (!isFirstFieldRendered)
                    {
                        row.Text = "</div>";
                    }

                    if (field.attribute.GetType() == typeof(cTextAttribute) && ((cTextAttribute)field.attribute).format == AttributeFormat.FormattedText)
                    {
                        row.Text += "<div class=\"onecolumnlarge\">";
                    }
                    else if (field.attribute.GetType() == typeof(cTextAttribute) && ((cTextAttribute)field.attribute).format == AttributeFormat.MultiLine)
                    {
                        row.Text += "<div class=\"onecolumn\">";
                    }
                    else
                    {
                        row.Text += "<div class=\"twocolumn\">";
                    }
                    holder.Controls.Add(row);
                }
                if (field.attribute.GetType() != typeof(cOneToManyRelationship))
                {
                    lbl = new Label();
                    lbl.Text = field.attribute.displayname;
                    lbl.AssociatedControlID = controlid;
                    if (field.attribute.mandatory && field.attribute.fieldtype != FieldType.TickBox && field.attribute.fieldtype != FieldType.Hyperlink)
                    {
                        lbl.Text += "*";
                        lbl.CssClass = "labeltd";
                    }
                    holder.Controls.Add(lbl);
                }
                cell = new Literal();
                cell.Text = "<span class=\"inputs\">";
                holder.Controls.Add(cell);
                switch (field.attribute.fieldtype)
                {
                    case FieldType.Text:
                    case FieldType.Currency:
                    case FieldType.Number:
                    case FieldType.Integer:
                    case FieldType.DateTime:
                    case FieldType.DynamicHyperlink:
                        txtbox = new TextBox();
                        txtbox.ID = "txt" + field.attribute.attributeid;
                        txtbox.CssClass = "fillspan";
                        if (CurrentValidationGroup != "")
                        {
                            txtbox.ValidationGroup = CurrentValidationGroup;
                        }
                        if (field.attribute.GetType() == typeof(cTextAttribute))
                        {
                            txtatt = (cTextAttribute)field.attribute;
                            if (txtatt.maxlength.HasValue)
                            {
                                if (txtatt.maxlength.Value > 0)
                                {
                                    txtbox.MaxLength = txtatt.maxlength.Value;
                                }
                            }
                            if (txtatt.format == AttributeFormat.MultiLine)
                            {
                                txtbox.TextMode = TextBoxMode.MultiLine;
                            }
                        }
                        holder.Controls.Add(txtbox);
                        controlClientID = txtbox.ClientID;
                        if (!outJavascriptIncludesAutocompleteBindingsOnly)
                        {
                            javascript.Append("userdefinedField.push('" + txtbox.ClientID + "');\n");
                        }
                        break;
                    case FieldType.LargeText:
                        txtatt = (cTextAttribute)field.attribute;
                        if (txtatt.format == AttributeFormat.MultiLine)
                        {
                            txtbox = new TextBox();
                            txtbox.TextMode = TextBoxMode.MultiLine;
                            txtbox.ID = "txt" + field.attribute.attributeid;
                            txtbox.CssClass = "fillspan";
                            if (CurrentValidationGroup != "")
                            {
                                txtbox.ValidationGroup = CurrentValidationGroup;
                            }
                            holder.Controls.Add(txtbox);
                            if (!outJavascriptIncludesAutocompleteBindingsOnly)
                            {
                                javascript.Append("userdefinedField.push('" + txtbox.ClientID + "');\n");
                            }
                        }
                        else if (txtatt.format == AttributeFormat.FormattedText)
                        {
                            txtrichtext = new AjaxControlToolkit.HtmlEditorExtender();
                            txtrichtext.TargetControlID = "";
                            txtrichtext.ID = "editor" + field.attribute.attributeid;
                            txtrichtext.EnableSanitization = false;

                            TextBox txtContent = new TextBox();
                            txtContent.Height = Unit.Pixel(260);
                            txtContent.Width = Unit.Pixel(579);
                            txtContent.ID = "txt" + field.attribute.attributeid;
                            txtrichtext.TargetControlID = txtContent.ClientID;

                            holder.Controls.Add(txtContent);
                            holder.Controls.Add(txtrichtext);
                            if (!outJavascriptIncludesAutocompleteBindingsOnly)
                            {
                                javascript.Append("userdefinedField.push('" + txtContent.ClientID + "');\n");
                            }
                        }

                        break;
                    case FieldType.TickBox:
                        cmbbox = new DropDownList();
                        cmbbox.ID = "cmb" + field.attribute.attributeid;
                        cmbbox.Items.Add(new ListItem("Yes", "1"));
                        cmbbox.Items.Add(new ListItem("No", "0"));
                        cmbbox.CssClass = "fillspan";
                        cTickboxAttribute tickatt = (cTickboxAttribute)field.attribute;
                        if (tickatt.defaultvalue != "")
                        {
                            if (cmbbox.Items.FindByText(tickatt.defaultvalue) != null)
                            {
                                cmbbox.Items.FindByText(tickatt.defaultvalue).Selected = true;
                            }
                        }
                        if (CurrentValidationGroup != "")
                        {
                            cmbbox.ValidationGroup = CurrentValidationGroup;
                        }
                        holder.Controls.Add(cmbbox);
                        if (!outJavascriptIncludesAutocompleteBindingsOnly)
                        {
                            javascript.Append("userdefinedField.push('" + cmbbox.ClientID + "');\n");
                        }
                        break;
                    case FieldType.List:
                        listatt = (cListAttribute)field.attribute;
                        cmbbox = new DropDownList();
                        cmbbox.ID = "cmb" + field.attribute.attributeid;
                        cmbbox.Items.Add(new ListItem("[None]", "0"));
                        cmbbox.CssClass = "fillspan";
                        List<cListAttributeElement> lstLI = new List<cListAttributeElement>();
                        foreach (KeyValuePair<int, cListAttributeElement> listitem in listatt.items)
                        {
                            cListAttributeElement listelement = (cListAttributeElement)listitem.Value;
                            lstLI.Add(listelement);
                        }

                        // sort the list by element order
                        lstLI.Sort(delegate(cListAttributeElement liAttEle1, cListAttributeElement liAttEle2) { return liAttEle1.elementOrder.CompareTo(liAttEle2.elementOrder); });

                        foreach (cListAttributeElement lae in lstLI)
                        {
                            cmbbox.Items.Add(new ListItem(lae.elementText, lae.elementValue.ToString(), !lae.Archived));
                        }

                        if (CurrentValidationGroup != "")
                        {
                            cmbbox.ValidationGroup = CurrentValidationGroup;
                        }
                        holder.Controls.Add(cmbbox);
                        if (!outJavascriptIncludesAutocompleteBindingsOnly)
                        {
                            javascript.Append("userdefinedField.push('" + cmbbox.ClientID + "');\n");
                        }
                        break;
                    case FieldType.Relationship:
                        if (field.attribute.GetType() == typeof(cOneToManyRelationship))
                        {
                            txtbox = new TextBox();
                            txtbox.ID = "txt" + field.attribute.attributeid;
                            if (CurrentValidationGroup != "")
                            {
                                txtbox.ValidationGroup = CurrentValidationGroup;
                            }
                            holder.Controls.Add(txtbox);
                            if (!outJavascriptIncludesAutocompleteBindingsOnly)
                            {
                                javascript.Append("userdefinedField.push('" + txtbox.ClientID + "');\n");
                            }
                        }
                        else
                        {
                            cManyToOneRelationship reqManyToOneRelationship = (cManyToOneRelationship)field.attribute;

                            SortedList<int, FieldFilter> listOfFilters = new SortedList<int, FieldFilter>();

                            foreach (KeyValuePair<int, FieldFilter> kvp in reqManyToOneRelationship.filters)
                            {
                                listOfFilters.Add(kvp.Key, kvp.Value);
                            }

                            List<WebControl> relCntls = AutoComplete.createAutoCompleteControls("txt" + reqManyToOneRelationship.attributeid.ToString(), reqManyToOneRelationship.displayname, "fillspan");
                            foreach (WebControl wc in relCntls)
                            {
                                holder.Controls.Add(wc);

                                if (wc.GetType() == typeof(TextBox) && !wc.ID.EndsWith("_ID") && !outJavascriptIncludesAutocompleteBindingsOnly)
                                {
                                    javascript.Append("userdefinedField.push('" + wc.ClientID + "');\n");
                                }
                            }

                            if (pkExcludeList != null)
                            {
                                byte order = Convert.ToByte(reqManyToOneRelationship.filters.Count + 1);

                                object[] value1 = (from x in pkExcludeList select (object)x).ToArray();

                                listOfFilters.Add(order, new FieldFilter(reqManyToOneRelationship.relatedtable.GetPrimaryKey(), ConditionType.DoesNotEqual, String.Join(",", value1), null, order, null));
                            }

                            autocompBindStr.Add(AutoComplete.createAutoCompleteBindString("txt" + reqManyToOneRelationship.attributeid.ToString(), reqManyToOneRelationship.AutoCompleteMatchRows, reqManyToOneRelationship.relatedtable.TableID, reqManyToOneRelationship.AutoCompleteDisplayField, reqManyToOneRelationship.AutoCompleteMatchFieldIDList, fieldFilters: listOfFilters));
                        }
                        break;
                    case FieldType.Hyperlink:
                        HyperLink lnk = new HyperLink();
                        lnk.ID = "lnk" + field.attribute.attributeid;
                        cHyperlinkAttribute hyperlinkAtt = (cHyperlinkAttribute)field.attribute;
                        lnk.NavigateUrl = hyperlinkAtt.hyperlinkPath;
                        lnk.Target = "_blank";
                        lnk.Text = hyperlinkAtt.hyperlinkText;
                        holder.Controls.Add(lnk);
                        if (!outJavascriptIncludesAutocompleteBindingsOnly)
                        {
                            javascript.Append("userdefinedField.push('" + lnk.ClientID + "');\n");
                        }
                        break;
                }
                cell = new Literal();
                cell.Text = "</span><span class=\"inputicon\">";
                holder.Controls.Add(cell);

                if (field.attribute.fieldtype == FieldType.DateTime && ((cDateTimeAttribute)field.attribute).format == AttributeFormat.DateOnly)
                {
                    img = new Image();
                    img.ImageUrl = "~/shared/images/icons/cal.gif";
                    img.ID = "img" + field.attribute.attributeid;
                    holder.Controls.Add(img);
                }
                else if (field.attribute.fieldtype == FieldType.DynamicHyperlink)
                {
                    // output the image link that will open whatever link is in the textbox
                    img = new Image();
                    img.ImageUrl = "~/shared/images/icons/16/Plain/earth.png";
                    img.ID = "img" + field.attribute.attributeid;
                    img.Attributes.Add("onclick", "launchURL('" + controlClientID + "');");
                    img.CssClass = "btn";
                    holder.Controls.Add(img);
                }
                cell = new Literal();
                cell.Text += "</span>";
                cell.Text += "<span class=\"inputtooltipfield\">";

                string tooltipText = cMisc.EscapeLinebreaks(field.attribute.tooltip);
                if (tooltipText != "")
                {
                    if (tooltipOnHover)
                    {
                        cell.Text += "<img id=\"imgtooltip" + field.attribute.attributeid + "\" src=\"/shared/images/icons/16/plain/tooltip.png\" onmouseover=\"SEL.Tooltip.Show('" + tooltipText + "', 'sm', this);\" />";
                    }
                    else
                    {
                        cell.Text += "<img id=\"imgtooltip" + field.attribute.attributeid + "\" src=\"/shared/images/icons/16/plain/tooltip.png\" onclick=\"SEL.Tooltip.customToolTip('imgtooltip" + field.attribute.attributeid + "','" + tooltipText + "');\" />";
                    }
                }

                cell.Text += "</span>";
                cell.Text += "<span class=\"inputvalidatorfield\" id=\"spanvalidate" + field.attribute.attributeid + "\">";
                holder.Controls.Add(cell);

                if (field.attribute.mandatory)
                {
                    switch (field.attribute.fieldtype)
                    {
                        case FieldType.List:
                            //case FieldType.TickBox:
                            compval = new CompareValidator();
                            compval.ID = "req" + field.attribute.attributeid;
                            compval.ControlToValidate = "cmb" + field.attribute.attributeid;
                            compval.Operator = ValidationCompareOperator.GreaterThan;
                            compval.ValueToCompare = "0";
                            compval.Text = "*";
                            compval.ErrorMessage = field.attribute.displayname + " is a mandatory field. Please select a value from the list provided";
                            if (CurrentValidationGroup != "")
                            {
                                compval.ValidationGroup = CurrentValidationGroup;
                            }
                            holder.Controls.Add(compval);

                            break;
                        case FieldType.Hyperlink:
                        case FieldType.TickBox:
                            //RequiredFieldValidator reqlnkval = new RequiredFieldValidator();
                            //reqlnkval.ID = "req" + field.attribute.attributeid;
                            //reqlnkval.ControlToValidate = "lnk" + field.attribute.attributeid;
                            //if (CurrentValidationGroup != "")
                            //{
                            //    reqlnkval.ValidationGroup = CurrentValidationGroup;
                            //}
                            //reqlnkval.ErrorMessage = field.attribute.displayname + " is a mandatory field. Please enter a value in the box provided.";
                            //reqlnkval.Text = "*";
                            //holder.Controls.Add(reqlnkval);
                            break;
                        //case FieldType.LargeText:
                        //    txtatt = (cTextAttribute)field.attribute;
                        //    if (txtatt.format == AttributeFormat.FormattedText)
                        //    {
                        //        //CustomValidator custval = new CustomValidator();
                        //        //custval.ID = "custmand" + field.attribute.attributeid;
                        //        //custval.ControlToValidate = "txt" + field.attribute.attributeid; ;
                        //        //custval.ClientValidationFunction = "HTMLUdf_ClientValidate";
                        //        //custval.ErrorMessage = field.label + " is a mandatory field. Please enter a value in the box provided.";
                        //        //custval.Text = "*";
                        //        //if (CurrentValidationGroup != "")
                        //        //{
                        //        //    custval.ValidationGroup = CurrentValidationGroup;
                        //        //}
                        //        //holder.Controls.Add(custval);

                        //        RequiredFieldValidator reqval2 = new RequiredFieldValidator();
                        //        reqval2.ID = "req" + field.attribute.attributeid;
                        //        reqval2.ControlToValidate = "txt" + field.attribute.attributeid;
                        //        reqval2.EnableClientScript = true;
                        //        if (CurrentValidationGroup != "")
                        //        {
                        //            reqval2.ValidationGroup = CurrentValidationGroup;
                        //        }
                        //        reqval2.ErrorMessage = field.attribute.displayname + " is a mandatory field. Please enter a value in the box provided.";
                        //        reqval2.Text = "*";
                        //        holder.Controls.Add(reqval2);

                        //    }
                        //    else
                        //    {
                        //        goto default;
                        //    }
                        //    break;
                        case FieldType.Relationship:
                            RequiredFieldValidator relReqVal = AutoComplete.getAutoCompleteMandatoryValidator("txt" + field.attribute.attributeid.ToString(), ((cManyToOneRelationship)field.attribute).displayname, CurrentValidationGroup);
                            holder.Controls.Add(relReqVal);
                            break;
                        default:
                            RequiredFieldValidator reqval = new RequiredFieldValidator();
                            reqval.ID = "req" + field.attribute.attributeid;
                            reqval.ControlToValidate = "txt" + field.attribute.attributeid;
                            if (CurrentValidationGroup != "")
                            {
                                reqval.ValidationGroup = CurrentValidationGroup;
                            }
                            reqval.ErrorMessage = field.attribute.displayname + " is a mandatory field. Please enter a value in the box provided.";
                            reqval.Text = "*";
                            holder.Controls.Add(reqval);
                            break;
                    }
                }
                switch (field.attribute.fieldtype)
                {
                    case FieldType.Currency:
                        compval = new CompareValidator();
                        compval.ID = "comp" + field.attribute.attributeid;
                        compval.ControlToValidate = "txt" + field.attribute.attributeid;
                        compval.Type = ValidationDataType.Currency;
                        compval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid. Valid characters are the numbers 0-9 and a full stop (.)";
                        compval.Text = "*";
                        compval.Operator = ValidationCompareOperator.DataTypeCheck;
                        if (CurrentValidationGroup != "")
                        {
                            compval.ValidationGroup = CurrentValidationGroup;
                        }
                        holder.Controls.Add(compval);
                        break;
                    case FieldType.Integer:
                        compval = new CompareValidator();
                        compval.ID = "comp" + field.attribute.attributeid;
                        compval.ControlToValidate = "txt" + field.attribute.attributeid;
                        compval.Type = ValidationDataType.Integer;
                        compval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid. Valid characters are the numbers 0-9";
                        compval.Text = "*";
                        compval.Operator = ValidationCompareOperator.DataTypeCheck;
                        if (CurrentValidationGroup != "")
                        {
                            compval.ValidationGroup = CurrentValidationGroup;
                        }
                        holder.Controls.Add(compval);
                        break;
                    case FieldType.Number:
                        compval = new CompareValidator();
                        compval.ID = "comp" + field.attribute.attributeid;
                        compval.ControlToValidate = "txt" + field.attribute.attributeid;
                        compval.Type = ValidationDataType.Double;
                        compval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid. Valid characters are the numbers 0-9 and a full stop (.)";
                        compval.Text = "*";
                        compval.Operator = ValidationCompareOperator.DataTypeCheck;
                        if (CurrentValidationGroup != "")
                        {
                            compval.ValidationGroup = CurrentValidationGroup;
                        }
                        holder.Controls.Add(compval);
                        break;
                    case FieldType.DateTime:
                        cDateTimeAttribute datetimeatt = (cDateTimeAttribute)field.attribute;
                        switch (datetimeatt.format)
                        {
                            case AttributeFormat.DateOnly:
                                maskededit = new MaskedEditExtender();
                                maskededit.TargetControlID = "txt" + field.attribute.attributeid;
                                maskededit.Mask = "99/99/9999";
                                maskededit.MaskType = MaskedEditType.Date;
                                maskededit.ID = "mskstartdate" + field.attribute.attributeid;

                                cal = new CalendarExtender();
                                cal.TargetControlID = "txt" + field.attribute.attributeid;
                                cal.Format = "dd/MM/yyyy";
                                cal.PopupButtonID = "img" + field.attribute.attributeid;
                                cal.ID = "cal" + field.attribute.attributeid;
                                holder.Controls.Add(cal);
                                holder.Controls.Add(maskededit);

                                compval = new CompareValidator();
                                compval.ID = "compmax" + field.attribute.attributeid;
                                compval.ControlToValidate = "txt" + field.attribute.attributeid;
                                compval.Type = ValidationDataType.Date;
                                compval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid. Date must be earlier than 31/12/3000";
                                compval.Text = "*";
                                compval.Operator = ValidationCompareOperator.LessThan;
                                compval.ValueToCompare = "31/12/3000";
                                if (CurrentValidationGroup != "")
                                {
                                    compval.ValidationGroup = CurrentValidationGroup;
                                }
                                holder.Controls.Add(compval);

                                compval = new CompareValidator();
                                compval.ID = "compmin" + field.attribute.attributeid;
                                compval.ControlToValidate = "txt" + field.attribute.attributeid;
                                compval.Type = ValidationDataType.Date;
                                compval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid. Date must be later than 01/01/1900";
                                compval.Text = "*";
                                compval.Operator = ValidationCompareOperator.GreaterThanEqual;
                                compval.ValueToCompare = "01/01/1900";
                                if (CurrentValidationGroup != "")
                                {
                                    compval.ValidationGroup = CurrentValidationGroup;
                                }
                                holder.Controls.Add(compval);

                                compval = new CompareValidator();
                                compval.ID = "compdtype" + field.attribute.attributeid;
                                compval.ControlToValidate = "txt" + field.attribute.attributeid;
                                compval.Type = ValidationDataType.Date;
                                compval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid. Valid characters are the numbers 0-9 and a forward slash (/)";
                                compval.Text = "*";
                                compval.Operator = ValidationCompareOperator.DataTypeCheck;
                                if (CurrentValidationGroup != "")
                                {
                                    compval.ValidationGroup = CurrentValidationGroup;
                                }
                                holder.Controls.Add(compval);
                                break;

                            case AttributeFormat.TimeOnly:
                                maskededit = new MaskedEditExtender();
                                maskededit.TargetControlID = "txt" + field.attribute.attributeid;
                                maskededit.Mask = "99:99";
                                maskededit.UserTimeFormat = MaskedEditUserTimeFormat.TwentyFourHour;
                                maskededit.MaskType = MaskedEditType.Time;
                                maskededit.ID = "msk" + field.attribute.attributeid;



                                holder.Controls.Add(maskededit);
                                regexval = new RegularExpressionValidator();
                                regexval.ID = "regextime" + field.attribute.attributeid;
                                regexval.ControlToValidate = "txt" + field.attribute.attributeid;
                                //regexval.ValidationExpression = "[0-2][0-9]:[0-5][0-9]";
                                regexval.ValidationExpression = @"([0-1]\d|2[0-3]):([0-5]\d)";
                                regexval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid. Valid time format is 00:00 to 23:59";
                                regexval.Text = "*";

                                if (CurrentValidationGroup != "")
                                {
                                    regexval.ValidationGroup = CurrentValidationGroup;
                                }
                                holder.Controls.Add(regexval);
                                break;

                            case AttributeFormat.DateTime:
                                maskededit = new MaskedEditExtender();
                                maskededit.TargetControlID = "txt" + field.attribute.attributeid;
                                maskededit.Mask = "99/99/9999 99:99";
                                maskededit.UserDateFormat = MaskedEditUserDateFormat.DayMonthYear;
                                //maskededit.AcceptAMPM = true; if we need to fix date time, this makes the entry work, would then have to check the way it's saved and retrieved
                                maskededit.MaskType = MaskedEditType.DateTime;
                                maskededit.ID = "msk" + field.attribute.attributeid;
                                holder.Controls.Add(maskededit);

                                regexval = new RegularExpressionValidator();
                                regexval.ValidationExpression = "^(?=\\d)(?:(?:31(?!.(?:0?[2469]|11))|(?:30|29)(?!.0?2)|29(?=.0?2.(?:(?:(?:1[6-9]|[2-9]\\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(?:\x20|$))|(?:2[0-8]|1\\d|0?[1-9]))([-./])(?:1[012]|0?[1-9])\\1(?:1[6-9]|[2-9]\\d)?\\d\\d(?:(?=\x20\\d)\x20|$))?(((0?[1-9]|1[012])(:[0-5]\\d){0,2}(\x20[AP]M))|([01]\\d|2[0-3])(:[0-5]\\d){1,2})?$";
                                //regexval.ValidationExpression = "[0-3][0-9]/[0-1][0-9]/[0-2][0-9][0-9][0-9] [0-2][0-9]:[0-5][0-9]";
                                regexval.ID = "compregdate" + field.attribute.attributeid;
                                regexval.ControlToValidate = "txt" + field.attribute.attributeid;
                                regexval.Text = "*";
                                regexval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid.";
                                if (CurrentValidationGroup != "")
                                {
                                    regexval.ValidationGroup = CurrentValidationGroup;
                                }
                                holder.Controls.Add(regexval);

                                break;
                        }
                        break;
                    case FieldType.DynamicHyperlink:
                        regexval = new RegularExpressionValidator();
                        regexval.ValidationExpression = @"^(https?|ftps?)://(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.*|^mailto:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
                        regexval.ID = "revlink" + field.attribute.attributeid;
                        regexval.ControlToValidate = "txt" + field.attribute.attributeid;
                        regexval.Text = "*";
                        regexval.ErrorMessage = "The value you have entered for " + field.attribute.displayname + " is invalid.";
                        if (CurrentValidationGroup != "")
                        {
                            regexval.ValidationGroup = CurrentValidationGroup;
                        }
                        holder.Controls.Add(regexval);
                        break;
                    case FieldType.Relationship:
                        holder.Controls.Add(AutoComplete.getAutoCompleteInvalidEntryValidator("txt" + field.attribute.attributeid.ToString(), field.attribute.displayname, CurrentValidationGroup));
                        break;
                }
                cell = new Literal();
                cell.Text = "</span>";
                holder.Controls.Add(cell);
                if (column == 0 && !(field.attribute.GetType() == typeof(cTextAttribute) && ((cTextAttribute)field.attribute).format == AttributeFormat.FormattedText) && !(field.attribute.GetType() == typeof(cTextAttribute) && ((cTextAttribute)field.attribute).format == AttributeFormat.MultiLine))
                {
                    column = 1;
                }
                else
                {
                    column = 0;
                }

                if (!outJavascriptIncludesAutocompleteBindingsOnly)
                {
                    javascript.Append("userdefinedField.push('" + CurrentValidationGroup + "');\n");
                    if (field.fieldtype == FieldType.DateTime)
                    {
                        cDateTimeAttribute dateType = (cDateTimeAttribute)field.attribute;
                        javascript.Append("userdefinedField.push('" + dateType.format + "');\n");
                    }
                    else
                    {
                        javascript.Append("userdefinedField.push('');\n");
                    }

                    // output default value if one exists - needed for clear form functions to set back to default for new records.
                    switch (field.fieldtype)
                    {
                        case FieldType.TickBox:
                            cTickboxAttribute tmpChk = (cTickboxAttribute)field.attribute;
                            javascript.Append("userdefinedField.push('" + tmpChk.defaultvalue + "');\n");
                            break;
                        default:
                            javascript.Append("userdefinedField.push('');\n");
                            break;
                    }

                    javascript.Append("lstUserdefined.push(userdefinedField);\n");
                }

                isFirstFieldRendered = false;
            }

            if (autocompBindStr.Count > 0)
            {
                javascript.Append(AutoComplete.generateScriptRegisterBlock(autocompBindStr));
            }
            cell = new Literal();
            cell.Text = "</div>";
            holder.Controls.Add(cell);
        }

        public List<object[]> getUserdefinedValuesForClient(SortedList<int, object> lstUserdefinedFields, cTable tbl)
        {
            List<cUserDefinedField> fields = GetFieldsByTable(tbl);
            List<object[]> lstUserdefinedVals = new List<object[]>();
            object[] arrObj;

            foreach (cUserDefinedField field in fields)
            {
                arrObj = new object[3];
                arrObj[0] = field.userdefineid;

                switch (field.attribute.fieldtype)
                {
                    case FieldType.Text:

                        if (lstUserdefinedFields.ContainsKey(field.attribute.attributeid))
                        {
                            if (lstUserdefinedFields[field.attribute.attributeid] != DBNull.Value)
                            {
                                arrObj[1] = (string)lstUserdefinedFields[field.attribute.attributeid];
                            }
                        }

                        break;
                    case FieldType.LargeText:


                        if (lstUserdefinedFields.ContainsKey(field.attribute.attributeid))
                        {
                            if (lstUserdefinedFields[field.attribute.attributeid] != DBNull.Value)
                            {
                                arrObj[1] = (string)lstUserdefinedFields[field.attribute.attributeid];
                            }
                        }



                        break;
                    case FieldType.Currency:

                        if (lstUserdefinedFields.ContainsKey(field.attribute.attributeid))
                        {
                            if (lstUserdefinedFields[field.attribute.attributeid] != DBNull.Value)
                            {
                                decimal currency = (decimal)lstUserdefinedFields[field.attribute.attributeid];
                                arrObj[1] = currency.ToString("########0.00");
                            }
                        }

                        break;
                    case FieldType.Integer:
                    case FieldType.Number:

                        if (lstUserdefinedFields.ContainsKey(field.attribute.attributeid))
                        {
                            if (lstUserdefinedFields[field.attribute.attributeid] != DBNull.Value)
                            {
                                arrObj[1] = lstUserdefinedFields[field.attribute.attributeid].ToString();
                            }
                        }

                        break;
                    case FieldType.TickBox:

                        if (lstUserdefinedFields.ContainsKey(field.attribute.attributeid))
                        {
                            if (lstUserdefinedFields[field.attribute.attributeid] != DBNull.Value)
                            {
                                if ((bool)lstUserdefinedFields[field.attribute.attributeid] == true)
                                {
                                    arrObj[1] = true;
                                }
                                else
                                {
                                    arrObj[1] = false;
                                }
                            }
                        }

                        break;
                    case FieldType.List:

                        if (lstUserdefinedFields.ContainsKey(field.attribute.attributeid))
                        {

                            if (lstUserdefinedFields[field.attribute.attributeid] != DBNull.Value)
                            {
                                arrObj[1] = lstUserdefinedFields[field.attribute.attributeid].ToString();
                            }
                        }

                        break;
                    case FieldType.DateTime:

                        if (lstUserdefinedFields.ContainsKey(field.attribute.attributeid))
                        {
                            if (lstUserdefinedFields[field.attribute.attributeid] != DBNull.Value)
                            {
                                cDateTimeAttribute dateatt = (cDateTimeAttribute)field.attribute;
                                if (lstUserdefinedFields[field.attribute.attributeid] != DBNull.Value)
                                {
                                    DateTime date = (DateTime)lstUserdefinedFields[field.attribute.attributeid];
                                    switch (dateatt.format)
                                    {
                                        case AttributeFormat.DateOnly:
                                            arrObj[1] = date.ToShortDateString();
                                            break;
                                        case AttributeFormat.TimeOnly:
                                            arrObj[1] = date.ToShortTimeString();
                                            break;
                                        case AttributeFormat.DateTime:
                                            arrObj[1] = date.ToShortDateString() + " " + date.ToShortTimeString();
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    case FieldType.RelationshipTextbox:
                        if (lstUserdefinedFields.ContainsKey(field.attribute.attributeid))
                        {
                            if (lstUserdefinedFields[field.attribute.attributeid] != DBNull.Value)
                            {
                                cRelationshipTextBoxAttribute relTxt = (cRelationshipTextBoxAttribute)field.attribute;
                                if (relTxt != null)
                                {
                                    int idVal = (int)lstUserdefinedFields[field.attribute.attributeid];

                                    cRelationshipTextBoxAttribute relationship = (cRelationshipTextBoxAttribute)field.attribute;
                                    cQueryBuilder query = new cQueryBuilder(AccountID, cAccounts.getConnectionString(AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, relationship.relatedtable, new cTables(AccountID), clsFields);
                                    query.addColumn(relationship.relatedtable.GetKeyField());
                                    query.addFilter(relationship.relatedtable.GetPrimaryKey(), ConditionType.Equals, new object[] { idVal }, null, ConditionJoiner.None, null); // null as genlist? !!!!!
                                    string txtvalue = "";

                                    using (SqlDataReader reader = query.getReader())
                                    {
                                        while (reader.Read())
                                        {
                                            if (!reader.IsDBNull(0))
                                            {
                                                txtvalue = reader.GetString(0);
                                                break;
                                            }
                                        }
                                        reader.Close();
                                    }

                                    arrObj[1] = txtvalue;
                                }
                            }
                            else
                            {
                                arrObj[1] = "";
                            }
                        }
                        break;
                    case FieldType.Relationship:
                        if (lstUserdefinedFields.ContainsKey(field.attribute.attributeid))
                        {
                            if (lstUserdefinedFields[field.attribute.attributeid] != DBNull.Value)
                            {
                                int idVal = (int)lstUserdefinedFields[field.attribute.attributeid];
                                if (field.attribute.GetType() == typeof(cManyToOneRelationship))
                                {
                                    cManyToOneRelationship relationship = (cManyToOneRelationship)field.attribute;
                                    arrObj[1] = idVal.ToString();
                                    cQueryBuilder query = new cQueryBuilder(AccountID, cAccounts.getConnectionString(AccountID), ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, relationship.relatedtable, new cTables(AccountID), clsFields);
                                    query.addColumn(clsFields.GetFieldByID(relationship.AutoCompleteDisplayField));
                                    query.addFilter(relationship.relatedtable.GetPrimaryKey(), ConditionType.Equals, new object[] { idVal }, null, ConditionJoiner.None, null); // null as genlist? !!!!!
                                    string txtvalue = "";

                                    using (SqlDataReader reader = query.getReader())
                                    {
                                        while (reader.Read())
                                        {
                                            if (!reader.IsDBNull(0))
                                            {
                                                txtvalue = reader.GetString(0);
                                                break;
                                            }
                                        }
                                        reader.Close();
                                    }
                                    arrObj[2] = txtvalue;
                                }
                            }
                        }
                        break;
                    case FieldType.DynamicHyperlink:
                        if (lstUserdefinedFields.ContainsKey(field.attribute.attributeid))
                        {
                            if (lstUserdefinedFields[field.attribute.attributeid] != DBNull.Value)
                            {
                                arrObj[1] = (string)lstUserdefinedFields[field.attribute.attributeid];
                            }
                        }
                        break;
                }

                lstUserdefinedVals.Add(arrObj);
            }
            return lstUserdefinedVals;
        }

        /// <summary>
        /// Save the order of user defined fields
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="appliesTo"></param>
        public void SaveOrders(Dictionary<int, int> orders, cTable appliesTo)
        {
            DateTime lastUpdated;
            base.SaveOrdersToDB(orders, appliesTo, out lastUpdated);
            ResetCache(lastUpdated);
        }

        /// <summary>
        /// Save UDF
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public int SaveUserDefinedField(cUserDefinedField field)
        {
            CurrentUser clsCurrentUser = cMisc.GetCurrentUser();
            DateTime lastUpdated;
            int newId = SaveUserDefinedFieldToDB(clsCurrentUser, field, out lastUpdated);
            ResetCache(lastUpdated);
            return newId;
        }

        /// <summary>
        /// Delete UDF
        /// </summary>
        /// <param name="userdefineid"></param>
        /// <returns></returns>
        public int DeleteUserDefined(int userdefineid)
        {
            CurrentUser clsCurrentUser = cMisc.GetCurrentUser();
            var current = base.GetUserDefinedById(userdefineid);
            DateTime lastUpdated;
            int success = base.DeleteUserDefinedToDB(clsCurrentUser, userdefineid, out lastUpdated);
            ResetCache(lastUpdated);
            return success;
        }

        /// <summary>
        /// Gets userdefined fields for ESR Outbound import mapping matching a particular data type
        /// </summary>
        /// <param name="tableId">User defined table ID</param>
        /// <param name="dataType">ESR Outbound Import data type to match</param>
        /// <returns>List of sFieldBasics records</returns>
        public List<sFieldBasics> GetUserdefinedFieldBasicsByType(Guid tableId, DataType dataType)
        {
            FieldType udfFieldType = FieldType.Text;
            switch (dataType)
            {
                case DataType.intVal:
                    udfFieldType = FieldType.Integer;
                    break;
                case DataType.booleanVal:
                    udfFieldType = FieldType.TickBox;
                    break;
                case DataType.currencyVal:
                    udfFieldType = FieldType.Currency;
                    break;
                case DataType.decimalVal:
                    udfFieldType = FieldType.Number;
                    break;
                case DataType.dateVal:
                    udfFieldType = FieldType.DateTime;
                    break;
            }

            List<sFieldBasics> fields = (from x in this.UserdefinedFields.Values
                                         orderby x.description
                                         where x.fieldtype == udfFieldType && x.table.TableID == tableId
                                         select new sFieldBasics(x.attribute.fieldid, x.attribute.displayname, cFields.ConvertDataTypeToFieldType(dataType), x.attribute.attributename)).ToList();

            // need to get any relationship fields where a match field matches the type

            fields.AddRange(
                (from x in this.UserdefinedFields.Values
                 where
                     x.table.TableID == tableId && x.fieldtype == FieldType.Relationship
                     && (from y in ((cManyToOneRelationship)x.attribute).AutoCompleteMatchFieldIDList where clsFields.GetFieldByID(y).FieldType == cFields.ConvertDataTypeToFieldType(dataType) select y).Any()
                 select new sFieldBasics(x.attribute.fieldid, x.attribute.displayname, cFields.ConvertDataTypeToFieldType(dataType), x.attribute.attributename)).ToList());

            return fields;
        }

        /// <summary>
        /// Creates a grid of report data belonging UDF
        /// </summary>
        /// <param name="userDefinedID">The user defined id for the UDF</param>
        /// <returns>HTML Grid</returns>
        public string[] CreateUdfReportGrid(int userDefinedID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cUserdefinedFields userDefinedFields = new cUserdefinedFields(user.AccountID);
            cUserDefinedField userDefinedField = userDefinedFields.GetUserDefinedById(userDefinedID);

            const string sql =
                "select [ReportID], [Report Name], [Report Owner], [Sub Account] from [dbo].userDefinedFieldsReportColumns";
            cGridNew grid = new cGridNew(user.AccountID, user.EmployeeID, "gridUDFReports", sql);
            grid.WhereClause = string.Format("fieldID = '{0}'", userDefinedField.attribute.fieldid);
            grid.ID = "gridUDFReports";
            grid.Width = new Unit(100, UnitType.Percentage);
            grid.pagesize = GlobalVariables.DefaultModalGridPageSize;
            grid.EnableSorting = false;
            grid.getColumnByName("ReportID").hidden = true;

            return grid.generateGrid();
        }

        /// <summary>
        /// Processes and saves a user defined field.
        /// </summary>
        /// <param name="id">
        /// The id of the user defined field. Should be 0 for new
        /// </param>
        /// <param name="udfName">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="toolTip">
        /// The tooltip text.
        /// </param>
        /// <param name="fieldType">
        /// The fieldType.
        /// </param>
        /// <param name="udfGroupId">
        /// The udf group Id.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="format">
        /// The format applied to the user defined field.
        /// </param>
        /// <param name="itemSpecific">
        /// Whther the user defined field is item specific.
        /// </param>
        /// <param name="allowSearch">
        /// Whether searing is permitted.
        /// </param>
        /// <param name="appliesToTableID">
        /// The table GUID Id the user defined field applied to i.e. cost codes
        /// </param>
        /// <param name="hyperlinkText">
        /// The hyperlink text.
        /// </param>
        /// <param name="hyperlinkPath">
        /// The hyperlink path.
        /// </param>
        /// <param name="relatedTableID">
        /// The related table id, if relationship type
        /// </param>
        /// <param name="mandatory">
        /// Whether it is mandatory
        /// </param>
        /// <param name="acDisplayField">
        /// The ac display field.
        /// </param>
        /// <param name="acMatchFields">
        /// The ac match fields.
        /// </param>
        /// <param name="maxLength">
        /// The max length permitted
        /// </param>
        /// <param name="precision">
        /// The precision.
        /// </param>
        /// <param name="listItems">
        /// The items in the list, if list type
        /// </param>
        /// <param name="defaultValue">
        /// The default value, if yes/no type.
        /// </param>
        /// <param name="maxRows">
        /// The max rows permitted.
        /// </param>
        /// <param name="allowEmployeeToPopulate">
        /// The allow employee to populate.
        /// </param>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        /// <param name="tables">
        /// An instance of <see cref="cTables"/>
        /// </param>
        /// <param name="groupings">
        /// AN instance of <see cref="cUserdefinedFieldGroupings"/>
        /// </param>
        /// <param name="encrypted">True if this user defined field is encrypted.</param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveUserDefinedField(
         int id,
         string udfName,
         string description,
         string toolTip,
         byte fieldtype,
         int udfGroupId,
         int order,
         byte format,
         bool itemSpecific,
         bool allowSearch,
         string appliesToTableID,
         string hyperlinkText,
         string hyperlinkPath,
         string relatedTableID,
         bool mandatory,
         string acDisplayField,
         string[] acMatchFields,
         int maxLength,
         int precision,
         string[] listItems,
         string defaultValue,
         int maxRows,
         bool allowEmployeeToPopulate,
         ICurrentUser currentUser,
         cTables tables,
         cUserdefinedFieldGroupings groupings,
         bool encrypted)
        {               
            cUserdefinedFieldGrouping group = null;
 
            DateTime createdon;
            int createdby;
            DateTime? modifiedon = null;
            int? modifiedby = null;
            Guid fieldid;
            cAttribute attribute = null;
            int? max_length = null;
            if (maxLength > 0) max_length = maxLength;

            Guid tableid;
            bool archived = false;
            cTable relatedTable = null;

            if (Guid.TryParse(relatedTableID, out tableid))
            {
                relatedTable = tables.GetTableByID(tableid);
            }

            FieldType ft = (FieldType)fieldtype;
            AttributeFormat aFormat;
            tableid = new Guid(appliesToTableID);
            bool allowsearch = false;
            var encrypt = false;

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.contracts:
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                    allowsearch = allowSearch;
                    break;
            }

            if (udfGroupId > 0)
            {
                @group = groupings.GetGroupingByID(udfGroupId);
            }

            string attributename = udfName;
            string displayname = udfName;

            if (id > 0)
            {
                cUserDefinedField oldfield = this.GetUserDefinedById(id);
                if (encrypted && !oldfield.Encrypted)
                {
                    encrypt = true;
                }

                if (oldfield.Encrypted && !encrypted)
                {
                    encrypted = true;
                }

                createdon = oldfield.createdon;
                createdby = oldfield.createdby;
                modifiedby = currentUser.EmployeeID;
                modifiedon = DateTime.Now;
                attributename = oldfield.attribute.attributename;
                fieldid = oldfield.attribute.fieldid;
                archived = oldfield.Archived;
            }
            else
            {
                createdon = DateTime.Now;
                createdby = currentUser.EmployeeID;
                fieldid = Guid.Empty;
                encrypt = encrypted;
            }

            if (order == 0)
            {
                order = this.GetNextOrder();
            }

            switch (ft)
            {
                case FieldType.Currency:
                    attribute = new cNumberAttribute(
                        id,
                        attributename,
                        displayname,
                        description,
                        toolTip,
                        mandatory,
                        ft,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        2,
                        fieldid,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false);
                    break;
                case FieldType.DateTime:
                    aFormat = (AttributeFormat)format;
                    attribute = new cDateTimeAttribute(
                        id,
                        attributename,
                        displayname,
                        description,
                        toolTip,
                        mandatory,
                        ft,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        aFormat,
                        fieldid,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false);
                    break;
                case FieldType.Integer:
                    attribute = new cNumberAttribute(
                        id,
                        attributename,
                        displayname,
                        description,
                        toolTip,
                        mandatory,
                        ft,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        0,
                        fieldid,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false);
                    break;
                case FieldType.LargeText:
                    aFormat = (AttributeFormat)format;
                    attribute = new cTextAttribute(
                        id,
                        attributename,
                        displayname,
                        description,
                        toolTip,
                        mandatory,
                        ft,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        max_length,
                        aFormat,
                        fieldid,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false);
                    break;
                case FieldType.Number:
                    byte bytePrecision = Convert.ToByte(precision);

                    attribute = new cNumberAttribute(
                        id,
                        attributename,
                        displayname,
                        description,
                        toolTip,
                        mandatory,
                        ft,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        bytePrecision,
                        fieldid,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false);
                    break;
                case FieldType.Text:
                    aFormat = (AttributeFormat)format;
                    attribute = new cTextAttribute(
                        id,
                        attributename,
                        displayname,
                        description,
                        toolTip,
                        mandatory,
                        ft,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        max_length,
                        aFormat,
                        fieldid,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false);
                    break;
                case FieldType.TickBox:
                    attribute = new cTickboxAttribute(
                        id,
                        attributename,
                        displayname,
                        description,
                        toolTip,
                        mandatory,
                        ft,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        defaultValue,
                        fieldid,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false);
                    break;
                case FieldType.List:
                    aFormat = AttributeFormat.ListStandard; // List Wide not implemented to udf at present
                    var list = new SortedList<int, cListAttributeElement>();
                    int valID = 0;
                    var js = new JavaScriptSerializer();

                    for (int i = 0; i < listItems.GetLength(0); i++)
                    {
                        var arrVal = js.Deserialize<CustomEntityListItem>(listItems[i]);
                        arrVal.elementOrder = i;
                        list.Add(i, arrVal.ToListAttributeElement());
                    }

                    attribute = new cListAttribute(
                        id,
                        attributename,
                        displayname,
                        description,
                        toolTip,
                        mandatory,
                        ft,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        list,
                        fieldid,
                        false,
                        false,
                        aFormat,
                        false,
                        false,
                        false,
                        false,
                        false);
                    break;
                case FieldType.Hyperlink:
                    attribute = new cHyperlinkAttribute(
                        id,
                        attributename,
                        displayname,
                        description,
                        toolTip,
                        mandatory,
                        ft,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        fieldid,
                        false,
                        false,
                        hyperlinkText,
                        hyperlinkPath,
                        false,
                        false,
                        false,
                        false,
                        false);
                    break;
                case FieldType.DynamicHyperlink:
                    attribute = new cHyperlinkAttribute(
                        id,
                        attributename,
                        displayname,
                        description,
                        toolTip,
                        mandatory,
                        ft,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        fieldid,
                        false,
                        false,
                        string.Empty,
                        string.Empty,
                        false,
                        false,
                        false,
                        false,
                        false);
                    break;
                case FieldType.Relationship:
                    Guid displayFieldID = Guid.Empty;
                    Guid.TryParse(acDisplayField, out displayFieldID);           
                    List<Guid> lgMatchFields = null;

                    if (acMatchFields.Length > 0)
                    {
                        Guid tmp = Guid.Empty;

                        lgMatchFields = new List<Guid>();
                        foreach (string x in acMatchFields)
                        {
                            if (Guid.TryParse(x, out tmp))
                            {
                                lgMatchFields.Add(tmp);
                            }
                        }
                    }

                    attribute = new cManyToOneRelationship(
                        id,
                        attributename,
                        displayname,
                        description,
                        toolTip,
                        mandatory,
                        false,
                        createdon,
                        createdby,
                        modifiedon,
                        modifiedby,
                        relatedTable,
                        fieldid,
                        false,
                        false,
                        false,
                        null,
                        displayFieldID,
                        lgMatchFields,
                        maxRows,
                        new List<Guid>(),
                        new SortedList<int, FieldFilter>(),
                        false,
                        null);
                    break;
            }

            cTable tbl = tables.GetTableByID(tableid);
            cTable udftbl = tables.GetTableByID(tbl.UserDefinedTableID);
            cUserDefinedField userdefined = new cUserDefinedField(
                id,
                udftbl,
                order,
                null,
                createdon,
                createdby,
                modifiedon,
                modifiedby,
                attribute,
                @group,
                archived,
                itemSpecific,
                allowsearch,
                allowEmployeeToPopulate,
                encrypted);

            if (this.AlreadyExists(userdefined) == false)
            {
                int newUDF_ID = this.SaveUserDefinedField(userdefined);
                if (encrypt)
                {
                    var encryptor = new AttributeEncryptor(currentUser);
                    attribute.attributeid = newUDF_ID;
                    encryptor.Encrypt(attribute, tbl.UserDefinedTableID);
                }

                return newUDF_ID;
            }

            return -1;
        }

    }
}

