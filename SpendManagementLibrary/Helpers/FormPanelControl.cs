using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Globalization;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// The base control structure for formpanel
    /// </summary>
    public class FormPanelControl : PlaceHolder
    {
        #region Fields
        private TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;
        private Label oLabel;
        private WebControl oInput;
        private Image oIcon;
        private Image oToolTip;
        private List<BaseValidator> lstValidators;
        #endregion Fields

        #region Properties
        public Label Label
        {
            get { return oLabel; }
        }
        public WebControl Input
        {
            get { return oInput; }
        }
        public Image Icon
        {
            get { return oIcon; }
        }
        public Image ToolTip
        {
            get { return oToolTip; }
        }
        public List<BaseValidator> Validators
        {
            get { return lstValidators; }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Basic control for placement into a twocolumn, etc.
        /// </summary>
        /// <param name="labelText"></param>
        /// <param name="inputIDPrefix"></param>
        /// <param name="iconImageURL"></param>
        /// <param name="iconJSEvents"></param>
        /// <param name="tooltipID"></param>
        public FormPanelControl(string controlID, string labelText, string inputIDPrefix, bool mandatory = false, string iconImageURL = "", Dictionary<string, string> iconJSEvents = null, string tooltipID = "")
        {
            #region Check inputs' validity
            #endregion Check inputs' validity

            #region Initialise control
            this.ID = "ph" + controlID;
            #endregion Initialise control

            #region Spans
            Label inputContainer = new Label();
            inputContainer.CssClass = "inputs";
            Label iconContainer = new Label();
            iconContainer.CssClass = "inputicon";
            Label toolTipContainer = new Label();
            toolTipContainer.CssClass = "inputtooltipfield";
            Label validatorContainer = new Label();
            validatorContainer.CssClass = "inputvalidatorfield";
            #endregion Spans

            #region Label
            oLabel = new Label();
            oLabel.ID = "lbl" + controlID;
            oLabel.Text = labelText + ((mandatory) ? "*" : "");
            #endregion Label

            #region Input
            oInput = new TextBox();
            oInput.ID = inputIDPrefix + controlID;
            inputContainer.Controls.Add(oInput);

            oLabel.AssociatedControlID = oInput.ID;
            #endregion Input

            #region Icon
            if (!string.IsNullOrWhiteSpace(iconImageURL))
            {
                oIcon = new Image();
                oIcon.ID = "img" + controlID;
                oIcon.ImageUrl = iconImageURL;
                iconContainer.Controls.Add(oIcon);
            }
            #endregion Icon

            #region Tooltip
            if (!string.IsNullOrEmpty(tooltipID))
            {
                oToolTip = new Image();
                oToolTip.ID = "imgTooltip" + controlID;
                oToolTip.ImageUrl = "~/shared/images/icons/16/plain/tooltip.png";
                oToolTip.Attributes.Add("onmouseover", "SEL.Tooltip.Show(" + tooltipID + ", 'sm', this);");
                toolTipContainer.Controls.Add(oToolTip);
            }
            #endregion Tooltip

            #region Validators
            lstValidators = new List<BaseValidator>();
            foreach (BaseValidator v in lstValidators)
            {
                validatorContainer.Controls.Add(v);
            }
            #endregion Validators

            #region Add the spans to the control placeholder
            this.Controls.Add(oLabel);
            this.Controls.Add(inputContainer);
            this.Controls.Add(iconContainer);
            this.Controls.Add(toolTipContainer);
            this.Controls.Add(validatorContainer);
            #endregion Add the spans to the control placeholder
        }
        #endregion Constructors

        #region Methods
        #endregion Methods
    }

    /// <summary>
    /// Textbox, half page width
    /// </summary>
    public class FormPanelTextBox : FormPanelControl
    {
        /// <summary>
        /// A basic textbox
        /// </summary>
        public FormPanelTextBox(string controlID, string labelText, bool mandatory = false, string tooltipID = "")
            : base(controlID, labelText, "txt", mandatory: mandatory, tooltipID: tooltipID)
        {
            List<BaseValidator> lstValidators = new List<BaseValidator>();
            if (mandatory)
            {
                RequiredFieldValidator reqVal = new RequiredFieldValidator();
                reqVal.ID = "rfv" + controlID;
                reqVal.ControlToValidate = "txt" + controlID;
                reqVal.Text = "*";
                reqVal.Display = ValidatorDisplay.Dynamic;
                reqVal.ErrorMessage = ValidatorMessages.Mandatory(labelText);
            }
        }
    }
}
