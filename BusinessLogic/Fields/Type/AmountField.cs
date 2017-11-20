namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    /// <summary>
    /// A <see cref="NumericField"/> as an <see cref="AmountField"/>.
    /// </summary>
    public class AmountField : NumericField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmountField"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="NumericField"/> to decorate as a <see cref="AmountField"/>.
        /// </param>
        public AmountField(NumericField field)
            : base(field)
        {
        }
    }
}
