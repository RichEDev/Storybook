namespace SpendManagementLibrary.Helpers
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The EsrInboundFormatting interface.
    /// </summary>
    public interface IEsrInboundFormatting
    {
        /// <summary>
        /// Format numeric output for Esr Inbound.
        /// </summary>
        /// <param name="value">
        /// The value to format.
        /// </param>
        /// <param name="esrRowOutput">
        ///     The esr row output.
        /// </param>
        /// <param name="globalElementField">
        ///     The global element field.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        decimal FormatNumeric(object value, StringBuilder esrRowOutput, cGlobalESRElementField globalElementField);

        /// <summary>
        /// Returns true if this field should be rounded.
        /// </summary>
        /// <param name="globalElementField">
        /// The global element field.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// True if this is a rounded field.
        /// </returns>
        bool RoundedField(cGlobalESRElementField globalElementField);

        cGlobalESRElementField GetElementField(List<cGlobalESRElement> globalElements, string elementName, string elementFieldName);
    }
}