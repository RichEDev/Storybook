namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    public class FunctionIntegerField : IntegerField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionIntegerField"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="FunctionIntegerField"/> to decorate as a <see cref="IntegerField"/>.
        /// </param>
        public FunctionIntegerField(IntegerField field) : base(field)
        {
        }
    }
}
