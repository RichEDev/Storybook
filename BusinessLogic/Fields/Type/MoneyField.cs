namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    public class MoneyField : NumericField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyField"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="MoneyField"/> to decorate as a <see cref="NumericField"/>.
        /// </param>
        public MoneyField(NumericField field) : base(field)
        {
        }
    }
}
