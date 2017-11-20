namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    public class FunctionDecimalField : DecimalField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionDecimalField"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="FunctionDecimalField"/> to decorate as a <see cref="DecimalField"/>.
        /// </param>
        public FunctionDecimalField(DecimalField field) : base(field)
        {
        }
    }
}
