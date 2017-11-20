namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    public class FunctionStringField : StringField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionStringField"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="FunctionStringField"/> to decorate as a <see cref="StringField"/>.
        /// </param>
        public FunctionStringField(StringField field) : base(field)
        {
        }
    }
}
