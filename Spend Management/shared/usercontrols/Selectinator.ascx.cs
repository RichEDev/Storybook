namespace Spend_Management.shared.usercontrols
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Selectinator;
    
    /// <summary>
    ///     SELECTINATOR class is an auto complete when there are more than 25 entries, otherwise it is a drop down list.
    /// </summary>
    public partial class Selectinator : UserControl
    {
        #region Properties
        /// <summary>
        ///     A list of Field Filters to apply to the list.
        /// </summary>
        public List<JSFieldFilter> Filters { get; set; }

        /// <summary>
        ///     The GUID of the base table for the list.
        /// </summary>
        public Guid TableGuid { get; set; }

        /// <summary>
        ///     The field to display in the drop down or text box.
        /// </summary>
        public Guid DisplayField { get; set; }

        /// <summary>
        ///     A list of field to match the input valule against.
        /// </summary>
        public List<Guid> MatchFields { get; set; }

        /// <summary>
        /// Gets or sets  list of field to display in autocomplete search results.
        /// </summary>
        public List<Guid> AutocompleteFields { get; set; }

        /// <summary>
        ///     The name of the autocomplete.  This is used as a label and for titles etc.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Set to true if the control is redered as a drop down list.
        /// </summary>
        public bool ShownAsDropDownList { get; private set; }

        /// <summary>
        ///     The Text or GUID to be shown for this controls tooltip.  If blank no tooltip is shown.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        ///     A list of Look up Display fields associated with this control.  These will be updated when the value is changed.
        /// </summary>
        public List<AutoCompleteTriggerField> AutoCompleteTriggerFields { get; set; }

        /// <summary>
        ///     The Validation control group to use for Mandatory fields.  If blank, this field will not be mandatory.
        /// </summary>
        public string ValidationControlGroup { get; set; }

        /// <summary>
        /// Set to true if this field is mandatory.
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets to true if the autocomplete input field has set of display fields set to display in serach result
        /// </summary>
        public bool SetAutocompleteMultipleResultFields { get; set; }

        /// <summary>
        ///     The selected text value.
        /// </summary>
        public string Text
        {
            get
            {
                var id = 0;
                int.TryParse(SelectedId, out id);
                if (id > 0)
                {
                    return ShownAsDropDownList ? (this.SelectinatorTextSelect.Text == string.Empty ? this.SelectinatorText_ID.Text : this.SelectinatorTextSelect.Text) : (this.SelectinatorText.Style.Value == null && this.SelectinatorText.Text == string.Empty ? this.SelectinatorText_ID.Text : this.SelectinatorText.Text);    
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        ///     The selected ID value.
        /// </summary>
        public string SelectedId
        {
            get { return SelectinatorText_ID.Text; }
        }

        /// <summary>
        ///  Determines if the controls that make up the selectinator should be enabled or not
        /// </summary>
        public bool IsEnabled { get; set; }

        #endregion
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (TableGuid != new Guid() && DisplayField != new Guid() && MatchFields != null && Name != string.Empty)
            {
                if (Filters == null)
                {
                    Filters = new List<JSFieldFilter>();
                }

                if (AutoCompleteTriggerFields == null)
                {
                    AutoCompleteTriggerFields = new List<AutoCompleteTriggerField>();
                }
                if (this.AutocompleteFields == null)
                {
                    this.AutocompleteFields = new List<Guid>();
                }
                SetupControl();
            }
            else
            {
                if (lblMessage != null)
                {
                    lblMessage.Text = "Control cannot be shown.";
                    lblMessage.Visible = true;
                    SelectinatorTextSelect.Visible = false;
                    SelectinatorLabel.Visible = false;
                    SelectinatorText.Visible = false;
                    SelectinatorTextSearchIcon.Visible = false;
                    SelectinatorTooltip.Visible = false;
                }
            }
        }

        #region PublicMethods

        /// <summary>
        ///     Create and initialise all the controls and javascript based on the parameters given.
        /// </summary>
        public void SetupControl()
        {
            Name = Name.Replace("'", string.Empty).Replace("\"", string.Empty);
            var javascriptId = ID + "selectinator";
            var jsSelectinator = new JsSelectinator
            {
                DisplayField = DisplayField,
                Filters = Filters,
                MatchFields = MatchFields,
                Name = Name,
                TableGuid = TableGuid,
                AutocompleteFields = this.AutocompleteFields
            };

            var serializer = new JavaScriptSerializer();
            var user = cMisc.GetCurrentUser();
            SetupLabel();
            SetupValidators();

            var autoCompleteMatches = this.GetAutoCompleteCount(user);
            SelectinatorJavascript(serializer, javascriptId);

            if (autoCompleteMatches.Count <= 25)
            {
                if (this.SetAutocompleteMultipleResultFields == true && (this.AutocompleteFields.Count > 0) && autoCompleteMatches.Count > 0)
                {
                    this.ShowAsDropDownListCustomEntity(user, javascriptId);
                }
                else
                {
                    this.ShowAsDropDownList(autoCompleteMatches, javascriptId);
                }

                if (this.Filters.Exists(filter => filter.FilterOnEdit))
                {
                    var filters = new SortedList<int, FieldFilter>();
                    foreach (var jsFieldFilter in this.Filters)
                    {
                        if (!jsFieldFilter.FilterOnEdit)
                        {
                            filters.Add(jsFieldFilter.Order, FieldFilters.JsToCs(jsFieldFilter, user));
                        }
                        
                    }

                    var autoCompleteBinding = AutoComplete.createAutoCompleteBindString(
                        this.SelectinatorText.ClientID,
                        15,
                        this.TableGuid,
                        this.DisplayField,
                        this.MatchFields,
                        fieldFilters: filters,
                        triggerFields: this.AutoCompleteTriggerFields,
                        AutoCompleteDisplayFields: this.AutocompleteFields,
                        displayAutocompleteMultipleResultsFields: this.SetAutocompleteMultipleResultFields);

                        this.SelectinatorText.Enabled = this.IsEnabled;
                        this.Parent.Page.ClientScript.RegisterStartupScript(
                        this.GetType(),
                        this.ID + "AutoCompleteBind",
                        AutoComplete.generateScriptRegisterBlock(new List<string> { autoCompleteBinding }),
                        true);

                    this.SelectinatorTextSearchIcon.Attributes.Add("onclick", $"javascript:AutoCompleteSearches.Modal(divmessage, '{this.Name}', '{javascriptId}', '{serializer.Serialize(jsSelectinator)}');");
                }
            }
            else
            {
                ShowAsSelectinator(user, javascriptId, serializer, jsSelectinator);
            }

            SetupTooltip();
        }

        /// <summary>
        /// Set the value of the SELECTINATOR.  If the text / id combination are not in the drop down list, then add them.
        /// </summary>
        /// <param name="text">The text to appear in the text box or drop down list.</param>
        /// <param name="id">The ID of the selected item.</param>
        public void SetValue(string text, string id)
        {
            this.SelectinatorText_ID.Text = id;
            this.SelectinatorText.Text = text;
            var newItem = new ListItem(text, id);
            if (this.SelectinatorTextSelect.Items.Contains(newItem))
            {
                this.SelectinatorTextSelect.Items[this.SelectinatorTextSelect.Items.IndexOf(newItem)].Selected = true;
            }
            else
            {
                newItem.Selected = true;
                this.SelectinatorTextSelect.Items.Add(newItem);
            }
        }

        /// <summary>
        /// Sets a css class with the name of the parent filter attribute
        /// </summary>
        /// <param name="parentAttributeId">The Id of the parent filter attribute</param>
        public void AddParentCssClass(string parentAttributeId)
        {
            this.SelectinatorTextSelect.CssClass += " parenttxt" + parentAttributeId;
        }


        #endregion

        #region PrivateMethods

        /// <summary>
        /// Get all the records available for the list.
        /// </summary>
        /// <param name="user">
        /// The current user of the system
        /// </param>
        /// <returns></returns>
        private List<sAutoCompleteResult> GetAutoCompleteCount(CurrentUser user)
        {
            if (this.Filters.Count > 0 && this.Filters.Exists(filter => filter.FilterOnEdit))
            {
                foreach (var filter in this.Filters)
                {
                    if (filter.FilterOnEdit)
                    {
                        filter.Order = (byte) (this.Filters.Count - 1);
                    }
                }
            }

            return AutoComplete.GetAutoCompleteMatches(
                user,
                26,
                TableGuid.ToString(),
                DisplayField.ToString(),
                string.Join(",", MatchFields.ToArray()),
                string.Empty,
                true,
                Filters.ToDictionary(fieldFilter => fieldFilter.Order.ToString()));
        }
        
        /// <summary>
        /// Render the control as a Drop Down list.  Hide the text box and set the items in the list to be the whole list.
        /// </summary>
        /// <param name="autoCompleteMatches">
        /// The list returned from GetAutoCompleteCount which is used to populate the drop down list.
        /// </param>
        /// <param name="javascriptId">
        /// The identification used for the javascript selectinator object.
        /// </param>
        private void ShowAsDropDownList(IEnumerable<sAutoCompleteResult> autoCompleteMatches, string javascriptId)
        {
            SelectinatorText.Style.Add(HtmlTextWriterStyle.Display, "none");
            SelectinatorTextSearchIcon.Style.Add(HtmlTextWriterStyle.Display, "none");
            SelectinatorTextSelect.Items.Add(new ListItem("[None]", "0"));
            SelectinatorTextSelect.Items.AddRange(
                autoCompleteMatches.Select(x => new ListItem(x.label, x.value)).ToArray());
            SelectinatorTextSelect.Attributes.Add("onchange", string.Format("{0}.SelectChange(this);", javascriptId));
            SelectinatorTextSelect.Enabled = IsEnabled;
            ShownAsDropDownList = true;           
        }

        /// <summary>
        /// Render the control as a Drop Down list.  Hide the text box and set the items in the list to be the whole list.
        /// </summary>
        /// <param name="user">
        /// The current user of the system
        /// </param>
        /// <param name="javascriptId">
        /// The identification used for the javascript selectinator object.
        /// </param>
        private void ShowAsDropDownListCustomEntity(CurrentUser user, string javascriptId)
        {
            this.SelectinatorText.Style.Add(HtmlTextWriterStyle.Display, "none");
            this.SelectinatorTextSearchIcon.Style.Add(HtmlTextWriterStyle.Display, "none");
            this.SelectinatorTextSelect.Attributes.Add("onchange", $"{javascriptId}.SelectChange(this);");
            this.SelectinatorTextSelect.Enabled = this.IsEnabled;
            this.ShownAsDropDownList = true;

            var filters = new SortedList<int, FieldFilter>();
            foreach (JSFieldFilter jsFieldFilter in this.Filters)
            {
                filters.Add(jsFieldFilter.Order, FieldFilters.JsToCs(jsFieldFilter, user));
            }

            var autocompleteBinding = AutoComplete.CreateAutoCompleteComboString(SelectinatorTextSelect.ClientID,15,TableGuid,DisplayField,MatchFields,fieldFilters: filters,triggerFields: AutoCompleteTriggerFields, AutoCompleteDisplayFields: this.AutocompleteFields, displayAutocompleteMultipleResultsFields: this.SetAutocompleteMultipleResultFields);
            this.SelectinatorText.Enabled = this.IsEnabled;
            this.Parent.Page.ClientScript.RegisterStartupScript(this.GetType(),ID + "AutoCompleteComboBind",AutoComplete.generateScriptRegisterBlock(new List<string> { autocompleteBinding }),true);
        }

        /// <summary>
        /// Render the control as an autocomplete with binoculars.
        /// </summary>
        /// <param name="user">
        /// The current user of the system
        /// </param>
        /// <param name="javascriptId">
        /// The identification used for the javascript selectinator object.
        /// </param>
        /// <param name="serializer">
        /// A javascriptseriazer object.
        /// </param>
        /// <param name="jsSelectinator">
        /// An internal serializable class to hold the selectinator data.
        /// </param>
        private void ShowAsSelectinator(CurrentUser user, string javascriptId, JavaScriptSerializer serializer,
            JsSelectinator jsSelectinator)
        {
            var filters = new SortedList<int, FieldFilter>();
            foreach (JSFieldFilter jsFieldFilter in Filters)
            {
                filters.Add(jsFieldFilter.Order, FieldFilters.JsToCs(jsFieldFilter, user));
            }

            SelectinatorTextSelect.Style.Add(HtmlTextWriterStyle.Display, "none");
            SelectinatorTextSearchIcon.ImageUrl = GlobalVariables.StaticContentLibrary +
                                                  "/icons/16/new-icons/find.png";

            var autocompleteBinding = AutoComplete.createAutoCompleteBindString(
                SelectinatorText.ClientID,
                15,
                TableGuid,
                DisplayField,
                MatchFields,
                fieldFilters: filters,
                triggerFields: AutoCompleteTriggerFields,AutoCompleteDisplayFields:this.AutocompleteFields, displayAutocompleteMultipleResultsFields:this.SetAutocompleteMultipleResultFields);

            SelectinatorText.Enabled = IsEnabled;
            Parent.Page.ClientScript.RegisterStartupScript(
                GetType(),
                ID + "AutoCompleteBind",
                AutoComplete.generateScriptRegisterBlock(new List<string> {autocompleteBinding}),
                true);

            if (IsEnabled)
            {
                SelectinatorTextSearchIcon.Attributes.Add("onclick",
                string.Format("javascript:AutoCompleteSearches.Modal(divmessage, '{0}', '{1}', '{2}');",
                Name, javascriptId, serializer.Serialize(jsSelectinator)));
            }
                 
            ShownAsDropDownList = false;
        }

        /// <summary>
        /// Create and register the selectinator javascript
        /// </summary>
        /// <param name="serializer">
        ///     A javascript serializer objetc.
        /// </param>
        /// <param name="javascriptId">
        ///     The identification used for the javascript selectinator object.
        /// </param>
        private void SelectinatorJavascript(JavaScriptSerializer serializer, string javascriptId)
        {
            var js = new StringBuilder();
            string triggers = "null";
            if (AutoCompleteTriggerFields != null && AutoCompleteTriggerFields.Count > 0)
            {
                triggers = serializer.Serialize(AutoCompleteTriggerFields);
            }

            js.Append(
                "var " + javascriptId + " = AutoCompleteSearches.New('Selectinator" + ID + "', '" +
                SelectinatorText.ClientID +
                "', divmessage, '', " + triggers + ", '" + TableGuid + "');\n");
            Parent.Page.ClientScript.RegisterStartupScript(GetType(), ID + "JS", js.ToString(), true);
        }

        /// <summary>
        /// Set the label text from the Name parameter.
        /// </summary>
        private void SetupLabel()
        {
            SelectinatorLabel.Text = Name;
        }

        /// <summary>
        /// Set the validators error messages and validation group.
        /// </summary>
        private void SetupValidators()
        {
            reqSelectinator.ErrorMessage = reqSelectinator.ErrorMessage.Replace("{0}", Name);
            cmpValidatorSelectinatorId.ErrorMessage = cmpValidatorSelectinatorId.ErrorMessage.Replace("{0}", Name);
            cmpValidatorSelectinatorId.ValidationGroup = ValidationControlGroup;
            if (Mandatory)
            {
                reqSelectinator.ValidationGroup = ValidationControlGroup;
                SelectinatorLabel.Text += "*";
                SelectinatorLabel.Attributes.Add("class", "mandatory");
            }
        }

        /// <summary>
        /// Set the tooltip text GUID or hide if not used.
        /// </summary>
        private void SetupTooltip()
        {
            if (string.IsNullOrEmpty(Tooltip))
            {
                SelectinatorTooltip.Visible = false;
            }
            else
            {
                SelectinatorTooltip.Attributes.Add("onmouseover",
                    string.Format("SEL.Tooltip.Show('{0}', 'sm', this);", Tooltip));
            }
        }

        #endregion

    }
}