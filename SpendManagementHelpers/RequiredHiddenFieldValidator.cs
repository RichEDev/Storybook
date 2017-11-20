namespace SpendManagementHelpers
{
    using System.Web.UI.WebControls;

    /// <summary>
    /// Allows validation to be added to asp:HiddenField controls
    /// </summary>
    public class RequiredHiddenFieldValidator : RequiredFieldValidator
    {
        /// <summary>
        /// Bypasses the code which "Determines whether the control specified by the System.Web.UI.WebControls.BaseValidator.ControlToValidate property is a valid control."
        /// </summary>
        /// <returns>always true</returns>
        protected override bool ControlPropertiesValid()
        {
            return true;
        }
    }
}
