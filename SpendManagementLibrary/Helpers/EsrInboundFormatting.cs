namespace SpendManagementLibrary.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using SpendManagementLibrary.Enumerators;

    /// <summary>
    /// A class to help with formatting the Esr Inbound File.
    /// </summary>
    public class EsrInboundFormatting : IEsrInboundFormatting
    {
        /// <summary>
        /// True if a summary esr inbound file is created.
        /// </summary>
        private readonly bool _summaryEsrInboundFile;

        /// <summary>
        /// The esr rounding type.
        /// </summary>
        private readonly EsrRoundingType _esrRoundingType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EsrInboundFormatting"/> class. 
        /// </summary>
        /// <param name="accountProperties">
        /// An instance of <see cref="cAccountProperties"/> for this subaccount.
        /// </param>
        public EsrInboundFormatting(cAccountProperties accountProperties)
        {
            this._summaryEsrInboundFile = accountProperties.SummaryEsrInboundFile;
            this._esrRoundingType = accountProperties.EsrRounding;
        }

        /// <summary>
        /// Format numeric output for Esr Inbound.
        /// </summary>
        /// <param name="value">
        /// The value to format.
        /// </param>
        /// <param name="esrRowOutput">
        /// The esr row output.
        /// </param>
        /// <param name="globalElementField">
        /// The global element field.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/> rounded value if rounded.
        /// 0 if null
        /// -1 if not a decimal.
        /// </returns>
        public decimal FormatNumeric(object value, StringBuilder esrRowOutput, cGlobalESRElementField globalElementField)
        {
            if (value == DBNull.Value)
            {
                return 0;
            }

            decimal decimalValue;
            if (globalElementField.SummaryColumn && decimal.TryParse(value.ToString(), out decimalValue))
            {
                var num = this.GetRoundedValue(decimalValue, globalElementField);
                if (!this._summaryEsrInboundFile && this.RoundedField(globalElementField))
                {
                    esrRowOutput.Append(num.EsrRounding(this._esrRoundingType));
                }
                else
                {
                    esrRowOutput.Append(decimalValue.ToString("########0.00"));
                }
                return num;
            }

            esrRowOutput.Append(value.ToString());
            return 0;
        }

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
        public bool RoundedField(cGlobalESRElementField globalElementField)
        {
            return globalElementField.Rounded;
        }

        /// <summary>
        /// Returns true if this field should be rounded.
        /// </summary>
        /// <param name="globalElements">
        ///     The global elements.
        /// </param>
        /// <param name="elementName">
        ///     The element name.
        /// </param>
        /// <param name="elementFieldName">
        ///     The element field name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public cGlobalESRElementField GetElementField(List<cGlobalESRElement> globalElements, string elementName, string elementFieldName)
        {
            if (string.IsNullOrEmpty(elementName) || string.IsNullOrEmpty(elementFieldName))
            {
                return null;
            }

            var globalElement = globalElements.FirstOrDefault(g => g.GlobalElementName == elementName.TrimEnd(1).Substring(1));
            var element = globalElement?.Fields.FirstOrDefault(x => x.globalElementID == globalElement.GlobalElementID
                                                                    && x.globalElementFieldName == elementFieldName.TrimEnd(1).Substring(1));
            return element;
        }

        /// <summary>
        /// Get the rounded version of the given decimal.
        /// </summary>
        /// <param name="value">
        /// The decimal value to round.
        /// </param>
        /// <param name="globalElementField">
        /// The global element field.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// rounded decimal
        /// </returns>
        private decimal GetRoundedValue(decimal value, cGlobalESRElementField globalElementField)
        {
            return this.RoundedField(globalElementField) ? value.EsrRounding(this._esrRoundingType) : value;
        }
    }
}
