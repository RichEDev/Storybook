using System.Text;
using AjaxControlToolkit;
using SpendManagementHelpers.TreeControl.Drop;

namespace SpendManagementHelpers.TreeControl
{
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    public class TreeControls
    {
        public static HtmlGenericControl CreateGenericControl(string htmlControlType, string id = "", string cssClass="", string innerText = "", string innerHtml="", List<Control> contentControls = null)
        {
            HtmlGenericControl span = new HtmlGenericControl(htmlControlType);

            if (!string.IsNullOrEmpty(id))
            {
                span.ID = id;
            }

            if (!string.IsNullOrEmpty(cssClass))
            {
                span.Attributes.Add("class", cssClass);
            }

            if (!string.IsNullOrEmpty(innerText))
            {
                span.InnerText = innerText;
            }

            if (!string.IsNullOrEmpty(innerHtml))
            {
                span.InnerHtml = innerHtml;
            }

            if (contentControls != null)
            {
                foreach(Control c in contentControls)
                {
                    span.Controls.Add(c);
                }
            }
        
            return span;
        }

        public static Image CreateTooltip(string id = "", string tooltipid = "", string altText = "", string cssClass = "")
        {
            Image img = new Image { ImageUrl = "~/shared/images/icons/16/plain/tooltip.png" };

            if (!string.IsNullOrEmpty(id))
            {
                img.ID = id;
            }

            if (!string.IsNullOrEmpty(altText))
            {
                img.AlternateText = altText;
            }

            if (!string.IsNullOrEmpty(cssClass))
            {
                img.CssClass = cssClass;
            }

            if (!string.IsNullOrEmpty(tooltipid))
            {
                img.Attributes.Add("onmouseover", "SEL.Tooltip.Show('" + tooltipid + "', 'sm', this);");
            }

            return img;
        }

        public static void CreateCriteriaModalPopup(ControlCollection controls, string staticLibraryPath, ref Panel filterPanel, ref CheckBox filterRuntime, string validatorCssClass = "inputvalidatorfield", string tooltipCssClass = "tooltipicon", string iconCssClass = "inputicon", string dataFieldCssClass = "inputs", string fieldContainerCssClass = "twocolumn", string mandatoryCssClass = "mandatory", string filterValidationGroup = "", string titleCssClass = "sectiontitle", bool renderReportOptions = false, string domain = "SEL.Trees", bool includeModal = true)
        {
            filterPanel = new Panel { ID = "pnlFilter", CssClass = "modalpanel formpanel formpanelsmall" };
            filterPanel.Attributes.CssStyle.Add("display", "none");
            if (!staticLibraryPath.EndsWith("/"))
            {
                staticLibraryPath += "/";
            }

            var _filterModalHelpIcon = new Image
            {
                ID = "imgFilterModalHelp",
                ClientIDMode = ClientIDMode.Static,
                ImageUrl = staticLibraryPath + "icons/24/plain/information.png",
                AlternateText = "Show Filter Editor Information"
            };
            _filterModalHelpIcon.Attributes.CssStyle.Add("display", "none");
            _filterModalHelpIcon.Attributes.CssStyle.Add("position", "absolute");
            filterPanel.Controls.Add(_filterModalHelpIcon);

            var _div = TreeControls.CreateGenericControl("div", "divFilterModalHeading", cssClass: titleCssClass,
                innerText: "Filter Details");

            filterPanel.Controls.Add(_div);

            var _filterCriteria = new DropDownList { ID = "ddlFilter" };
            _filterCriteria.Items.Add(new ListItem("[None]", "0"));
            _filterCriteria.Attributes.Add("onchange", string.Format("$.filterModal.Filters.FilterModal.ChangeFilterCriteria(false);", domain));

            var _filterLabel = new Label
            {
                ID = "lblCriteriaFilter",
                AssociatedControlID = _filterCriteria.ID,
                CssClass = mandatoryCssClass,
                Text = "Filter criteria*"
            };

            _div = TreeControls.CreateGenericControl("div", cssClass: fieldContainerCssClass,
                contentControls: new List<Control> { _filterLabel });

            _div.Controls.Add(TreeControls.CreateGenericControl("span", cssClass: dataFieldCssClass,
                contentControls: new List<Control> { _filterCriteria }));

            _div.Controls.Add(TreeControls.CreateGenericControl("span", cssClass: iconCssClass));

            _div.Controls.Add(TreeControls.CreateGenericControl("span", cssClass: tooltipCssClass,
                contentControls:
                    new List<Control>
                    {
                        TreeControls.CreateTooltip(cssClass: tooltipCssClass, tooltipid: "033EC02E-AA57-4B3A-A0B5-FCB3459AEC00")
                    }));

            var _span = TreeControls.CreateGenericControl("span", cssClass: validatorCssClass);
            var reqVal = new RequiredFieldValidator
            {
                ID = "reqFilter",
                ValidationGroup = filterValidationGroup,
                ControlToValidate = _filterCriteria.ID,
                ErrorMessage = "Please select a value for Filter criteria.",
                Text = "*"
            };
            _span.Controls.Add(reqVal);
            _div.Controls.Add(_span);
            filterPanel.Controls.Add(_div);

            if (renderReportOptions)
            {
                filterRuntime = new CheckBox { ID = "chkRuntime" };
                filterRuntime.Attributes.Add("onclick", string.Format("$.filterModal.Filters.FilterModal.ChangeFilterCriteria(false, true);", domain));
                _filterLabel = new Label
                {
                    ID = "lblRuntime",
                    AssociatedControlID = filterRuntime.ID,
                    Text = "I'll decide when I run the report"
                };

                _div = TreeControls.CreateGenericControl(
                    "div",
                    cssClass: fieldContainerCssClass,
                    id: "filterRuntimeRow",
                    contentControls: new List<Control> { _filterLabel });

                _div.Controls.Add(
                    TreeControls.CreateGenericControl(
                        "span",
                        cssClass: dataFieldCssClass,
                        contentControls: new List<Control> { filterRuntime }));

                _div.Controls.Add(TreeControls.CreateGenericControl("span", cssClass: iconCssClass));

                _div.Controls.Add(TreeControls.CreateGenericControl("span", cssClass: tooltipCssClass));

                _span = TreeControls.CreateGenericControl("span", cssClass: validatorCssClass);

                _div.Controls.Add(_span);
                filterPanel.Controls.Add(_div);
            }

            _div = TreeControls.CreateGenericControl("div", "divCriteria1", cssClass: fieldContainerCssClass);

            _filterLabel = new Label { ID = "lblFilterValue1", CssClass = mandatoryCssClass, Text = "Value 1*" };
            _filterLabel.Attributes.Add("onclick", string.Format("{0}.Filters.FilterModal.ToggleCalendar(1)", domain));

            _span = TreeControls.CreateGenericControl("span", cssClass: dataFieldCssClass);
            _span.Controls.Add(new TextBox { ID = "txtFilterCriteria1", CssClass = "fillspan", MaxLength = 150 });
            _span.Controls.Add(TreeControls.CreateGenericControl("span", id: "Criteria1Spacer", innerHtml: "&nbsp;"));
            _span.Controls.Add(new TextBox { ID = "txtTimeCriteria1", CssClass = "fillspan", MaxLength = 5 });
            _filterCriteria = new DropDownList { ID = "cmbFilterCriteria1" };
            _filterCriteria.Items.Add(new ListItem("[None]", "0"));
            _filterCriteria.Attributes.Add("onchange",
                string.Format("$.filterModal.Filters.FilterModal.UpdateDropdownValidatorIfNoneSelected({0}.DomIDs.Filters.FilterModal.Criteria1DropDown, {0}.DomIDs.Filters.FilterModal.RequiredValidator1);", domain));
            _filterLabel.AssociatedControlID = _filterCriteria.ID;
            // which control doesn't matter, just to create 'for' attribute
            _span.Controls.Add(_filterCriteria);

            _div.Controls.Add(_filterLabel);
            _div.Controls.Add(_span);

            _span = TreeControls.CreateGenericControl("span", cssClass: iconCssClass);
            HtmlGenericControl img = TreeControls.CreateGenericControl("img", "imgFilter1Calc");
            img.Attributes.Add("alt", "Open the date picker");
            img.Attributes.Add("src", "/shared/images/icons/cal.gif");
            img.Attributes.Add("onclick", string.Format("$.filterModal.Filters.FilterModal.ToggleCalendar(1);", domain));
            _span.Controls.Add(img);
            _div.Controls.Add(_span);

            _div.Controls.Add(TreeControls.CreateGenericControl("span", cssClass: tooltipCssClass,
                contentControls:
                    new List<Control>
                    {
                        TreeControls.CreateTooltip(cssClass: tooltipCssClass, tooltipid: "D8B49CC8-95E9-48B7-9C90-13A9C2E5CC0B")
                    }));

            _span = TreeControls.CreateGenericControl("span", "spanCriteria1Validator", cssClass: validatorCssClass);
            reqVal = new RequiredFieldValidator
            {
                ID = "reqFilterCriteria1",
                Text = "*",
                ValidationGroup = filterValidationGroup,
                ControlToValidate = "txtFilterCriteria1",
                ErrorMessage = "Please enter a Value.",
                Display = ValidatorDisplay.Dynamic,
                Enabled = false
            };
            var cmpVal = new CompareValidator
            {
                ID = "cmpFilterCriteria1DataType",
                CultureInvariantValues = true,
                ValidationGroup = filterValidationGroup,
                ControlToValidate = "txtFilterCriteria1",
                Type = ValidationDataType.Integer,
                Operator = ValidationCompareOperator.DataTypeCheck,
                Enabled = false,
                ErrorMessage = "Please enter an integer value for Value 1.",
                Text = "*",
                Display = ValidatorDisplay.Dynamic
            };
            var custVal = new CustomValidator
            {
                ID = "cvTime1",
                ErrorMessage = "Please enter a valid time for Time 1 (hh:mm).",
                ValidationGroup = filterValidationGroup,
                Text = "*",
                Enabled = true,
                ClientValidationFunction = string.Format("$.filterModal.Filters.FilterModal.ValidateTime", domain),
                Display = ValidatorDisplay.Dynamic
            };
            var rngVal = new RangeValidator
            {
                ID = "rngIntFilterID1",
                ControlToValidate = "txtFilterCriteria1",
                ValidationGroup = filterValidationGroup,
                CultureInvariantValues = true,
                Display = ValidatorDisplay.Dynamic,
                Enabled = false,
                ErrorMessage = " must be between -2,147,483,674 and 2,147,483,674.",
                Text = "*",
                Type = ValidationDataType.Integer,
                MaximumValue = "2147483647",
                MinimumValue = "-2147483647"
            };
            _span.Controls.Add(reqVal);
            _span.Controls.Add(cmpVal);
            _span.Controls.Add(custVal);
            _span.Controls.Add(rngVal);
            _div.Controls.Add(_span);
            filterPanel.Controls.Add(_div);

            _div = TreeControls.CreateGenericControl("div", "divCriteria2", cssClass: fieldContainerCssClass);

            _filterLabel = new Label { ID = "lblFilterValue2", CssClass = mandatoryCssClass, Text = "Value 2*" };
            _filterLabel.Attributes.Add("onclick", string.Format("$.filterModal.Filters.FilterModal.ToggleCalendar(2)", domain));

            _span = TreeControls.CreateGenericControl("span", cssClass: dataFieldCssClass);
            var txt = new TextBox { ID = "txtFilterCriteria2", CssClass = "fillspan", MaxLength = 150 };
            _span.Controls.Add(txt);
            _span.Controls.Add(TreeControls.CreateGenericControl("span", id: "Criteria2Spacer", innerHtml: "&nbsp;"));
            _span.Controls.Add(new TextBox { ID = "txtTimeCriteria2", CssClass = "fillspan", MaxLength = 5 });

            _filterLabel.AssociatedControlID = txt.ID; // which control doesn't matter, just to create 'for' attribute
            _div.Controls.Add(_filterLabel);
            _div.Controls.Add(_span);

            _span = TreeControls.CreateGenericControl("span", cssClass: iconCssClass);
            img = TreeControls.CreateGenericControl("img", "imgFilter2Calc");
            img.Attributes.Add("alt", "Open the date picker");
            img.Attributes.Add("src", "/shared/images/icons/cal.gif");
            img.Attributes.Add("onclick", string.Format("$.filterModal.Filters.FilterModal.ToggleCalendar(2);", domain)); // NEEDS LOCAL JS CALL
            _span.Controls.Add(img);
            _div.Controls.Add(_span);

            _div.Controls.Add(TreeControls.CreateGenericControl("span", cssClass: tooltipCssClass,
                contentControls:
                    new List<Control>
                    {
                        TreeControls.CreateTooltip(cssClass: tooltipCssClass, tooltipid: "4104F301-F852-4EE1-9755-B0BBFB02C16C")
                    }));

            _span = TreeControls.CreateGenericControl("span", "spanCriteria2Validator", cssClass: validatorCssClass);
            reqVal = new RequiredFieldValidator
            {
                ID = "reqFilterCriteria2",
                Text = "*",
                ValidationGroup = filterValidationGroup,
                ControlToValidate = "txtFilterCriteria2",
                ErrorMessage = "Please enter a Value.",
                Display = ValidatorDisplay.Dynamic,
                Enabled = false
            };
            _span.Controls.Add(reqVal);

            cmpVal = new CompareValidator
            {
                ID = "cmpFilterCriteria2DataType",
                CultureInvariantValues = true,
                ValidationGroup = filterValidationGroup,
                ControlToValidate = "txtFilterCriteria2",
                Type = ValidationDataType.Integer,
                Operator = ValidationCompareOperator.DataTypeCheck,
                Enabled = false,
                ErrorMessage = "Please enter an integer value for Value 2.",
                Text = "*",
                Display = ValidatorDisplay.Dynamic
            };
            _span.Controls.Add(cmpVal);

            cmpVal = new CompareValidator
            {
                ID = "cmpFilterCriteriaRange",
                CultureInvariantValues = true,
                ValidationGroup = filterValidationGroup,
                ControlToValidate = "txtFilterCriteria2",
                Type = ValidationDataType.Integer,
                Operator = ValidationCompareOperator.GreaterThan,
                Enabled = false,
                ErrorMessage = "Please enter a value greater than X for Value 2.",
                Text = "*",
                Display = ValidatorDisplay.Dynamic,
                ControlToCompare = "txtFilterCriteria1"
            };
            _span.Controls.Add(cmpVal);

            custVal = new CustomValidator
            {
                ID = "cvTime2",
                ErrorMessage = "Please enter a valid time for Time 2 (hh:mm).",
                ValidationGroup = filterValidationGroup,
                Text = "*",
                Enabled = true,
                ClientValidationFunction = string.Format("$.filterModal.Filters.FilterModal.ValidateTime", domain),
                Display = ValidatorDisplay.Dynamic
            };
            _span.Controls.Add(custVal);

            custVal = new CustomValidator
            {
                ID = "cvDateTime",
                ErrorMessage = "Please enter a valid date greater than Date and time 1 for Date and time 2.",
                ValidationGroup = filterValidationGroup,
                Text = "*",
                Enabled = true,
                ClientValidationFunction = string.Format("$.filterModal.Filters.FilterModal.ValidateDateTime", domain),
                Display = ValidatorDisplay.Dynamic
            };
            _span.Controls.Add(custVal);

            custVal = new CustomValidator
            {
                ID = "cvTimeRange",
                ErrorMessage = "Please enter a valid time greater than Time 1 for Time 2.",
                ValidationGroup = filterValidationGroup,
                Text = "*",
                Enabled = true,
                ClientValidationFunction = string.Format("$.filterModal.Filters.FilterModal.ValidateTimeRange", domain),
                Display = ValidatorDisplay.Dynamic
            };
            _span.Controls.Add(custVal);

            rngVal = new RangeValidator
            {
                ID = "rngIntFilterID2",
                ControlToValidate = "txtFilterCriteria2",
                ValidationGroup = filterValidationGroup,
                CultureInvariantValues = true,
                Display = ValidatorDisplay.Dynamic,
                Enabled = false,
                ErrorMessage = " must be between -2,147,483,674 and 2,147,483,674.",
                Text = "*",
                Type = ValidationDataType.Integer,
                MaximumValue = "2147483647",
                MinimumValue = "-2147483647"
            };
            _span.Controls.Add(rngVal);

            _div.Controls.Add(_span);
            filterPanel.Controls.Add(_div);

            _div = TreeControls.CreateGenericControl("div", "divFilterList");
            _div.ClientIDMode = ClientIDMode.Static;
            _div.Attributes.CssStyle.Add("width", "400px");
            _div.Attributes.CssStyle.Add("height", "300px");
            _div.Attributes.CssStyle.Add("display", "none");
            _filterCriteria = new DropDownList { ID = "cmbFilterList", CssClass = "multiselect" };
            _filterCriteria.Attributes.CssStyle.Add("width", "200px");
            _filterCriteria.Attributes.Add("multiple", "multiple");
            _filterCriteria.Attributes.Add("name", "cmbFilterList");
            _div.Controls.Add(_filterCriteria);
            filterPanel.Controls.Add(_div);

            _div = TreeControls.CreateGenericControl("div", cssClass: "formbuttons");
            var filterSaveButton = new CSSButton
            {
                ID = "btnFilterSave",
                Text = "save",
                OnClientClick = string.Format("$.filterModal.Filters.FilterModal.Save('{0}');return false;", filterValidationGroup, domain),
                ButtonSize = CSSButtonSize.Standard,
                UseSubmitBehavior = false
            };
            var filterCloseButton = new CSSButton
            {
                ID = "btnFilterClose",
                Text = "cancel",
                OnClientClick = string.Format("$.filterModal.Filters.FilterModal.Cancel();return false;", domain),
                ButtonSize = CSSButtonSize.Standard,
                UseSubmitBehavior = false
            };
            _div.Controls.Add(filterSaveButton);
            _div.Controls.Add(filterCloseButton);
            filterPanel.Controls.Add(_div);

            controls.Add(filterPanel);

            // output a span to contain list item information
            _span = TreeControls.CreateGenericControl("span", "tcPopupListItemContainer");
            _span.ClientIDMode = ClientIDMode.Static;
            controls.Add(_span);

            if (includeModal)
            {
                // output a modal popup extender
                var lnk = new LinkButton { ID = "lnkFilter" };
                lnk.Attributes.CssStyle.Add("display", "none");
                controls.Add(lnk);

                var modExt = new ModalPopupExtender
                {
                    ID = "modFilter",
                    TargetControlID = "lnkFilter",
                    PopupControlID = "pnlFilter",
                    BackgroundCssClass = "modalBackground"
                };
                controls.Add(modExt);
            }
        }

        public static string GeneratePanelDomIDsForFilterModal(bool filterPanelRendered, bool renderReportOptions, Panel filterPanel, Tree tree, TreeDrop drop, CheckBox filterRuntime, string domain = "SEL.Trees", bool includeModal = true)
        {
            var str = new StringBuilder();
            if (filterPanelRendered)
            {
                str.Append(string.Format("var filterModal = jQuery.filterModal([ domainRoot: {0} ]);", domain)).Replace("[", "{").Replace("]","}");
                str.Append("(function (t) {");
                str.Append("t.Panel = '" + filterPanel.ClientID + "';\n");
                if (tree != null && drop != null)
                {
                    str.Append("t.Tree = '" + tree.ClientID + "';\n");
                    str.Append("t.Drop = '" + drop.ClientID + "';\n");
                }
                if (includeModal)
                {
                    str.Append("t.FilterModalObj = '" + filterPanel.FindControl("modFilter").ClientID + "';\n");
                }

                str.Append("t.FilterModal.Heading = '" + filterPanel.FindControl("divFilterModalHeading").ClientID + "';\n");
                str.Append("t.FilterModal.FilterDropDown ='" + filterPanel.FindControl("ddlFilter").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1 = '" + filterPanel.FindControl("txtFilterCriteria1").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2 = '" + filterPanel.FindControl("txtFilterCriteria2").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1DropDown = '" + filterPanel.FindControl("cmbFilterCriteria1").ClientID + "';\n");
                str.Append("t.FilterModal.CriteriaListDropDown = '" + filterPanel.FindControl("cmbFilterList").ClientID + "';\n");
                str.Append("t.FilterModal.FilterRequiredValidator = '" + filterPanel.FindControl("reqFilter").ClientID + "';\n");
                str.Append("t.FilterModal.RequiredValidator1 = '" + filterPanel.FindControl("reqFilterCriteria1").ClientID + "';\n");
                str.Append("t.FilterModal.RequiredValidator2 = '" + filterPanel.FindControl("reqFilterCriteria2").ClientID + "';\n");
                str.Append("t.FilterModal.DataTypeValidator1 = '" + filterPanel.FindControl("cmpFilterCriteria1DataType").ClientID + "';\n");
                str.Append("t.FilterModal.DataTypeValidator2 = '" + filterPanel.FindControl("cmpFilterCriteria2DataType").ClientID + "';\n");
                str.Append("t.FilterModal.CompareValidator = '" + filterPanel.FindControl("cmpFilterCriteriaRange").ClientID + "';\n");
                str.Append("t.FilterModal.TimeValidator1 = '" + filterPanel.FindControl("cvTime1").ClientID + "';\n");
                str.Append("t.FilterModal.TimeValidator2 = '" + filterPanel.FindControl("cvTime2").ClientID + "';\n");
                str.Append("t.FilterModal.DateAndTimeCompareValidator = '" + filterPanel.FindControl("cvDateTime").ClientID + "';\n");
                str.Append("t.FilterModal.TimeRangeCompareValidator = '" + filterPanel.FindControl("cvTimeRange").ClientID + "';\n");
                str.Append("t.FilterModal.CriteriaRow1 = '" + filterPanel.FindControl("divCriteria1").ClientID + "';\n");
                str.Append("t.FilterModal.CriteriaRow2 = '" + filterPanel.FindControl("divCriteria2").ClientID + "';\n");
                str.Append("t.FilterModal.CriteriaListRow = '" + filterPanel.FindControl("divFilterList").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1ImageCalc = '" + filterPanel.FindControl("imgFilter1Calc").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2ImageCalc = '" + filterPanel.FindControl("imgFilter2Calc").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1Time = '" + filterPanel.FindControl("txtTimeCriteria1").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2Time = '" + filterPanel.FindControl("txtTimeCriteria2").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1Spacer = '" + filterPanel.FindControl("Criteria1Spacer").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2Spacer = '" + filterPanel.FindControl("Criteria2Spacer").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1Label = '" + filterPanel.FindControl("lblFilterValue1").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2Label = '" + filterPanel.FindControl("lblFilterValue2").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria1ValidatorSpan = '" + filterPanel.FindControl("spanCriteria1Validator").ClientID + "';\n");
                str.Append("t.FilterModal.Criteria2ValidatorSpan = '" + filterPanel.FindControl("spanCriteria2Validator").ClientID + "';\n");
                str.Append("t.FilterModal.IntegerValidator1 = '" + filterPanel.FindControl("rngIntFilterID1").ClientID + "';\n");
                str.Append("t.FilterModal.IntegerValidator2 = '" + filterPanel.FindControl("rngIntFilterID2").ClientID + "';\n");

                if (renderReportOptions)
                {
                    str.Append("t.FilterModal.Runtime = '" + filterRuntime.FindControl("chkRuntime").ClientID + "';\n");
                    str.Append("t.FilterModal.RuntimeRow = '" + filterPanel.FindControl("filterRuntimeRow").ClientID + "';\n");
                }

                str.Append("} ");
                str.Append(string.Format("({0}.DomIDs.Filters));", domain));
            }
           
            return str.ToString();
        }       
    }
}