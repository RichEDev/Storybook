namespace BusinessLogic.Fields.Type
{
    public class FunctionCurrency : CurrencyField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionCurrency"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="FunctionCurrency"/> to decorate as a <see cref="CurrencyField"/>.
        /// </param>
        public FunctionCurrency(CurrencyField field) : base(field)
        {
        }
    }
}
