namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    /// <summary>
    /// A <see cref="NumericField"/> as a <see cref="CurrencyField"/>.
    /// </summary>
    public class CurrencyField : NumericField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyField"/> class. 
        /// Create a new instance of <see cref="CurrencyField"/>
        /// </summary>
        /// <param name="field">
        /// The base <see cref="CurrencyField"/> to decorate as a <see cref="NumericField"/>.
        /// </param>
        public CurrencyField(NumericField field) : base(field)
        {
        }
    }
}
