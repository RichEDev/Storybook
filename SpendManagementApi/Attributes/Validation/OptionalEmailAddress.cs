using System.ComponentModel.DataAnnotations;

namespace SpendManagementApi.Attributes.Validation
{
    /// <summary>
    /// Validates an email if supplied.
    /// </summary>
    public class OptionalEmailAddressAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// Creates a new OptionalEmailAddressAttribute.
        /// </summary>
        public OptionalEmailAddressAttribute() :
            base(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,5}))\z")
        { }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> class. 
        /// </returns>
        /// <param name="value">The value to validate.</param><param name="validationContext">The context information about the validation operation.</param>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = (string) value;
            return string.IsNullOrWhiteSpace(email) ? ValidationResult.Success : base.IsValid(value, validationContext);
        }
    }
}