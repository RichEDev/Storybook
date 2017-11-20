using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using SpendManagementLibrary;
using Spend_Management;

namespace FWClasses
{
    public class cControls
    {
        private cFWSettings fws;
        private UserInfo uinfo;
        private short tabIdx;
        private short curTabIdx;

        public cControls(cFWSettings cFWS, UserInfo cUInfo, short tabStartIdx)
        {
            SetTabIndex(tabStartIdx);
            fws = cFWS;
            uinfo = cUInfo;
        }

        public void SetTabIndex(short tabIndex)
        {
            curTabIdx = tabIndex;
            tabIdx = tabIndex;
        }

        private void GetRequiredValidator(ref TableCell cell, string targetControlId, string targetFieldLabel)
        {
            RequiredFieldValidator req = new RequiredFieldValidator();
            req.ID = "reqUF" + targetControlId;
            req.Text = "*";
            req.ErrorMessage = targetFieldLabel + " is mandatory";
            req.ControlToValidate = targetControlId;
            req.SetFocusOnError = true;

            ValidatorCalloutExtender reqex = new ValidatorCalloutExtender();
            reqex.ID = "reqex" + targetControlId;
            reqex.TargetControlID = req.ID;

            cell.Controls.Add(req);
            cell.Controls.Add(reqex);
        }

        public TextBox GetTextBoxControl(string cntlID, string contentdata, string tooltip, int maxlength, bool isMultiRow, int numRows)
        {
            TextBox txt = new TextBox();
            txt.ID = cntlID;
            txt.Text = contentdata;
            if (isMultiRow)
            {
                txt.TextMode = TextBoxMode.MultiLine;
                txt.Rows = numRows;
            }

            if (txt.MaxLength > 0)
            {
                txt.MaxLength = maxlength;
            }
            txt.TabIndex = curTabIdx;
            curTabIdx++;
            return txt;
        }

        public TextBox GetCharacterControl(string cntlID, string contentdata, string tooltip)
        {
            TextBox txt = new TextBox();
            txt.ID = cntlID;
            txt.Text = contentdata;
            txt.TabIndex = curTabIdx;
            curTabIdx++;
            return txt;
        }

        public CheckBox GetCheckBoxControl(string cntlID, bool isChecked, string tooltip)
        {
            CheckBox chk = new CheckBox();
            chk.ID = cntlID;
            chk.TabIndex = curTabIdx;
            chk.Checked = isChecked;

            return chk;
        }

        public DropDownList GetDDListControl(string cntlID, ListItem[] items, bool addNoneSelection, int selectedId)
        {
            DropDownList ddl = new DropDownList();
            ddl.ID = cntlID;
            ddl.Items.AddRange(items);
            if (addNoneSelection)
            {
                ddl.Items.Insert(0, new ListItem("[None]", "0"));
            }

            if (selectedId > 0)
            {
                ddl.Items.FindByValue(selectedId.ToString()).Selected = true;
            }
            else
            {
                if (addNoneSelection)
                {
                    // can only select 0 if using none selection, otherwise no selection set
                    ddl.Items.FindByValue(selectedId.ToString()).Selected = true;
                }
            }
            return ddl;
        }

        public System.Web.UI.WebControls.TableCell[] GetCharCell(string cntlID, string labeltext, string displayData, string tooltip, bool isMandatory, UserFieldType fieldtype, int maxlength)
        {
            TableCell[] cells = new TableCell[2];
            string data;
            if (displayData == null)
            {
                data = "";
            }
            else
            {
                data = displayData;
            }

            // character or text box
            cells[0] = new TableCell();

            if (isMandatory)
            {
                cells[0].CssClass = "labeltdmand";
                cells[0].Text = labeltext + " (*)";
            }
            else
            {
                cells[0].CssClass = "labeltd";
                cells[0].Text = labeltext;
            }

            cells[1] = new TableCell();
            cells[1].CssClass = "inputtd";
            
            if (fieldtype == UserFieldType.Text)
            {
                // text field
                cells[1].Controls.Add(GetTextBoxControl(cntlID, data, tooltip, maxlength, true, 2));
            }
            else
            {
                // just char field
                cells[1].Controls.Add(GetTextBoxControl(cntlID, data, tooltip, maxlength, false, 0));
            }

            // add cell for validation if necessary
            if (isMandatory)
            {
                GetRequiredValidator(ref cells[1], cntlID, labeltext);
            }

            return cells;
        }

        public System.Web.UI.WebControls.TableCell[] GetDateCell(string cntlID, string labeltext, DateTime displayData, string tooltip, bool isMandatory)
        {
            // date
            TableCell[] cells = new TableCell[2];
            string data;
            if (displayData == DateTime.MinValue)
            {
                data = "";
            }
            else
            {
                data = displayData.ToShortDateString();
            }

            // character or text box
            cells[0] = new TableCell();
            if (isMandatory)
            {
                cells[0].CssClass = "labeltdmand";
                cells[0].Text = cntlID + " (*)";
            }
            else
            {
                cells[0].CssClass = "labeltd";
                cells[0].Text = cntlID;
            }
            cells[1] = new TableCell();
            cells[1].CssClass = "inputtd";
            TextBox txt = GetTextBoxControl(cntlID, data, tooltip, 10, false, 0);
            cells[1].Controls.Add(txt);

            CalendarExtender calex = new CalendarExtender();
            calex.ID = "calex" + txt.ID;
            calex.TargetControlID = txt.ID;
			calex.Format = cDef.DATE_FORMAT;

            cells[1].Controls.Add(calex);

            CompareValidator cmp = new CompareValidator();
            cmp.ID = "cmp" + txt.ID;
            cmp.ErrorMessage = "Invalid Date format entered";
            cmp.Text = "*";
            cmp.Operator = ValidationCompareOperator.DataTypeCheck;
            cmp.ControlToValidate = txt.ID;
            cmp.SetFocusOnError = true;
            cmp.Type = ValidationDataType.Date;
            cells[1].Controls.Add(cmp);

            ValidatorCalloutExtender cmpex = new ValidatorCalloutExtender();
            cmpex.ID = "cmpex" + txt.ID;
            cmpex.TargetControlID = cmp.ID;
            cells[1].Controls.Add(cmpex);

            // add cell for validation if necessary
            if (isMandatory)
            {
                GetRequiredValidator(ref cells[1], cntlID, labeltext);
            }

            return cells;
        }

        public System.Web.UI.WebControls.TableCell[] GetCheckBoxCell(string cntlID, string labeltext, bool displayData, string tooltip)
        {
            // chk box
            TableCell[] cells = new TableCell[2];

            // character or text box
            cells[0] = new TableCell();
            cells[0].CssClass = "labeltd";
            cells[0].Text = labeltext;

            cells[1] = new TableCell();
            cells[1].CssClass = "inputtd";
            cells[1].Controls.Add(GetCheckBoxControl(cntlID, displayData, tooltip));

            return cells;
        }

        public System.Web.UI.WebControls.TableCell[] GetNumberCell(string cntlID, string labeltext, int displayData, string tooltip, bool isMandatory)
        {
            // number or decimal
            TableCell[] cells = new TableCell[2];

            // character or text box
            cells[0] = new TableCell();
            if (isMandatory)
            {
                cells[0].CssClass = "labeltdmand";
                cells[0].Text = labeltext + " (*)";
            }
            else
            {
                cells[0].CssClass = "labeltd";
                cells[0].Text = labeltext;
            }

            cells[1] = new TableCell();
            cells[1].CssClass = "inputtd";
            TextBox txt = GetCharacterControl(cntlID, displayData.ToString(), tooltip);
            cells[1].Controls.Add(txt);

            CompareValidator cmp = new CompareValidator();
            cmp.ID = "cmp" + txt.ID;
            cmp.ControlToValidate = txt.ID;
            cmp.Operator = ValidationCompareOperator.DataTypeCheck;
            cmp.Type = ValidationDataType.Integer;
            cmp.ErrorMessage = "Numeric (Integer) data only is permitted in this field";
            cmp.Text = "*";
            cmp.SetFocusOnError = true;
            cells[1].Controls.Add(cmp);

            ValidatorCalloutExtender cmpex = new ValidatorCalloutExtender();
            cmpex.ID = "cmpex" + txt.ID;
            cmpex.TargetControlID = cmp.ID;
            cells[1].Controls.Add(cmpex);

            // add cell for validation if necessary
            if (isMandatory)
            {
                GetRequiredValidator(ref cells[1], cntlID, labeltext);
            }

            return cells;
        }

        public System.Web.UI.WebControls.TableCell[] GetDecimalCell(string cntlID, string labeltext, double displayData, string tooltip, bool isMandatory)
        {
            // number or decimal
            TableCell[] cells = new TableCell[2];

            // character or text box
            cells[0] = new TableCell();
            if (isMandatory)
            {
                cells[0].CssClass = "labeltdmand";
                cells[0].Text = labeltext + " (*)";
            }
            else
            {
                cells[0].CssClass = "labeltd";
                cells[0].Text = labeltext;
            }

            cells[1] = new TableCell();
            cells[1].CssClass = "inputtd";
            TextBox txt = GetCharacterControl(cntlID, displayData.ToString(), tooltip);
            cells[1].Controls.Add(txt);

            CompareValidator cmp = new CompareValidator();
            cmp.ID = "cmp" + txt.ID;
            cmp.ControlToValidate = txt.ID;
            cmp.Operator = ValidationCompareOperator.DataTypeCheck;
            cmp.Type = ValidationDataType.Double;
            cmp.ErrorMessage = "Numeric (Decimal) data only is permitted in this field";
            cmp.Text = "*";
            cmp.SetFocusOnError = true;
            cells[1].Controls.Add(cmp);

            ValidatorCalloutExtender cmpex = new ValidatorCalloutExtender();
            cmpex.ID = "cmpex" + txt.ID;
            cmpex.TargetControlID = cmp.ID;
            cells[1].Controls.Add(cmpex);

            // add cell for validation if necessary
            if (isMandatory)
            {
                GetRequiredValidator(ref cells[1], cntlID, labeltext);
            }

            return cells;
        }


        public System.Web.UI.WebControls.TableCell[] GetDDListCell(string cntlID, string labeltext, ListItem[] listitems, string selectedText, bool addNoneSelection, string tooltip)
        {
            TableCell[] cells = new TableCell[2];
            int selectedId = 0;

            // character or text box
            cells[0] = new TableCell();
            cells[0].CssClass = "labeltd";
            cells[0].Text = labeltext;

            cells[1] = new TableCell();
            cells[1].CssClass = "inputtd";

            if (selectedText.Trim() != "")
            {
                for (int i = 0; i < listitems.Length; i++)
                {
                    if (listitems[i].Text == selectedText)
                    {
                        selectedId = int.Parse(listitems[i].Value);
                        break;
                    }
                }
            }

            DropDownList lst = GetDDListControl(cntlID, listitems, addNoneSelection, selectedId);
            cells[1].Controls.Add(lst);

            return cells;
        }

        public DropDownList GetAppAreaDDList(int selection)
        {
            cParams fwparams = new cParams(uinfo, fws, uinfo.ActiveLocation);
            string supplierStr = fwparams.GetParamByName("SUPPLIER_PRIMARY_TITLE").ParameterValue;
                        
            DropDownList lst = new DropDownList();
            lst.ID = "lstAppArea";
            if (selection == -1 || selection == 1 || selection == 5)
            {
                lst.Items.Add(new ListItem("Contract Details", ((int)AppAreas.CONTRACT_DETAILS).ToString()));
                lst.Items.Add(new ListItem("Contract Additional", ((int)AppAreas.CONTRACT_GROUPING).ToString()));
            }
            if (selection == -1 || selection == 2 || selection == 7)
            {
                lst.Items.Add(new ListItem("Contract Products", ((int)AppAreas.CONTRACT_PRODUCTS).ToString()));
                lst.Items.Add(new ListItem("Contract Product Group", ((int)AppAreas.CONPROD_GROUPING).ToString()));
            }
            if (selection == -1 || selection == 3)
            {
                lst.Items.Add(new ListItem("Product Details", ((int)AppAreas.PRODUCT_DETAILS).ToString()));
            }
            if (selection == -1 || selection == 8)
            {
                lst.Items.Add(new ListItem("Employee Details", ((int)AppAreas.STAFF_DETAILS).ToString()));
            }
            if (selection == -1 || selection == 4 || selection == 12)
            {
                lst.Items.Add(new ListItem(supplierStr + " Details", ((int)AppAreas.VENDOR_DETAILS).ToString()));
                lst.Items.Add(new ListItem(supplierStr + " Additional", ((int)AppAreas.VENDOR_GROUPING).ToString()));
            }
            if (selection == -1 || selection == 11)
            {
                lst.Items.Add(new ListItem(supplierStr + " Contacts", ((int)AppAreas.VENDOR_CONTACTS).ToString()));
            }
            if (fws.glUseRechargeFunction)
            {
                if (selection == -1 || selection == 6)
                {
                    lst.Items.Add(new ListItem("Recharge Template Grouping", ((int)AppAreas.RECHARGE_GROUPING).ToString()));
                }
            }

            if (selection == -1)
            {
                lst.Items.FindByValue((1).ToString()).Selected = true;
            }
            else
            {
                lst.Items.FindByValue(((int)selection).ToString()).Selected = true;
            }
            

            return lst;
        }

        public DropDownList GetUFieldTypeDDList(cRechargeSetting rs, string selectedId)
        {
            if (selectedId == "")
            {
                selectedId = "1";
            }

            DropDownList lstufType = new DropDownList();
            lstufType.ID = "lstFieldType";
            lstufType.Items.Add(new ListItem("Character", "1"));
            lstufType.Items.Add(new ListItem("Numeric", "2"));
            lstufType.Items.Add(new ListItem("Decimal", "7"));
            lstufType.Items.Add(new ListItem("Date", "3"));
            lstufType.Items.Add(new ListItem("Drop Down List", "4"));
            lstufType.Items.Add(new ListItem("Checkbox", "5"));
            lstufType.Items.Add(new ListItem("Text", "6"));
            lstufType.Items.Add(new ListItem("Hyperlink", "8"));
            lstufType.Items.Add(new ListItem("Employee Ref.", "100"));
            lstufType.Items.Add(new ListItem("Site Ref.", "101"));

            if (fws.glUseRechargeFunction && rs != null)
            {
                lstufType.Items.Add(new ListItem("Recharge " + rs.ReferenceAs, "102"));
                lstufType.Items.Add(new ListItem("Recharge Acc.Codes", "103"));
            }

            lstufType.Items.FindByValue(selectedId).Selected = true;

            return lstufType;
        }
    }
}
