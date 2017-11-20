using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using AjaxControlToolkit;

namespace Spend_Management
{
    /// <summary>
    /// cFieldGenerator class
    /// </summary>
    public class cFieldGenerator
    {
        private string closeSpan = "</span>";
        private string ttipSpan = "<span class=\"inputtooltip\">";
        private string iconSpan = "<span class=\"inputicon\">";
        private string inputSpan = "<span class=\"inputs\">";
        private string validatorSpan = "<span class=\"inputvalidatorfield\">";

        #region properties
        /// <summary>
        /// Current Tab Index
        /// </summary>
        private short nTabIndex;
        /// <summary>
        /// Gets or sets the current tab index
        /// </summary>
        private short TabIndex
        {
            get { return nTabIndex; }
            set { nTabIndex = value; }
        }
        /// <summary>
        /// Validation group name
        /// </summary>
        private string sValidationGroup;
        /// <summary>
        /// Gets or sets the validation group name
        /// </summary>
        private string ValidationGroup
        {
            get { return sValidationGroup; }
            set { sValidationGroup = value; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validationGroup">Validation group name</param>
        public cFieldGenerator(string validationGroup)
        {
            TabIndex = 1;
            ValidationGroup = validationGroup;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validationGroup">Validation group name</param>
        /// <param name="startTabIndex"></param>
        public cFieldGenerator(string validationGroup, short startTabIndex)
        {
            TabIndex = startTabIndex;
            ValidationGroup = validationGroup;
        }

        /// <summary>
        /// Obtain a required field validator control
        /// </summary>
        /// <param name="targetControlId">Control ID</param>
        /// <param name="targetFieldLabel">Target field description for error message</param>
        /// <returns>RequiredFieldValidator control</returns>
        private RequiredFieldValidator GetRequiredValidator(string targetControlId, string targetFieldLabel)
        {
            RequiredFieldValidator req = new RequiredFieldValidator();
            req.ID = "req" + targetControlId;
            req.Text = "*";
            req.ErrorMessage = targetFieldLabel + " is mandatory";
            req.ControlToValidate = targetControlId;
            req.ValidationGroup = ValidationGroup;
            //req.SetFocusOnError = true;

            return req;
        }

        /// <summary>
        /// Obtain a Text Box control
        /// </summary>
        /// <param name="cntlID">Control ID</param>
        /// <param name="contentdata">Any data to populate into the control</param>
        /// <param name="maxlength">Maximum number of characters permitted for entry</param>
        /// <returns>TextBox control</returns>
        public TextBox GetTextBoxControl(string cntlID, string contentdata, int maxlength, string onEnterPressAction)
        {
            TextBox txt = new TextBox();
            txt.ID = "txt" + cntlID;
            txt.Text = contentdata;
            txt.CssClass = "fillspan";
            txt.ValidationGroup = ValidationGroup;

            if (onEnterPressAction != string.Empty)
            {
                txt.Attributes.Add("onkeypress", "return RunOnEnter(event, '" + onEnterPressAction + "');");
            }

            if (maxlength > 0)
            {
                txt.MaxLength = maxlength;
            }

            txt.TabIndex = TabIndex++;
            return txt;
        }

        /// <summary>
        /// Gets a checkbox control
        /// </summary>
        /// <param name="cntlID">Control ID</param>
        /// <param name="isChecked">Initial selection (true = checked)</param>
        /// <returns>Checkbox Control</returns>
        public CheckBox GetCheckBoxControl(string cntlID, bool isChecked, string onEnterPressAction)
        {
            CheckBox chk = new CheckBox();
            chk.ID = "chk" + cntlID;
            chk.CssClass = "fillspan";
            chk.ValidationGroup = ValidationGroup;
            chk.TabIndex = TabIndex++;
            chk.Checked = isChecked;

            if (onEnterPressAction != string.Empty)
            {
                chk.Attributes.Add("onkeypress", "return RunOnEnter(event, '" + onEnterPressAction + "');");
            }

            return chk;
        }

        /// <summary>
        /// Gets a dropdown list control
        /// </summary>
        /// <param name="cntlID">Control ID</param>
        /// <param name="items">Sorted List collection of items to populate list control with</param>
        /// <param name="addNoneSelection">TRUE to include [None] option at head of list</param>
        /// <param name="selectedId">Initial selection (NULL if no selection)</param>
        /// <returns>Dropdown List Control</returns>
        public DropDownList GetDDListControl(string cntlID, SortedList<object, string> items, bool addNoneSelection, int? selectedId, string onEnterPressAction)
        {
            List<ListItem> converted = new List<ListItem>();
            SortedList<string, int> sorted = new SortedList<string, int>();

            foreach (int itemKey in items.Keys)
            {
                sorted.Add((string)items[itemKey], itemKey);
            }

            foreach (KeyValuePair<string, int> kvp in sorted)
            {
                ListItem item = new ListItem(kvp.Key, kvp.Value.ToString());
                converted.Add(item);
            }

            return GetDDListControl(cntlID, converted.ToArray(), addNoneSelection, selectedId, onEnterPressAction);
        }

        /// <summary>
        /// Gets a dropdown list control
        /// </summary>
        /// <param name="cntlID">Control ID</param>
        /// <param name="items">Array of ListItem to populate list control with</param>
        /// <param name="addNoneSelection">TRUE to include [None] option at head of list</param>
        /// <param name="selectedId">Initial selection (NULL if no selection)</param>
        /// <returns>Dropdown List Control</returns>
        public DropDownList GetDDListControl(string cntlID, ListItem[] items, bool addNoneSelection, int? selectedId, string onEnterPressAction)
        {
            DropDownList ddl = new DropDownList();
            ddl.ID = "lst" + cntlID;
            ddl.Items.AddRange(items);
            ddl.TabIndex = TabIndex++;

            if (onEnterPressAction != string.Empty)
            {
                ddl.Attributes.Add("onkeypress", "return RunOnEnter(event, '" + onEnterPressAction + "');");
            }

            if (addNoneSelection)
            {
                ddl.Items.Insert(0, new ListItem("[None]", "0"));
            }

            if (selectedId.HasValue)
            {
                ddl.Items.FindByValue(selectedId.ToString()).Selected = true;
            }
            else
            {
                //if (addNoneSelection)
                //{
                //    // can only select 0 if using none selection, otherwise no selection set
                //    ddl.Items.FindByValue(selectedId.Value.ToString()).Selected = true;
                //}
            }
            return ddl;
        }

        /// <summary>
        /// Constructs a label, text field, icon, tooltip and validator into placeholder to rendering to the screen
        /// </summary>
        /// <param name="cntlID">Control ID</param>
        /// <param name="field">Field being rendered from the fields_base</param>
        /// <param name="displayData">Any data to be populated into the control</param>
        /// <param name="helpID">Tooltip Help ID</param>
        /// <param name="helpArea">Tooltip Area (sm, fw, ex)</param>
        /// <returns>Placeholder containing collection of controls</returns>
        public PlaceHolder GetCharFieldEntry(string cntlID, cField field, string displayData, string helpID, string helpArea, string onEnterPressAction)
        {
            PlaceHolder ph = new PlaceHolder();
            string data = "";
            StringBuilder sb = new StringBuilder();

            if (displayData != null)
            {
                data = displayData;
            }

            // character or text box
            Literal cell = new Literal();
            Label lbl = new Label();
            lbl.ID = "lbl" + cntlID;
            lbl.Text = field.Description;

            if (field.Mandatory)
            {
                lbl.CssClass = "mandatory";
                lbl.Text += " *";
            }

            TextBox txt = GetTextBoxControl(cntlID, data, field.Length, onEnterPressAction);
            lbl.AssociatedControlID = txt.ID;

            // add cell for validation if necessary
            RequiredFieldValidator req = null;
            if (field.Mandatory)
            {
                req = GetRequiredValidator(txt.ID, lbl.Text);
            }

            ph.Controls.Add(lbl);

            cell = new Literal();
            cell.Text = inputSpan;
            ph.Controls.Add(cell);
            ph.Controls.Add(txt);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = "&nbsp;" + iconSpan;
            ph.Controls.Add(cell);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = ttipSpan;
            ph.Controls.Add(cell);
            cell = new Literal();
            if (!string.IsNullOrEmpty(helpID))
            {
                cell.Text = "<img id=\"imgtooltip" + cntlID + "\" onclick=\"SEL.Tooltip.Show('" + helpID + "', '" + helpArea + "', 'this" + ");\" src=\"/shared/images/icons/16/plain/tooltip.png\" alt=\"\" />"; 
            }
            else
            {
                cell.Text = "&nbsp;";
            }
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = validatorSpan;
            ph.Controls.Add(cell);

            if (req != null)
            {
                ph.Controls.Add(req);
            }
            else
            {
                cell = new Literal();
                cell.Text = "&nbsp;";
                ph.Controls.Add(cell);
            }
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            return ph;
        }

        /// <summary>
        /// Constructs a label, date field, calendar icon, tooltip and validator into placeholder to rendering to the screen
        /// </summary>
        /// <param name="cntlID">Control ID</param>
        /// <param name="field">Field being rendered from the fields_base</param>
        /// <param name="displayData">Any data to be populated into the control</param>
        /// <param name="helpID">Tooltip Help ID</param>
        /// <param name="helpArea">Tooltip Area (sm, fw, ex)</param>
        /// <returns>Placeholder containing collection of controls</returns>
        public PlaceHolder GetDateFieldEntry(string cntlID, cField field, DateTime? displayData, string helpID, string helpArea, string onEnterPressAction)
        {
            PlaceHolder ph = new PlaceHolder();
            string data = "";
            StringBuilder sb = new StringBuilder();

            if (displayData.HasValue)
            {
                data = displayData.Value.ToShortDateString();
            }

            // character or text box
            Literal cell = new Literal();
            Label lbl = new Label();
            lbl.ID = "lbl" + cntlID;
            lbl.Text = field.Description;

            if (field.Mandatory)
            {
                lbl.CssClass = "mandatory";
                lbl.Text += " *";
            }

            TextBox txt = GetTextBoxControl(cntlID, data, field.Length, onEnterPressAction);
            lbl.AssociatedControlID = txt.ID;

            // add cell for validation if necessary
            RequiredFieldValidator req = null;
            if (field.Mandatory)
            {
                req = GetRequiredValidator(txt.ID, lbl.Text);
            }

            ph.Controls.Add(lbl);

            cell = new Literal();
            cell.Text = inputSpan;
            ph.Controls.Add(cell);
            ph.Controls.Add(txt);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            Image calimg = new Image();
            calimg.ID = "img" + cntlID;

            CalendarExtender calex = new CalendarExtender();
            calex.ID = "calex" + cntlID;
            calex.PopupButtonID = calimg.ID;
            calex.Format = "dd/MM/yyyy";
            calex.PopupPosition = CalendarPosition.Right;
            calex.TargetControlID = txt.ID;

            cell = new Literal();
            cell.Text = iconSpan;
            ph.Controls.Add(cell);
            ph.Controls.Add(calimg);
            ph.Controls.Add(calex);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = ttipSpan;
            ph.Controls.Add(cell);
            cell = new Literal();
            if (!string.IsNullOrEmpty(helpID))
            {
                cell.Text = "<img id=\"imgtooltip" + cntlID + "\" onclick=\"SEL.Tooltip.Show('" + helpID + "', '" + helpArea + "', 'this" + ");\" src=\"/shared/images/icons/16/plain/tooltip.png\" alt=\"\" />";
            }
            else
            {
                cell.Text = "&nbsp;";
            }
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = validatorSpan;
            ph.Controls.Add(cell);

            if (req != null)
            {
                ph.Controls.Add(req);
            }
            else
            {
                cell = new Literal();
                cell.Text = "&nbsp;";
                ph.Controls.Add(cell);
            }
            // add date validators
            CompareValidator cmpDate = new CompareValidator();
            cmpDate.ID = "cmpIsDate";
            cmpDate.Operator = ValidationCompareOperator.DataTypeCheck;
            cmpDate.Type = ValidationDataType.Date;
            cmpDate.ValidationGroup = ValidationGroup;
            cmpDate.ControlToValidate = txt.ID;
            cmpDate.Text = "*";
            cmpDate.ErrorMessage = "Date supplied for " + field.Description + " is invalid";
            ph.Controls.Add(cmpDate);

            cmpDate = new CompareValidator();
            cmpDate.ID = "cmpMinDate";
            cmpDate.Type = ValidationDataType.Date;
            cmpDate.ValidationGroup = ValidationGroup;
            cmpDate.ControlToValidate = txt.ID;
            cmpDate.Text = "*";
            cmpDate.ErrorMessage = "Minimum date permitted for field " + field.Description + " is 01/01/1900";
            cmpDate.Operator = ValidationCompareOperator.GreaterThanEqual;
            cmpDate.ValueToCompare = "01/01/1900";
            ph.Controls.Add(cmpDate);

            cmpDate = new CompareValidator();
            cmpDate.ID = "cmpMaxDate";
            cmpDate.Type = ValidationDataType.Date;
            cmpDate.ValidationGroup = ValidationGroup;
            cmpDate.ControlToValidate = txt.ID;
            cmpDate.Text = "*";
            cmpDate.ErrorMessage = "Maximum date permitted for field " + field.Description + " is 31/12/3000";
            cmpDate.Operator = ValidationCompareOperator.LessThanEqual;
            cmpDate.ValueToCompare = "31/12/3000";
            ph.Controls.Add(cmpDate);

            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            return ph;
        }

        /// <summary>
        /// Constructs a label, checkbox field, tooltip into placeholder to rendering to the screen
        /// </summary>
        /// <param name="cntlID">Control ID</param>
        /// <param name="field">Field being rendered from the fields_base</param>
        /// <param name="displayData">Current selection for the checkbox</param>
        /// <param name="helpID">Tooltip Help ID</param>
        /// <param name="helpArea">Tooltip Area (sm, fw, ex)</param>
        /// <returns>Placeholder containing collection of controls</returns>
        public PlaceHolder GetCheckBoxFieldEntry(string cntlID, cField field, bool displayData, string helpID, string helpArea, string onEnterPressAction)
        {
            PlaceHolder ph = new PlaceHolder();
            StringBuilder sb = new StringBuilder();

            // character or text box
            Literal cell = new Literal();
            Label lbl = new Label();
            lbl.ID = "lbl" + cntlID;
            lbl.Text = field.Description;

            CheckBox chk = GetCheckBoxControl(cntlID, displayData, onEnterPressAction);
            lbl.AssociatedControlID = chk.ID;
            ph.Controls.Add(lbl);

            cell = new Literal();
            cell.Text = inputSpan;
            ph.Controls.Add(cell);
            ph.Controls.Add(chk);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = "&nbsp;" + iconSpan;
            ph.Controls.Add(cell);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = ttipSpan;
            ph.Controls.Add(cell);
            cell = new Literal();
            if (!string.IsNullOrEmpty(helpID))
            {
                cell.Text = "<img id=\"imgtooltip" + cntlID + "\" onclick=\"SEL.Tooltip.Show('" + helpID + "', '" + helpArea + "', 'this" + ");\" src=\"/shared/images/icons/16/plain/tooltip.png\" alt=\"\" />";
            }
            else
            {
                cell.Text = "&nbsp;";
            }
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = validatorSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = "&nbsp;";
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            return ph;
        }

        /// <summary>
        /// Constructs a label, integer field, tooltip and validator into placeholder to rendering to the screen
        /// </summary>
        /// <param name="cntlID">Control ID</param>
        /// <param name="field">Field being rendered from the fields_base</param>
        /// <param name="displayData">Any data to be populated into the control (NULL for none)</param>
        /// <param name="helpID">Tooltip Help ID</param>
        /// <param name="helpArea">Tooltip Area (sm, fw, ex)</param>
        /// <returns>Placeholder containing collection of controls</returns>
        public PlaceHolder GetNumberFieldEntry(string cntlID, cField field, int? displayData, string helpID, string helpArea, string onEnterPressAction)
        {
            PlaceHolder ph = new PlaceHolder();
            string data = "";
            StringBuilder sb = new StringBuilder();

            if (displayData.HasValue)
            {
                data = displayData.Value.ToString();
            }

            // character or text box
            Literal cell = new Literal();
            Label lbl = new Label();
            lbl.ID = "lbl" + cntlID;
            lbl.Text = field.Description;

            if (field.Mandatory)
            {
                lbl.CssClass = "mandatory";
                lbl.Text += " *";
            }

            TextBox txt = GetTextBoxControl(cntlID, data, field.Length, onEnterPressAction);
            lbl.AssociatedControlID = txt.ID;

            // add cell for validation if necessary
            RequiredFieldValidator req = null;
            if (field.Mandatory)
            {
                req = GetRequiredValidator(txt.ID, lbl.Text);
            }

            ph.Controls.Add(lbl);

            cell = new Literal();
            cell.Text = inputSpan;
            ph.Controls.Add(cell);
            ph.Controls.Add(txt);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = "&nbsp;" + iconSpan;
            ph.Controls.Add(cell);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = ttipSpan;
            ph.Controls.Add(cell);
            cell = new Literal();
            if (!string.IsNullOrEmpty(helpID))
            {
                cell.Text = "<img id=\"imgtooltip" + cntlID + "\" onclick=\"SEL.Tooltip.Show('" + helpID + "', '" + helpArea + "', 'this" + ");\" src=\"/shared/images/icons/16/plain/tooltip.png\" alt=\"\" />";
            }
            else
            {
                cell.Text = "&nbsp;";
            }
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = validatorSpan;
            ph.Controls.Add(cell);

            if (req != null)
            {
                ph.Controls.Add(req);
            }

            // add number validators
            CompareValidator cmp = new CompareValidator();
            cmp.ID = "cmp" + cntlID;
            cmp.ControlToValidate = txt.ID;
            cmp.Operator = ValidationCompareOperator.DataTypeCheck;
            cmp.Type = ValidationDataType.Integer;
            cmp.ErrorMessage = "Numeric (Integer) data only is permitted in the " + field.Description + " field";
            cmp.Text = "*";
            cmp.ValidationGroup = ValidationGroup;
            ph.Controls.Add(cmp);

            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            return ph;
        }

        /// <summary>
        /// Constructs a label, decimal field, tooltip and validators into placeholder to rendering to the screen
        /// </summary>
        /// <param name="cntlID">Control ID</param>
        /// <param name="field">Field being rendered from the fields_base</param>
        /// <param name="displayData">Any data to be populated into the control</param>
        /// <param name="helpID">Tooltip Help ID</param>
        /// <param name="helpArea">Tooltip Area (sm, fw, ex)</param>
        /// <returns>Placeholder containing collection of controls</returns>
        public PlaceHolder GetDecimalFieldEntry(string cntlID, cField field, double? displayData, string helpID, string helpArea, string onEnterPressAction)
        {
            PlaceHolder ph = new PlaceHolder();
            string data = "";
            StringBuilder sb = new StringBuilder();

            if (displayData.HasValue)
            {
                data = displayData.Value.ToString();
            }

            // character or text box
            Literal cell = new Literal();
            Label lbl = new Label();
            lbl.ID = "lbl" + cntlID;
            lbl.Text = field.Description;

            if (field.Mandatory)
            {
                lbl.CssClass = "mandatory";
                lbl.Text += " *";
            }

            TextBox txt = GetTextBoxControl(cntlID, data, field.Length, onEnterPressAction);
            lbl.AssociatedControlID = txt.ID;

            // add cell for validation if necessary
            RequiredFieldValidator req = null;
            if (field.Mandatory)
            {
                req = GetRequiredValidator(txt.ID, lbl.Text);
            }

            ph.Controls.Add(lbl);

            cell = new Literal();
            cell.Text = inputSpan;
            ph.Controls.Add(cell);
            ph.Controls.Add(txt);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = "&nbsp;" + iconSpan;
            ph.Controls.Add(cell);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = ttipSpan;
            ph.Controls.Add(cell);
            cell = new Literal();
            if (!string.IsNullOrEmpty(helpID))
            {
                cell.Text = "<img id=\"imgtooltip" + cntlID + "\" onclick=\"SEL.Tooltip.Show('" + helpID + "', '" + helpArea + "', 'this" + ");\" src=\"/shared/images/icons/16/plain/tooltip.png\" alt=\"\" />";
            }
            else
            {
                cell.Text = "&nbsp;";
            }
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = validatorSpan;
            ph.Controls.Add(cell);

            if (req != null)
            {
                ph.Controls.Add(req);
            }

            CompareValidator cmp = new CompareValidator();
            cmp.ID = "cmp" + cntlID;
            cmp.ControlToValidate = txt.ID;
            cmp.Operator = ValidationCompareOperator.DataTypeCheck;
            cmp.Type = ValidationDataType.Double;
            cmp.Text = "*";
            cmp.ErrorMessage = "The numeric amount specified for field " + field.Description + " is invalid";
            cmp.ValidationGroup = ValidationGroup;
            ph.Controls.Add(cmp);

            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            return ph;
        }

        /// <summary>
        /// Constructs a label, dropdown list field, tooltip and validators into placeholder to rendering to the screen
        /// </summary>
        /// <param name="cntlID">Control ID</param>
        /// <param name="field">Field being rendered from the fields_base</param>
        /// <param name="selectedId">ID of initial selection (NULL if none)</param>
        /// <param name="helpID">Tooltip Help ID</param>
        /// <param name="helpArea">Tooltip Area (sm, fw, ex)</param>
        /// <returns>Placeholder containing collection of controls</returns>
        public PlaceHolder GetDDListFieldEntry(string cntlID, cField field, int? selectedId, string helpID, string helpArea, string onEnterPressAction)
        {
            PlaceHolder ph = new PlaceHolder();

            // character or text box
            Literal cell = new Literal();
            Label lbl = new Label();
            lbl.ID = "lbl" + cntlID;
            lbl.Text = field.Description;

            if (field.Mandatory)
            {
                lbl.CssClass = "mandatory";
                lbl.Text += " *";
            }

            DropDownList lst = GetDDListControl(cntlID, field.ListItems, !field.Mandatory, selectedId, onEnterPressAction);
            lbl.AssociatedControlID = lst.ID;

            // add cell for validation if necessary
            RequiredFieldValidator req = null;
            if (field.Mandatory)
            {
                req = GetRequiredValidator(lst.ID, lbl.Text);
            }

            ph.Controls.Add(lbl);

            cell = new Literal();
            cell.Text = inputSpan;
            ph.Controls.Add(cell);
            ph.Controls.Add(lst);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = "&nbsp;" + iconSpan;
            ph.Controls.Add(cell);
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = ttipSpan;
            ph.Controls.Add(cell);
            cell = new Literal();
            if (!string.IsNullOrEmpty(helpID))
            {
                cell.Text = "<img id=\"imgtooltip" + cntlID + "\" onclick=\"SEL.Tooltip.Show('" + helpID + "', '" + helpArea + "', 'this" + ");\" src=\"/shared/images/icons/16/plain/tooltip.png\" alt=\"\" />";
            }
            else
            {
                cell.Text = "&nbsp;";
            }
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            cell = new Literal();
            cell.Text = validatorSpan;
            ph.Controls.Add(cell);

            if (req != null)
            {
                ph.Controls.Add(req);
            }

            if (field.Mandatory)
            {
                CompareValidator cmp = new CompareValidator();
                cmp.ID = "cmp" + cntlID;
                cmp.ControlToValidate = lst.ID;
                cmp.Operator = ValidationCompareOperator.GreaterThan;
                cmp.ValueToCompare = "0";
                cmp.Text = "*";
                cmp.ErrorMessage = "The numeric amount specified for field " + field.Description + " is invalid";
                cmp.ValidationGroup = ValidationGroup;
                ph.Controls.Add(cmp);
            }
            else
            {
                cell = new Literal();
                cell.Text = "&nbsp;";
                ph.Controls.Add(cell);
            }
            cell = new Literal();
            cell.Text = closeSpan;
            ph.Controls.Add(cell);

            return ph;
        }
    }
}
