namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using System.Collections.Generic;
    using SpendManagementLibrary;

    /// <summary>
    /// Represents a User Defined Field. This is a method of extending the system to allow for user properties to be saved along with Expenses objects.
    /// </summary>
    public class UserDefinedFieldListElement : BaseExternalType, IEquatable<UserDefinedFieldListElement>
    {

        /// <summary>
        /// The element value.
        /// </summary>
        public int ElementValue { get; set; }

        /// <summary>
        /// The element text.
        /// </summary>
        public string ElementText { get; set; }

        /// <summary>
        /// The element order.
        /// </summary>
        public int ElementOrder { get; set; }

        /// <summary>
        /// Whether this list element is archived.
        /// </summary>
        public bool Archived { get; set; }        

        /// <summary>
        /// Overrides Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Returns a boolean indicating whether items are equal</returns>
        public bool Equals(UserDefinedFieldListElement other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Archived.Equals(other.Archived) && this.ElementOrder.Equals(other.ElementOrder)
                   && this.ElementText.Equals(other.ElementText) && this.ElementValue.Equals(other.ElementValue);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as UserDefinedFieldListElement);
        }
    }

    internal static class UserDefinedFieldListElementConversion
    {
        internal static TResult Cast<TResult>(this cListAttributeElement userDefinedFieldListElement)
            where TResult : UserDefinedFieldListElement, new()
        {
            return new TResult
            {
                Archived = userDefinedFieldListElement.Archived,
                ElementOrder = userDefinedFieldListElement.elementOrder,
                ElementText = userDefinedFieldListElement.elementText,
                ElementValue = userDefinedFieldListElement.elementValue
            };
        }
    }
}